using ConversorDrawind.UI.Wpf.Main.ViewModels;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow : Window
    {
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();
        private readonly ConverterEditorViewModel viewModel = new ConverterEditorViewModel();
        private GetInfo teklaDrawingBlock;
        private string teklaDrawingBlockPath = string.Empty;
        private GetInfo scaleDrawing;
        private string scaleDrawingPath = string.Empty;
        private bool isInitializing;
        private bool isSynchronizingConverterSelection;
        private string loadedConverterName = string.Empty;
        private StatusConversorItem loadedConverterStatus;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            InitializeUi();
        }

        public void InvokeUiAction(string action, object sender, RoutedEventArgs e)
        {
            switch (action)
            {
                case "AddDrawingsClick": AddDrawings(); break;
                case "ClearDrawingsClick": ConverterView.DrawingsListBox.Items.Clear(); break;
                case "RestoreLastConvertedClick": RestoreLastConverted(); break;
                case "StatusComboBoxSelectionChanged": if (!isInitializing) LoadConverterLists(); break;
                case "DrawingsDrop": DropDrawings((DragEventArgs)e); break;
                case "DrawingsDragEnter": e.Handled = true; break;
                case "ConvertersSelectionChanged": SelectConverterFromList(); break;
                case "ConverterComboBoxSelectionChanged": SelectConverterFromEditor(); break;
                case "ConvertClick": ConvertSelectedDrawings(); break;
                case "NewConverterClick": NewConverter(); break;
                case "SaveConverterClick": SaveConverter(); break;
                case "ImportConverterClick": ImportConverter(); break;
                case "LoadClientLayersClick": LoadClientLayers(); break;
                case "ConfigureClientLayersClick": using (ConfigurarLayers f = new ConfigurarLayers(configuration)) f.ShowDialog(); RefreshLayerDependentViews(); break;
                case "ConfigureTextStylesClick": using (ConfigurarTextStyle f = new ConfigurarTextStyle(configuration)) f.ShowDialog(); RefreshDimensionOptions(); break;
                case "DimensionArrowAdvancedClick": ConfigureAdvancedDimensionArrow(); break;
                case "OtherLineColorClick": AddOtherDimensionColor(EditorView.DimensionLineColorComboBox); break;
                case "OtherTextColorClick": AddOtherDimensionColor(EditorView.DimensionTextColorComboBox); break;
                case "SelectScalePointsClick": SelectScalePointsFromDrawing(); break;
                case "AddLayerRuleClick": AddLayerRule(); break;
                case "DeleteLayerRuleClick": DeleteSelectedLayerRules(); break;
                case "MoveLayerRuleUpClick": MoveLayerRule(-1); break;
                case "MoveLayerRuleDownClick": MoveLayerRule(1); break;
                case "LayerRulesGridDoubleClick": EditLayerRuleCell(); break;
                case "AddRemoveLayerClick": AddRemoveLayerRule(); break;
                case "DeleteRemoveLayerClick": DeleteSelectedRemoveLayers(); break;
                case "RemoveLayersGridDoubleClick": EditRemoveLayerCell(); break;
                case "AddExplodeLayerClick": AddExplodeLayer(); break;
                case "RemoveExplodeLayerClick": RemoveExplodeLayer(); break;
                case "ExplodeLayersDrop": MoveExplodeLayers(sender, (DragEventArgs)e); break;
                case "BrowseAttributedFormatClick": BrowseAttributedFormat(); break;
                case "EditBlockClick": EditTeklaBlock(); break;
                case "EditBlockDoubleClick": EditTeklaBlock(); break;
                case "RemoveBlockClick": RemoveTeklaBlock(); break;
                case "LoadInventorBlocksClick": LoadCadBlocks(); break;
                case "LoadOriginalBlocksClick": LoadOriginalBlocks(); break;
                case "CadBlocksSelectionChanged": UpdateRelationControls(); break;
                case "OriginalBlocksSelectionChanged": UpdateRelationControls(); break;
                case "BlockRelationsSelectionChanged": UpdateRelationControls(); break;
                case "BlockRelationsDoubleClick": EditBlockRelationParameters(); break;
                case "RelateBlocksClick": RelateSelectedBlocks(); break;
                case "EditBlockRelationClick": EditBlockRelationParameters(); break;
                case "RemoveBlockRelationClick": RemoveSelectedRelation(); break;
                case "LispListBoxSelectionChanged": break;
                case "LispListBoxDoubleClick": ModifyLispCommand(); break;
                case "AddLispClick": AddLispCommand(); break;
                case "ModifyLispClick": ModifyLispCommand(); break;
                case "DeleteLispClick": DeleteLispCommand(); break;
                case "MoveLispUpClick": MoveLispCommand(-1); break;
                case "MoveLispDownClick": MoveLispCommand(1); break;
            }
        }

        private StatusConversorItem CurrentStatus => viewModel.SelectedStatus ?? new StatusConversorItem(Localization.StatusActiveWorks, "TemplatesAtivos");

        private void InitializeUi()
        {
            isInitializing = true;
            EditorView.ClientLayersControl.ConfigurationChanged += ClientLayersConfigurationChanged;
            EditorView.ClientTextStylesControl.ConfigurationChanged += ClientTextStylesConfigurationChanged;
            StatusConversorItem[] statusItems = new[] { new StatusConversorItem(Localization.StatusActiveWorks, "TemplatesAtivos"), new StatusConversorItem(Localization.StatusInactiveWorks, "TemplatesInativos") };
            viewModel.SetStatusItems(statusItems);
            viewModel.Configuration = configuration;
            RefreshEditorFromConfiguration();
            RestoreInitialConverter(statusItems);
            isInitializing = false;
        }

    }
}

