using System.Windows;

namespace ConversorDrawind.UI.Wpf.Configuration
{
    public partial class AdvancedConfigurationWindow : Window
    {
        public AdvancedConfigurationWindow()
        {
            InitializeComponent();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
