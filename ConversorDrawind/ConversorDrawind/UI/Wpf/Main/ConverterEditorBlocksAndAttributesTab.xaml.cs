using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorBlocksAndAttributesTab : UserControl
    {
        public ConverterEditorBlocksAndAttributesTab()
        {
            InitializeComponent();
        }

        public TextBox AttributedFormatPath => AttributedFormatPathTextBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardMouse(string action, object sender, MouseButtonEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void BrowseAttributedFormatClick(object sender, RoutedEventArgs e) => Forward(nameof(BrowseAttributedFormatClick), sender, e);
        private void EditBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockClick), sender, e);
        private void RemoveBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockClick), sender, e);
        private void LoadInventorBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadInventorBlocksClick), sender, e);
        private void LoadOriginalBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadOriginalBlocksClick), sender, e);
        private void BlockRelationsDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(BlockRelationsDoubleClick), sender, e);
        private void RelateBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(RelateBlocksClick), sender, e);
        private void EditBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockRelationClick), sender, e);
        private void RemoveBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockRelationClick), sender, e);
    }
}
