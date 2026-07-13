using System.Collections.Generic;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerNameDialog : Window
    {
        private readonly ISet<string> existingNames;
        private readonly string originalName;

        public LayerNameDialog(string currentName, IEnumerable<string> allLayerNames)
        {
            InitializeComponent();
            originalName = currentName;
            existingNames = new HashSet<string>(allLayerNames);
            LayerName = currentName;
            NameTextBox.Text = currentName;
            NameTextBox.SelectAll();
            NameTextBox.Focus();
        }

        public string LayerName { get; private set; }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            string proposedName = NameTextBox.Text;
            if (!existingNames.Contains(proposedName) || proposedName == originalName)
            {
                LayerName = proposedName;
                DialogResult = true;
                return;
            }

            System.Windows.MessageBox.Show(
                Localization.MessageLayerAlreadyExists,
                Localization.TitleWarningNoExclamation,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
