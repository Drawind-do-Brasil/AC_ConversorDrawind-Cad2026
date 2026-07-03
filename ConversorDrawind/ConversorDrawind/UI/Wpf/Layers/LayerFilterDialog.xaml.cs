using ConversorDrawind.UI.Wpf.Common;
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
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerFilterDialog : Window
    {
        private readonly Arranjos arranjos;
        private readonly Filter filter;
        private List<string> lineTypes;

        public LayerFilterDialog(Filter filter, Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
            this.filter = filter;
            lineTypes = arranjos.allLineType1.ToList();

            LoadComboItems();
            LoadFilterValues();
        }

        public Filter Filter => filter;

        public void LoadLineTypes2(string line)
        {
            lineTypes = new List<string> { "ALL" };
            lineTypes.AddRange(arranjos.allLineType2);
            LineTypeComboBox.ItemsSource = lineTypes;
            LineTypeComboBox.Text = "ALL";

            Filter currentFilter = new Filter(arranjos);
            currentFilter.SetConjunto(line);
            foreach (string lineType in lineTypes)
            {
                if (lineType.Split(',').First().ToUpper() == currentFilter.tipoLinha)
                {
                    LineTypeComboBox.Text = lineType;
                    break;
                }
            }
        }

        public void DisableText()
        {
            ObjectTypeComboBox.Text = "TEXT";
            ObjectTypeGroupBox.IsEnabled = false;
            LineTypeGroupBox.IsEnabled = false;
        }

        public void DisableOrientation()
        {
            OrientationGroupBox.IsEnabled = false;
        }

        private void LoadComboItems()
        {
            ObjectTypeComboBox.ItemsSource = arranjos.allobjects;
            ColorComboBox.ItemsSource = arranjos.allcolor;
            LineTypeComboBox.ItemsSource = lineTypes;
            OrientationComboBox.ItemsSource = new[] { "ALL", "HORIZONTAL", "VERTICAL" };
        }

        private void LoadFilterValues()
        {
            ColorComboBox.Text = filter.cor;
            ObjectTypeComboBox.Text = filter.tipoObjeto;
            LineTypeComboBox.Text = filter.tipoLinha;
            TextHeightTextBox.Text = filter.alturaTexto;
            TextContentTextBox.Text = filter.conteudoTexto;
            OrientationComboBox.Text = filter.orientacao;
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

            if (!arranjos.allcolor.Contains(dialog.ColorValue))
            {
                arranjos.allcolor.Add(dialog.ColorValue);
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
            arranjos.allobjects.Remove(dialog.Entity);
            arranjos.allobjects.Add(dialog.Entity);
            ObjectTypeComboBox.Items.Refresh();
            ObjectTypeComboBox.Text = dialog.Entity;
        }

        private void LoadLineTypesButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Drawing (*.dwg)|*.dwg"
                };

                if (openFileDialog.ShowDialog(this) != true)
                {
                    return;
                }

                Thread loadThread = new Thread(GetLineType);
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.Start(openFileDialog.FileName);

                Thread statusThread = new Thread(Form_0_JanelaPrincipal.ThreadMethodAnalisando);
                statusThread.SetApartmentState(ApartmentState.STA);
                statusThread.Start();

                loadThread.Join();
                Form_0_JanelaPrincipal.StopStatusThread(statusThread);

                ImportTempLineTypes();
                lineTypes = arranjos.allLineType1.ToList();
                LineTypeComboBox.ItemsSource = lineTypes;
                LineTypeComboBox.Text = arranjos.allLineType1.FirstOrDefault();
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
                    LoadFiles.SendCommand("DRAWINDCAD_LoadLineType\n", acadDocument);
                    acadDocument.Close(false);
                    acadApplication.Quit();
                }
            }
            catch (Exception e)
            {
                Form_0_JanelaPrincipal.ControladorT = false;
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ImportTempLineTypes()
        {
            string filetxt = new global::ConversorDrawind.Configuration().GetPROGRAMDirectoryTemp() + "TempImporLineType.Temp";
            if (!File.Exists(filetxt))
            {
                return;
            }

            foreach (string line in File.ReadLines(filetxt, Encoding.UTF8))
            {
                string lineType = line.ToUpper();
                arranjos.allLineType1.Remove(lineType);
                arranjos.allLineType1.Add(lineType);
            }

            arranjos.allLineType1.Sort();
            File.Delete(filetxt);
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (arranjos.allcolor.Contains(ColorComboBox.Text))
            {
                filter.cor = ColorComboBox.Text;
            }

            if (lineTypes.Contains(LineTypeComboBox.Text))
            {
                filter.tipoLinha = LineTypeComboBox.Text.Split(',').First().ToUpper();
            }

            if (arranjos.allobjects.Contains(ObjectTypeComboBox.Text))
            {
                filter.tipoObjeto = ObjectTypeComboBox.Text;
            }

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
