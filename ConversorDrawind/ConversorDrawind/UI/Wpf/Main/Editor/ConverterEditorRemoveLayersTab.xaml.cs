using ConversorDrawind.UI.Wpf.Main.Rows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorRemoveLayersTab : UserControl
    {
        public ConverterEditorRemoveLayersTab()
        {
            InitializeComponent();
        }

        public ListBox LayerBaseListBox => RemoveLayerBaseListBox;
        public DataGrid LayersGrid => RemoveLayersGrid;

        public void ShowRemoveLayers(IEnumerable<string> layerNames, IEnumerable<LayerRemoveRule> rules)
        {
            RemoveLayerBaseListBox.ItemsSource = (layerNames ?? Enumerable.Empty<string>())
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(System.StringComparer.OrdinalIgnoreCase)
                .ToList();

            RemoveLayersGrid.ItemsSource = (rules ?? Enumerable.Empty<LayerRemoveRule>())
                .Select(rule => new RemoveLayerRow(rule))
                .ToList();
        }

        public void AddRemoveLayerRules(global::ConversorDrawind.Configuration configuration)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            foreach (string layer in RemoveLayerBaseListBox.SelectedItems.OfType<string>())
            {
                if (!configuration.Layers.RemoveRules
                    .Select(rule => new RemoveLayerRow(rule))
                    .Any(row => string.Equals(row.Layer, layer, System.StringComparison.OrdinalIgnoreCase)))
                {
                    configuration.Layers.RemoveRules.Add(CreateDefaultRemoveLayerRule(layer));
                }
            }
        }

        public void DeleteSelectedRemoveLayers(global::ConversorDrawind.Configuration configuration)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            List<RemoveLayerRow> rows = ItemsSourceList();
            List<int> selectedIndexes = RemoveLayersGrid.SelectedItems
                .OfType<RemoveLayerRow>()
                .Select(row => rows.IndexOf(row))
                .Where(index => index >= 0)
                .OrderByDescending(index => index)
                .ToList();

            foreach (int index in selectedIndexes)
            {
                configuration.Layers.RemoveRules.RemoveAt(index);
            }
        }

        public void EditRemoveLayer(global::ConversorDrawind.Configuration configuration)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            if (!(RemoveLayersGrid.SelectedItem is RemoveLayerRow row) || RemoveLayersGrid.CurrentColumn == null)
            {
                return;
            }

            int columnIndex = RemoveLayersGrid.Columns.IndexOf(RemoveLayersGrid.CurrentColumn);
            if (columnIndex != 1)
            {
                return;
            }

            int index = RemoveLayersGrid.SelectedIndex;
            if (index < 0 || index >= configuration.Layers.RemoveRules.Count)
            {
                return;
            }

            using (LayersFilter dialog = new LayersFilter(row.Rule.Filter, configuration))
            {
                dialog.DisableOrientacao();
                if (dialog.ShowDialog() == UiDialogResult.OK)
                {
                    configuration.Layers.RemoveRules[index].Filter = dialog.EntityFilter;
                    RemoveLayersGrid.SelectedIndex = index;
                }
            }
        }

        private List<RemoveLayerRow> ItemsSourceList()
        {
            return (RemoveLayersGrid.ItemsSource as IEnumerable<RemoveLayerRow>)?.ToList() ?? RemoveLayersGrid.Items.OfType<RemoveLayerRow>().ToList();
        }

        private static LayerRemoveRule CreateDefaultRemoveLayerRule(string layer)
        {
            return new LayerRemoveRule
            {
                Filter = new EntityFilter
                {
                    BaseLayer = layer,
                    ObjectType = "ALL",
                    Color = "ALL",
                    LineType = "ALL",
                    TextContent = string.Empty,
                    TextHeight = string.Empty,
                    Orientation = "ALL"
                }
            };
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardMouse(string action, object sender, MouseButtonEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddRemoveLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(AddRemoveLayerClick), sender, e);
        private void DeleteRemoveLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteRemoveLayerClick), sender, e);
        private void RemoveLayersGridDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(RemoveLayersGridDoubleClick), sender, e);
    }
}
