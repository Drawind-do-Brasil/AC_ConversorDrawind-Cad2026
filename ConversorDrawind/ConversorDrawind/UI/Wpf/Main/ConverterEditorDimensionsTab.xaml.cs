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
        public ComboBox LineColorComboBox => DimensionLineColorComboBox;
        public ComboBox TextColorComboBox => DimensionTextColorComboBox;
        public ComboBox ArrowTypeComboBox => DimensionArrowTypeComboBox;
        public ComboBox TextStyleComboBoxControl => TextStyleComboBox;
        public TextBox ScaleTextBox => DimensionScaleTextBox;
        public TextBox ArrowSizeTextBox => DimensionArrowSizeTextBox;
        public TextBox OffsetTextBox => DimensionOffsetTextBox;
        public TextBox LineExtTextBox => DimensionLineExtTextBox;
        public ComboBox LinearPrecisionComboBox => DimensionLinearPrecisionComboBox;
        public ComboBox AngularPrecisionComboBox => DimensionAngularPrecisionComboBox;
        public ComboBox LinearUnitComboBox => DimensionLinearUnitComboBox;
        public ComboBox AngularUnitComboBox => DimensionAngularUnitComboBox;
        public ComboBox OutsideAlignComboBox => DimensionOutsideAlignComboBox;
        public ComboBox LineForcedComboBox => DimensionLineForcedComboBox;
        public ComboBox TextInsideComboBox => DimensionTextInsideComboBox;
        public ComboBox TextAlignmentComboBox => DimensionTextAlignmentComboBox;
        public ComboBox TextPlacementComboBox => DimensionTextPlacementComboBox;
        public ComboBox BaseLayerComboBox => DimensionBaseLayerComboBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void DimensionArrowAdvancedClick(object sender, RoutedEventArgs e) => Forward(nameof(DimensionArrowAdvancedClick), sender, e);
        private void OtherLineColorClick(object sender, RoutedEventArgs e) => Forward(nameof(OtherLineColorClick), sender, e);
        private void OtherTextColorClick(object sender, RoutedEventArgs e) => Forward(nameof(OtherTextColorClick), sender, e);
    }
}
