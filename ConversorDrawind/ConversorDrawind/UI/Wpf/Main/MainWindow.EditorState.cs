namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void RefreshEditorFromConfiguration()
        {
            DisposeTeklaDrawingBlock();
            EditorView.ClientLayersControl.LoadConfiguration(configuration);
            EditorView.ClientTextStylesControl.LoadConfiguration(configuration);
            RefreshEditorOptions();
            RefreshLayerRuleRows();
            RefreshRemoveLayerViews();
            RefreshExplodeLayerViews();
            RefreshDimensionOptions();
            RefreshLispCommandRows();
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void ApplyEditorPendingChanges()
        {
            EditorView.ClientLayersControl.ApplyRowsToConfiguration(false);
            EditorView.ClientTextStylesControl.ApplyRowsToConfiguration();
        }

    }
}
