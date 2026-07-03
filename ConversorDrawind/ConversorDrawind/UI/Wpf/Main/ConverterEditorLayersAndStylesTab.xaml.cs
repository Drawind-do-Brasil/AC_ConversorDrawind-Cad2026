using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorLayersAndStylesTab : UserControl
    {
        public ConverterEditorLayersAndStylesTab()
        {
            InitializeComponent();
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void LoadClientLayersClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadClientLayersClick), sender, e);
        private void ConfigureClientLayersClick(object sender, RoutedEventArgs e) => Forward(nameof(ConfigureClientLayersClick), sender, e);
        private void ConfigureTextStylesClick(object sender, RoutedEventArgs e) => Forward(nameof(ConfigureTextStylesClick), sender, e);
    }
}
