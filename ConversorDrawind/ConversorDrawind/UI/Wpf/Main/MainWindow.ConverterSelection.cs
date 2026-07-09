using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void LoadConverterLists()
        {
            List<string> names = ConverterFileService.ListConverterNames(CurrentStatus);
            viewModel.SetConverterNames(names);
        }

        private List<string> CurrentConverterNames()
        {
            return viewModel.ConverterNames.ToList();
        }

        private void RestoreInitialConverter(IEnumerable<StatusConversorItem> statusItems)
        {
            string converterName = string.Empty;
            if (UserSettingsService.TryReadLastConverter(out string statusFolder, out string lastConverterName))
            {
                StatusConversorItem savedStatus = statusItems.FirstOrDefault(status => string.Equals(status.Pasta, statusFolder, StringComparison.OrdinalIgnoreCase));
                if (savedStatus != null)
                {
                    viewModel.SelectedStatus = savedStatus;
                    converterName = lastConverterName;
                }
            }

            LoadConverterLists();

            List<string> names = CurrentConverterNames();
            if (string.IsNullOrWhiteSpace(converterName) || !names.Contains(converterName))
            {
                converterName = names.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(converterName))
            {
                return;
            }

            LoadConverter(converterName);
            SelectLoadedConverter(converterName);
        }

        private void AddDrawings()
        {
            OpenFileDialog dialog = new OpenFileDialog { Multiselect = true, Filter = Localization.FilterCadDrawings };
            if (dialog.ShowDialog(this) != true) return;
            foreach (string file in dialog.FileNames.Where(File.Exists)) if (!ConverterView.DrawingsListBox.Items.Contains(file)) ConverterView.DrawingsListBox.Items.Add(file);
        }

        private void DropDrawings(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop)) if (File.Exists(file) && !ConverterView.DrawingsListBox.Items.Contains(file)) ConverterView.DrawingsListBox.Items.Add(file);
        }

        private void RestoreLastConverted()
        {
            if (!File.Exists(ApplicationRuntime.LOGarqConvertidos)) { WpfMessageBox.Show(Localization.MessageNoPreviousConvertedFiles, Localization.AppTitle); return; }
            ConverterView.DrawingsListBox.Items.Clear();
            foreach (string file in File.ReadAllLines(ApplicationRuntime.LOGarqConvertidos).Where(File.Exists)) ConverterView.DrawingsListBox.Items.Add(file);
        }

        private void SelectConverterFromList()
        {
            string selected = viewModel.SelectedConverterName;
            if (isInitializing || isSynchronizingConverterSelection || string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            if (!ConfirmDiscardTemplateChanges())
            {
                RestoreLoadedConverterSelection();
                return;
            }

            LoadConverter(selected);
            SelectLoadedConverter(selected);
        }

        private void SelectConverterFromEditor()
        {
            string selected = viewModel.SelectedConverterName;
            if (isInitializing || isSynchronizingConverterSelection || string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            if (!ConfirmDiscardTemplateChanges())
            {
                RestoreLoadedConverterSelection();
                return;
            }

            LoadConverter(selected);
            SelectLoadedConverter(selected);
        }

        private void LoadConverter(string converterName)
        {
            configuration = ConverterFileService.LoadConverter(converterName, CurrentStatus);
            viewModel.Configuration = configuration;
            RefreshEditorFromConfiguration();
            UserSettingsService.SaveLastConverter(CurrentStatus, converterName);
            loadedConverterName = converterName;
            loadedConverterStatus = CurrentStatus;
        }

        private void SelectLoadedConverter(string converterName)
        {
            isSynchronizingConverterSelection = true;
            try
            {
                viewModel.SelectedConverterName = converterName;
                viewModel.TemplateName = converterName;
            }
            finally
            {
                isSynchronizingConverterSelection = false;
            }
        }

        private bool ConfirmDiscardTemplateChanges()
        {
            if (!HasLoadedTemplateChanged())
            {
                return true;
            }

            return WpfMessageBox.Show(
                Localization.MessageTemplateChangedConfirmSwitch,
                Localization.TitleWarningNoExclamation,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        private bool HasLoadedTemplateChanged()
        {
            if (string.IsNullOrWhiteSpace(loadedConverterName) || loadedConverterStatus == null)
            {
                return false;
            }

            string savedPath = ConverterFileService.GetTxmlPath(loadedConverterName, loadedConverterStatus);
            if (!File.Exists(savedPath))
            {
                return false;
            }

            ApplyEditorPendingChanges();

            global::ConversorDrawind.Configuration savedConfiguration = ConverterFileService.LoadConverter(loadedConverterName, loadedConverterStatus);
            string currentXml = StructuredConfigurationXmlWriter.CreateDocument(configuration.ToConverterConfiguration()).ToString();
            string savedXml = StructuredConfigurationXmlWriter.CreateDocument(savedConfiguration.ToConverterConfiguration()).ToString();

            return !string.Equals(currentXml, savedXml, StringComparison.Ordinal);
        }

        private void RestoreLoadedConverterSelection()
        {
            isSynchronizingConverterSelection = true;
            try
            {
                viewModel.SelectedConverterName = string.IsNullOrWhiteSpace(loadedConverterName) ? string.Empty : loadedConverterName;
                viewModel.TemplateName = loadedConverterName ?? string.Empty;
            }
            finally
            {
                isSynchronizingConverterSelection = false;
            }
        }

        private void ConvertSelectedDrawings()
        {
            ApplyEditorPendingChanges();
            string selectedConverter = viewModel.SelectedConverterName;
            if (string.IsNullOrWhiteSpace(selectedConverter)) { WpfMessageBox.Show(Localization.MessageSelectConverterBeforeConvert, Localization.AppTitle); return; }
            string[] drawingFiles = ConverterView.DrawingsListBox.Items.OfType<string>().ToArray();
            if (drawingFiles.Length == 0) { WpfMessageBox.Show(Localization.MessageAddAtLeastOneDrawingBeforeConvert, Localization.AppTitle); return; }
            ConversionPreflightResult preflight = ConversionPreflightValidator.ValidateFormatPath(configuration);
            if (!preflight.CanConvert) { WpfMessageBox.Show(Localization.FormatNotFoundMessage(preflight.MissingFormatPath), Localization.AppTitle); return; }
            ApplicationRuntime.ExtensaoGeral = viewModel.Extension;
            Param1 param = new Param1 { conversorName = selectedConverter, desenhosName = drawingFiles, closedesenhos = ConverterView.KeepFilesOpenCheckBox.IsChecked != true, configuration = configuration, StatusConversorItem = CurrentStatus };
            using (Processo processo = new Processo(param)) processo.ShowDialog();
            if (!Processo.IsCanceled) using (ProcessoEnd processoEnd = new ProcessoEnd()) processoEnd.ShowDialog();
        }

    }
}
