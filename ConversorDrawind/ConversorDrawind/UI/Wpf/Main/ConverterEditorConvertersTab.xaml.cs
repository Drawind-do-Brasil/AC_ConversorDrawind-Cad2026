using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorConvertersTab : UserControl
    {
        public ConverterEditorConvertersTab()
        {
            InitializeComponent();
        }

        public TextBox ScaleTextBox => LtScaleTextBox;
        public ComboBox TeklaLayerComboBox => TeklaTextLayerComboBox;
        public ComboBox FormatLayerComboBox => FormatBlockLayerComboBox;
        public DataGrid RulesGrid => LayerRulesGrid;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardMouse(string action, object sender, MouseButtonEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddLayerRuleClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLayerRuleClick), sender, e);
        private void DeleteLayerRuleClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteLayerRuleClick), sender, e);
        private void MoveLayerRuleUpClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLayerRuleUpClick), sender, e);
        private void MoveLayerRuleDownClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLayerRuleDownClick), sender, e);
        private void LayerRulesGridDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(LayerRulesGridDoubleClick), sender, e);
    }
}
