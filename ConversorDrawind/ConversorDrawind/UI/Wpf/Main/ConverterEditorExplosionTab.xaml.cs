using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorExplosionTab : UserControl
    {
        public ConverterEditorExplosionTab()
        {
            InitializeComponent();
        }

        public ListBox AllLayersListBox => AllExplodeLayersListBox;
        public ListBox SelectedLayersListBox => SelectedExplodeLayersListBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddExplodeLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(AddExplodeLayerClick), sender, e);
        private void RemoveExplodeLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveExplodeLayerClick), sender, e);
    }
}
