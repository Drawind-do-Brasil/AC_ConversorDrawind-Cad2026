using ConversorDrawind.UI.Wpf.LispDll;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();
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
            EditorView.ClientLayersControl.ConfigurationChanged += ClientLayersConfigurationChanged;
            EditorView.ClientTextStylesControl.ConfigurationChanged += ClientTextStylesConfigurationChanged;
            ConverterView.ExtensionComboBox.ItemsSource = new[] { "DWG", "DXF" };
            ConverterView.ExtensionComboBox.SelectedItem = ApplicationRuntime.ExtensaoGeral;
            StatusConversorItem[] statusItems = new[] { new StatusConversorItem(Localization.StatusActiveWorks, "TemplatesAtivos"), new StatusConversorItem(Localization.StatusInactiveWorks, "TemplatesInativos") };
            ConverterView.StatusComboBox.ItemsSource = statusItems;
            ConverterView.StatusComboBox.SelectedIndex = 0;
            LoadConfigurationToControls();
            RestoreInitialConverter(statusItems);
            isInitializing = false;
        }

        private void LoadConverterLists()
        {
            List<string> names = ConverterFileService.ListConverterNames(CurrentStatus);
            ConverterView.ConvertersListBox.ItemsSource = names;
            EditorView.ConverterComboBox.ItemsSource = names;
        }

        private List<string> CurrentConverterNames()
        {
            return (ConverterView.ConvertersListBox.ItemsSource as IEnumerable<string>)?.ToList() ?? new List<string>();
        }

        private static List<T> ItemsSourceList<T>(ItemsControl control)
        {
            return (control.ItemsSource as IEnumerable<T>)?.ToList() ?? control.Items.OfType<T>().ToList();
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

            List<string> names = CurrentConverterNames();
            if (string.IsNullOrWhiteSpace(converterName) || !names.Contains(converterName))
            {
                converterName = names.FirstOrDefault();
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
            foreach (string file in dialog.FileNames.Where(File.Exists)) if (!ConverterView.DrawingsListBox.Items.Contains(file)) ConverterView.DrawingsListBox.Items.Add(file);
        }

        private void DropDrawings(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop)) if (File.Exists(file) && !ConverterView.DrawingsListBox.Items.Contains(file)) ConverterView.DrawingsListBox.Items.Add(file);
        }

        private void RestoreLastConverted()
        {
            if (!File.Exists(ApplicationRuntime.LOGarqConvertidos)) { WpfMessageBox.Show(Localization.MessageNoPreviousConvertedFiles, Localization.AppTitle); return; }
            ConverterView.DrawingsListBox.Items.Clear();
            foreach (string file in File.ReadAllLines(ApplicationRuntime.LOGarqConvertidos).Where(File.Exists)) ConverterView.DrawingsListBox.Items.Add(file);
        }

        private void SelectConverterFromList()
        {
            if (isInitializing || isSynchronizingConverterSelection || !(ConverterView.ConvertersListBox.SelectedItem is string selected))
            {
                return;
            }

            if (!ConfirmDiscardTemplateChanges())
            {
                RestoreLoadedConverterSelection();
                return;
            }

            LoadConverter(selected);
            SelectLoadedConverter(selected);
        }

        private void SelectConverterFromEditor()
        {
            if (isInitializing || isSynchronizingConverterSelection || !(EditorView.ConverterComboBox.SelectedItem is string selected))
            {
                return;
            }

            if (!ConfirmDiscardTemplateChanges())
            {
                RestoreLoadedConverterSelection();
                return;
            }

            LoadConverter(selected);
            SelectLoadedConverter(selected);
        }

        private void LoadConverter(string converterName)
        {
            configuration = ConverterFileService.LoadConverter(converterName, CurrentStatus);
            LoadConfigurationToControls();
            UserSettingsService.SaveLastConverter(CurrentStatus, converterName);
            loadedConverterName = converterName;
            loadedConverterStatus = CurrentStatus;
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
            EditorView.ConverterNameTextBox.Text = string.Empty;
            LoadConfigurationToControls();
            loadedConverterName = string.Empty;
            loadedConverterStatus = null;
        }

        private void SaveConverter()
        {
            string name = EditorView.ConverterNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { WpfMessageBox.Show(Localization.MessageEnterConverterNameBeforeSave, Localization.AppTitle); return; }
            ReadConfigurationFromControls();
            ConverterFileService.SaveConverter(name, CurrentStatus, configuration);
            LoadConverterLists();
            SelectLoadedConverter(name);
            UserSettingsService.SaveLastConverter(CurrentStatus, name);
            loadedConverterName = name;
            loadedConverterStatus = CurrentStatus;
        }

        private void ImportConverter()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = Localization.FilterTemplateXml };
            if (dialog.ShowDialog(this) != true) return;
            configuration = ConverterFileService.LoadConverter(dialog.FileName, CurrentStatus);
            EditorView.ConverterNameTextBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            LoadConfigurationToControls();
            UserSettingsService.SaveLastConverter(CurrentStatus, EditorView.ConverterNameTextBox.Text);
            loadedConverterName = string.Empty;
            loadedConverterStatus = null;
        }

        private bool ConfirmDiscardTemplateChanges()
        {
            if (!HasLoadedTemplateChanged())
            {
                return true;
            }

            return WpfMessageBox.Show(
                Localization.MessageTemplateChangedConfirmSwitch,
                Localization.TitleWarningNoExclamation,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        private bool HasLoadedTemplateChanged()
        {
            if (string.IsNullOrWhiteSpace(loadedConverterName) || loadedConverterStatus == null)
            {
                return false;
            }

            string savedPath = ConverterFileService.GetTxmlPath(loadedConverterName, loadedConverterStatus);
            if (!File.Exists(savedPath))
            {
                return false;
            }

            ReadConfigurationFromControls();

            global::ConversorDrawind.Configuration savedConfiguration = ConverterFileService.LoadConverter(loadedConverterName, loadedConverterStatus);
            string currentXml = StructuredConfigurationXmlWriter.CreateDocument(configuration.ToConverterConfiguration()).ToString();
            string savedXml = StructuredConfigurationXmlWriter.CreateDocument(savedConfiguration.ToConverterConfiguration()).ToString();

            return !string.Equals(currentXml, savedXml, StringComparison.Ordinal);
        }

        private void RestoreLoadedConverterSelection()
        {
            isSynchronizingConverterSelection = true;
            try
            {
                ConverterView.ConvertersListBox.SelectedItem = string.IsNullOrWhiteSpace(loadedConverterName) ? null : loadedConverterName;
                EditorView.ConverterComboBox.SelectedItem = string.IsNullOrWhiteSpace(loadedConverterName) ? null : loadedConverterName;
                EditorView.ConverterNameTextBox.Text = loadedConverterName ?? string.Empty;
            }
            finally
            {
                isSynchronizingConverterSelection = false;
            }
        }

        private void ConvertSelectedDrawings()
        {
            ReadConfigurationFromControls();
            if (!(ConverterView.ConvertersListBox.SelectedItem is string selectedConverter)) { WpfMessageBox.Show(Localization.MessageSelectConverterBeforeConvert, Localization.AppTitle); return; }
            string[] drawingFiles = ConverterView.DrawingsListBox.Items.OfType<string>().ToArray();
            if (drawingFiles.Length == 0) { WpfMessageBox.Show(Localization.MessageAddAtLeastOneDrawingBeforeConvert, Localization.AppTitle); return; }
            ConversionPreflightResult preflight = ConversionPreflightValidator.ValidateFormatPath(configuration);
            if (!preflight.CanConvert) { WpfMessageBox.Show(Localization.FormatNotFoundMessage(preflight.MissingFormatPath), Localization.AppTitle); return; }
            ApplicationRuntime.ExtensaoGeral = Convert.ToString(ConverterView.ExtensionComboBox.Text);
            Param1 param = new Param1 { conversorName = selectedConverter, desenhosName = drawingFiles, closedesenhos = ConverterView.KeepFilesOpenCheckBox.IsChecked != true, configuration = configuration, StatusConversorItem = CurrentStatus };
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
            EditorView.LayerRulesGrid.ItemsSource = configuration.Layers.ConversionRules
                .Select(rule => new LayerRuleRow(rule))
                .ToList();
        }

        private void RefreshRemoveLayerViews()
        {
            EditorView.RemoveLayerBaseListBox.ItemsSource = AllConfiguredLayerNames()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            EditorView.RemoveLayersGrid.ItemsSource = configuration.Layers.RemoveRules
                .Select(rule => new RemoveLayerRow(rule))
                .ToList();
        }

        private void RefreshExplodeLayerViews()
        {
            EditorView.AllExplodeLayersListBox.ItemsSource = configuration.Layers.BaseLayers
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            configuration.Layers.ExplodeLayers = configuration.Layers.ExplodeLayers
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            EditorView.SelectedExplodeLayersListBox.ItemsSource = configuration.Layers.ExplodeLayers.ToList();
        }

        private LayerConversionRule CreateDefaultLayerConversionRule()
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

        private void AddLayerRule()
        {
            int insertIndex = EditorView.LayerRulesGrid.SelectedIndex >= 0 ? EditorView.LayerRulesGrid.SelectedIndex : configuration.Layers.ConversionRules.Count;
            configuration.Layers.ConversionRules.Insert(insertIndex, CreateDefaultLayerConversionRule());
            RefreshLayerRuleRows();
            EditorView.LayerRulesGrid.SelectedIndex = insertIndex;
        }

        private void DeleteSelectedLayerRules()
        {
            List<LayerRuleRow> rows = ItemsSourceList<LayerRuleRow>(EditorView.LayerRulesGrid);
            List<int> selectedIndexes = EditorView.LayerRulesGrid.SelectedItems
                .OfType<LayerRuleRow>()
                .Select(row => rows.IndexOf(row))
                .Where(index => index >= 0)
                .OrderByDescending(index => index)
                .ToList();

            foreach (int index in selectedIndexes)
            {
                configuration.Layers.ConversionRules.RemoveAt(index);
            }

            RefreshLayerRuleRows();
        }

        private void MoveLayerRule(int direction)
        {
            int index = EditorView.LayerRulesGrid.SelectedIndex;
            int newIndex = index + direction;
            if (index < 0 || newIndex < 0 || newIndex >= configuration.Layers.ConversionRules.Count)
            {
                return;
            }

            LayerConversionRule item = configuration.Layers.ConversionRules[index];
            configuration.Layers.ConversionRules.RemoveAt(index);
            configuration.Layers.ConversionRules.Insert(newIndex, item);
            RefreshLayerRuleRows();
            EditorView.LayerRulesGrid.SelectedIndex = newIndex;
        }

        private void EditLayerRuleCell()
        {
            if (!(EditorView.LayerRulesGrid.SelectedItem is LayerRuleRow row) || EditorView.LayerRulesGrid.CurrentColumn == null)
            {
                return;
            }

            int selectedIndex = EditorView.LayerRulesGrid.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= configuration.Layers.ConversionRules.Count)
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
                        row.Rule.Source.BaseLayer = dialog.layerBase;
                        RefreshLayerRuleRows();
                        EditorView.LayerRulesGrid.SelectedIndex = selectedIndex;
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
                        RefreshLayerRuleRows();
                        EditorView.LayerRulesGrid.SelectedIndex = selectedIndex;
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
                        RefreshLayerRuleRows();
                        EditorView.LayerRulesGrid.SelectedIndex = selectedIndex;
                    }
                }
            }
        }

        private void AddRemoveLayerRule()
        {
            foreach (string layer in EditorView.RemoveLayerBaseListBox.SelectedItems.OfType<string>())
            {
                if (!configuration.Layers.RemoveRules
                    .Select(rule => new RemoveLayerRow(rule))
                    .Any(row => string.Equals(row.Layer, layer, StringComparison.OrdinalIgnoreCase)))
                {
                    configuration.Layers.RemoveRules.Add(CreateDefaultRemoveLayerRule(layer));
                }
            }

            RefreshRemoveLayerViews();
        }

        private void DeleteSelectedRemoveLayers()
        {
            List<RemoveLayerRow> rows = ItemsSourceList<RemoveLayerRow>(EditorView.RemoveLayersGrid);
            List<int> selectedIndexes = EditorView.RemoveLayersGrid.SelectedItems
                .OfType<RemoveLayerRow>()
                .Select(row => rows.IndexOf(row))
                .Where(index => index >= 0)
                .OrderByDescending(index => index)
                .ToList();

            foreach (int index in selectedIndexes)
            {
                configuration.Layers.RemoveRules.RemoveAt(index);
            }

            RefreshRemoveLayerViews();
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

            int index = EditorView.RemoveLayersGrid.SelectedIndex;
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
                    RefreshRemoveLayerViews();
                    EditorView.RemoveLayersGrid.SelectedIndex = index;
                }
            }
        }

        private void AddExplodeLayer()
        {
            foreach (string layer in EditorView.AllExplodeLayersListBox.SelectedItems.OfType<string>())
            {
                if (!configuration.Layers.ExplodeLayers.Contains(layer))
                {
                    configuration.Layers.ExplodeLayers.Add(layer);
                }
            }

            RefreshExplodeLayerViews();
        }

        private void RemoveExplodeLayer()
        {
            List<string> selectedLayers = EditorView.SelectedExplodeLayersListBox.SelectedItems.OfType<string>().ToList();
            foreach (string layer in selectedLayers)
            {
                configuration.Layers.ExplodeLayers.Remove(layer);
            }

            RefreshExplodeLayerViews();
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
                    if (!configuration.Layers.ExplodeLayers.Contains(layer))
                    {
                        configuration.Layers.ExplodeLayers.Add(layer);
                    }
                }
            }
            else if (targetListBox == EditorView.AllExplodeLayersListBox)
            {
                foreach (string layer in layers)
                {
                    configuration.Layers.ExplodeLayers.Remove(layer);
                }
            }

            RefreshExplodeLayerViews();
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
            RefreshLispCommandRows();
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
        }

        private void RefreshLispCommandRows()
        {
            EditorView.LispCommandsListBox.ItemsSource = configuration.Commands.LispCommands
                .Select(LispCommandRow.FromCommandEntry)
                .ToList();
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
            EditorView.TeklaBlocksListBox.ItemsSource = configuration.Blocks.TeklaBlocks
                .Select(block => block.Name)
                .ToList();
        }

        private void RefreshCadBlocksView()
        {
            EditorView.CadBlocksListBox.ItemsSource = configuration.Blocks.CadBlocks
                .Select(block => block.Name)
                .ToList();
        }

        private void RefreshOriginalBlocksView()
        {
            EditorView.OriginalBlocksListBox.ItemsSource = configuration.Blocks.OriginalBlocks
                .Select(block => block.Name)
                .ToList();
        }

        private void RefreshRelationView()
        {
            List<BlockRelationRow> relations = new List<BlockRelationRow>();
            foreach (BlockDefinition original in configuration.Blocks.OriginalBlocks)
            {
                if (string.IsNullOrWhiteSpace(original.RelatedName))
                {
                    continue;
                }

                BlockDefinition cad = configuration.Blocks.CadBlocks
                    .FirstOrDefault(block => string.Equals(block.Name, original.RelatedName, StringComparison.OrdinalIgnoreCase));
                if (cad != null)
                {
                    relations.Add(new BlockRelationRow(cad, original));
                }
            }

            EditorView.BlockRelationsListBox.ItemsSource = relations;
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
                configuration.Blocks.TeklaBlocks.Clear();
                teklaDrawingBlockPath = string.Empty;
                RefreshTeklaBlocksView();
                UpdateRelationControls();
                return;
            }

            DisposeTeklaDrawingBlock();
            teklaDrawingBlock = drawingBlock;
            teklaDrawingBlockPath = filePath;
            configuration.Blocks.TeklaBlocks = DeduplicateBlocks(drawingBlock.GetListBlocks())
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
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

            configuration.Blocks.CadBlocks = blocks
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
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

            configuration.Blocks.OriginalBlocks = blocks
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
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

            Point p1 = new Point();
            Point p2 = new Point();
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

        private void SetScalePointFields(Point p1, Point p2)
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

        private static void ResetTagReference(BlockDefinition block)
        {
            foreach (BlockTagDefinition tag in block.Tags ?? new List<BlockTagDefinition>())
            {
                tag.RelatedIndex = -1;
                tag.IsAssociated = false;
            }
        }

        private void ResetBlockRelationsState()
        {
            foreach (BlockDefinition block in configuration.Blocks.CadBlocks)
            {
                block.RelatedName = string.Empty;
                ResetTagReference(block);
                block.ColorArgb = Color.Black.ToArgb();
            }

            foreach (BlockDefinition block in configuration.Blocks.OriginalBlocks)
            {
                block.RelatedName = string.Empty;
                ResetTagReference(block);
                block.ColorArgb = Color.Black.ToArgb();
            }

            RefreshRelationView();
        }

        private void EditTeklaBlock()
        {
            int index = GetSelectedBlockIndex(EditorView.TeklaBlocksListBox, configuration.Blocks.TeklaBlocks);
            if (index < 0)
            {
                return;
            }

            EnsureTeklaDrawingBlock();
            if (teklaDrawingBlock == null)
            {
                return;
            }

            Block editableBlock = ConfigurationCompatibilityMapper.ToBlockModel(configuration.Blocks.TeklaBlocks[index]);
            using (AttFormat dialog = new AttFormat(editableBlock, configuration, teklaDrawingBlock))
            {
                if (dialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                teklaDrawingBlock = dialog.myDrawingBlock;
                configuration.Blocks.TeklaBlocks[index] = ConfigurationCompatibilityMapper.ToBlockDefinition(editableBlock);
                RefreshTeklaBlocksView();
            }
        }

        private void RemoveTeklaBlock()
        {
            int index = GetSelectedBlockIndex(EditorView.TeklaBlocksListBox, configuration.Blocks.TeklaBlocks);
            if (index < 0 || index >= configuration.Blocks.TeklaBlocks.Count)
            {
                return;
            }

            configuration.Blocks.TeklaBlocks.RemoveAt(index);
            RefreshTeklaBlocksView();
        }

        private void RelateSelectedBlocks()
        {
            int cadIndex = GetSelectedBlockIndex(EditorView.CadBlocksListBox, configuration.Blocks.CadBlocks);
            int originalIndex = GetSelectedBlockIndex(EditorView.OriginalBlocksListBox, configuration.Blocks.OriginalBlocks);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return;
            }

            BlockDefinition cadBlock = configuration.Blocks.CadBlocks[cadIndex];
            BlockDefinition originalBlock = configuration.Blocks.OriginalBlocks[originalIndex];
            if (!string.IsNullOrWhiteSpace(originalBlock.RelatedName) || IsCadBlockAlreadyRelated(cadBlock.Name))
            {
                return;
            }

            originalBlock.RelatedName = cadBlock.Name;
            originalBlock.ColorArgb = Color.LightGray.ToArgb();
            cadBlock.ColorArgb = Color.LightGray.ToArgb();

            RefreshBlockViews();
            SelectRelatedBlocks(cadBlock.Name, originalBlock.Name);
            UpdateRelationControls();
        }

        private void EditBlockRelationParameters()
        {
            if (!TryGetSelectedRelation(out BlockDefinition cadBlock, out BlockDefinition originalBlock, out int cadIndex, out int originalIndex))
            {
                return;
            }

            using (AttFormatInventor dialog = new AttFormatInventor(
                ConfigurationCompatibilityMapper.ToBlockModel(cadBlock),
                ConfigurationCompatibilityMapper.ToBlockModel(originalBlock)))
            {
                if (dialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                configuration.Blocks.CadBlocks[cadIndex] = ConfigurationCompatibilityMapper.ToBlockDefinition(dialog.Inventor);
                configuration.Blocks.OriginalBlocks[originalIndex] = ConfigurationCompatibilityMapper.ToBlockDefinition(dialog.Original);
                RefreshBlockViews();
                SelectRelatedBlocks(dialog.Inventor.blockName, dialog.Original.blockName);
            }
        }

        private void RemoveSelectedRelation()
        {
            if (!TryGetSelectedRelation(out BlockDefinition cadBlock, out BlockDefinition originalBlock, out int cadIndex, out int originalIndex))
            {
                return;
            }

            originalBlock.RelatedName = string.Empty;
            ResetTagReference(originalBlock);
            originalBlock.ColorArgb = Color.Black.ToArgb();
            ResetTagReference(cadBlock);
            cadBlock.ColorArgb = Color.Black.ToArgb();

            RefreshBlockViews();
            SelectRelatedBlocks(cadBlock.Name, originalBlock.Name);
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
            int cadIndex = GetSelectedBlockIndex(EditorView.CadBlocksListBox, configuration.Blocks.CadBlocks);
            int originalIndex = GetSelectedBlockIndex(EditorView.OriginalBlocksListBox, configuration.Blocks.OriginalBlocks);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return false;
            }

            BlockDefinition cadBlock = configuration.Blocks.CadBlocks[cadIndex];
            BlockDefinition originalBlock = configuration.Blocks.OriginalBlocks[originalIndex];
            return string.IsNullOrWhiteSpace(originalBlock.RelatedName) && !IsCadBlockAlreadyRelated(cadBlock.Name);
        }

        private bool IsCadBlockAlreadyRelated(string cadBlockName)
        {
            return configuration.Blocks.OriginalBlocks.Any(block => string.Equals(block.RelatedName, cadBlockName, StringComparison.OrdinalIgnoreCase));
        }

        private int GetSelectedBlockIndex(System.Windows.Controls.ListBox listBox, List<BlockDefinition> blocks)
        {
            if (!(listBox.SelectedItem is string selectedName))
            {
                return -1;
            }

            return blocks.FindIndex(block => string.Equals(block.Name, selectedName, StringComparison.OrdinalIgnoreCase));
        }

        private bool TryGetSelectedRelation(out BlockDefinition cadBlock, out BlockDefinition originalBlock, out int cadIndex, out int originalIndex)
        {
            cadBlock = null;
            originalBlock = null;
            cadIndex = -1;
            originalIndex = -1;

            if (!(EditorView.BlockRelationsListBox.SelectedItem is BlockRelationRow relation))
            {
                return false;
            }

            cadIndex = configuration.Blocks.CadBlocks.IndexOf(relation.CadBlock);
            originalIndex = configuration.Blocks.OriginalBlocks.IndexOf(relation.OriginalBlock);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return false;
            }

            cadBlock = configuration.Blocks.CadBlocks[cadIndex];
            originalBlock = configuration.Blocks.OriginalBlocks[originalIndex];
            return true;
        }

        private void SelectRelatedBlocks(string cadBlockName, string originalBlockName)
        {
            int cadIndex = configuration.Blocks.CadBlocks.FindIndex(block => string.Equals(block.Name, cadBlockName, StringComparison.OrdinalIgnoreCase));
            int originalIndex = configuration.Blocks.OriginalBlocks.FindIndex(block => string.Equals(block.Name, originalBlockName, StringComparison.OrdinalIgnoreCase));
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
                configuration.Commands.LispCommands.Add(dialog.CommandEntry);
                RefreshLispCommandRows();
                EditorView.LispCommandsListBox.SelectedIndex = configuration.Commands.LispCommands.Count - 1;
            }
        }

        private void ModifyLispCommand()
        {
            int index = EditorView.LispCommandsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            LispDllCommandDialog dialog = new LispDllCommandDialog(configuration.Commands.LispCommands[index])
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                configuration.Commands.LispCommands[index] = dialog.CommandEntry;
                RefreshLispCommandRows();
                EditorView.LispCommandsListBox.SelectedIndex = index;
            }
        }

        private void DeleteLispCommand()
        {
            int index = EditorView.LispCommandsListBox.SelectedIndex;
            if (index < 0 || index >= configuration.Commands.LispCommands.Count)
            {
                return;
            }

            configuration.Commands.LispCommands.RemoveAt(index);
            RefreshLispCommandRows();
            if (configuration.Commands.LispCommands.Count > 0)
            {
                EditorView.LispCommandsListBox.SelectedIndex = index < configuration.Commands.LispCommands.Count ? index : configuration.Commands.LispCommands.Count - 1;
            }
        }

        private void MoveLispCommand(int direction)
        {
            int index = EditorView.LispCommandsListBox.SelectedIndex;
            int newIndex = index + direction;
            if (index < 0 || newIndex < 0 || newIndex >= configuration.Commands.LispCommands.Count)
            {
                return;
            }

            string item = configuration.Commands.LispCommands[index];
            configuration.Commands.LispCommands[index] = configuration.Commands.LispCommands[newIndex];
            configuration.Commands.LispCommands[newIndex] = item;
            RefreshLispCommandRows();
            EditorView.LispCommandsListBox.SelectedIndex = newIndex;
        }

        protected override void OnClosed(EventArgs e)
        {
            DisposeTeklaDrawingBlock();
            DisposeScaleDrawing();
            base.OnClosed(e);
        }

    }
}






