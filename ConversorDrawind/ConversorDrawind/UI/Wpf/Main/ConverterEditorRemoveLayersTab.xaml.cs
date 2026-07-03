using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorRemoveLayersTab : UserControl
    {
        public ConverterEditorRemoveLayersTab()
        {
            InitializeComponent();
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardMouse(string action, object sender, MouseButtonEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddRemoveLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(AddRemoveLayerClick), sender, e);
        private void DeleteRemoveLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteRemoveLayerClick), sender, e);
        private void RemoveLayersGridDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(RemoveLayersGridDoubleClick), sender, e);
    }
}
