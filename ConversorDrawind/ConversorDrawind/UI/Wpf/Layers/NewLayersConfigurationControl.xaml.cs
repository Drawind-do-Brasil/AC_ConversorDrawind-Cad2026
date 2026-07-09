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
        private Arranjos arranjos;

        public NewLayersConfigurationControl()
        {
            InitializeComponent();
            Rows = new ObservableCollection<LayerRow>();
            DataContext = this;
        }

        public NewLayersConfigurationControl(Arranjos arranjos)
            : this()
        {
            LoadArranjos(arranjos);
        }

        public event EventHandler ConfigurationChanged;

        public ObservableCollection<LayerRow> Rows { get; }

        public void LoadArranjos(Arranjos arranjos)
        {
            this.arranjos = arranjos;
            LoadRows();
        }

        private void LoadRows()
        {
            Rows.Clear();
            if (arranjos == null)
            {
                return;
            }

            for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
            {
                string[] listTemp = arranjos.allNewLayerComposition[i].Split(':');
                if (listTemp.Length >= 3)
                {
                    Rows.Add(new LayerRow(listTemp[0], listTemp[1], listTemp[2]));
                }
            }

            if (Rows.Count == 0)
            {
                NewLayer novoLayer = new NewLayer(arranjos);
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

        public static void CheckLines(Arranjos arranjos)
        {
            var linhasErradas = new System.Collections.Generic.List<CorrecaoLinhas>();
            for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
            {
                string[] listTemp = arranjos.allNewLayerComposition[i].Split(':');
                bool lineOK = false;
                foreach (string line in arranjos.allLineType2)
                {
                    string[] lineTemp = line.Split(',');
                    if (listTemp.Last().ToUpper() == lineTemp.First().ToUpper())
                    {
                        lineOK = true;
                    }
                }

                if (!lineOK)
                {
                    CorrecaoLinhas linhaerrada = new CorrecaoLinhas();
                    linhaerrada.linha = arranjos.allNewLayerComposition[i];
                    linhaerrada.nomeLayer = listTemp[0];
                    linhaerrada.oldLinha = listTemp[2];
                    linhaerrada.posLinha = i;
                    linhasErradas.Add(linhaerrada);
                }
            }

            if (linhasErradas.Count > 0)
            {
                LinhasErradas formLinhasErradas = new LinhasErradas(linhasErradas, arranjos);
                formLinhasErradas.ShowDialog();
            }
        }

        public void OpenAcadLoadLayer()
        {
            if (arranjos == null)
            {
                return;
            }

            UpdateAllNewLayerComposition();
            UpdateAllNewLayer();
            ImportLayersFromAcad(false);
        }

        public void OpenAcadLoadLayerExterno()
        {
            if (arranjos == null)
            {
                return;
            }

            ImportLayersFromAcad(true);
        }

        public bool ApplyRowsToArranjos(bool showWarnings = true)
        {
            if (arranjos == null)
            {
                return false;
            }

            bool isRemove = false;
            arranjos.allNewLayerComposition.Clear();
            arranjos.allNewLayer.Clear();
            foreach (LayerRow row in Rows)
            {
                if (!arranjos.allNewLayer.Contains(row.Layer) || row.Layer == "")
                {
                    arranjos.allNewLayerComposition.Add(row.Layer + ":" + row.Color + ":" + row.Line);
                    arranjos.allNewLayer.Add(row.Layer);
                }
                else
                {
                    isRemove = true;
                }
            }

            if (!arranjos.allNewLayer.Contains("0"))
            {
                NewLayer novoLayer = new NewLayer(arranjos);
                novoLayer.SetConjuntoEspecial();
                arranjos.allNewLayerComposition.Add(novoLayer.layer + ":" + novoLayer.cor + ":" + novoLayer.tipoLinha.Split(',').First().ToUpper());
                arranjos.allNewLayer.Add(novoLayer.layer);
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

            arranjos.allNewLayer.Sort();
            arranjos.allNewLayerComposition.Sort();
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

                    string filetxt = new global::ConversorDrawind.Configuration().GetTempDirectory() + "TempImporNewLayer.Temp";
                    if (File.Exists(filetxt))
                    {
                        using (StreamReader streamReader = new StreamReader(filetxt, Encoding.UTF8, true))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                string lineLayer = streamReader.ReadLine().ToUpper();
                                string[] lineLayerArray = lineLayer.Split(':');
                                if (!arranjos.allNewLayer.Contains(lineLayerArray.First()))
                                {
                                    arranjos.allNewLayer.Add(lineLayerArray.First());
                                    arranjos.allNewLayerComposition.Remove(lineLayer);
                                    arranjos.allNewLayerComposition.Add(lineLayer);
                                }
                                else if (lineLayerArray.First() == "0")
                                {
                                    arranjos.allNewLayer.Remove(lineLayerArray.First());
                                    arranjos.allNewLayer.Add(lineLayerArray.First());
                                    bool exists = false;
                                    for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
                                    {
                                        string[] lineLayerArray2 = arranjos.allNewLayerComposition[i].Split(':');
                                        if (lineLayerArray2.First() == "0")
                                        {
                                            arranjos.allNewLayerComposition[i] = lineLayer;
                                            exists = true;
                                            break;
                                        }
                                    }

                                    if (externalMode && !exists)
                                    {
                                        arranjos.allNewLayerComposition.Add(lineLayer);
                                    }
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
                                        for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
                                        {
                                            string[] lineLayerArray2 = arranjos.allNewLayerComposition[i].Split(':');
                                            if (lineLayerArray2.First() == lineLayerArray.First())
                                            {
                                                arranjos.allNewLayerComposition[i] = lineLayer;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isAddAll = false;
                                    }
                                }
                            }
                        }

                        File.Delete(filetxt);
                        CheckLines(arranjos);
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

        private void UpdateAllNewLayerComposition()
        {
            arranjos.allNewLayerComposition.Clear();
            foreach (LayerRow row in Rows)
            {
                arranjos.allNewLayerComposition.Add(row.Layer + ":" + row.Color + ":" + row.Line);
            }
        }

        private void UpdateAllNewLayer()
        {
            arranjos.allNewLayer.Clear();
            foreach (LayerRow row in Rows)
            {
                arranjos.allNewLayer.Add(row.Layer);
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            OpenAcadLoadLayer();
        }

        private void ClearAllClick(object sender, RoutedEventArgs e)
        {
            if (arranjos == null || Rows.Count == 0 || LayersGrid.SelectedIndex == -1)
            {
                return;
            }

            if (MessageBox.Show(
                    Localization.MessageClearAll,
                    Localization.TitleAttentionPlain,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                arranjos.allNewLayerComposition.Clear();
                arranjos.allNewLayer.Clear();
                arranjos.allNewLayer.Add("0");
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
                    ApplyRowsToArranjos(false);
                }
            }
        }

        private void AddRowClick(object sender, RoutedEventArgs e)
        {
            if (arranjos == null)
            {
                return;
            }

            int novosNomes = 0;
            bool isOK = false;
            string name = "";
            while (!isOK)
            {
                ++novosNomes;
                UpdateAllNewLayer();
                if (!arranjos.allNewLayer.Contains("0"))
                {
                    isOK = true;
                    name = "0";
                }
                else if (!arranjos.allNewLayer.Contains("NovoLayer_" + novosNomes))
                {
                    isOK = true;
                    name = "NovoLayer_" + novosNomes;
                }
            }

            if (Rows.Count == 0)
            {
                NewLayer novoLayer = new NewLayer(arranjos);
                novoLayer.SetConjuntoEspecial();
                Rows.Add(new LayerRow(name, novoLayer.cor, novoLayer.tipoLinha.Split(',').First().ToUpper()));
            }
            else if (LayersGrid.SelectedItem is LayerRow selected && selected.Layer != "")
            {
                Rows.Add(new LayerRow(name, selected.Color, selected.Line));
            }

            LayersGrid.SelectedIndex = Rows.Count - 1;
            ApplyRowsToArranjos(false);
        }

        private void LayersGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (arranjos == null || LayersGrid.CurrentItem is not LayerRow row || LayersGrid.CurrentColumn == null)
            {
                return;
            }

            int columnIndex = LayersGrid.Columns.IndexOf(LayersGrid.CurrentColumn);
            if (columnIndex == 0)
            {
                ConfigurarLayersNome dialog = new ConfigurarLayersNome(row.Layer, arranjos);
                dialog.ShowDialog();
                row.Layer = dialog.nome;
                dialog.Dispose();
            }
            else if (columnIndex == 1)
            {
                ConfigurarLayersCor dialog = new ConfigurarLayersCor(row.Color, arranjos);
                dialog.ShowDialog();
                row.Color = dialog.cor;
                dialog.Dispose();
            }
            else if (columnIndex == 2)
            {
                ConfigurarLayersLinha dialog = new ConfigurarLayersLinha(row.Line, arranjos);
                dialog.ShowDialog();
                row.Line = dialog.linha.Split(',').First().ToUpper();
                dialog.Dispose();
            }

            ApplyRowsToArranjos(false);
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
