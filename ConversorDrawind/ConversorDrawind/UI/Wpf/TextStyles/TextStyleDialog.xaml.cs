using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.TextStyles
{
    public partial class TextStyleDialog : Window
    {
        public TextStyleDialog(string[] values)
        {
            InitializeComponent();
            Values = new string[7];
            LoadFonts();
            LoadValues(values);
        }

        public string[] Values { get; private set; }

        private void LoadFonts()
        {
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                FontComboBox.Items.Add(font.Name);
            }

            if (!FontComboBox.Items.Contains("RomanS"))
            {
                FontComboBox.Items.Add("RomanS");
            }

            FontComboBox.Text = "RomanS";
        }

        private void LoadValues(string[] values)
        {
            NameTextBox.Text = values[0] ?? string.Empty;
            FontComboBox.Text = string.IsNullOrEmpty(values[1]) ? "RomanS" : values[1];
            ObliqueCheckBox.IsChecked = bool.TryParse(values[2], out bool oblique) && oblique;
            BoldCheckBox.IsChecked = bool.TryParse(values[3], out bool bold) && bold;
            SizeTextBox.Text = string.IsNullOrEmpty(values[4]) ? "2.5" : values[4];
            WidthFactorTextBox.Text = string.IsNullOrEmpty(values[5]) ? "1" : values[5];
            AngleTextBox.Text = string.IsNullOrEmpty(values[6]) ? "0" : values[6];
            UpdateFontStyleControls();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            NormalizeDefaults();
            Values = new[]
            {
                NameTextBox.Text,
                FontComboBox.Text,
                (ObliqueCheckBox.IsChecked == true).ToString(),
                (BoldCheckBox.IsChecked == true).ToString(),
                SizeTextBox.Text,
                WidthFactorTextBox.Text,
                AngleTextBox.Text
            };

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void FontComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFontStyleControls();
        }

        private void DecimalTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, e.Text);

            e.Handled = !proposed.All(character => char.IsDigit(character) || character == ',')
                || proposed.Count(character => character == ',') > 1;
        }

        private void AngleTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, e.Text);

            if (proposed == "-")
            {
                e.Handled = false;
                return;
            }

            e.Handled = !double.TryParse(proposed, NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out double value)
                || value < -85
                || value > 85;
        }

        private void SizeTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SizeTextBox.Text))
            {
                SizeTextBox.Text = "2,5";
            }
        }

        private void WidthFactorTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(WidthFactorTextBox.Text))
            {
                WidthFactorTextBox.Text = "1";
            }
        }

        private void AngleTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AngleTextBox.Text))
            {
                AngleTextBox.Text = "0";
            }
        }

        private void NormalizeDefaults()
        {
            SizeTextBoxLostFocus(this, new RoutedEventArgs());
            WidthFactorTextBoxLostFocus(this, new RoutedEventArgs());
            AngleTextBoxLostFocus(this, new RoutedEventArgs());
        }

        private void UpdateFontStyleControls()
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            bool isShx = File.Exists(Path.Combine(roaming, @"\Autodesk\AutoCAD 2026\R25.1\enu\support\") + FontComboBox.Text + ".shx");
            ObliqueCheckBox.IsEnabled = !isShx;
            BoldCheckBox.IsEnabled = !isShx;
        }
    }
}
