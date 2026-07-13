using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Configuration
{
    public partial class DimensionAdvancedDialog : Window
    {
        public DimensionAdvancedDialog(bool concerta, string tipoSeta, double distancia)
        {
            InitializeComponent();
            ArrowTypeComboBox.ItemsSource = new[]
            {
                "Architectural Tick",
                "Box",
                "Box Filled",
                "Closed",
                "Closed Blank",
                "Closed Filled",
                "Datum Triangle",
                "Datum Triangle Filled",
                "Dot",
                "Dot Blank",
                "Dot Small",
                "Dot Small Blank",
                "Integral",
                "None",
                "Oblique",
                "Open",
                "Open 30",
                "Origin Indicator",
                "Origin Indicator 2",
                "Right Angle"
            };

            EXTDIMCorrigeSeta = concerta;
            EXTDIMCorrigeSetaTipoSeta = tipoSeta;
            EXTDIMCorrigeSetaFactor = distancia;

            FixArrowCheckBox.IsChecked = concerta;
            ArrowTypeComboBox.Text = tipoSeta;
            DistanceTextBox.Text = Convert.ToString(distancia);
        }

        public bool EXTDIMCorrigeSeta { get; private set; }
        public string EXTDIMCorrigeSetaTipoSeta { get; private set; }
        public double EXTDIMCorrigeSetaFactor { get; private set; }

        private void DistanceTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, e.Text);

            e.Handled = !proposed.All(character => char.IsDigit(character) || character == ',')
                || proposed.Count(character => character == ',') > 1;
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            EXTDIMCorrigeSeta = FixArrowCheckBox.IsChecked == true;
            EXTDIMCorrigeSetaTipoSeta = ArrowTypeComboBox.Text;
            EXTDIMCorrigeSetaFactor = Convert.ToDouble(DistanceTextBox.Text);
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}



