using Microsoft.Win32;
using System.IO;
using WpfMessageBox = System.Windows.MessageBox;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void NewConverter()
        {
            configuration = new global::ConversorDrawind.Configuration();
            viewModel.Configuration = configuration;
            viewModel.TemplateName = string.Empty;
            RefreshEditorFromConfiguration();
            loadedConverterName = string.Empty;
            loadedConverterStatus = null;
        }

        private void SaveConverter()
        {
            string name = viewModel.TemplateName.Trim();
            if (string.IsNullOrWhiteSpace(name)) { WpfMessageBox.Show(Localization.MessageEnterConverterNameBeforeSave, Localization.AppTitle); return; }
            ApplyEditorPendingChanges();
            ConverterFileService.SaveConverter(name, CurrentStatus, configuration);
            LoadConverterLists();
            SelectLoadedConverter(name);
            UserSettingsService.SaveLastConverter(CurrentStatus, name);
            loadedConverterName = name;
            loadedConverterStatus = CurrentStatus;
        }

        private void ImportConverter()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = Localization.FilterTemplateXml };
            if (dialog.ShowDialog(this) != true) return;
            configuration = ConverterFileService.LoadConverter(dialog.FileName, CurrentStatus);
            viewModel.Configuration = configuration;
            viewModel.TemplateName = Path.GetFileNameWithoutExtension(dialog.FileName);
            RefreshEditorFromConfiguration();
            UserSettingsService.SaveLastConverter(CurrentStatus, viewModel.TemplateName);
            loadedConverterName = string.Empty;
            loadedConverterStatus = null;
        }

    }
}
