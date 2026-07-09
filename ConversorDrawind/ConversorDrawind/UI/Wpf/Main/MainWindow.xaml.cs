using ConversorDrawind.UI.Wpf.LispDll;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfMessageBox = System.Windows.MessageBox;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> drawings = new ObservableCollection<string>();
        private readonly ObservableCollection<string> converterNames = new ObservableCollection<string>();
        private readonly ObservableCollection<LispCommandRow> lispCommands = new ObservableCollection<LispCommandRow>();
        private readonly ObservableCollection<string> teklaBlockNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> cadBlockNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> originalBlockNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> blockRelations = new ObservableCollection<string>();
        private readonly ObservableCollection<LayerRuleRow> layerRuleRows = new ObservableCollection<LayerRuleRow>();
        private readonly ObservableCollection<RemoveLayerRow> removeLayerRows = new ObservableCollection<RemoveLayerRow>();
        private readonly ObservableCollection<string> removeLayerBaseNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> allExplodeLayerNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> selectedExplodeLayerNames = new ObservableCollection<string>();
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();
        private List<Block> listBlocks = new List<Block>();
        private List<Block> listBlocksInv = new List<Block>();
        private List<Block> listBlocksOrig = new List<Block>();
        private GetInfo teklaDrawingBlock;
        private string teklaDrawingBlockPath = string.Empty;
        private GetInfo scaleDrawing;
        private string scaleDrawingPath = string.Empty;
        private bool isInitializing;
        private bool isSynchronizingConverterSelection;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUi();
        }

        public void InvokeUiAction(string action, object sender, RoutedEventArgs e)
        {
            switch (action)
            {
                case "AddDrawingsClick": AddDrawings(); break;
                case "ClearDrawingsClick": drawings.Clear(); break;
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
                case "ConfigureTextStylesClick": using (ConfigurarTextStyle f = new ConfigurarTextStyle(configuration)) f.ShowDialog(); PopulateDimensionComboBoxes(); break;
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

        private StatusConversorItem CurrentStatus => ConverterView.StatusComboBox.SelectedItem as StatusConversorItem ?? new StatusConversorItem(Localization.StatusActiveWorks, "TemplatesAtivos");

        private void InitializeUi()
        {
            isInitializing = true;
            ConverterView.DrawingsListBox.ItemsSource = drawings;
            ConverterView.ConvertersListBox.ItemsSource = converterNames;
            EditorView.LispCommandsListBox.ItemsSource = lispCommands;
            EditorView.TeklaBlocksListBox.ItemsSource = teklaBlockNames;
            EditorView.CadBlocksListBox.ItemsSource = cadBlockNames;
            EditorView.OriginalBlocksListBox.ItemsSource = originalBlockNames;
            EditorView.BlockRelationsListBox.ItemsSource = blockRelations;
            EditorView.LayerRulesGrid.ItemsSource = layerRuleRows;
            EditorView.RemoveLayerBaseListBox.ItemsSource = removeLayerBaseNames;
            EditorView.RemoveLayersGrid.ItemsSource = removeLayerRows;
            EditorView.AllExplodeLayersListBox.ItemsSource = allExplodeLayerNames;
            EditorView.SelectedExplodeLayersListBox.ItemsSource = selectedExplodeLayerNames;
            EditorView.ClientLayersControl.ConfigurationChanged += ClientLayersConfigurationChanged;
            EditorView.ClientTextStylesControl.ConfigurationChanged += ClientTextStylesConfigurationChanged;
            ConverterView.ExtensionComboBox.ItemsSource = new[] { "DWG", "DXF" };
            ConverterView.ExtensionComboBox.SelectedItem = ApplicationRuntime.ExtensaoGeral;
            StatusConversorItem[] statusItems = new[] { new StatusConversorItem(Localization.StatusActiveWorks, "TemplatesAtivos"), new StatusConversorItem(Localization.StatusInactiveWorks, "TemplatesInativos") };
            ConverterView.StatusComboBox.ItemsSource = statusItems;
            ConverterView.StatusComboBox.SelectedIndex = 0;
            EditorView.ConverterComboBox.ItemsSource = converterNames;
            LoadConfigurationToControls();
            RestoreInitialConverter(statusItems);
            isInitializing = false;
        }

        private void LoadConverterLists()
        {
            converterNames.Clear();
            foreach (string name in ConverterFileService.ListConverterNames(CurrentStatus)) converterNames.Add(name);
        }

        private void RestoreInitialConverter(IEnumerable<StatusConversorItem> statusItems)
        {
            string converterName = string.Empty;
            if (UserSettingsService.TryReadLastConverter(out string statusFolder, out string lastConverterName))
            {
                StatusConversorItem savedStatus = statusItems.FirstOrDefault(status => string.Equals(status.Pasta, statusFolder, StringComparison.OrdinalIgnoreCase));
                if (savedStatus != null)
                {
                    ConverterView.StatusComboBox.SelectedItem = savedStatus;
                    converterName = lastConverterName;
                }
            }

            LoadConverterLists();

            if (string.IsNullOrWhiteSpace(converterName) || !converterNames.Contains(converterName))
            {
                converterName = converterNames.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(converterName))
            {
                return;
            }

            LoadConverter(converterName);
            SelectLoadedConverter(converterName);
        }

        private void AddDrawings()
        {
            OpenFileDialog dialog = new OpenFileDialog { Multiselect = true, Filter = Localization.FilterCadDrawings };
            if (dialog.ShowDialog(this) != true) return;
            foreach (string file in dialog.FileNames.Where(File.Exists)) if (!drawings.Contains(file)) drawings.Add(file);
        }

        private void DropDrawings(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop)) if (File.Exists(file) && !drawings.Contains(file)) drawings.Add(file);
        }

        private void RestoreLastConverted()
        {
            if (!File.Exists(ApplicationRuntime.LOGarqConvertidos)) { WpfMessageBox.Show(Localization.MessageNoPreviousConvertedFiles, Localization.AppTitle); return; }
            drawings.Clear();
            foreach (string file in File.ReadAllLines(ApplicationRuntime.LOGarqConvertidos).Where(File.Exists)) drawings.Add(file);
        }

        private void SelectConverterFromList()
        {
            if (!isInitializing && !isSynchronizingConverterSelection && ConverterView.ConvertersListBox.SelectedItem is string selected) { LoadConverter(selected); SelectLoadedConverter(selected); }
        }

        private void SelectConverterFromEditor()
        {
            if (!isInitializing && !isSynchronizingConverterSelection && EditorView.ConverterComboBox.SelectedItem is string selected) { LoadConverter(selected); SelectLoadedConverter(selected); }
        }

        private void LoadConverter(string converterName)
        {
            configuration = ConverterFileService.LoadConverter(converterName, CurrentStatus);
            listBlocks = new List<Block>();
            listBlocksInv = new List<Block>();
            listBlocksOrig = new List<Block>();
            ConfigurationCompatibilityMapper.ApplyBlocksToLegacyLists(configuration, listBlocks, listBlocksInv, listBlocksOrig);
            LoadConfigurationToControls();
            UserSettingsService.SaveLastConverter(CurrentStatus, converterName);
        }

        private void SelectLoadedConverter(string converterName)
        {
            isSynchronizingConverterSelection = true;
            try
            {
                ConverterView.ConvertersListBox.SelectedItem = converterName;
                EditorView.ConverterComboBox.SelectedItem = converterName;
                EditorView.ConverterNameTextBox.Text = converterName;
            }
            finally
            {
                isSynchronizingConverterSelection = false;
            }
        }

        private void NewConverter()
        {
            configuration = new global::ConversorDrawind.Configuration();
            listBlocks.Clear();
            listBlocksInv.Clear();
            listBlocksOrig.Clear();
            EditorView.ConverterNameTextBox.Text = string.Empty;
            LoadConfigurationToControls();
        }

        private void SaveConverter()
        {
            string name = EditorView.ConverterNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { WpfMessageBox.Show(Localization.MessageEnterConverterNameBeforeSave, Localization.AppTitle); return; }
            ReadConfigurationFromControls();
            ConfigurationCompatibilityMapper.ApplyBlocksFromLegacyLists(configuration, listBlocks, listBlocksInv, listBlocksOrig);
            ConverterFileService.SaveConverter(name, CurrentStatus, configuration);
            LoadConverterLists();
            SelectLoadedConverter(name);
            UserSettingsService.SaveLastConverter(CurrentStatus, name);
        }

        private void ImportConverter()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = Localization.FilterTemplateXml };
            if (dialog.ShowDialog(this) != true) return;
            configuration = ConverterFileService.LoadConverter(dialog.FileName, CurrentStatus);
            ConfigurationCompatibilityMapper.ApplyBlocksToLegacyLists(configuration, listBlocks, listBlocksInv, listBlocksOrig);
            EditorView.ConverterNameTextBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            LoadConfigurationToControls();
            UserSettingsService.SaveLastConverter(CurrentStatus, EditorView.ConverterNameTextBox.Text);
        }

        private void ConvertSelectedDrawings()
        {
            ReadConfigurationFromControls();
            if (!(ConverterView.ConvertersListBox.SelectedItem is string selectedConverter)) { WpfMessageBox.Show(Localization.MessageSelectConverterBeforeConvert, Localization.AppTitle); return; }
            if (drawings.Count == 0) { WpfMessageBox.Show(Localization.MessageAddAtLeastOneDrawingBeforeConvert, Localization.AppTitle); return; }
            ConversionPreflightResult preflight = ConversionPreflightValidator.ValidateFormatPath(configuration);
            if (!preflight.CanConvert) { WpfMessageBox.Show(Localization.FormatNotFoundMessage(preflight.MissingFormatPath), Localization.AppTitle); return; }
            ApplicationRuntime.ExtensaoGeral = Convert.ToString(ConverterView.ExtensionComboBox.Text);
            Param1 param = new Param1 { conversorName = selectedConverter, desenhosName = drawings.ToArray(), closedesenhos = ConverterView.KeepFilesOpenCheckBox.IsChecked != true, configuration = configuration, StatusConversorItem = CurrentStatus };
            using (Processo processo = new Processo(param)) processo.ShowDialog();
            if (!Processo.IsCanceled) using (ProcessoEnd processoEnd = new ProcessoEnd()) processoEnd.ShowDialog();
        }

        private void ConfigureAdvancedDimensionArrow()
        {
            using (ConfAvancadaDeCota f = new ConfAvancadaDeCota(configuration.Dimensions.FixArrow, configuration.Dimensions.FixArrowType, configuration.Dimensions.FixArrowFactor)) if (f.ShowDialog() == UiDialogResult.OK) { configuration.Dimensions.FixArrow = f.EXTDIMCorrigeSeta; configuration.Dimensions.FixArrowType = f.EXTDIMCorrigeSetaTipoSeta; configuration.Dimensions.FixArrowFactor = f.EXTDIMCorrigeSetaFactor; }
        }

        private void AddOtherDimensionColor(ComboBox targetComboBox)
        {
            using (GenericNewColor colorDialog = new GenericNewColor(targetComboBox.Text))
            {
                if (colorDialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                if (!configuration.Catalogs.Colors.Contains(colorDialog.colorClass))
                {
                    configuration.Catalogs.Colors.Add(colorDialog.colorClass);
                }

                PopulateDimensionComboBoxes();
                targetComboBox.Text = colorDialog.colorClass;
            }
        }

        private void PopulateDimensionComboBoxes()
        {
            SetComboItems(EditorView.DimensionLayerComboBox, NewLayerNames(), configuration.Dimensions.Layer);
            SetComboItems(EditorView.DimensionLineColorComboBox, configuration.Catalogs.Colors.Skip(1), configuration.Dimensions.LineColor);
            SetComboItems(EditorView.DimensionTextColorComboBox, configuration.Catalogs.Colors.Skip(1), configuration.Dimensions.TextColor);
            SetComboItems(EditorView.DimensionArrowTypeComboBox, DimensionArrowTypes(), configuration.Dimensions.ArrowType);
            SetComboItems(EditorView.DimensionTextStyleComboBox, TextStyleNames(), configuration.Text.DefaultStyleName);
            SetComboItems(EditorView.DimensionLinearPrecisionComboBox, Enumerable.Range(0, 9).Select(i => Convert.ToString(i)), Convert.ToString(configuration.Dimensions.Precision));
            SetComboItems(EditorView.DimensionAngularPrecisionComboBox, Enumerable.Range(0, 9).Select(i => Convert.ToString(i)), Convert.ToString(configuration.Dimensions.AngularPrecision));
            SetComboItems(EditorView.DimensionLinearUnitComboBox, Enumerable.Range(1, 6).Select(i => Convert.ToString(i)), Convert.ToString(configuration.Dimensions.Unit));
            SetComboItems(EditorView.DimensionAngularUnitComboBox, Enumerable.Range(1, 6).Select(i => Convert.ToString(i)), Convert.ToString(configuration.Dimensions.AngularUnit));
            SetComboItems(EditorView.DimensionOutsideAlignComboBox, BooleanOptions(), Convert.ToString(configuration.Dimensions.OutsideAlign));
            SetComboItems(EditorView.DimensionLineForcedComboBox, BooleanOptions(), Convert.ToString(configuration.Dimensions.ForceDimensionLine));
            SetComboItems(EditorView.DimensionTextInsideComboBox, BooleanOptions(), Convert.ToString(configuration.Dimensions.ForceTextInside));
            SetComboItems(EditorView.DimensionTextAlignmentComboBox, BooleanOptions(), Convert.ToString(configuration.Dimensions.TextRelativeToDimensionLine));
            SetComboItems(EditorView.DimensionTextPlacementComboBox, Enumerable.Range(0, 5).Select(i => Convert.ToString(i)), Convert.ToString(configuration.Dimensions.TextVerticalPosition));
            SetComboItems(EditorView.DimensionBaseLayerComboBox, DimensionBaseLayers(), configuration.Dimensions.BaseLayer);
        }

        private void SetComboItems(ComboBox comboBox, IEnumerable<string> values, string selectedValue)
        {
            List<string> items = values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!string.IsNullOrWhiteSpace(selectedValue) && !items.Contains(selectedValue))
            {
                items.Add(selectedValue);
            }

            comboBox.ItemsSource = items;
            comboBox.Text = selectedValue ?? string.Empty;
        }

        private IEnumerable<string> TextStyleNames()
        {
            configuration.EnsureDefaults();
            return configuration.Text.Styles.Select(style => style.Name);
        }

        private IEnumerable<string> DimensionBaseLayers()
        {
            yield return "DIMENSION";
            foreach (string layer in configuration.Layers.BaseLayers.Where(layer => !string.Equals(layer, "DIMENSION", StringComparison.OrdinalIgnoreCase)))
            {
                yield return layer;
            }
        }

        private IEnumerable<string> NewLayerNames()
        {
            return configuration.Layers.NewLayers.Select(layer => layer.Name);
        }

        private IEnumerable<string> AllConfiguredLayerNames()
        {
            return NewLayerNames().Concat(configuration.Layers.BaseLayers);
        }

        private IEnumerable<string> BooleanOptions()
        {
            return new[] { "True", "False" };
        }

        private IEnumerable<string> DimensionArrowTypes()
        {
            return new[]
            {
                "Architectural Tick",
                "Box",
                "Box Filled",
                "Closed",
                "Closed Blank",
                "Closed Filled",
                "Datum Triangle",
                "Datum Triangle Filled",
                "Dot",
                "Dot Blank",
                "Dot Small",
                "Integral",
                "None",
                "Oblique",
                "Open",
                "Origin Indicator",
                "Origin Indicator 2",
                "Right Angle"
            };
        }

        private static double ReadDouble(string text, double fallback)
        {
            try
            {
                return Convert.ToDouble((text ?? string.Empty).Replace('.', ','));
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        private static int ReadInt(string text, int fallback)
        {
            try
            {
                return Convert.ToInt32(text);
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        private static bool ReadBool(string text, bool fallback)
        {
            return bool.TryParse(text, out bool result) ? result : fallback;
        }

        private void PopulateEditorComboBoxes()
        {
            SetComboItems(EditorView.TeklaTextLayerComboBox, PreferredLayerFirst("DRAWING SHEET"), configuration.Layers.TeklaDrawingSheetLayer);
            SetComboItems(EditorView.FormatBlockLayerComboBox, PreferredLayerFirst("OTHER OBJECT TYPE"), configuration.Layers.BlockAttributeLayer);
            SetComboItems(EditorView.ScaleLayerComboBox, AllConfiguredLayerNames(), configuration.Scale.Layer);
        }

        private void ClientLayersConfigurationChanged(object sender, EventArgs e)
        {
            RefreshLayerDependentViews();
        }

        private void ClientTextStylesConfigurationChanged(object sender, EventArgs e)
        {
            PopulateDimensionComboBoxes();
        }

        private void RefreshLayerDependentViews()
        {
            PopulateEditorComboBoxes();
            PopulateDimensionComboBoxes();
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
            layerRuleRows.Clear();
            foreach (LayerConversionRule rule in configuration.Layers.ConversionRules)
            {
                string item = LegacyConfigurationParsers.FormatLayerConversionRule(rule);
                string[] parts = item.Split(new[] { ';' }, 3);
                if (parts.Length == 3)
                {
                    layerRuleRows.Add(new LayerRuleRow(parts[0], parts[1], parts[2]));
                }
            }
        }

        private void SaveLayerRuleRowsToConfiguration()
        {
            configuration.Layers.ConversionRules.Clear();
            foreach (LayerRuleRow row in layerRuleRows)
            {
                configuration.Layers.ConversionRules.Add(
                    LegacyConfigurationParsers.ParseLayerConversionRule((row.BaseLayer ?? string.Empty) + ";" + (row.Filter ?? string.Empty) + ";" + (row.NewLayer ?? string.Empty)));
            }
        }

        private void RefreshRemoveLayerViews()
        {
            removeLayerBaseNames.Clear();
            foreach (string layer in AllConfiguredLayerNames().Distinct(StringComparer.OrdinalIgnoreCase))
            {
                removeLayerBaseNames.Add(layer);
            }

            removeLayerRows.Clear();
            foreach (LayerRemoveRule rule in configuration.Layers.RemoveRules)
            {
                string formatted = LegacyConfigurationParsers.FormatLayerRemoveRule(rule);
                string[] parts = formatted.Split(new[] { ';' }, 2);
                if (parts.Length == 2)
                {
                    removeLayerRows.Add(new RemoveLayerRow(parts[0], parts[1]));
                }
            }
        }

        private void SaveRemoveLayerRowsToConfiguration()
        {
            configuration.Layers.RemoveRules.Clear();
            foreach (RemoveLayerRow row in removeLayerRows)
            {
                configuration.Layers.RemoveRules.Add(
                    LegacyConfigurationParsers.ParseLayerRemoveRule((row.Layer ?? string.Empty) + ";" + (row.Filter ?? string.Empty)));
            }
        }

        private void RefreshExplodeLayerViews()
        {
            allExplodeLayerNames.Clear();
            foreach (string layer in configuration.Layers.BaseLayers.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                allExplodeLayerNames.Add(layer);
            }

            selectedExplodeLayerNames.Clear();
            foreach (string layer in configuration.Layers.ExplodeLayers.Where(layer => !string.IsNullOrWhiteSpace(layer)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                selectedExplodeLayerNames.Add(layer);
            }
        }

        private void SaveExplodeLayerRowsToConfiguration()
        {
            configuration.Layers.ExplodeLayers = selectedExplodeLayerNames
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void AddLayerRule()
        {
            Filter filter = new Filter(configuration.Catalogs);
            filter.SetConjunto();
            NewLayer newLayer = new NewLayer(configuration);
            newLayer.SetConjunto();

            int insertIndex = EditorView.LayerRulesGrid.SelectedIndex >= 0 ? EditorView.LayerRulesGrid.SelectedIndex : layerRuleRows.Count;
            layerRuleRows.Insert(insertIndex, new LayerRuleRow(string.Empty, filter.GetConjunto(), newLayer.GetConjunto()));
            EditorView.LayerRulesGrid.SelectedIndex = insertIndex;
        }

        private void DeleteSelectedLayerRules()
        {
            List<LayerRuleRow> selectedRows = EditorView.LayerRulesGrid.SelectedItems.OfType<LayerRuleRow>().ToList();
            foreach (LayerRuleRow row in selectedRows)
            {
                layerRuleRows.Remove(row);
            }
        }

        private void MoveLayerRule(int direction)
        {
            if (!(EditorView.LayerRulesGrid.SelectedItem is LayerRuleRow selected))
            {
                return;
            }

            int index = layerRuleRows.IndexOf(selected);
            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= layerRuleRows.Count)
            {
                return;
            }

            layerRuleRows.Move(index, newIndex);
            EditorView.LayerRulesGrid.SelectedIndex = newIndex;
        }

        private void EditLayerRuleCell()
        {
            if (!(EditorView.LayerRulesGrid.SelectedItem is LayerRuleRow row) || EditorView.LayerRulesGrid.CurrentColumn == null)
            {
                return;
            }

            int columnIndex = EditorView.LayerRulesGrid.Columns.IndexOf(EditorView.LayerRulesGrid.CurrentColumn);
            if (columnIndex == 0)
            {
                using (LayersLayerBase dialog = new LayersLayerBase(row.BaseLayer, configuration))
                {
                    if (dialog.ShowDialog() == UiDialogResult.OK)
                    {
                        row.BaseLayer = dialog.layerBase;
                        EditorView.LayerRulesGrid.Items.Refresh();
                    }
                }
            }
            else if (columnIndex == 1)
            {
                using (LayersFilter dialog = new LayersFilter(row.Filter, configuration))
                {
                    if (dialog.ShowDialog() == UiDialogResult.OK)
                    {
                        row.Filter = dialog.filtro.GetConjunto();
                        EditorView.LayerRulesGrid.Items.Refresh();
                    }
                }
            }
            else if (columnIndex == 2)
            {
                using (LayersNewLayer dialog = new LayersNewLayer(row.NewLayer, configuration))
                {
                    if (dialog.ShowDialog() == UiDialogResult.OK)
                    {
                        row.NewLayer = dialog.novoLayer.GetConjunto();
                        EditorView.LayerRulesGrid.Items.Refresh();
                    }
                }
            }
        }

        private void AddRemoveLayerRule()
        {
            Filter filter = new Filter(configuration.Catalogs);
            filter.SetConjunto2();
            foreach (string layer in EditorView.RemoveLayerBaseListBox.SelectedItems.OfType<string>())
            {
                if (!removeLayerRows.Any(row => string.Equals(row.Layer, layer, StringComparison.OrdinalIgnoreCase)))
                {
                    removeLayerRows.Add(new RemoveLayerRow(layer, filter.GetConjunto()));
                }
            }
        }

        private void DeleteSelectedRemoveLayers()
        {
            List<RemoveLayerRow> selectedRows = EditorView.RemoveLayersGrid.SelectedItems.OfType<RemoveLayerRow>().ToList();
            foreach (RemoveLayerRow row in selectedRows)
            {
                removeLayerRows.Remove(row);
            }
        }

        private void EditRemoveLayerCell()
        {
            if (!(EditorView.RemoveLayersGrid.SelectedItem is RemoveLayerRow row) || EditorView.RemoveLayersGrid.CurrentColumn == null)
            {
                return;
            }

            int columnIndex = EditorView.RemoveLayersGrid.Columns.IndexOf(EditorView.RemoveLayersGrid.CurrentColumn);
            if (columnIndex != 1)
            {
                return;
            }

            using (LayersFilter dialog = new LayersFilter(row.Filter, configuration))
            {
                dialog.DisableOrientacao();
                if (dialog.ShowDialog() == UiDialogResult.OK)
                {
                    row.Filter = dialog.filtro.GetConjunto();
                    EditorView.RemoveLayersGrid.Items.Refresh();
                }
            }
        }

        private void AddExplodeLayer()
        {
            foreach (string layer in EditorView.AllExplodeLayersListBox.SelectedItems.OfType<string>())
            {
                if (!selectedExplodeLayerNames.Contains(layer))
                {
                    selectedExplodeLayerNames.Add(layer);
                }
            }
        }

        private void RemoveExplodeLayer()
        {
            List<string> selectedLayers = EditorView.SelectedExplodeLayersListBox.SelectedItems.OfType<string>().ToList();
            foreach (string layer in selectedLayers)
            {
                selectedExplodeLayerNames.Remove(layer);
            }
        }

        private void MoveExplodeLayers(object sender, DragEventArgs e)
        {
            const string layerFormat = "ConversorDrawind.ExplodeLayerNames";
            const string sourceFormat = "ConversorDrawind.ExplodeLayerSource";

            if (!(sender is ListBox targetListBox) || !e.Data.GetDataPresent(layerFormat))
            {
                return;
            }

            string sourceName = e.Data.GetDataPresent(sourceFormat) ? e.Data.GetData(sourceFormat) as string : string.Empty;
            if (sourceName == targetListBox.Name)
            {
                return;
            }

            List<string> layers = ((string[])e.Data.GetData(layerFormat)).ToList();
            if (targetListBox == EditorView.SelectedExplodeLayersListBox)
            {
                foreach (string layer in layers)
                {
                    if (!selectedExplodeLayerNames.Contains(layer))
                    {
                        selectedExplodeLayerNames.Add(layer);
                    }
                }
            }
            else if (targetListBox == EditorView.AllExplodeLayersListBox)
            {
                foreach (string layer in layers)
                {
                    selectedExplodeLayerNames.Remove(layer);
                }
            }

            e.Handled = true;
        }

        private void LoadConfigurationToControls()
        {
            DisposeTeklaDrawingBlock();
            ConverterView.TemplateCommentsTextBox.Text = configuration.Comments ?? string.Empty;
            EditorView.AddCommentsTextBox.Text = configuration.Comments ?? string.Empty;
            EditorView.ConvertDimensionsCheckBox.IsChecked = configuration.General.ConvertDimensions;
            EditorView.ConvertLayersCheckBox.IsChecked = configuration.General.ConvertLayers;
            EditorView.ScaleDrawingCheckBox.IsChecked = configuration.General.ApplyDrawingScale;
            EditorView.AttributeFormatCheckBox.IsChecked = configuration.General.ExchangeFormat;
            EditorView.RunCommandsCheckBox.IsChecked = configuration.General.ExecuteLisp;
            EditorView.ShowErrorMessagesCheckBox.IsChecked = configuration.General.ShowMessages;
            EditorView.DeleteTeklaCheckBox.IsChecked = configuration.General.DeleteTeklaStructures;
            EditorView.PurgeCheckBox.IsChecked = configuration.General.Purge;
            EditorView.ExplodeCheckBox.IsChecked = configuration.General.InventorExplode;
            EditorView.ExplodeBlocksCheckBox.IsChecked = configuration.General.ExplodeBlocks;
            EditorView.DmBlockCheckBox.IsChecked = configuration.Blocks.DimensionBlockEnabled;
            EditorView.TeklaOriginRadio.IsChecked = configuration.General.SourceMode == 0;
            EditorView.CadOriginRadio.IsChecked = configuration.General.SourceMode != 0;
            EditorView.AttributedFormatPathTextBox.Text = configuration.Blocks.TeklaBlockPath;
            EditorView.CadBlocksPathTextBox.Text = configuration.Blocks.CadBlockPath;
            EditorView.OriginalBlocksPathTextBox.Text = configuration.Blocks.CadBlockPath;
            EditorView.ClientLayersControl.LoadConfiguration(configuration);
            EditorView.ClientTextStylesControl.LoadConfiguration(configuration);
            PopulateEditorComboBoxes();
            RefreshLayerRuleRows();
            RefreshRemoveLayerViews();
            RefreshExplodeLayerViews();
            EditorView.TeklaTextLayerComboBox.Text = configuration.Layers.TeklaDrawingSheetLayer;
            EditorView.FormatBlockLayerComboBox.Text = configuration.Layers.BlockAttributeLayer;
            EditorView.LayerLtScaleTextBox.Text = Convert.ToString(configuration.Lines.LineTypeScale);
            PopulateDimensionComboBoxes();
            EditorView.DimensionLayerComboBox.Text = configuration.Dimensions.Layer;
            EditorView.DimensionStyleTextBox.Text = configuration.Dimensions.StyleName;
            EditorView.DimensionLineColorComboBox.Text = configuration.Dimensions.LineColor;
            EditorView.DimensionTextColorComboBox.Text = configuration.Dimensions.TextColor;
            EditorView.DimensionArrowTypeComboBox.Text = configuration.Dimensions.ArrowType;
            EditorView.DimensionTextStyleComboBox.Text = configuration.Text.DefaultStyleName;
            EditorView.DimensionScaleTextBox.Text = Convert.ToString(configuration.Dimensions.Scale);
            EditorView.DimensionArrowSizeTextBox.Text = Convert.ToString(configuration.Dimensions.ArrowSize);
            EditorView.DimensionOffsetTextBox.Text = Convert.ToString(configuration.Dimensions.OffsetLineFromReferencePoint);
            EditorView.DimensionLineExtTextBox.Text = Convert.ToString(configuration.Dimensions.ExtensionLineOffset);
            EditorView.DimensionLinearPrecisionComboBox.Text = Convert.ToString(configuration.Dimensions.Precision);
            EditorView.DimensionAngularPrecisionComboBox.Text = Convert.ToString(configuration.Dimensions.AngularPrecision);
            EditorView.DimensionLinearUnitComboBox.Text = Convert.ToString(configuration.Dimensions.Unit);
            EditorView.DimensionAngularUnitComboBox.Text = Convert.ToString(configuration.Dimensions.AngularUnit);
            EditorView.DimensionOutsideAlignComboBox.Text = Convert.ToString(configuration.Dimensions.OutsideAlign);
            EditorView.DimensionLineForcedComboBox.Text = Convert.ToString(configuration.Dimensions.ForceDimensionLine);
            EditorView.DimensionTextInsideComboBox.Text = Convert.ToString(configuration.Dimensions.ForceTextInside);
            EditorView.DimensionTextAlignmentComboBox.Text = Convert.ToString(configuration.Dimensions.TextRelativeToDimensionLine);
            EditorView.DimensionTextPlacementComboBox.Text = Convert.ToString(configuration.Dimensions.TextVerticalPosition);
            EditorView.DimensionBaseLayerComboBox.Text = configuration.Dimensions.BaseLayer;
            EditorView.ManualScaleRadio.IsChecked = configuration.Scale.Manual;
            EditorView.AutoScaleRadio.IsChecked = !configuration.Scale.Manual;
            EditorView.ScaleP1XTextBox.Text = Convert.ToString(configuration.Scale.Point1.X);
            EditorView.ScaleP1YTextBox.Text = Convert.ToString(configuration.Scale.Point1.Y);
            EditorView.ScaleP1ZTextBox.Text = Convert.ToString(configuration.Scale.Point1.Z);
            EditorView.ScaleP2XTextBox.Text = Convert.ToString(configuration.Scale.Point2.X);
            EditorView.ScaleP2YTextBox.Text = Convert.ToString(configuration.Scale.Point2.Y);
            EditorView.ScaleP2ZTextBox.Text = Convert.ToString(configuration.Scale.Point2.Z);
            EditorView.ScaleLayerComboBox.Text = configuration.Scale.Layer;
            EditorView.ScaleTextSizeTextBox.Text = Convert.ToString(configuration.Scale.TextSize);
            lispCommands.Clear();
            foreach (string command in configuration.Commands.LispCommands)
            {
                lispCommands.Add(LispCommandRow.FromCommandEntry(command));
            }
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void ReadConfigurationFromControls()
        {
            configuration.Comments = EditorView.AddCommentsTextBox.Text;
            configuration.General.ConvertDimensions = EditorView.ConvertDimensionsCheckBox.IsChecked == true;
            configuration.General.ConvertLayers = EditorView.ConvertLayersCheckBox.IsChecked == true;
            configuration.General.ApplyDrawingScale = EditorView.ScaleDrawingCheckBox.IsChecked == true;
            configuration.General.ExchangeFormat = EditorView.AttributeFormatCheckBox.IsChecked == true;
            configuration.General.ExecuteLisp = EditorView.RunCommandsCheckBox.IsChecked == true;
            configuration.General.ShowMessages = EditorView.ShowErrorMessagesCheckBox.IsChecked == true;
            configuration.General.DeleteTeklaStructures = EditorView.DeleteTeklaCheckBox.IsChecked == true;
            configuration.General.Purge = EditorView.PurgeCheckBox.IsChecked == true;
            configuration.General.InventorExplode = EditorView.ExplodeCheckBox.IsChecked == true;
            configuration.General.ExplodeBlocks = EditorView.ExplodeBlocksCheckBox.IsChecked == true;
            configuration.Blocks.DimensionBlockEnabled = EditorView.DmBlockCheckBox.IsChecked == true;
            configuration.General.SourceMode = EditorView.TeklaOriginRadio.IsChecked == true ? 0 : 1;
            configuration.Blocks.TeklaBlockPath = EditorView.AttributedFormatPathTextBox.Text;
            configuration.Blocks.CadBlockPath = string.IsNullOrWhiteSpace(EditorView.OriginalBlocksPathTextBox.Text)
                ? EditorView.CadBlocksPathTextBox.Text
                : EditorView.OriginalBlocksPathTextBox.Text;
            configuration.Layers.TeklaDrawingSheetLayer = EditorView.TeklaTextLayerComboBox.Text;
            configuration.Layers.BlockAttributeLayer = EditorView.FormatBlockLayerComboBox.Text;
            configuration.Lines.LineTypeScale = ReadDouble(EditorView.LayerLtScaleTextBox.Text, configuration.Lines.LineTypeScale);
            configuration.Dimensions.Layer = EditorView.DimensionLayerComboBox.Text;
            configuration.Dimensions.StyleName = EditorView.DimensionStyleTextBox.Text;
            configuration.Dimensions.LineColor = EditorView.DimensionLineColorComboBox.Text;
            configuration.Dimensions.TextColor = EditorView.DimensionTextColorComboBox.Text;
            configuration.Dimensions.ArrowType = EditorView.DimensionArrowTypeComboBox.Text;
            configuration.Text.DefaultStyleName = EditorView.DimensionTextStyleComboBox.Text;
            configuration.Dimensions.Scale = ReadDouble(EditorView.DimensionScaleTextBox.Text, configuration.Dimensions.Scale);
            configuration.Dimensions.ArrowSize = ReadDouble(EditorView.DimensionArrowSizeTextBox.Text, configuration.Dimensions.ArrowSize);
            configuration.Dimensions.OffsetLineFromReferencePoint = ReadDouble(EditorView.DimensionOffsetTextBox.Text, configuration.Dimensions.OffsetLineFromReferencePoint);
            configuration.Dimensions.ExtensionLineOffset = ReadDouble(EditorView.DimensionLineExtTextBox.Text, configuration.Dimensions.ExtensionLineOffset);
            configuration.Dimensions.Precision = ReadInt(EditorView.DimensionLinearPrecisionComboBox.Text, configuration.Dimensions.Precision);
            configuration.Dimensions.AngularPrecision = ReadInt(EditorView.DimensionAngularPrecisionComboBox.Text, configuration.Dimensions.AngularPrecision);
            configuration.Dimensions.Unit = ReadInt(EditorView.DimensionLinearUnitComboBox.Text, configuration.Dimensions.Unit);
            configuration.Dimensions.AngularUnit = ReadInt(EditorView.DimensionAngularUnitComboBox.Text, configuration.Dimensions.AngularUnit);
            configuration.Dimensions.OutsideAlign = ReadBool(EditorView.DimensionOutsideAlignComboBox.Text, configuration.Dimensions.OutsideAlign);
            configuration.Dimensions.ForceDimensionLine = ReadBool(EditorView.DimensionLineForcedComboBox.Text, configuration.Dimensions.ForceDimensionLine);
            configuration.Dimensions.ForceTextInside = ReadBool(EditorView.DimensionTextInsideComboBox.Text, configuration.Dimensions.ForceTextInside);
            configuration.Dimensions.TextRelativeToDimensionLine = ReadBool(EditorView.DimensionTextAlignmentComboBox.Text, configuration.Dimensions.TextRelativeToDimensionLine);
            configuration.Dimensions.TextVerticalPosition = ReadInt(EditorView.DimensionTextPlacementComboBox.Text, configuration.Dimensions.TextVerticalPosition);
            configuration.Dimensions.BaseLayer = EditorView.DimensionBaseLayerComboBox.Text;
            configuration.Scale.Manual = EditorView.ManualScaleRadio.IsChecked == true;
            configuration.Scale.Point1.X = ReadDouble(EditorView.ScaleP1XTextBox.Text, configuration.Scale.Point1.X);
            configuration.Scale.Point1.Y = ReadDouble(EditorView.ScaleP1YTextBox.Text, configuration.Scale.Point1.Y);
            configuration.Scale.Point1.Z = ReadDouble(EditorView.ScaleP1ZTextBox.Text, configuration.Scale.Point1.Z);
            configuration.Scale.Point2.X = ReadDouble(EditorView.ScaleP2XTextBox.Text, configuration.Scale.Point2.X);
            configuration.Scale.Point2.Y = ReadDouble(EditorView.ScaleP2YTextBox.Text, configuration.Scale.Point2.Y);
            configuration.Scale.Point2.Z = ReadDouble(EditorView.ScaleP2ZTextBox.Text, configuration.Scale.Point2.Z);
            configuration.Scale.Layer = EditorView.ScaleLayerComboBox.Text;
            configuration.Scale.TextSize = ReadDouble(EditorView.ScaleTextSizeTextBox.Text, configuration.Scale.TextSize);
            EditorView.ClientLayersControl.ApplyRowsToConfiguration(false);
            EditorView.ClientTextStylesControl.ApplyRowsToConfiguration();
            SaveLayerRuleRowsToConfiguration();
            SaveRemoveLayerRowsToConfiguration();
            SaveExplodeLayerRowsToConfiguration();
            configuration.Commands.LispCommands = lispCommands.Select(command => command.ToCommandEntry()).ToList();
        }

        private void RefreshBlockViews()
        {
            RefreshTeklaBlocksView();
            RefreshCadBlocksView();
            RefreshOriginalBlocksView();
            RefreshRelationView();
        }

        private void RefreshTeklaBlocksView()
        {
            teklaBlockNames.Clear();
            foreach (Block block in listBlocks)
            {
                teklaBlockNames.Add(block.blockName);
            }
        }

        private void RefreshCadBlocksView()
        {
            cadBlockNames.Clear();
            foreach (Block block in listBlocksInv)
            {
                cadBlockNames.Add(block.blockName);
            }
        }

        private void RefreshOriginalBlocksView()
        {
            originalBlockNames.Clear();
            foreach (Block block in listBlocksOrig)
            {
                originalBlockNames.Add(block.blockName);
            }
        }

        private void RefreshRelationView()
        {
            blockRelations.Clear();
            foreach (Block original in listBlocksOrig)
            {
                if (string.IsNullOrWhiteSpace(original.blockNameRelacao))
                {
                    continue;
                }

                blockRelations.Add(original.blockNameRelacao + "    = >    " + original.blockName);
            }
        }

        private void BrowseAttributedFormat()
        {
            string fileName = BrowseDrawingFile(EditorView.AttributedFormatPathTextBox.Text);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            EditorView.AttributedFormatPathTextBox.Text = fileName;
            LoadTeklaBlocks(fileName);
        }

        private void LoadTeklaBlocks()
        {
            LoadTeklaBlocks(EditorView.AttributedFormatPathTextBox.Text);
        }

        private void LoadTeklaBlocks(string filePath)
        {
            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                listBlocks.Clear();
                teklaDrawingBlockPath = string.Empty;
                RefreshTeklaBlocksView();
                UpdateRelationControls();
                return;
            }

            DisposeTeklaDrawingBlock();
            teklaDrawingBlock = drawingBlock;
            teklaDrawingBlockPath = filePath;
            listBlocks = DeduplicateBlocks(drawingBlock.GetListBlocks());
            RefreshTeklaBlocksView();
            UpdateRelationControls();
        }

        private void LoadCadBlocks()
        {
            string fileName = BrowseDrawingFile(EditorView.CadBlocksPathTextBox.Text);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            EditorView.CadBlocksPathTextBox.Text = fileName;
            LoadCadBlocks(fileName);
        }

        private void LoadCadBlocks(string filePath)
        {
            List<Block> blocks = LoadBlockListFromPath(filePath);
            if (blocks == null)
            {
                return;
            }

            listBlocksInv = blocks;
            ResetBlockRelationsState();
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void LoadOriginalBlocks()
        {
            string fileName = BrowseDrawingFile(EditorView.OriginalBlocksPathTextBox.Text);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            EditorView.OriginalBlocksPathTextBox.Text = fileName;
            LoadOriginalBlocks(fileName);
        }

        private void LoadOriginalBlocks(string filePath)
        {
            List<Block> blocks = LoadBlockListFromPath(filePath);
            if (blocks == null)
            {
                return;
            }

            listBlocksOrig = blocks;
            ResetBlockRelationsState();
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private string BrowseDrawingFile(string currentPath)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = Localization.FilterCadDrawings,
                Multiselect = false
            };

            if (File.Exists(currentPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(currentPath);
                dialog.FileName = Path.GetFileName(currentPath);
            }

            return dialog.ShowDialog(this) == true ? dialog.FileName : string.Empty;
        }

        private GetInfo OpenDrawingBlock(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            Thread statusThread = new Thread(new ThreadStart(ApplicationRuntime.ThreadMethodAbrindoCad));
            statusThread.SetApartmentState(ApartmentState.STA);
            statusThread.Start();
            GetInfo drawingBlock = new GetInfo(filePath);
            ApplicationRuntime.StopStatusThread(statusThread);
            return drawingBlock.Status() == "ERROR" ? null : drawingBlock;
        }

        private List<Block> LoadBlockListFromPath(string filePath)
        {
            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                return null;
            }

            List<Block> blocks = DeduplicateBlocks(drawingBlock.GetListBlocks());
            drawingBlock.Dispose();
            return blocks;
        }

        private void SelectScalePointsFromDrawing()
        {
            GetInfo drawing = OpenScaleDrawing();
            if (drawing == null)
            {
                return;
            }

            PointEspecial p1 = new PointEspecial();
            PointEspecial p2 = new PointEspecial();
            drawing.Get2Point(ref p1, ref p2);

            if (drawing.Status() == "ERROR")
            {
                return;
            }

            SetScalePointFields(p1, p2);
            Thread.Sleep(5);
            Activate();
        }

        private GetInfo OpenScaleDrawing()
        {
            if (scaleDrawing != null)
            {
                scaleDrawing.UpdateStatus();
            }

            if (scaleDrawing != null && scaleDrawing.Status() != "ERROR")
            {
                return scaleDrawing;
            }

            DisposeScaleDrawing();

            string fileName = BrowseDrawingFile(scaleDrawingPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            GetInfo drawing = OpenDrawingBlock(fileName);
            if (drawing == null)
            {
                scaleDrawingPath = string.Empty;
                return null;
            }

            scaleDrawing = drawing;
            scaleDrawingPath = fileName;
            return scaleDrawing;
        }

        private void SetScalePointFields(PointEspecial p1, PointEspecial p2)
        {
            EditorView.ScaleP1XTextBox.Text = Convert.ToString(p1.X);
            EditorView.ScaleP1YTextBox.Text = Convert.ToString(p1.Y);
            EditorView.ScaleP1ZTextBox.Text = Convert.ToString(p1.Z);
            EditorView.ScaleP2XTextBox.Text = Convert.ToString(p2.X);
            EditorView.ScaleP2YTextBox.Text = Convert.ToString(p2.Y);
            EditorView.ScaleP2ZTextBox.Text = Convert.ToString(p2.Z);

            configuration.Scale.Point1.X = p1.X;
            configuration.Scale.Point1.Y = p1.Y;
            configuration.Scale.Point1.Z = p1.Z;
            configuration.Scale.Point2.X = p2.X;
            configuration.Scale.Point2.Y = p2.Y;
            configuration.Scale.Point2.Z = p2.Z;
        }

        private List<Block> DeduplicateBlocks(List<Block> blocks)
        {
            return blocks
                .GroupBy(block => block.blockName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();
        }

        private void ResetBlockRelationsState()
        {
            foreach (Block block in listBlocksInv)
            {
                block.blockNameRelacao = string.Empty;
                block.ResetTagReference();
                block.cor = Color.Black;
            }

            foreach (Block block in listBlocksOrig)
            {
                block.blockNameRelacao = string.Empty;
                block.ResetTagReference();
                block.cor = Color.Black;
            }

            blockRelations.Clear();
        }

        private void EditTeklaBlock()
        {
            int index = GetSelectedBlockIndex(EditorView.TeklaBlocksListBox, listBlocks);
            if (index < 0)
            {
                return;
            }

            EnsureTeklaDrawingBlock();
            if (teklaDrawingBlock == null)
            {
                return;
            }

            using (AttFormat dialog = new AttFormat(listBlocks[index], configuration, teklaDrawingBlock))
            {
                if (dialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                teklaDrawingBlock = dialog.myDrawingBlock;
                RefreshTeklaBlocksView();
            }
        }

        private void RemoveTeklaBlock()
        {
            int index = GetSelectedBlockIndex(EditorView.TeklaBlocksListBox, listBlocks);
            if (index < 0 || index >= listBlocks.Count)
            {
                return;
            }

            listBlocks.RemoveAt(index);
            RefreshTeklaBlocksView();
        }

        private void RelateSelectedBlocks()
        {
            int cadIndex = GetSelectedBlockIndex(EditorView.CadBlocksListBox, listBlocksInv);
            int originalIndex = GetSelectedBlockIndex(EditorView.OriginalBlocksListBox, listBlocksOrig);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return;
            }

            Block cadBlock = listBlocksInv[cadIndex];
            Block originalBlock = listBlocksOrig[originalIndex];
            if (!string.IsNullOrWhiteSpace(originalBlock.blockNameRelacao) || IsCadBlockAlreadyRelated(cadBlock.blockName))
            {
                return;
            }

            originalBlock.blockNameRelacao = cadBlock.blockName;
            originalBlock.cor = Color.LightGray;
            cadBlock.cor = Color.LightGray;

            RefreshBlockViews();
            SelectRelatedBlocks(cadBlock.blockName, originalBlock.blockName);
            UpdateRelationControls();
        }

        private void EditBlockRelationParameters()
        {
            if (!TryGetSelectedRelation(out Block cadBlock, out Block originalBlock, out int cadIndex, out int originalIndex))
            {
                return;
            }

            using (AttFormatInventor dialog = new AttFormatInventor(cadBlock, originalBlock))
            {
                if (dialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                listBlocksInv[cadIndex] = dialog.Inventor;
                listBlocksOrig[originalIndex] = dialog.Original;
                RefreshBlockViews();
                SelectRelatedBlocks(dialog.Inventor.blockName, dialog.Original.blockName);
            }
        }

        private void RemoveSelectedRelation()
        {
            if (!TryGetSelectedRelation(out Block cadBlock, out Block originalBlock, out int cadIndex, out int originalIndex))
            {
                return;
            }

            originalBlock.blockNameRelacao = string.Empty;
            originalBlock.ResetTagReference();
            originalBlock.cor = Color.Black;
            cadBlock.ResetTagReference();
            cadBlock.cor = Color.Black;

            RefreshBlockViews();
            SelectRelatedBlocks(cadBlock.blockName, originalBlock.blockName);
            UpdateRelationControls();
        }

        private void UpdateRelationControls()
        {
            if (EditorView.RelateButton != null)
            {
                EditorView.RelateButton.IsEnabled = CanRelateSelectedBlocks();
            }

            bool hasRelationSelection = EditorView.BlockRelationsListBox.SelectedIndex >= 0;
            if (EditorView.RemoveRelationButton != null)
            {
                EditorView.RemoveRelationButton.IsEnabled = hasRelationSelection;
            }

            if (EditorView.EditRelationButton != null)
            {
                EditorView.EditRelationButton.IsEnabled = hasRelationSelection;
            }
        }

        private bool CanRelateSelectedBlocks()
        {
            int cadIndex = GetSelectedBlockIndex(EditorView.CadBlocksListBox, listBlocksInv);
            int originalIndex = GetSelectedBlockIndex(EditorView.OriginalBlocksListBox, listBlocksOrig);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return false;
            }

            Block cadBlock = listBlocksInv[cadIndex];
            Block originalBlock = listBlocksOrig[originalIndex];
            return string.IsNullOrWhiteSpace(originalBlock.blockNameRelacao) && !IsCadBlockAlreadyRelated(cadBlock.blockName);
        }

        private bool IsCadBlockAlreadyRelated(string cadBlockName)
        {
            return listBlocksOrig.Any(block => string.Equals(block.blockNameRelacao, cadBlockName, StringComparison.OrdinalIgnoreCase));
        }

        private int GetSelectedBlockIndex(System.Windows.Controls.ListBox listBox, List<Block> blocks)
        {
            if (!(listBox.SelectedItem is string selectedName))
            {
                return -1;
            }

            return blocks.FindIndex(block => string.Equals(block.blockName, selectedName, StringComparison.OrdinalIgnoreCase));
        }

        private bool TryGetSelectedRelation(out Block cadBlock, out Block originalBlock, out int cadIndex, out int originalIndex)
        {
            cadBlock = null;
            originalBlock = null;
            cadIndex = -1;
            originalIndex = -1;

            if (!(EditorView.BlockRelationsListBox.SelectedItem is string relation))
            {
                return false;
            }

            string[] parts = relation.Split(new[] { "    = >    " }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                return false;
            }

            cadIndex = listBlocksInv.FindIndex(block => string.Equals(block.blockName, parts[0], StringComparison.OrdinalIgnoreCase));
            originalIndex = listBlocksOrig.FindIndex(block => string.Equals(block.blockName, parts[1], StringComparison.OrdinalIgnoreCase));
            if (cadIndex < 0 || originalIndex < 0)
            {
                return false;
            }

            cadBlock = listBlocksInv[cadIndex];
            originalBlock = listBlocksOrig[originalIndex];
            return true;
        }

        private void SelectRelatedBlocks(string cadBlockName, string originalBlockName)
        {
            int cadIndex = listBlocksInv.FindIndex(block => string.Equals(block.blockName, cadBlockName, StringComparison.OrdinalIgnoreCase));
            int originalIndex = listBlocksOrig.FindIndex(block => string.Equals(block.blockName, originalBlockName, StringComparison.OrdinalIgnoreCase));
            if (cadIndex >= 0)
            {
                EditorView.CadBlocksListBox.SelectedIndex = cadIndex;
            }

            if (originalIndex >= 0)
            {
                EditorView.OriginalBlocksListBox.SelectedIndex = originalIndex;
            }
        }

        private void EnsureTeklaDrawingBlock()
        {
            string filePath = EditorView.AttributedFormatPathTextBox.Text;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (teklaDrawingBlock != null)
            {
                teklaDrawingBlock.UpdateStatus();
                if (teklaDrawingBlock.Status() != "ERROR" &&
                    string.Equals(teklaDrawingBlockPath, filePath, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                return;
            }

            DisposeTeklaDrawingBlock();
            teklaDrawingBlock = drawingBlock;
            teklaDrawingBlockPath = filePath;
        }

        private void DisposeTeklaDrawingBlock()
        {
            if (teklaDrawingBlock == null)
            {
                return;
            }

            teklaDrawingBlock.Dispose();
            teklaDrawingBlock = null;
            teklaDrawingBlockPath = string.Empty;
        }

        private void DisposeScaleDrawing()
        {
            if (scaleDrawing == null)
            {
                return;
            }

            scaleDrawing.Dispose();
            scaleDrawing = null;
            scaleDrawingPath = string.Empty;
        }

        private void AddLispCommand()
        {
            LispDllCommandDialog dialog = new LispDllCommandDialog
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                lispCommands.Add(LispCommandRow.FromCommandEntry(dialog.CommandEntry));
                EditorView.LispCommandsListBox.SelectedIndex = lispCommands.Count - 1;
            }
        }

        private void ModifyLispCommand()
        {
            int index = EditorView.LispCommandsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            LispDllCommandDialog dialog = new LispDllCommandDialog(lispCommands[index].ToCommandEntry())
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                lispCommands[index] = LispCommandRow.FromCommandEntry(dialog.CommandEntry);
                EditorView.LispCommandsListBox.SelectedIndex = index;
            }
        }

        private void DeleteLispCommand()
        {
            int index = EditorView.LispCommandsListBox.SelectedIndex;
            if (index < 0 || index >= lispCommands.Count)
            {
                return;
            }

            lispCommands.RemoveAt(index);
            if (lispCommands.Count > 0)
            {
                EditorView.LispCommandsListBox.SelectedIndex = index < lispCommands.Count ? index : lispCommands.Count - 1;
            }
        }

        private void MoveLispCommand(int direction)
        {
            int index = EditorView.LispCommandsListBox.SelectedIndex;
            int newIndex = index + direction;
            if (index < 0 || newIndex < 0 || newIndex >= lispCommands.Count)
            {
                return;
            }

            LispCommandRow item = lispCommands[index];
            lispCommands[index] = lispCommands[newIndex];
            lispCommands[newIndex] = item;
            EditorView.LispCommandsListBox.SelectedIndex = newIndex;
        }

        protected override void OnClosed(EventArgs e)
        {
            DisposeTeklaDrawingBlock();
            DisposeScaleDrawing();
            base.OnClosed(e);
        }

        public class LayerRuleRow
        {
            public LayerRuleRow(string baseLayer, string filter, string newLayer)
            {
                BaseLayer = baseLayer;
                Filter = filter;
                NewLayer = newLayer;
            }

            public string BaseLayer { get; set; }
            public string Filter { get; set; }
            public string NewLayer { get; set; }

            public string FilterDisplay => FormatFilterDisplay(Filter);
            public string NewLayerDisplay => FormatNewLayerDisplay(NewLayer);

            private static string FormatFilterDisplay(string value)
            {
                string[] parts = SplitLegacyConjunto(value, 6);
                if (parts == null)
                {
                    return value ?? string.Empty;
                }

                string line1 = "Objeto: " + parts[0] + "  |  Cor: " + parts[1];
                string line2 = "Linha: " + parts[2] + "  |  Orientacao: " + parts[5];
                string line3 = JoinDisplayParts(
                    DisplayPart("Texto", parts[3]),
                    DisplayPart("Altura", parts[4]));

                return JoinDisplayLines(line1, line2, line3);
            }

            private static string FormatNewLayerDisplay(string value)
            {
                string[] parts = SplitLegacyConjunto(value, 6);
                if (parts == null)
                {
                    return value ?? string.Empty;
                }

                string line1 = "Layer: " + parts[0] + "  |  Cor: " + parts[1];
                string line2 = "Linha: " + parts[2] + "  |  Estilo: " + parts[5];
                string line3 = JoinDisplayParts(
                    DisplayPart("Altura", parts[3]),
                    DisplayPart("Largura", parts[4]));

                return JoinDisplayLines(line1, line2, line3);
            }

            private static string[] SplitLegacyConjunto(string value, int expectedParts)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                string[] parts = value.Split(':');
                if (parts.Length < expectedParts)
                {
                    return null;
                }

                return parts;
            }

            private static string DisplayPart(string label, string value)
            {
                return string.IsNullOrWhiteSpace(value) ? string.Empty : label + ": " + value;
            }

            private static string JoinDisplayParts(params string[] values)
            {
                return string.Join("  |  ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
            }

            private static string JoinDisplayLines(params string[] values)
            {
                return string.Join(Environment.NewLine, values.Where(value => !string.IsNullOrWhiteSpace(value)));
            }
        }

        public class RemoveLayerRow
        {
            public RemoveLayerRow(string layer, string filter)
            {
                Layer = layer;
                Filter = filter;
            }

            public string Layer { get; set; }
            public string Filter { get; set; }
            public string FilterDisplay => FormatFilterDisplay(Filter);

            private static string FormatFilterDisplay(string value)
            {
                string[] parts = SplitLegacyConjunto(value, 6);
                if (parts == null)
                {
                    return value ?? string.Empty;
                }

                string line1 = "Objeto: " + parts[0] + "  |  Cor: " + parts[1];
                string line2 = "Linha: " + parts[2] + "  |  Orientacao: " + parts[5];
                string line3 = JoinDisplayParts(
                    DisplayPart("Texto", parts[3]),
                    DisplayPart("Altura", parts[4]));

                return JoinDisplayLines(line1, line2, line3);
            }

            private static string[] SplitLegacyConjunto(string value, int expectedParts)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                string[] parts = value.Split(':');
                if (parts.Length < expectedParts)
                {
                    return null;
                }

                return parts;
            }

            private static string DisplayPart(string label, string value)
            {
                return string.IsNullOrWhiteSpace(value) ? string.Empty : label + ": " + value;
            }

            private static string JoinDisplayParts(params string[] values)
            {
                return string.Join("  |  ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
            }

            private static string JoinDisplayLines(params string[] values)
            {
                return string.Join(Environment.NewLine, values.Where(value => !string.IsNullOrWhiteSpace(value)));
            }
        }

        public class LispCommandRow
        {
            public LispCommandRow(string name, string path, bool runOnlyAtEnd)
            {
                Name = name;
                Path = path;
                RunOnlyAtEnd = runOnlyAtEnd;
            }

            public string Name { get; set; }
            public string Path { get; set; }
            public bool RunOnlyAtEnd { get; set; }

            public static LispCommandRow FromCommandEntry(string commandEntry)
            {
                string[] parts = (commandEntry ?? string.Empty).Split(new[] { '@' }, 3);
                return new LispCommandRow(
                    parts.Length > 0 ? parts[0] : string.Empty,
                    parts.Length > 1 ? parts[1] : string.Empty,
                    parts.Length == 3);
            }

            public string ToCommandEntry()
            {
                string entry = (Name ?? string.Empty) + "@" + (Path ?? string.Empty);
                if (RunOnlyAtEnd)
                {
                    entry += "@True";
                }

                return entry;
            }
        }
    }
}






