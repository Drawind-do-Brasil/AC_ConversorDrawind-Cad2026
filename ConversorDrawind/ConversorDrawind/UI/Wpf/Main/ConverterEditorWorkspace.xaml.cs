using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void NewConverterClick(object sender, RoutedEventArgs e) => Forward(nameof(NewConverterClick), sender, e);
        private void SaveConverterClick(object sender, RoutedEventArgs e) => Forward(nameof(SaveConverterClick), sender, e);
        private void ImportConverterClick(object sender, RoutedEventArgs e) => Forward(nameof(ImportConverterClick), sender, e);
        private void ConverterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => Forward(nameof(ConverterComboBoxSelectionChanged), sender, e);
        private void ConfigurationTabsSelectionChanged(object sender, SelectionChangedEventArgs e) => Forward(nameof(ConfigurationTabsSelectionChanged), sender, e);
        private void FeatureCheckChanged(object sender, RoutedEventArgs e) => Forward(nameof(FeatureCheckChanged), sender, e);
        private void OriginChanged(object sender, RoutedEventArgs e) => Forward(nameof(OriginChanged), sender, e);
        private void LoadClientLayersClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadClientLayersClick), sender, e);
        private void ConfigureClientLayersClick(object sender, RoutedEventArgs e) => Forward(nameof(ConfigureClientLayersClick), sender, e);
        private void ConfigureTextStylesClick(object sender, RoutedEventArgs e) => Forward(nameof(ConfigureTextStylesClick), sender, e);
        private void AddLayerRuleClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLayerRuleClick), sender, e);
        private void DeleteLayerRuleClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteLayerRuleClick), sender, e);
        private void MoveLayerRuleUpClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLayerRuleUpClick), sender, e);
        private void MoveLayerRuleDownClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLayerRuleDownClick), sender, e);
        private void LayerRulesGridDoubleClick(object sender, MouseButtonEventArgs e) => Forward(nameof(LayerRulesGridDoubleClick), sender, e);
        private void DimensionArrowAdvancedClick(object sender, RoutedEventArgs e) => Forward(nameof(DimensionArrowAdvancedClick), sender, e);
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
