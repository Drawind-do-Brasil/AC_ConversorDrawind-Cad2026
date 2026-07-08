using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConversorDrawind.UI.Wpf.Layers;
using ConversorDrawind.UI.Wpf.TextStyles;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorWorkspace : UserControl
    {
        public ConverterEditorWorkspace()
        {
            InitializeComponent();
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;
        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        public TextBox CommentsTextBox => MainTab.CommentsTextBox;
        public TextBox AddCommentsTextBox => MainTab.CommentsTextBox;
        public CheckBox ConvertDimensionsCheckBox => MainTab.ConvertDimensionsCheckBoxControl;
        public CheckBox ConvertLayersCheckBox => MainTab.ConvertLayersCheckBoxControl;
        public CheckBox ScaleDrawingCheckBox => MainTab.ScaleDrawingCheckBoxControl;
        public CheckBox AttributeFormatCheckBox => MainTab.AttributeFormatCheckBoxControl;
        public CheckBox RunCommandsCheckBox => MainTab.RunCommandsCheckBoxControl;
        public CheckBox ShowErrorMessagesCheckBox => MainTab.ShowErrorMessagesCheckBoxControl;
        public CheckBox DeleteTeklaCheckBox => MainTab.DeleteTeklaCheckBoxControl;
        public CheckBox PurgeCheckBox => MainTab.PurgeCheckBoxControl;
        public CheckBox ExplodeCheckBox => MainTab.ExplodeCheckBoxControl;
        public CheckBox ExplodeBlocksCheckBox => MainTab.ExplodeBlocksCheckBoxControl;
        public CheckBox DmBlockCheckBox => MainTab.DmBlockCheckBoxControl;
        public RadioButton TeklaOriginRadio => MainTab.TeklaOriginRadioControl;
        public RadioButton CadOriginRadio => MainTab.CadOriginRadioControl;
        public TextBox AttributedFormatPathTextBox => BlocksAndAttributesTab.AttributedFormatPathTextBoxControl;
        public TextBox CadBlocksPathTextBox => BlocksAndAttributesTab.CadBlocksPathTextBoxControl;
        public TextBox OriginalBlocksPathTextBox => BlocksAndAttributesTab.OriginalBlocksPathTextBoxControl;
        public ListBox TeklaBlocksListBox => BlocksAndAttributesTab.TeklaBlocksListBoxControl;
        public ListBox CadBlocksListBox => BlocksAndAttributesTab.CadBlocksListBoxControl;
        public ListBox OriginalBlocksListBox => BlocksAndAttributesTab.OriginalBlocksListBoxControl;
        public ListBox BlockRelationsListBox => BlocksAndAttributesTab.BlockRelationsListBoxControl;
        public Button RelateButton => BlocksAndAttributesTab.RelateButtonControl;
        public Button RemoveRelationButton => BlocksAndAttributesTab.RemoveRelationButtonControl;
        public Button EditRelationButton => BlocksAndAttributesTab.EditRelationButtonControl;
        public ComboBox TeklaTextLayerComboBox => ConvertersTab.TeklaLayerComboBox;
        public ComboBox FormatBlockLayerComboBox => ConvertersTab.FormatLayerComboBox;
        public NewLayersConfigurationControl ClientLayersControl => ClientLayersTab;
        public TextStyleConfigurationControl ClientTextStylesControl => ClientTextStylesTab;
        public ComboBox DimensionLayerComboBox => DimensionsTab.LayerComboBox;
        public TextBox DimensionStyleTextBox => DimensionsTab.StyleTextBox;
        public ComboBox DimensionLineColorComboBox => DimensionsTab.LineColorComboBox;
        public ComboBox DimensionTextColorComboBox => DimensionsTab.TextColorComboBox;
        public ComboBox DimensionArrowTypeComboBox => DimensionsTab.ArrowTypeComboBox;
        public ComboBox DimensionTextStyleComboBox => DimensionsTab.TextStyleComboBoxControl;
        public TextBox DimensionScaleTextBox => DimensionsTab.ScaleTextBox;
        public TextBox DimensionArrowSizeTextBox => DimensionsTab.ArrowSizeTextBox;
        public TextBox DimensionOffsetTextBox => DimensionsTab.OffsetTextBox;
        public TextBox DimensionLineExtTextBox => DimensionsTab.LineExtTextBox;
        public ComboBox DimensionLinearPrecisionComboBox => DimensionsTab.LinearPrecisionComboBox;
        public ComboBox DimensionAngularPrecisionComboBox => DimensionsTab.AngularPrecisionComboBox;
        public ComboBox DimensionLinearUnitComboBox => DimensionsTab.LinearUnitComboBox;
        public ComboBox DimensionAngularUnitComboBox => DimensionsTab.AngularUnitComboBox;
        public ComboBox DimensionOutsideAlignComboBox => DimensionsTab.OutsideAlignComboBox;
        public ComboBox DimensionLineForcedComboBox => DimensionsTab.LineForcedComboBox;
        public ComboBox DimensionTextInsideComboBox => DimensionsTab.TextInsideComboBox;
        public ComboBox DimensionTextAlignmentComboBox => DimensionsTab.TextAlignmentComboBox;
        public ComboBox DimensionTextPlacementComboBox => DimensionsTab.TextPlacementComboBox;
        public ComboBox DimensionBaseLayerComboBox => DimensionsTab.BaseLayerComboBox;
        public RadioButton ManualScaleRadio => ScaleTab.ManualRadio;
        public RadioButton AutoScaleRadio => ScaleTab.AutoRadio;
        public TextBox ScaleP1XTextBox => ScaleTab.P1XTextBoxControl;
        public TextBox ScaleP1YTextBox => ScaleTab.P1YTextBoxControl;
        public TextBox ScaleP1ZTextBox => ScaleTab.P1ZTextBoxControl;
        public TextBox ScaleP2XTextBox => ScaleTab.P2XTextBoxControl;
        public TextBox ScaleP2YTextBox => ScaleTab.P2YTextBoxControl;
        public TextBox ScaleP2ZTextBox => ScaleTab.P2ZTextBoxControl;
        public ComboBox ScaleLayerComboBox => ScaleTab.ZoomLayerFilterComboBoxControl;
        public TextBox ScaleTextSizeTextBox => ScaleTab.ZoomTextSizeTextBoxControl;
        public TextBox LayerLtScaleTextBox => ConvertersTab.ScaleTextBox;
        public DataGrid LayerRulesGrid => ConvertersTab.RulesGrid;
        public ListBox RemoveLayerBaseListBox => RemoveLayersTab.LayerBaseListBox;
        public DataGrid RemoveLayersGrid => RemoveLayersTab.LayersGrid;
        public ListBox AllExplodeLayersListBox => ExplosionTab.AllLayersListBox;
        public ListBox SelectedExplodeLayersListBox => ExplosionTab.SelectedLayersListBox;
        public ListBox LispCommandsListBox => LispDllTab.CommandsListBox;

        private void NewConverterClick(object sender, RoutedEventArgs e) => Forward(nameof(NewConverterClick), sender, e);
        private void SaveConverterClick(object sender, RoutedEventArgs e) => Forward(nameof(SaveConverterClick), sender, e);
        private void ImportConverterClick(object sender, RoutedEventArgs e) => Forward(nameof(ImportConverterClick), sender, e);
        private void ConverterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => Forward(nameof(ConverterComboBoxSelectionChanged), sender, e);
        private void ConfigurationTabsSelectionChanged(object sender, SelectionChangedEventArgs e) => Forward(nameof(ConfigurationTabsSelectionChanged), sender, e);
        private void FeatureCheckChanged(object sender, RoutedEventArgs e) => Forward(nameof(FeatureCheckChanged), sender, e);
        private void OriginChanged(object sender, RoutedEventArgs e) => Forward(nameof(OriginChanged), sender, e);
        private void AddLayerRuleClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLayerRuleClick), sender, e);
        private void DeleteLayerRuleClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteLayerRuleClick), sender, e);
        private void MoveLayerRuleUpClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLayerRuleUpClick), sender, e);
        private void MoveLayerRuleDownClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLayerRuleDownClick), sender, e);
        private void LayerRulesGridDoubleClick(object sender, MouseButtonEventArgs e) => Forward(nameof(LayerRulesGridDoubleClick), sender, e);
        private void DimensionArrowAdvancedClick(object sender, RoutedEventArgs e) => Forward(nameof(DimensionArrowAdvancedClick), sender, e);
        private void OtherLineColorClick(object sender, RoutedEventArgs e) => Forward(nameof(OtherLineColorClick), sender, e);
        private void OtherTextColorClick(object sender, RoutedEventArgs e) => Forward(nameof(OtherTextColorClick), sender, e);
        private void ScaleModeChanged(object sender, RoutedEventArgs e) => Forward(nameof(ScaleModeChanged), sender, e);
        private void AddRemoveLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(AddRemoveLayerClick), sender, e);
        private void DeleteRemoveLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteRemoveLayerClick), sender, e);
        private void RemoveLayersGridDoubleClick(object sender, MouseButtonEventArgs e) => Forward(nameof(RemoveLayersGridDoubleClick), sender, e);
        private void BrowseAttributedFormatClick(object sender, RoutedEventArgs e) => Forward(nameof(BrowseAttributedFormatClick), sender, e);
        private void EditBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockClick), sender, e);
        private void RemoveBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockClick), sender, e);
        private void LoadInventorBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadInventorBlocksClick), sender, e);
        private void LoadOriginalBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadOriginalBlocksClick), sender, e);
        private void BlockRelationsDoubleClick(object sender, MouseButtonEventArgs e) => Forward(nameof(BlockRelationsDoubleClick), sender, e);
        private void RelateBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(RelateBlocksClick), sender, e);
        private void EditBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockRelationClick), sender, e);
        private void RemoveBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockRelationClick), sender, e);
        private void AddLispClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLispClick), sender, e);
        private void AddDllClick(object sender, RoutedEventArgs e) => Forward(nameof(AddDllClick), sender, e);
        private void AddExplodeLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(AddExplodeLayerClick), sender, e);
        private void RemoveExplodeLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveExplodeLayerClick), sender, e);
    }
}
