using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Common
{
    public partial class ColorPickerDialog : Window
    {
        public ColorPickerDialog()
        {
            InitializeComponent();
            ColorValue = "BYLAYER";
            UpdateColorText();
        }

        public string ColorValue { get; private set; }

        private void ColorComponentPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValidComponentText((TextBox)sender, e.Text);
        }

        private void ColorComponentTextChanged(object sender, TextChangedEventArgs e)
        {
            NormalizeComponent((TextBox)sender);
            UpdateColorText();
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            NormalizeComponent(RedTextBox);
            NormalizeComponent(GreenTextBox);
            NormalizeComponent(BlueTextBox);

            ColorValue = HasCompleteRgb()
                ? string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", RedTextBox.Text, GreenTextBox.Text, BlueTextBox.Text)
                : "BYLAYER";

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private static bool IsValidComponentText(TextBox textBox, string text)
        {
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, text);

            return int.TryParse(proposed, NumberStyles.None, CultureInfo.InvariantCulture, out int value)
                && value >= 0
                && value <= 255;
        }

        private static void NormalizeComponent(TextBox textBox)
        {
            if (!int.TryParse(textBox.Text, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
            {
                return;
            }

            value = Math.Max(0, Math.Min(255, value));
            string normalized = value.ToString(CultureInfo.InvariantCulture);

            if (textBox.Text == normalized)
            {
                return;
            }

            int caretIndex = textBox.CaretIndex;
            textBox.Text = normalized;
            textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length);
        }

        private bool HasCompleteRgb()
        {
            return RedTextBox.Text.Length > 0
                && GreenTextBox.Text.Length > 0
                && BlueTextBox.Text.Length > 0;
        }

        private void UpdateColorText()
        {
            ColorTextBox.Text = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}",
                RedTextBox.Text,
                GreenTextBox.Text,
                BlueTextBox.Text);
        }
    }
}



