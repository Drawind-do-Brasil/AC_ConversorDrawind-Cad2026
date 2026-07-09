using System;
using System.Linq;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void RefreshExplodeLayerViews()
        {
            configuration.Layers.ExplodeLayers = configuration.Layers.ExplodeLayers
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            EditorView.ShowExplodeLayers(configuration.Layers.BaseLayers, configuration.Layers.ExplodeLayers);
        }

        private void AddExplodeLayer()
        {
            EditorView.AddExplodeLayers(configuration);
        }

        private void RemoveExplodeLayer()
        {
            EditorView.RemoveSelectedExplodeLayers(configuration);
        }

        private void MoveExplodeLayers(object sender, DragEventArgs e)
        {
            EditorView.MoveExplodeLayers(configuration, sender, e);
        }

    }
}
