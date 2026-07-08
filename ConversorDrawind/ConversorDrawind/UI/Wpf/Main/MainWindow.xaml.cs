using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using ConversorDrawind.UI.Wpf.Blocks;
using ConversorDrawind.UI.Wpf.LispDll;
using WpfMessageBox = System.Windows.MessageBox;
using ConversorDrawind;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> drawings = new ObservableCollection<string>();
        private readonly ObservableCollection<string> converterNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> lispCommands = new ObservableCollection<string>();
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
        private Arranjos arranjos = new Arranjos();
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
                case "ConfigureClientLayersClick": using (ConfigurarLayers f = new ConfigurarLayers(arranjos)) f.ShowDialog(); break;
                case "ConfigureTextStylesClick": using (ConfigurarTextStyle f = new ConfigurarTextStyle(arranjos)) f.ShowDialog(); break;
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
            configuration = new global::ConversorDrawind.Configuration(); arranjos = new Arranjos(); listBlocks = new List<Block>(); listBlocksInv = new List<Block>(); listBlocksOrig = new List<Block>();
            ConverterFileService.LoadConverter(configuration, converterName, arranjos, listBlocks, listBlocksInv, listBlocksOrig, CurrentStatus);
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
            configuration = new global::ConversorDrawind.Configuration(); arranjos = new Arranjos(); listBlocks.Clear(); listBlocksInv.Clear(); listBlocksOrig.Clear(); EditorView.ConverterNameTextBox.Text = string.Empty; LoadConfigurationToControls();
        }

        private void SaveConverter()
        {
            string name = EditorView.ConverterNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { WpfMessageBox.Show(Localization.MessageEnterConverterNameBeforeSave, Localization.AppTitle); return; }
            ReadConfigurationFromControls();
            ConverterFileService.SaveConverter(configuration, name, arranjos, listBlocks, listBlocksInv, listBlocksOrig, CurrentStatus);
            LoadConverterLists();
            SelectLoadedConverter(name);
            UserSettingsService.SaveLastConverter(CurrentStatus, name);
        }

        private void ImportConverter()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = Localization.FilterTemplateXml };
            if (dialog.ShowDialog(this) != true) return;
            configuration.LoadXML(dialog.FileName, arranjos, listBlocks, listBlocksInv, listBlocksOrig, CurrentStatus);
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
            Param1 param = new Param1 { conversorName = selectedConverter, desenhosName = drawings.ToArray(), closedesenhos = ConverterView.KeepFilesOpenCheckBox.IsChecked != true, configuration = configuration, arranjos = arranjos, StatusConversorItem = CurrentStatus };
            using (Processo processo = new Processo(param)) processo.ShowDialog();
            if (!Processo.IsCanceled) using (ProcessoEnd processoEnd = new ProcessoEnd()) processoEnd.ShowDialog();
        }

        private void ConfigureAdvancedDimensionArrow()
        {
            using (ConfAvancadaDeCota f = new ConfAvancadaDeCota(configuration.EXTDIMCorrigeSeta, configuration.EXTDIMCorrigeSetaTipoSeta, configuration.EXTDIMCorrigeSetaFactor)) if (f.ShowDialog() == UiDialogResult.OK) { configuration.EXTDIMCorrigeSeta = f.EXTDIMCorrigeSeta; configuration.EXTDIMCorrigeSetaTipoSeta = f.EXTDIMCorrigeSetaTipoSeta; configuration.EXTDIMCorrigeSetaFactor = f.EXTDIMCorrigeSetaFactor; }
        }

        private void AddOtherDimensionColor(ComboBox targetComboBox)
        {
            using (GenericNewColor colorDialog = new GenericNewColor(targetComboBox.Text))
            {
                if (colorDialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                if (!arranjos.allcolor.Contains(colorDialog.colorClass))
                {
                    arranjos.allcolor.Add(colorDialog.colorClass);
                }

                PopulateDimensionComboBoxes();
                targetComboBox.Text = colorDialog.colorClass;
            }
        }

        private void PopulateDimensionComboBoxes()
        {
            SetComboItems(EditorView.DimensionLayerComboBox, arranjos.allNewLayer, configuration.EXTDIMlayer);
            SetComboItems(EditorView.DimensionLineColorComboBox, arranjos.allcolor.Skip(1), configuration.EXTDIMColorLine);
            SetComboItems(EditorView.DimensionTextColorComboBox, arranjos.allcolor.Skip(1), configuration.EXTDIMColorText);
            SetComboItems(EditorView.DimensionArrowTypeComboBox, DimensionArrowTypes(), configuration.EXTDIMSeta);
            SetComboItems(EditorView.DimensionTextStyleComboBox, TextStyleNames(), configuration.EXTTEXTStyleName);
            SetComboItems(EditorView.DimensionLinearPrecisionComboBox, Enumerable.Range(0, 9).Select(i => Convert.ToString(i)), Convert.ToString(configuration.EXTDIMPrecision));
            SetComboItems(EditorView.DimensionAngularPrecisionComboBox, Enumerable.Range(0, 9).Select(i => Convert.ToString(i)), Convert.ToString(configuration.EXTDIMAngularPrecision));
            SetComboItems(EditorView.DimensionLinearUnitComboBox, Enumerable.Range(1, 6).Select(i => Convert.ToString(i)), Convert.ToString(configuration.EXTDIMUnit));
            SetComboItems(EditorView.DimensionAngularUnitComboBox, Enumerable.Range(1, 6).Select(i => Convert.ToString(i)), Convert.ToString(configuration.EXTDIMAngularUnit));
            SetComboItems(EditorView.DimensionOutsideAlignComboBox, BooleanOptions(), Convert.ToString(configuration.EXTDIMOutsideAlign));
            SetComboItems(EditorView.DimensionLineForcedComboBox, BooleanOptions(), Convert.ToString(configuration.EXTDIMLineForced));
            SetComboItems(EditorView.DimensionTextInsideComboBox, BooleanOptions(), Convert.ToString(configuration.EXTDIMTextForced));
            SetComboItems(EditorView.DimensionTextAlignmentComboBox, BooleanOptions(), Convert.ToString(configuration.EXTDIMDimensionPosition));
            SetComboItems(EditorView.DimensionTextPlacementComboBox, Enumerable.Range(0, 5).Select(i => Convert.ToString(i)), Convert.ToString(configuration.EXTDIMTad));
            SetComboItems(EditorView.DimensionBaseLayerComboBox, DimensionBaseLayers(), configuration.EXTDIMBaseLayer);
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
            if (arranjos.allTextSyles.Count == 0)
            {
                arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);
            }

            return arranjos.allTextSyles.Select(style => style.Split(':').First());
        }

        private IEnumerable<string> DimensionBaseLayers()
        {
            yield return "DIMENSION";
            foreach (string layer in arranjos.allBaseLayer.Where(layer => !string.Equals(layer, "DIMENSION", StringComparison.OrdinalIgnoreCase)))
            {
                yield return layer;
            }
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
            SetComboItems(EditorView.TeklaTextLayerComboBox, PreferredLayerFirst("DRAWING SHEET"), configuration.LayerTeklaString);
            SetComboItems(EditorView.FormatBlockLayerComboBox, PreferredLayerFirst("OTHER OBJECT TYPE"), configuration.LayerBlockAttribute);
            SetComboItems(EditorView.ScaleLayerComboBox, arranjos.allNewLayer.Concat(arranjos.allBaseLayer), configuration.EXTSCALELayer);
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
            foreach (string layer in arranjos.allBaseLayer.Where(layer => !string.Equals(layer, preferredLayer, StringComparison.OrdinalIgnoreCase)))
            {
                yield return layer;
            }
        }

        private void LoadClientLayers()
        {
            using (ConfigurarLayers dialog = new ConfigurarLayers(arranjos))
            {
                dialog.OpenAcadLoadLayerExterno();
            }

            RefreshLayerDependentViews();
        }

        private void RefreshLayerRuleRows()
        {
            layerRuleRows.Clear();
            foreach (string item in arranjos.conversor)
            {
                string[] parts = item.Split(new[] { ';' }, 3);
                if (parts.Length == 3)
                {
                    layerRuleRows.Add(new LayerRuleRow(parts[0], parts[1], parts[2]));
                }
            }
        }

        private void SaveLayerRuleRowsToArranjos()
        {
            arranjos.conversor.Clear();
            foreach (LayerRuleRow row in layerRuleRows)
            {
                arranjos.conversor.Add((row.BaseLayer ?? string.Empty) + ";" + (row.Filter ?? string.Empty) + ";" + (row.NewLayer ?? string.Empty));
            }
        }

        private void RefreshRemoveLayerViews()
        {
            removeLayerBaseNames.Clear();
            foreach (string layer in arranjos.allNewLayer.Concat(arranjos.allBaseLayer).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                removeLayerBaseNames.Add(layer);
            }

            removeLayerRows.Clear();
            foreach (Filter filter in arranjos.layerRemove)
            {
                removeLayerRows.Add(new RemoveLayerRow(filter.layerBase, filter.GetConjunto()));
            }
        }

        private void SaveRemoveLayerRowsToArranjos()
        {
            arranjos.layerRemove.Clear();
            foreach (RemoveLayerRow row in removeLayerRows)
            {
                Filter filter = new Filter(arranjos)
                {
                    layerBase = row.Layer ?? string.Empty
                };
                filter.SetConjunto(row.Filter ?? string.Empty);
                arranjos.layerRemove.Add(filter);
            }
        }

        private void RefreshExplodeLayerViews()
        {
            allExplodeLayerNames.Clear();
            foreach (string layer in arranjos.allBaseLayer.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                allExplodeLayerNames.Add(layer);
            }

            selectedExplodeLayerNames.Clear();
            foreach (string layer in arranjos.allExplodeLayers.Where(layer => !string.IsNullOrWhiteSpace(layer)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                selectedExplodeLayerNames.Add(layer);
            }
        }

        private void SaveExplodeLayerRowsToArranjos()
        {
            arranjos.allExplodeLayers.Clear();
            arranjos.allExplodeLayers.AddRange(selectedExplodeLayerNames.Where(layer => !string.IsNullOrWhiteSpace(layer)));
        }

        private void AddLayerRule()
        {
            Filter filter = new Filter(arranjos);
            filter.SetConjunto();
            NewLayer newLayer = new NewLayer(arranjos);
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
                using (LayersLayerBase dialog = new LayersLayerBase(row.BaseLayer, arranjos))
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
                using (LayersFilter dialog = new LayersFilter(row.Filter, arranjos))
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
                using (LayersNewLayer dialog = new LayersNewLayer(row.NewLayer, arranjos))
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
            Filter filter = new Filter(arranjos);
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

            using (LayersFilter dialog = new LayersFilter(row.Filter, arranjos))
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
            ConverterView.TemplateCommentsTextBox.Text = configuration.EXTCONFComments ?? string.Empty;
            EditorView.AddCommentsTextBox.Text = configuration.EXTCONFComments ?? string.Empty;
            EditorView.ConvertDimensionsCheckBox.IsChecked = configuration.EXTCONFIsConvertDimension;
            EditorView.ConvertLayersCheckBox.IsChecked = configuration.EXTCONFIsConvertLayer;
            EditorView.ScaleDrawingCheckBox.IsChecked = configuration.EXTCONFIsPutOnTheScaleDrawing;
            EditorView.AttributeFormatCheckBox.IsChecked = configuration.EXTCONFIsExchangeFormat;
            EditorView.RunCommandsCheckBox.IsChecked = configuration.EXTCONFIsExecuteLISP;
            EditorView.ShowErrorMessagesCheckBox.IsChecked = configuration.PROGRAMMessage;
            EditorView.DeleteTeklaCheckBox.IsChecked = configuration.EXTCONFIsDeleteTeklaStructures;
            EditorView.PurgeCheckBox.IsChecked = configuration.EXTCONFIsPurge;
            EditorView.ExplodeCheckBox.IsChecked = configuration.EXTCONFInventorExplode;
            EditorView.ExplodeBlocksCheckBox.IsChecked = configuration.ExplodeBlocks;
            EditorView.DmBlockCheckBox.IsChecked = configuration.DMBlock;
            EditorView.TeklaOriginRadio.IsChecked = configuration.EXTCONFOrigem == 0;
            EditorView.CadOriginRadio.IsChecked = configuration.EXTCONFOrigem != 0;
            EditorView.AttributedFormatPathTextBox.Text = configuration.PROGRAMblockFormatoCaminho;
            EditorView.CadBlocksPathTextBox.Text = configuration.EXTCONFCaminhoBlocoInv;
            EditorView.OriginalBlocksPathTextBox.Text = configuration.EXTCONFCaminhoBlocoInv;
            EditorView.ClientLayersControl.LoadArranjos(arranjos);
            EditorView.ClientTextStylesControl.LoadArranjos(arranjos);
            PopulateEditorComboBoxes();
            RefreshLayerRuleRows();
            RefreshRemoveLayerViews();
            RefreshExplodeLayerViews();
            EditorView.TeklaTextLayerComboBox.Text = configuration.LayerTeklaString;
            EditorView.FormatBlockLayerComboBox.Text = configuration.LayerBlockAttribute;
            EditorView.LayerLtScaleTextBox.Text = Convert.ToString(configuration.EXTLINELtscale);
            PopulateDimensionComboBoxes();
            EditorView.DimensionLayerComboBox.Text = configuration.EXTDIMlayer;
            EditorView.DimensionStyleTextBox.Text = configuration.EXTDIMStyleName;
            EditorView.DimensionLineColorComboBox.Text = configuration.EXTDIMColorLine;
            EditorView.DimensionTextColorComboBox.Text = configuration.EXTDIMColorText;
            EditorView.DimensionArrowTypeComboBox.Text = configuration.EXTDIMSeta;
            EditorView.DimensionTextStyleComboBox.Text = configuration.EXTTEXTStyleName;
            EditorView.DimensionScaleTextBox.Text = Convert.ToString(configuration.EXTDIMScale);
            EditorView.DimensionArrowSizeTextBox.Text = Convert.ToString(configuration.EXTDIMSizeSeta);
            EditorView.DimensionOffsetTextBox.Text = Convert.ToString(configuration.EXTDIMOffsetLineFromRefPoint);
            EditorView.DimensionLineExtTextBox.Text = Convert.ToString(configuration.EXTDIMDIMEX);
            EditorView.DimensionLinearPrecisionComboBox.Text = Convert.ToString(configuration.EXTDIMPrecision);
            EditorView.DimensionAngularPrecisionComboBox.Text = Convert.ToString(configuration.EXTDIMAngularPrecision);
            EditorView.DimensionLinearUnitComboBox.Text = Convert.ToString(configuration.EXTDIMUnit);
            EditorView.DimensionAngularUnitComboBox.Text = Convert.ToString(configuration.EXTDIMAngularUnit);
            EditorView.DimensionOutsideAlignComboBox.Text = Convert.ToString(configuration.EXTDIMOutsideAlign);
            EditorView.DimensionLineForcedComboBox.Text = Convert.ToString(configuration.EXTDIMLineForced);
            EditorView.DimensionTextInsideComboBox.Text = Convert.ToString(configuration.EXTDIMTextForced);
            EditorView.DimensionTextAlignmentComboBox.Text = Convert.ToString(configuration.EXTDIMDimensionPosition);
            EditorView.DimensionTextPlacementComboBox.Text = Convert.ToString(configuration.EXTDIMTad);
            EditorView.DimensionBaseLayerComboBox.Text = configuration.EXTDIMBaseLayer;
            EditorView.ManualScaleRadio.IsChecked = configuration.EXTSCALEManual;
            EditorView.AutoScaleRadio.IsChecked = !configuration.EXTSCALEManual;
            EditorView.ScaleP1XTextBox.Text = Convert.ToString(configuration.EXTSCALEp1.X);
            EditorView.ScaleP1YTextBox.Text = Convert.ToString(configuration.EXTSCALEp1.Y);
            EditorView.ScaleP1ZTextBox.Text = Convert.ToString(configuration.EXTSCALEp1.Z);
            EditorView.ScaleP2XTextBox.Text = Convert.ToString(configuration.EXTSCALEp2.X);
            EditorView.ScaleP2YTextBox.Text = Convert.ToString(configuration.EXTSCALEp2.Y);
            EditorView.ScaleP2ZTextBox.Text = Convert.ToString(configuration.EXTSCALEp2.Z);
            EditorView.ScaleLayerComboBox.Text = configuration.EXTSCALELayer;
            EditorView.ScaleTextSizeTextBox.Text = Convert.ToString(configuration.EXTSCALETextSize);
            lispCommands.Clear();
            foreach (string command in arranjos.listLISPCommand)
            {
                lispCommands.Add(command);
            }
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void ReadConfigurationFromControls()
        {
            configuration.EXTCONFComments = EditorView.AddCommentsTextBox.Text;
            configuration.EXTCONFIsConvertDimension = EditorView.ConvertDimensionsCheckBox.IsChecked == true;
            configuration.EXTCONFIsConvertLayer = EditorView.ConvertLayersCheckBox.IsChecked == true;
            configuration.EXTCONFIsPutOnTheScaleDrawing = EditorView.ScaleDrawingCheckBox.IsChecked == true;
            configuration.EXTCONFIsExchangeFormat = EditorView.AttributeFormatCheckBox.IsChecked == true;
            configuration.EXTCONFIsExecuteLISP = EditorView.RunCommandsCheckBox.IsChecked == true;
            configuration.PROGRAMMessage = EditorView.ShowErrorMessagesCheckBox.IsChecked == true;
            configuration.EXTCONFIsDeleteTeklaStructures = EditorView.DeleteTeklaCheckBox.IsChecked == true;
            configuration.EXTCONFIsPurge = EditorView.PurgeCheckBox.IsChecked == true;
            configuration.EXTCONFInventorExplode = EditorView.ExplodeCheckBox.IsChecked == true;
            configuration.ExplodeBlocks = EditorView.ExplodeBlocksCheckBox.IsChecked == true;
            configuration.DMBlock = EditorView.DmBlockCheckBox.IsChecked == true;
            configuration.EXTCONFOrigem = EditorView.TeklaOriginRadio.IsChecked == true ? 0 : 1;
            configuration.PROGRAMblockFormatoCaminho = EditorView.AttributedFormatPathTextBox.Text;
            configuration.EXTCONFCaminhoBlocoInv = string.IsNullOrWhiteSpace(EditorView.OriginalBlocksPathTextBox.Text)
                ? EditorView.CadBlocksPathTextBox.Text
                : EditorView.OriginalBlocksPathTextBox.Text;
            configuration.LayerTeklaString = EditorView.TeklaTextLayerComboBox.Text;
            configuration.LayerBlockAttribute = EditorView.FormatBlockLayerComboBox.Text;
            configuration.EXTLINELtscale = ReadDouble(EditorView.LayerLtScaleTextBox.Text, configuration.EXTLINELtscale);
            configuration.EXTDIMlayer = EditorView.DimensionLayerComboBox.Text;
            configuration.EXTDIMStyleName = EditorView.DimensionStyleTextBox.Text;
            configuration.EXTDIMColorLine = EditorView.DimensionLineColorComboBox.Text;
            configuration.EXTDIMColorText = EditorView.DimensionTextColorComboBox.Text;
            configuration.EXTDIMSeta = EditorView.DimensionArrowTypeComboBox.Text;
            configuration.EXTTEXTStyleName = EditorView.DimensionTextStyleComboBox.Text;
            configuration.EXTDIMScale = ReadDouble(EditorView.DimensionScaleTextBox.Text, configuration.EXTDIMScale);
            configuration.EXTDIMSizeSeta = ReadDouble(EditorView.DimensionArrowSizeTextBox.Text, configuration.EXTDIMSizeSeta);
            configuration.EXTDIMOffsetLineFromRefPoint = ReadDouble(EditorView.DimensionOffsetTextBox.Text, configuration.EXTDIMOffsetLineFromRefPoint);
            configuration.EXTDIMDIMEX = ReadDouble(EditorView.DimensionLineExtTextBox.Text, configuration.EXTDIMDIMEX);
            configuration.EXTDIMPrecision = ReadInt(EditorView.DimensionLinearPrecisionComboBox.Text, configuration.EXTDIMPrecision);
            configuration.EXTDIMAngularPrecision = ReadInt(EditorView.DimensionAngularPrecisionComboBox.Text, configuration.EXTDIMAngularPrecision);
            configuration.EXTDIMUnit = ReadInt(EditorView.DimensionLinearUnitComboBox.Text, configuration.EXTDIMUnit);
            configuration.EXTDIMAngularUnit = ReadInt(EditorView.DimensionAngularUnitComboBox.Text, configuration.EXTDIMAngularUnit);
            configuration.EXTDIMOutsideAlign = ReadBool(EditorView.DimensionOutsideAlignComboBox.Text, configuration.EXTDIMOutsideAlign);
            configuration.EXTDIMLineForced = ReadBool(EditorView.DimensionLineForcedComboBox.Text, configuration.EXTDIMLineForced);
            configuration.EXTDIMTextForced = ReadBool(EditorView.DimensionTextInsideComboBox.Text, configuration.EXTDIMTextForced);
            configuration.EXTDIMDimensionPosition = ReadBool(EditorView.DimensionTextAlignmentComboBox.Text, configuration.EXTDIMDimensionPosition);
            configuration.EXTDIMTad = ReadInt(EditorView.DimensionTextPlacementComboBox.Text, configuration.EXTDIMTad);
            configuration.EXTDIMBaseLayer = EditorView.DimensionBaseLayerComboBox.Text;
            configuration.EXTSCALEManual = EditorView.ManualScaleRadio.IsChecked == true;
            configuration.EXTSCALEp1.X = ReadDouble(EditorView.ScaleP1XTextBox.Text, configuration.EXTSCALEp1.X);
            configuration.EXTSCALEp1.Y = ReadDouble(EditorView.ScaleP1YTextBox.Text, configuration.EXTSCALEp1.Y);
            configuration.EXTSCALEp1.Z = ReadDouble(EditorView.ScaleP1ZTextBox.Text, configuration.EXTSCALEp1.Z);
            configuration.EXTSCALEp2.X = ReadDouble(EditorView.ScaleP2XTextBox.Text, configuration.EXTSCALEp2.X);
            configuration.EXTSCALEp2.Y = ReadDouble(EditorView.ScaleP2YTextBox.Text, configuration.EXTSCALEp2.Y);
            configuration.EXTSCALEp2.Z = ReadDouble(EditorView.ScaleP2ZTextBox.Text, configuration.EXTSCALEp2.Z);
            configuration.EXTSCALELayer = EditorView.ScaleLayerComboBox.Text;
            configuration.EXTSCALETextSize = ReadDouble(EditorView.ScaleTextSizeTextBox.Text, configuration.EXTSCALETextSize);
            EditorView.ClientLayersControl.ApplyRowsToArranjos(false);
            EditorView.ClientTextStylesControl.ApplyRowsToArranjos();
            SaveLayerRuleRowsToArranjos();
            SaveRemoveLayerRowsToArranjos();
            SaveExplodeLayerRowsToArranjos();
            arranjos.listLISPCommand.Clear();
            arranjos.listLISPCommand.AddRange(lispCommands);
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

            configuration.EXTSCALEp1.X = p1.X;
            configuration.EXTSCALEp1.Y = p1.Y;
            configuration.EXTSCALEp1.Z = p1.Z;
            configuration.EXTSCALEp2.X = p2.X;
            configuration.EXTSCALEp2.Y = p2.Y;
            configuration.EXTSCALEp2.Z = p2.Z;
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

            using (AttFormat dialog = new AttFormat(listBlocks[index], arranjos, teklaDrawingBlock))
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
                lispCommands.Add(dialog.CommandEntry);
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

            LispDllCommandDialog dialog = new LispDllCommandDialog(lispCommands[index])
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                lispCommands[index] = dialog.CommandEntry;
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

            string item = lispCommands[index];
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
        }
    }
}






