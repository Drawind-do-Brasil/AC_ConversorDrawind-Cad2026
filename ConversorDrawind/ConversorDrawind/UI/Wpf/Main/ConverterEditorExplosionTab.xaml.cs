using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorExplosionTab : UserControl
    {
        private const string ExplodeLayerDragFormat = "ConversorDrawind.ExplodeLayerNames";
        private const string ExplodeLayerDragSourceFormat = "ConversorDrawind.ExplodeLayerSource";

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

        private void ExplodeLayersMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !(sender is ListBox listBox) || listBox.SelectedItems.Count == 0)
            {
                return;
            }

            string[] selectedLayers = listBox.SelectedItems.OfType<string>().ToArray();
            if (selectedLayers.Length == 0)
            {
                return;
            }

            DataObject data = new DataObject();
            data.SetData(ExplodeLayerDragFormat, selectedLayers);
            data.SetData(ExplodeLayerDragSourceFormat, listBox.Name);
            DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);
        }

        private void ExplodeLayersDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(ExplodeLayerDragFormat) ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        private void ExplodeLayersDrop(object sender, DragEventArgs e) => Forward(nameof(ExplodeLayersDrop), sender, e);
    }
}
