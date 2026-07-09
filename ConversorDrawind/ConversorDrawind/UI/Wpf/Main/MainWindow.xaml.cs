using ConversorDrawind.UI.Wpf.Main.ViewModels;
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

        private void LoadConverterLists()
        {
            List<string> names = ConverterFileService.ListConverterNames(CurrentStatus);
            viewModel.SetConverterNames(names);
        }

        private List<string> CurrentConverterNames()
        {
            return viewModel.ConverterNames.ToList();
        }

        private void RestoreInitialConverter(IEnumerable<StatusConversorItem> statusItems)
        {
            string converterName = string.Empty;
            if (UserSettingsService.TryReadLastConverter(out string statusFolder, out string lastConverterName))
            {
                StatusConversorItem savedStatus = statusItems.FirstOrDefault(status => string.Equals(status.Pasta, statusFolder, StringComparison.OrdinalIgnoreCase));
                if (savedStatus != null)
                {
                    viewModel.SelectedStatus = savedStatus;
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
            string selected = viewModel.SelectedConverterName;
            if (isInitializing || isSynchronizingConverterSelection || string.IsNullOrWhiteSpace(selected))
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
            string selected = viewModel.SelectedConverterName;
            if (isInitializing || isSynchronizingConverterSelection || string.IsNullOrWhiteSpace(selected))
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
            viewModel.Configuration = configuration;
            RefreshEditorFromConfiguration();
            UserSettingsService.SaveLastConverter(CurrentStatus, converterName);
            loadedConverterName = converterName;
            loadedConverterStatus = CurrentStatus;
        }

        private void SelectLoadedConverter(string converterName)
        {
            isSynchronizingConverterSelection = true;
            try
            {
                viewModel.SelectedConverterName = converterName;
                viewModel.TemplateName = converterName;
            }
            finally
            {
                isSynchronizingConverterSelection = false;
            }
        }

        private void NewConverter()
        {
            configuration = new global::ConversorDrawind.Configuration();
            viewModel.Configuration = configuration;
            viewModel.TemplateName = string.Empty;
            RefreshEditorFromConfiguration();
            loadedConverterName = string.Empty;
            loadedConverterStatus = null;
        }

        private void SaveConverter()
        {
            string name = viewModel.TemplateName.Trim();
            if (string.IsNullOrWhiteSpace(name)) { WpfMessageBox.Show(Localization.MessageEnterConverterNameBeforeSave, Localization.AppTitle); return; }
            ApplyEditorPendingChanges();
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
            viewModel.Configuration = configuration;
            viewModel.TemplateName = Path.GetFileNameWithoutExtension(dialog.FileName);
            RefreshEditorFromConfiguration();
            UserSettingsService.SaveLastConverter(CurrentStatus, viewModel.TemplateName);
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

            ApplyEditorPendingChanges();

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
                viewModel.SelectedConverterName = string.IsNullOrWhiteSpace(loadedConverterName) ? string.Empty : loadedConverterName;
                viewModel.TemplateName = loadedConverterName ?? string.Empty;
            }
            finally
            {
                isSynchronizingConverterSelection = false;
            }
        }

        private void ConvertSelectedDrawings()
        {
            ApplyEditorPendingChanges();
            string selectedConverter = viewModel.SelectedConverterName;
            if (string.IsNullOrWhiteSpace(selectedConverter)) { WpfMessageBox.Show(Localization.MessageSelectConverterBeforeConvert, Localization.AppTitle); return; }
            string[] drawingFiles = ConverterView.DrawingsListBox.Items.OfType<string>().ToArray();
            if (drawingFiles.Length == 0) { WpfMessageBox.Show(Localization.MessageAddAtLeastOneDrawingBeforeConvert, Localization.AppTitle); return; }
            ConversionPreflightResult preflight = ConversionPreflightValidator.ValidateFormatPath(configuration);
            if (!preflight.CanConvert) { WpfMessageBox.Show(Localization.FormatNotFoundMessage(preflight.MissingFormatPath), Localization.AppTitle); return; }
            ApplicationRuntime.ExtensaoGeral = viewModel.Extension;
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

                RefreshDimensionOptions();
                targetComboBox.Text = colorDialog.colorClass;
            }
        }

        private void RefreshDimensionOptions()
        {
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionLayerOptions, NewLayerNames(), configuration.Dimensions.Layer);
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionColorOptions, configuration.Catalogs.Colors.Skip(1), configuration.Dimensions.LineColor);
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionArrowTypeOptions, DimensionArrowTypes(), configuration.Dimensions.ArrowType);
            ConverterEditorViewModel.ReplaceOptions(viewModel.TextStyleOptions, TextStyleNames(), configuration.Text.DefaultStyleName);
            ConverterEditorViewModel.ReplaceOptions(viewModel.LinearPrecisionOptions, Enumerable.Range(0, 9), configuration.Dimensions.Precision);
            ConverterEditorViewModel.ReplaceOptions(viewModel.AngularPrecisionOptions, Enumerable.Range(0, 9), configuration.Dimensions.AngularPrecision);
            ConverterEditorViewModel.ReplaceOptions(viewModel.LinearUnitOptions, Enumerable.Range(1, 6), configuration.Dimensions.Unit);
            ConverterEditorViewModel.ReplaceOptions(viewModel.AngularUnitOptions, Enumerable.Range(1, 6), configuration.Dimensions.AngularUnit);
            ConverterEditorViewModel.ReplaceOptions(viewModel.BooleanOptions, BooleanOptions(), false);
            ConverterEditorViewModel.ReplaceOptions(viewModel.TextPlacementOptions, Enumerable.Range(0, 5), configuration.Dimensions.TextVerticalPosition);
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionBaseLayerOptions, DimensionBaseLayers(), configuration.Dimensions.BaseLayer);
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

        private IEnumerable<bool> BooleanOptions()
        {
            return new[] { true, false };
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

        private void RefreshExplodeLayerViews()
        {
            configuration.Layers.ExplodeLayers = configuration.Layers.ExplodeLayers
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            EditorView.ShowExplodeLayers(configuration.Layers.BaseLayers, configuration.Layers.ExplodeLayers);
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

        private void RefreshLispCommandRows()
        {
            EditorView.ShowLispCommands(configuration.Commands.LispCommands);
        }

        private void RefreshBlockViews()
        {
            EditorView.RefreshBlockViews(configuration);
        }

        private void BrowseAttributedFormat()
        {
            string fileName = BrowseDrawingFile(configuration.Blocks.TeklaBlockPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            configuration.Blocks.TeklaBlockPath = fileName;
            viewModel.RefreshConfiguration();
            LoadTeklaBlocks(fileName);
        }

        private void LoadTeklaBlocks()
        {
            LoadTeklaBlocks(configuration.Blocks.TeklaBlockPath);
        }

        private void LoadTeklaBlocks(string filePath)
        {
            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                configuration.Blocks.TeklaBlocks.Clear();
                teklaDrawingBlockPath = string.Empty;
                RefreshBlockViews();
                UpdateRelationControls();
                return;
            }

            DisposeTeklaDrawingBlock();
            teklaDrawingBlock = drawingBlock;
            teklaDrawingBlockPath = filePath;
            configuration.Blocks.TeklaBlocks = DeduplicateBlocks(drawingBlock.GetListBlocks())
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void LoadCadBlocks()
        {
            string fileName = BrowseDrawingFile(configuration.Blocks.CadBlockPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            configuration.Blocks.CadBlockPath = fileName;
            viewModel.RefreshConfiguration();
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
            EditorView.ResetBlockRelationsState(configuration);
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void LoadOriginalBlocks()
        {
            string fileName = BrowseDrawingFile(configuration.Blocks.CadBlockPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            configuration.Blocks.CadBlockPath = fileName;
            viewModel.RefreshConfiguration();
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
            EditorView.ResetBlockRelationsState(configuration);
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
            configuration.Scale.Point1.X = p1.X;
            configuration.Scale.Point1.Y = p1.Y;
            configuration.Scale.Point1.Z = p1.Z;
            configuration.Scale.Point2.X = p2.X;
            configuration.Scale.Point2.Y = p2.Y;
            configuration.Scale.Point2.Z = p2.Z;
            viewModel.RefreshConfiguration();

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

        private void EditTeklaBlock()
        {
            int index = EditorView.GetSelectedTeklaBlockIndex(configuration);
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
                RefreshBlockViews();
            }
        }

        private void RemoveTeklaBlock()
        {
            int index = EditorView.GetSelectedTeklaBlockIndex(configuration);
            if (index < 0 || index >= configuration.Blocks.TeklaBlocks.Count)
            {
                return;
            }

            configuration.Blocks.TeklaBlocks.RemoveAt(index);
            RefreshBlockViews();
        }

        private void RelateSelectedBlocks()
        {
            EditorView.RelateSelectedBlocks(configuration);
        }

        private void EditBlockRelationParameters()
        {
            EditorView.EditBlockRelationParameters(configuration);
        }

        private void RemoveSelectedRelation()
        {
            EditorView.RemoveSelectedRelation(configuration);
        }

        private void UpdateRelationControls()
        {
            EditorView.UpdateRelationControls(configuration);
        }

        private void EnsureTeklaDrawingBlock()
        {
            string filePath = configuration.Blocks.TeklaBlockPath;
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
            EditorView.AddLispCommand(configuration, this);
        }

        private void ModifyLispCommand()
        {
            EditorView.ModifyLispCommand(configuration, this);
        }

        private void DeleteLispCommand()
        {
            EditorView.DeleteLispCommand(configuration);
        }

        private void MoveLispCommand(int direction)
        {
            EditorView.MoveLispCommand(configuration, direction);
        }

        protected override void OnClosed(EventArgs e)
        {
            DisposeTeklaDrawingBlock();
            DisposeScaleDrawing();
            base.OnClosed(e);
        }

    }
}






