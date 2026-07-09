using ConversorDrawind.UI.Wpf.Main.Rows;
using System.Collections.Generic;
using System.Linq;
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

        public void ShowLayerRules(IEnumerable<LayerConversionRule> rules)
        {
            LayerRulesGrid.ItemsSource = (rules ?? Enumerable.Empty<LayerConversionRule>())
                .Select(rule => new LayerRuleRow(rule))
                .ToList();
        }

        public void AddLayerRule(global::ConversorDrawind.Configuration configuration)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            int insertIndex = LayerRulesGrid.SelectedIndex >= 0 ? LayerRulesGrid.SelectedIndex : configuration.Layers.ConversionRules.Count;
            configuration.Layers.ConversionRules.Insert(insertIndex, CreateDefaultLayerConversionRule(configuration));
            ShowLayerRules(configuration.Layers.ConversionRules);
            LayerRulesGrid.SelectedIndex = insertIndex;
        }

        public void DeleteSelectedLayerRules(global::ConversorDrawind.Configuration configuration)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            List<LayerRuleRow> rows = ItemsSourceList();
            List<int> selectedIndexes = LayerRulesGrid.SelectedItems
                .OfType<LayerRuleRow>()
                .Select(row => rows.IndexOf(row))
                .Where(index => index >= 0)
                .OrderByDescending(index => index)
                .ToList();

            foreach (int index in selectedIndexes)
            {
                configuration.Layers.ConversionRules.RemoveAt(index);
            }

            ShowLayerRules(configuration.Layers.ConversionRules);
        }

        public void MoveLayerRule(global::ConversorDrawind.Configuration configuration, int direction)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            int index = LayerRulesGrid.SelectedIndex;
            int newIndex = index + direction;
            if (index < 0 || newIndex < 0 || newIndex >= configuration.Layers.ConversionRules.Count)
            {
                return;
            }

            LayerConversionRule item = configuration.Layers.ConversionRules[index];
            configuration.Layers.ConversionRules.RemoveAt(index);
            configuration.Layers.ConversionRules.Insert(newIndex, item);
            ShowLayerRules(configuration.Layers.ConversionRules);
            LayerRulesGrid.SelectedIndex = newIndex;
        }

        public void EditLayerRule(global::ConversorDrawind.Configuration configuration)
        {
            configuration = configuration ?? new global::ConversorDrawind.Configuration();
            if (!(LayerRulesGrid.SelectedItem is LayerRuleRow row) || LayerRulesGrid.CurrentColumn == null)
            {
                return;
            }

            int selectedIndex = LayerRulesGrid.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= configuration.Layers.ConversionRules.Count)
            {
                return;
            }

            int columnIndex = LayerRulesGrid.Columns.IndexOf(LayerRulesGrid.CurrentColumn);
            if (columnIndex == 0)
            {
                using (LayersLayerBase dialog = new LayersLayerBase(row.BaseLayer, configuration))
                {
                    if (dialog.ShowDialog() == UiDialogResult.OK)
                    {
                        row.Rule.Source.BaseLayer = dialog.layerBase;
                        ShowLayerRules(configuration.Layers.ConversionRules);
                        LayerRulesGrid.SelectedIndex = selectedIndex;
                    }
                }
            }
            else if (columnIndex == 1)
            {
                using (LayersFilter dialog = new LayersFilter(row.Rule.Source, configuration))
                {
                    if (dialog.ShowDialog() == UiDialogResult.OK)
                    {
                        configuration.Layers.ConversionRules[selectedIndex].Source = dialog.EntityFilter;
                        ShowLayerRules(configuration.Layers.ConversionRules);
                        LayerRulesGrid.SelectedIndex = selectedIndex;
                    }
                }
            }
            else if (columnIndex == 2)
            {
                using (LayersNewLayer dialog = new LayersNewLayer(row.Rule.Target, configuration))
                {
                    if (dialog.ShowDialog() == UiDialogResult.OK)
                    {
                        configuration.Layers.ConversionRules[selectedIndex].Target = dialog.LayerOutput;
                        ShowLayerRules(configuration.Layers.ConversionRules);
                        LayerRulesGrid.SelectedIndex = selectedIndex;
                    }
                }
            }
        }

        private List<LayerRuleRow> ItemsSourceList()
        {
            return (LayerRulesGrid.ItemsSource as IEnumerable<LayerRuleRow>)?.ToList() ?? LayerRulesGrid.Items.OfType<LayerRuleRow>().ToList();
        }

        private static LayerConversionRule CreateDefaultLayerConversionRule(global::ConversorDrawind.Configuration configuration)
        {
            return new LayerConversionRule
            {
                Source = new EntityFilter
                {
                    BaseLayer = string.Empty,
                    ObjectType = configuration.Catalogs.ObjectTypes.FirstOrDefault() ?? "ALL",
                    Color = configuration.Catalogs.Colors.FirstOrDefault() ?? "ALL",
                    LineType = configuration.Catalogs.FilterLineTypes.FirstOrDefault() ?? "ALL",
                    TextContent = string.Empty,
                    TextHeight = string.Empty,
                    Orientation = "ALL"
                },
                Target = new LayerOutput
                {
                    LayerName = "0",
                    Color = configuration.Catalogs.Colors.Skip(1).FirstOrDefault() ?? "BYLAYER",
                    LineType = configuration.Catalogs.LayerLineTypes.FirstOrDefault() ?? "BYLAYER",
                    TextContent = string.Empty,
                    TextHeight = string.Empty,
                    TextStyle = configuration.Text.Styles.FirstOrDefault()?.Name ?? configuration.Text.DefaultStyleName
                }
            };
        }

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
