using ConversorDrawind.UI.Wpf.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private IEnumerable<string> NewLayerNames()
        {
            return configuration.Layers.NewLayers.Select(layer => layer.Name);
        }

        private IEnumerable<string> AllConfiguredLayerNames()
        {
            return NewLayerNames().Concat(configuration.Layers.BaseLayers);
        }

        private void RefreshEditorOptions()
        {
            ConverterEditorViewModel.ReplaceOptions(viewModel.TeklaTextLayerOptions, PreferredLayerFirst("DRAWING SHEET"), configuration.Layers.TeklaDrawingSheetLayer);
            ConverterEditorViewModel.ReplaceOptions(viewModel.FormatBlockLayerOptions, PreferredLayerFirst("OTHER OBJECT TYPE"), configuration.Layers.BlockAttributeLayer);
            ConverterEditorViewModel.ReplaceOptions(viewModel.ScaleLayerOptions, AllConfiguredLayerNames(), configuration.Scale.Layer);
        }

        private void ClientLayersConfigurationChanged(object sender, EventArgs e)
        {
            RefreshLayerDependentViews();
        }

        private void ClientTextStylesConfigurationChanged(object sender, EventArgs e)
        {
            RefreshDimensionOptions();
        }

        private void RefreshLayerDependentViews()
        {
            RefreshEditorOptions();
            RefreshDimensionOptions();
            RefreshRemoveLayerViews();
        }

        private IEnumerable<string> PreferredLayerFirst(string preferredLayer)
        {
            yield return preferredLayer;
            foreach (string layer in configuration.Layers.BaseLayers.Where(layer => !string.Equals(layer, preferredLayer, StringComparison.OrdinalIgnoreCase)))
            {
                yield return layer;
            }
        }

        private void LoadClientLayers()
        {
            using (ConfigurarLayers dialog = new ConfigurarLayers(configuration))
            {
                dialog.OpenAcadLoadLayerExterno();
            }

            RefreshLayerDependentViews();
        }

        private void RefreshLayerRuleRows()
        {
            EditorView.ShowLayerRules(configuration.Layers.ConversionRules);
        }

        private void RefreshRemoveLayerViews()
        {
            EditorView.ShowRemoveLayers(AllConfiguredLayerNames(), configuration.Layers.RemoveRules);
        }

        private void AddLayerRule()
        {
            EditorView.AddLayerRule(configuration);
        }

        private void DeleteSelectedLayerRules()
        {
            EditorView.DeleteSelectedLayerRules(configuration);
        }

        private void MoveLayerRule(int direction)
        {
            EditorView.MoveLayerRule(configuration, direction);
        }

        private void EditLayerRuleCell()
        {
            EditorView.EditLayerRule(configuration);
        }

        private void AddRemoveLayerRule()
        {
            EditorView.AddRemoveLayerRules(configuration);
            RefreshRemoveLayerViews();
        }

        private void DeleteSelectedRemoveLayers()
        {
            EditorView.DeleteSelectedRemoveLayers(configuration);
            RefreshRemoveLayerViews();
        }

        private void EditRemoveLayerCell()
        {
            EditorView.EditRemoveLayer(configuration);
            RefreshRemoveLayerViews();
        }

    }
}
