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

        public TextBox AttributedFormatPathTextBoxControl => AttributedFormatPathTextBox;
        public TextBox CadBlocksPathTextBoxControl => CadBlocksPathTextBox;
        public TextBox OriginalBlocksPathTextBoxControl => OriginalBlocksPathTextBox;
        public ListBox TeklaBlocksListBoxControl => BlocksListBox;
        public ListBox CadBlocksListBoxControl => InventorBlocksListBox;
        public ListBox OriginalBlocksListBoxControl => OriginalBlocksListBox;
        public ListBox BlockRelationsListBoxControl => BlockRelationsListBox;
        public Button RelateButtonControl => FindName("RelateButton") as Button;
        public Button RemoveRelationButtonControl => FindName("RemoveRelationButton") as Button;
        public Button EditRelationButtonControl => FindName("EditRelationButton") as Button;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardMouse(string action, object sender, MouseButtonEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardSelection(string action, object sender, SelectionChangedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void BrowseAttributedFormatClick(object sender, RoutedEventArgs e) => Forward(nameof(BrowseAttributedFormatClick), sender, e);
        private void EditBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockClick), sender, e);
        private void EditBlockDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(EditBlockDoubleClick), sender, e);
        private void RemoveBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockClick), sender, e);
        private void LoadInventorBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadInventorBlocksClick), sender, e);
        private void LoadOriginalBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadOriginalBlocksClick), sender, e);
        private void CadBlocksSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(CadBlocksSelectionChanged), sender, e);
        private void OriginalBlocksSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(OriginalBlocksSelectionChanged), sender, e);
        private void BlockRelationsSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(BlockRelationsSelectionChanged), sender, e);
        private void BlockRelationsDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(BlockRelationsDoubleClick), sender, e);
        private void RelateBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(RelateBlocksClick), sender, e);
        private void EditBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockRelationClick), sender, e);
        private void RemoveBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockRelationClick), sender, e);
    }
}
