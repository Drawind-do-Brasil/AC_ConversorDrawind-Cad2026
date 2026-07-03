using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorDimensionsTab : UserControl
    {
        public ConverterEditorDimensionsTab()
        {
            InitializeComponent();
        }

        public ComboBox LayerComboBox => DimensionLayerComboBox;
        public TextBox StyleTextBox => DimensionStyleTextBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void DimensionArrowAdvancedClick(object sender, RoutedEventArgs e) => Forward(nameof(DimensionArrowAdvancedClick), sender, e);
    }
}
