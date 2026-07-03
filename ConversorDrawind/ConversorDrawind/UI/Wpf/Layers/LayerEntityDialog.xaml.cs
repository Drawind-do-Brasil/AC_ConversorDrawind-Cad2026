using System.Globalization;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerEntityDialog : Window
    {
        public LayerEntityDialog(string currentEntity)
        {
            InitializeComponent();
            Entity = currentEntity;
            EntityTextBox.Focus();
        }

        public string Entity { get; private set; }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            Entity = EntityTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
