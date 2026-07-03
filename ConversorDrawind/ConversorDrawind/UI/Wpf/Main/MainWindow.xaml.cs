using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> drawings = new ObservableCollection<string>();
        private readonly ObservableCollection<string> converterNames = new ObservableCollection<string>();
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();
        private Arranjos arranjos = new Arranjos();
        private List<Block> listBlocks = new List<Block>();
        private List<Block> listBlocksInv = new List<Block>();
        private List<Block> listBlocksOrig = new List<Block>();
        private bool isInitializing;

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
                case "ConfigureClientLayersClick": using (Form_3_ConfigurarLayers f = new Form_3_ConfigurarLayers(arranjos)) f.ShowDialog(); break;
                case "ConfigureTextStylesClick": using (Form_3_ConfigurarTextStyle f = new Form_3_ConfigurarTextStyle(arranjos)) f.ShowDialog(); break;
                case "DimensionArrowAdvancedClick": ConfigureAdvancedDimensionArrow(); break;
                case "BrowseAttributedFormatClick": BrowseAttributedFormat(); break;
            }
        }

        private StatusConversorItem CurrentStatus => ConverterView.StatusComboBox.SelectedItem as StatusConversorItem ?? new StatusConversorItem("Obras Ativas", "TemplatesAtivos");

        private void InitializeUi()
        {
            isInitializing = true;
            ConverterView.DrawingsListBox.ItemsSource = drawings;
            ConverterView.ConvertersListBox.ItemsSource = converterNames;
            ConverterView.ExtensionComboBox.ItemsSource = new[] { "DWG", "DXF" };
            ConverterView.ExtensionComboBox.SelectedItem = ApplicationRuntime.ExtensaoGeral;
            ConverterView.StatusComboBox.ItemsSource = new[] { new StatusConversorItem("Obras Ativas", "TemplatesAtivos"), new StatusConversorItem("Obras Inativas", "TemplatesInativos") };
            ConverterView.StatusComboBox.SelectedIndex = 0;
            EditorView.ConverterComboBox.ItemsSource = converterNames;
            LoadConfigurationToControls();
            isInitializing = false;
            LoadConverterLists();
        }

        private void LoadConverterLists()
        {
            converterNames.Clear();
            foreach (string name in ConverterFileService.ListConverterNames(CurrentStatus)) converterNames.Add(name);
        }

        private void AddDrawings()
        {
            OpenFileDialog dialog = new OpenFileDialog { Multiselect = true, Filter = "Desenhos CAD (*.dwg;*.dxf)|*.dwg;*.dxf|Todos os arquivos (*.*)|*.*" };
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
            if (!File.Exists(ApplicationRuntime.LOGarqConvertidos)) { WpfMessageBox.Show("Nenhum arquivo convertido anteriormente foi localizado.", "Conversor Drawind"); return; }
            drawings.Clear();
            foreach (string file in File.ReadAllLines(ApplicationRuntime.LOGarqConvertidos).Where(File.Exists)) drawings.Add(file);
        }

        private void SelectConverterFromList()
        {
            if (ConverterView.ConvertersListBox.SelectedItem is string selected) { LoadConverter(selected); EditorView.ConverterComboBox.SelectedItem = selected; EditorView.ConverterNameTextBox.Text = selected; }
        }

        private void SelectConverterFromEditor()
        {
            if (!isInitializing && EditorView.ConverterComboBox.SelectedItem is string selected) { LoadConverter(selected); ConverterView.ConvertersListBox.SelectedItem = selected; EditorView.ConverterNameTextBox.Text = selected; }
        }

        private void LoadConverter(string converterName)
        {
            configuration = new global::ConversorDrawind.Configuration(); arranjos = new Arranjos(); listBlocks = new List<Block>(); listBlocksInv = new List<Block>(); listBlocksOrig = new List<Block>();
            ConverterFileService.LoadConverter(configuration, converterName, arranjos, listBlocks, listBlocksInv, listBlocksOrig, CurrentStatus);
            LoadConfigurationToControls();
        }

        private void NewConverter()
        {
            configuration = new global::ConversorDrawind.Configuration(); arranjos = new Arranjos(); listBlocks.Clear(); listBlocksInv.Clear(); listBlocksOrig.Clear(); EditorView.ConverterNameTextBox.Text = string.Empty; LoadConfigurationToControls();
        }

        private void SaveConverter()
        {
            string name = EditorView.ConverterNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { WpfMessageBox.Show("Informe o nome do conversor antes de salvar.", "Conversor Drawind"); return; }
            ReadConfigurationFromControls();
            ConverterFileService.SaveConverter(configuration, name, arranjos, listBlocks, listBlocksInv, listBlocksOrig, CurrentStatus);
            LoadConverterLists();
        }

        private void ImportConverter()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "Template XML (*.txml)|*.txml|XML (*.xml)|*.xml|Todos os arquivos (*.*)|*.*" };
            if (dialog.ShowDialog(this) != true) return;
            configuration.LoadXML(dialog.FileName, arranjos, listBlocks, listBlocksInv, listBlocksOrig, CurrentStatus);
            EditorView.ConverterNameTextBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            LoadConfigurationToControls();
        }

        private void ConvertSelectedDrawings()
        {
            ReadConfigurationFromControls();
            if (!(ConverterView.ConvertersListBox.SelectedItem is string selectedConverter)) { WpfMessageBox.Show("Selecione um conversor antes de converter.", "Conversor Drawind"); return; }
            if (drawings.Count == 0) { WpfMessageBox.Show("Adicione pelo menos um desenho antes de converter.", "Conversor Drawind"); return; }
            ConversionPreflightResult preflight = ConversionPreflightValidator.ValidateFormatPath(configuration);
            if (!preflight.CanConvert) { WpfMessageBox.Show("O formato configurado não foi encontrado:\n" + preflight.MissingFormatPath, "Conversor Drawind"); return; }
            ApplicationRuntime.ExtensaoGeral = Convert.ToString(ConverterView.ExtensionComboBox.Text);
            Param1 param = new Param1 { conversorName = selectedConverter, desenhosName = drawings.ToArray(), closedesenhos = ConverterView.KeepFilesOpenCheckBox.IsChecked != true, configuration = configuration, arranjos = arranjos, StatusConversorItem = CurrentStatus };
            using (Form_2_Processo processo = new Form_2_Processo(param)) processo.ShowDialog();
            if (!Form_2_Processo.IsCanceled) using (Form_2_ProcessoEnd processoEnd = new Form_2_ProcessoEnd()) processoEnd.ShowDialog();
        }

        private void ConfigureAdvancedDimensionArrow()
        {
            using (Form_7_ConfAvaACADaDeCota f = new Form_7_ConfAvaACADaDeCota(configuration.EXTDIMCorrigeSeta, configuration.EXTDIMCorrigeSetaTipoSeta, configuration.EXTDIMCorrigeSetaFactor)) if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK) { configuration.EXTDIMCorrigeSeta = f.EXTDIMCorrigeSeta; configuration.EXTDIMCorrigeSetaTipoSeta = f.EXTDIMCorrigeSetaTipoSeta; configuration.EXTDIMCorrigeSetaFactor = f.EXTDIMCorrigeSetaFactor; }
        }

        private void BrowseAttributedFormat()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "Desenhos CAD (*.dwg;*.dxf)|*.dwg;*.dxf|Todos os arquivos (*.*)|*.*" };
            if (dialog.ShowDialog(this) == true) EditorView.AttributedFormatPathTextBox.Text = dialog.FileName;
        }

        private void LoadConfigurationToControls()
        {
            ConverterView.TemplateCommentsTextBox.Text = configuration.EXTCONFComments ?? string.Empty;
            EditorView.AddCommentsTextBox.Text = configuration.EXTCONFComments ?? string.Empty;
            EditorView.AttributedFormatPathTextBox.Text = configuration.PROGRAMblockFormatoCaminho;
            EditorView.TeklaTextLayerComboBox.Text = configuration.LayerTeklaString;
            EditorView.FormatBlockLayerComboBox.Text = configuration.LayerBlockAttribute;
            EditorView.DimensionLayerComboBox.Text = configuration.EXTDIMlayer;
            EditorView.DimensionStyleTextBox.Text = configuration.EXTDIMStyleName;
            EditorView.ManualScaleRadio.IsChecked = configuration.EXTSCALEManual;
            EditorView.AutoScaleRadio.IsChecked = !configuration.EXTSCALEManual;
        }

        private void ReadConfigurationFromControls()
        {
            configuration.EXTCONFComments = EditorView.AddCommentsTextBox.Text;
            configuration.PROGRAMblockFormatoCaminho = EditorView.AttributedFormatPathTextBox.Text;
            configuration.LayerTeklaString = EditorView.TeklaTextLayerComboBox.Text;
            configuration.LayerBlockAttribute = EditorView.FormatBlockLayerComboBox.Text;
            configuration.EXTDIMlayer = EditorView.DimensionLayerComboBox.Text;
            configuration.EXTDIMStyleName = EditorView.DimensionStyleTextBox.Text;
            configuration.EXTSCALEManual = EditorView.ManualScaleRadio.IsChecked == true;
        }
    }
}


