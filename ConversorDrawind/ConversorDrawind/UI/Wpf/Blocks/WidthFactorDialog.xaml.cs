using System.Windows;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Blocks
{
    public partial class WidthFactorDialog : Window
    {
        public WidthFactorDialog(string widthFactor)
        {
            InitializeComponent();
            WidthFactor = widthFactor;
            WidthFactorTextBox.Text = widthFactor;
            DataObject.AddPastingHandler(WidthFactorTextBox, WidthFactorPasteHandler);
        }

        public string WidthFactor { get; private set; } = "1";

        private void WidthFactorPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsAllowedText(e.Text);
        }

        private void WidthFactorPasteHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string text = e.DataObject.GetData(DataFormats.Text)?.ToString() ?? string.Empty;
            if (!IsAllowedPaste(text))
            {
                e.CancelCommand();
            }
        }

        private bool IsAllowedText(string text)
        {
            foreach (char character in text)
            {
                if (!char.IsDigit(character) && character != ',')
                {
                    return false;
                }
            }

            return text != "," || !WidthFactorTextBox.Text.Contains(",");
        }

        private bool IsAllowedPaste(string text)
        {
            int commaCount = WidthFactorTextBox.Text.Contains(",") ? 1 : 0;
            foreach (char character in text)
            {
                if (char.IsDigit(character))
                {
                    continue;
                }

                if (character == ',' && commaCount == 0)
                {
                    commaCount++;
                    continue;
                }

                return false;
            }

            return true;
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            WidthFactor = WidthFactorTextBox.Text;
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
