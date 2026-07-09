using ConversorDrawind;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class NewLayersConfigurationControl : UserControl
    {
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();

        public NewLayersConfigurationControl()
        {
            InitializeComponent();
            Rows = new ObservableCollection<LayerRow>();
            DataContext = this;
        }

        public event EventHandler ConfigurationChanged;

        public ObservableCollection<LayerRow> Rows { get; }

        public void LoadConfiguration(global::ConversorDrawind.Configuration configuration)
        {
            this.configuration = configuration ?? new global::ConversorDrawind.Configuration();
            this.configuration.EnsureDefaults();
            LoadRows();
        }

        private void LoadRows()
        {
            Rows.Clear();
            for (int i = 0; i < configuration.Layers.NewLayers.Count; i++)
            {
                LayerDefinition layer = configuration.Layers.NewLayers[i];
                if (!string.IsNullOrWhiteSpace(layer.Name))
                {
                    Rows.Add(new LayerRow(layer.Name, layer.Color, layer.LineType));
                }
            }

            if (Rows.Count == 0)
            {
                NewLayer novoLayer = new NewLayer(configuration);
                novoLayer.SetConjuntoEspecial();
                Rows.Add(new LayerRow(novoLayer.layer, novoLayer.cor, novoLayer.tipoLinha.Split(',').First().ToUpper()));
            }
        }

        private static void LoadNewLayerFromAcad(object fileName)
        {
            try
            {
                string file = (string)fileName;
                ACAD.AcadApplication acadApplication;
                ACAD.AcadDocument acadDocument;
                using (MessageFilter.ScopedRegistration())
                {
                    acadApplication = new ACAD.AcadApplication();
                    acadDocument = ComRetry.Invoke(() => acadApplication.Documents.Open(file, false), 120, 100);
                }

                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.LoadFile(DrawingProcess.DLLPath1, acadDocument);
                }

                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.SendCommand("DRAWINDCAD_NewLayer\n", acadDocument);
                }
            }
            catch (Exception e)
            {
                ApplicationRuntime.ControladorT = false;
                MessageBox.Show(e.Message, Localization.TitleWarningNoExclamation, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void CheckLines()
        {
            var linhasErradas = new System.Collections.Generic.List<CorrecaoLinhas>();
            ApplyRowsToConfiguration(false);
            for (int i = 0; i < configuration.Layers.NewLayers.Count; i++)
            {
                LayerDefinition layer = configuration.Layers.NewLayers[i];
                bool lineOK = false;
                foreach (string line in configuration.Catalogs.LayerLineTypes)
                {
                    string[] lineTemp = line.Split(',');
                    if ((layer.LineType ?? string.Empty).ToUpper() == lineTemp.First().ToUpper())
                    {
                        lineOK = true;
                    }
                }

                if (!lineOK)
                {
                    CorrecaoLinhas linhaerrada = new CorrecaoLinhas();
                    linhaerrada.linha = LegacyConfigurationParsers.FormatLayerDefinition(layer);
                    linhaerrada.nomeLayer = layer.Name;
                    linhaerrada.oldLinha = layer.LineType;
                    linhaerrada.posLinha = i;
                    linhasErradas.Add(linhaerrada);
                }
            }

            if (linhasErradas.Count > 0)
            {
                LinhasErradas formLinhasErradas = new LinhasErradas(linhasErradas, configuration);
                formLinhasErradas.ShowDialog();
                LoadRows();
            }
        }

        public void OpenAcadLoadLayer()
        {
            ApplyRowsToConfiguration(false);
            ImportLayersFromAcad(false);
        }

        public void OpenAcadLoadLayerExterno()
        {
            ImportLayersFromAcad(true);
        }

        public bool ApplyRowsToConfiguration(bool showWarnings = true)
        {
            bool isRemove = false;
            configuration.Layers.NewLayers.Clear();
            var names = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (LayerRow row in Rows)
            {
                if (names.Add(row.Layer) || row.Layer == "")
                {
                    configuration.Layers.NewLayers.Add(new LayerDefinition { Name = row.Layer, Color = row.Color, LineType = row.Line });
                }
                else
                {
                    isRemove = true;
                }
            }

            if (!configuration.Layers.NewLayers.Any(layer => string.Equals(layer.Name, "0", StringComparison.OrdinalIgnoreCase)))
            {
                NewLayer novoLayer = new NewLayer(configuration);
                novoLayer.SetConjuntoEspecial();
                configuration.Layers.NewLayers.Add(new LayerDefinition
                {
                    Name = novoLayer.layer,
                    Color = novoLayer.cor,
                    LineType = novoLayer.tipoLinha.Split(',').First().ToUpper()
                });
                if (showWarnings)
                {
                    MessageBox.Show(
                        Localization.MessageLayerZeroAdded,
                        Localization.TitleWarningNoExclamation,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }

            if (isRemove && showWarnings)
            {
                MessageBox.Show(
                    Localization.MessageDuplicateOrUnnamedLayersRemoved,
                    Localization.TitleWarningNoExclamation,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            configuration.Layers.NewLayers = configuration.Layers.NewLayers
                .OrderBy(layer => layer.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(layer => layer.Color, StringComparer.OrdinalIgnoreCase)
                .ThenBy(layer => layer.LineType, StringComparer.OrdinalIgnoreCase)
                .ToList();
            LoadRows();
            NotifyConfigurationChanged();
            return true;
        }

        private void ImportLayersFromAcad(bool externalMode)
        {
            bool isAddAll = true;
            bool isCheck = false;
            bool isUpdate = false;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = Localization.FilterDrawing };
                if (openFileDialog.ShowDialog(Window.GetWindow(this)) == true)
                {
                    Thread loadThread = new Thread(new ParameterizedThreadStart(LoadNewLayerFromAcad));
                    loadThread.SetApartmentState(ApartmentState.STA);
                    loadThread.Start(openFileDialog.FileName);

                    Thread statusThread = new Thread(new ThreadStart(ApplicationRuntime.ThreadMethodAnalisando));
                    statusThread.SetApartmentState(ApartmentState.STA);
                    statusThread.Start();

                    loadThread.Join();
                    ApplicationRuntime.StopStatusThread(statusThread);

                    string filetxt = configuration.GetTempDirectory() + "TempImporNewLayer.Temp";
                    if (File.Exists(filetxt))
                    {
                        using (StreamReader streamReader = new StreamReader(filetxt, Encoding.UTF8, true))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                string lineLayer = streamReader.ReadLine().ToUpper();
                                string[] lineLayerArray = lineLayer.Split(':');
                                LayerDefinition existing = FindLayer(lineLayerArray.First());
                                if (existing == null)
                                {
                                    configuration.Layers.NewLayers.Add(LegacyConfigurationParsers.ParseLayerDefinition(lineLayer));
                                }
                                else if (lineLayerArray.First() == "0")
                                {
                                    UpdateLayer(existing, lineLayer);
                                }
                                else
                                {
                                    if (!isCheck)
                                    {
                                        if (MessageBox.Show(
                                                Localization.MessageUpdateExistingLayers,
                                                Localization.TitleAttentionPlain,
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                                        {
                                            isUpdate = true;
                                        }

                                        isCheck = true;
                                    }

                                    if (isUpdate)
                                    {
                                        UpdateLayer(existing, lineLayer);
                                    }
                                    else
                                    {
                                        isAddAll = false;
                                    }
                                }
                            }
                        }

                        File.Delete(filetxt);
                        CheckLines();
                    }
                }
            }
            catch (Exception)
            {
            }

            LoadRows();
            NotifyConfigurationChanged();
            if (!isAddAll)
            {
                MessageBox.Show(
                    Localization.MessageSomeLayersNotAdded,
                    Localization.TitleWarningNoExclamation,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private LayerDefinition FindLayer(string name)
        {
            return configuration.Layers.NewLayers
                .FirstOrDefault(layer => string.Equals(layer.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private static void UpdateLayer(LayerDefinition target, string rawLayer)
        {
            LayerDefinition imported = LegacyConfigurationParsers.ParseLayerDefinition(rawLayer);
            target.Name = imported.Name;
            target.Color = imported.Color;
            target.LineType = imported.LineType;
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            OpenAcadLoadLayer();
        }

        private void ClearAllClick(object sender, RoutedEventArgs e)
        {
            if (Rows.Count == 0 || LayersGrid.SelectedIndex == -1)
            {
                return;
            }

            if (MessageBox.Show(
                    Localization.MessageClearAll,
                    Localization.TitleAttentionPlain,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                configuration.Layers.NewLayers.Clear();
                Rows.Clear();
                NotifyConfigurationChanged();
            }
        }

        private void DeleteRowClick(object sender, RoutedEventArgs e)
        {
            if (Rows.Count > 0 && LayersGrid.SelectedItem is LayerRow row)
            {
                if (MessageBox.Show(
                        Localization.MessageDeleteSelectedRow,
                        Localization.TitleAttentionPlain,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    Rows.Remove(row);
                    ApplyRowsToConfiguration(false);
                }
            }
        }

        private void AddRowClick(object sender, RoutedEventArgs e)
        {
            int novosNomes = 0;
            bool isOK = false;
            string name = "";
            while (!isOK)
            {
                ++novosNomes;
                var layerNames = Rows.Select(row => row.Layer).ToList();
                if (!layerNames.Contains("0"))
                {
                    isOK = true;
                    name = "0";
                }
                else if (!layerNames.Contains("NovoLayer_" + novosNomes))
                {
                    isOK = true;
                    name = "NovoLayer_" + novosNomes;
                }
            }

            if (Rows.Count == 0)
            {
                NewLayer novoLayer = new NewLayer(configuration);
                novoLayer.SetConjuntoEspecial();
                Rows.Add(new LayerRow(name, novoLayer.cor, novoLayer.tipoLinha.Split(',').First().ToUpper()));
            }
            else if (LayersGrid.SelectedItem is LayerRow selected && selected.Layer != "")
            {
                Rows.Add(new LayerRow(name, selected.Color, selected.Line));
            }

            LayersGrid.SelectedIndex = Rows.Count - 1;
            ApplyRowsToConfiguration(false);
        }

        private void LayersGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LayersGrid.CurrentItem is not LayerRow row || LayersGrid.CurrentColumn == null)
            {
                return;
            }

            int columnIndex = LayersGrid.Columns.IndexOf(LayersGrid.CurrentColumn);
            if (columnIndex == 0)
            {
                ConfigurarLayersNome dialog = new ConfigurarLayersNome(row.Layer, configuration);
                dialog.ShowDialog();
                row.Layer = dialog.nome;
                dialog.Dispose();
            }
            else if (columnIndex == 1)
            {
                ConfigurarLayersCor dialog = new ConfigurarLayersCor(row.Color, configuration);
                dialog.ShowDialog();
                row.Color = dialog.cor;
                dialog.Dispose();
            }
            else if (columnIndex == 2)
            {
                ConfigurarLayersLinha dialog = new ConfigurarLayersLinha(row.Line, configuration);
                dialog.ShowDialog();
                row.Line = dialog.linha.Split(',').First().ToUpper();
                dialog.Dispose();
            }

            ApplyRowsToConfiguration(false);
        }

        private void NotifyConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        public class LayerRow : INotifyPropertyChanged
        {
            private string layer;
            private string color;
            private string line;

            public LayerRow(string layer, string color, string line)
            {
                this.layer = layer;
                this.color = color;
                this.line = line;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Layer
            {
                get { return layer; }
                set { layer = value; OnPropertyChanged(); }
            }

            public string Color
            {
                get { return color; }
                set { color = value; OnPropertyChanged(); }
            }

            public string Line
            {
                get { return line; }
                set { line = value; OnPropertyChanged(); }
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
