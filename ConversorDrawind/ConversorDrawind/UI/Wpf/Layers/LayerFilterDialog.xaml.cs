using ConversorDrawind;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConversorDrawind.UI.Wpf.Common;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerFilterDialog : Window
    {
        private readonly CatalogConfiguration catalogs;
        private readonly Filter filter;
        private List<string> lineTypes = new List<string>();

        public LayerFilterDialog(Filter filter, CatalogConfiguration catalogs)
        {
            InitializeComponent();
            this.filter = filter;
            this.catalogs = catalogs ?? new global::ConversorDrawind.Configuration().Catalogs;
            LoadValues();
        }

        public Filter Filter => filter;

        private void LoadValues()
        {
            ObjectTypeComboBox.ItemsSource = catalogs.ObjectTypes;
            ColorComboBox.ItemsSource = catalogs.Colors;
            lineTypes = catalogs.FilterLineTypes.ToList();
            LineTypeComboBox.ItemsSource = lineTypes;
            OrientationComboBox.ItemsSource = new[] { "ALL", "HORIZONTAL", "VERTICAL" };

            ObjectTypeComboBox.Text = filter.tipoObjeto;
            ColorComboBox.Text = filter.cor;
            LineTypeComboBox.Text = filter.tipoLinha;
            TextHeightTextBox.Text = filter.alturaTexto;
            TextContentTextBox.Text = filter.conteudoTexto;
            OrientationComboBox.Text = filter.orientacao;
        }

        public void LoadLineTypes2(string line)
        {
            catalogs.LayerLineTypes.Clear();
            foreach (string item in line.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                catalogs.LayerLineTypes.Add(item);
            }

            lineTypes = catalogs.LayerLineTypes.ToList();
            LineTypeComboBox.ItemsSource = lineTypes;
            if (lineTypes.Count > 0 && string.IsNullOrWhiteSpace(LineTypeComboBox.Text))
            {
                LineTypeComboBox.Text = lineTypes[0];
            }
        }

        public void DisableText()
        {
            TextGroupBox.Visibility = Visibility.Collapsed;
        }

        public void DisableOrientation()
        {
            OrientationGroupBox.Visibility = Visibility.Collapsed;
        }

        private void OtherColorButtonClick(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog dialog = new ColorPickerDialog
            {
                Owner = this
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (!catalogs.Colors.Contains(dialog.ColorValue))
            {
                catalogs.Colors.Add(dialog.ColorValue);
                ColorComboBox.Items.Refresh();
            }

            ColorComboBox.Text = dialog.ColorValue;
        }

        private void AddEntityButtonClick(object sender, RoutedEventArgs e)
        {
            LayerEntityDialog dialog = new LayerEntityDialog(ObjectTypeComboBox.Text)
            {
                Owner = this
            };

            dialog.ShowDialog();
            catalogs.ObjectTypes.Remove(dialog.Entity);
            catalogs.ObjectTypes.Add(dialog.Entity);
            ObjectTypeComboBox.Items.Refresh();
            ObjectTypeComboBox.Text = dialog.Entity;
        }

        private void LoadLineTypesButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = Localization.FilterDrawing
                };

                if (openFileDialog.ShowDialog(this) != true)
                {
                    return;
                }

                Thread loadThread = new Thread(GetLineType);
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.Start(openFileDialog.FileName);

                Thread statusThread = new Thread(ApplicationRuntime.ThreadMethodAnalisando);
                statusThread.SetApartmentState(ApartmentState.STA);
                statusThread.Start();

                loadThread.Join();
                ApplicationRuntime.StopStatusThread(statusThread);

                ImportTempLineTypes();
                lineTypes = catalogs.FilterLineTypes.ToList();
                LineTypeComboBox.ItemsSource = lineTypes;
                LineTypeComboBox.Text = catalogs.FilterLineTypes.FirstOrDefault();
            }
            catch (Exception)
            {
            }
        }

        private static void GetLineType(object fileName)
        {
            try
            {
                string file = (string)fileName;
                ACAD.AcadApplication acadApplication;
                ACAD.AcadDocument acadDocument;
                using (MessageFilter.ScopedRegistration())
                {
                    acadApplication = new ACAD.AcadApplication();
                    acadDocument = acadApplication.Documents.Open(file, false);
                }
                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.LoadFile(DrawingProcess.DLLPath1, acadDocument);
                }
                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.SendCommand("CDwi_LoadLineType\n", acadDocument);
                    acadDocument.Close(false);
                    acadApplication.Quit();
                }
            }
            catch (Exception e)
            {
                ApplicationRuntime.ControladorT = false;
                System.Windows.MessageBox.Show(e.Message, Localization.TitleWarningNoExclamation, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ImportTempLineTypes()
        {
            string filetxt = new global::ConversorDrawind.Configuration().GetTempDirectory() + "TempImporLineType.Temp";
            if (!File.Exists(filetxt))
            {
                return;
            }

            foreach (string line in File.ReadLines(filetxt, Encoding.UTF8))
            {
                string lineType = line.ToUpper();
                catalogs.FilterLineTypes.Remove(lineType);
                catalogs.FilterLineTypes.Add(lineType);
            }

            catalogs.FilterLineTypes.Sort();
            File.Delete(filetxt);
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            filter.tipoObjeto = ObjectTypeComboBox.Text;
            filter.cor = ColorComboBox.Text;
            filter.tipoLinha = LineTypeComboBox.Text;
            filter.alturaTexto = TextHeightTextBox.Text;
            filter.conteudoTexto = TextContentTextBox.Text;
            filter.orientacao = GetSelectedOrientation();
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DecimalTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, e.Text);
            e.Handled = !proposed.All(character => char.IsDigit(character) || character == ',')
                || proposed.Count(character => character == ',') > 1;
        }

        private void OrientationComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetSelectedOrientation() == "ALL")
            {
                ObjectTypeGroupBox.IsEnabled = true;
                return;
            }

            ObjectTypeComboBox.Text = "LINE";
            ObjectTypeGroupBox.IsEnabled = false;
        }

        private string GetSelectedOrientation()
        {
            return OrientationComboBox.SelectedItem?.ToString() ?? OrientationComboBox.Text;
        }
    }
}
