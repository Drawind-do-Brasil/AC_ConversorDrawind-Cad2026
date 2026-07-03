using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerBaseDialog : Window
    {
        private readonly Arranjos arranjos;
        private bool initialLoadChecked;

        public LayerBaseDialog(string currentLayerBase, Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
            LayerBase = currentLayerBase;
            RefreshLayers(currentLayerBase);
        }

        public string LayerBase { get; private set; }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (initialLoadChecked)
            {
                return;
            }

            initialLoadChecked = true;
            if (arranjos.allBaseLayer.Count == 0)
            {
                LoadBaseLayersFromDrawing();
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            LoadBaseLayersFromDrawing();
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (arranjos.allBaseLayer.Contains(BaseLayerComboBox.Text))
            {
                LayerBase = BaseLayerComboBox.Text;
            }

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void LoadBaseLayersFromDrawing()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Drawing (*.dwg)|*.dwg"
                };

                if (openFileDialog.ShowDialog(this) != true)
                {
                    return;
                }

                Thread loadThread = new Thread(GetLayerDrawing);
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.Start(openFileDialog.FileName);

                Thread statusThread = new Thread(Form_0_JanelaPrincipal.ThreadMethodAnalisando);
                statusThread.SetApartmentState(ApartmentState.STA);
                statusThread.Start();

                loadThread.Join();
                Form_0_JanelaPrincipal.StopStatusThread(statusThread);

                ImportTempLayers();
                RefreshLayers(arranjos.allBaseLayer.FirstOrDefault() ?? LayerBase);
                Activate();
            }
            catch (Exception)
            {
            }
        }

        private static void GetLayerDrawing(object fileName)
        {
            try
            {
                ACAD.AcadApplication acadApplication;
                ACAD.AcadDocument acadDocument;
                using (MessageFilter.ScopedRegistration())
                {
                    string file = (string)fileName;
                    acadApplication = new ACAD.AcadApplication();
                    acadDocument = acadApplication.Documents.Open(file, false);
                }

                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.LoadFile(DrawingProcess.DLLPath1, acadDocument);
                }

                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.SendCommand("DRAWINDCAD_LoadLayer\n", acadDocument);
                    acadDocument.Close(false);
                    acadApplication.Quit();
                }
            }
            catch (Exception e)
            {
                Form_0_JanelaPrincipal.ControladorT = false;
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ImportTempLayers()
        {
            string filetxt = new global::ConversorDrawind.Configuration().GetPROGRAMDirectoryTemp() + "TempImporLayer.Temp";
            if (!File.Exists(filetxt))
            {
                return;
            }

            foreach (string line in File.ReadLines(filetxt, Encoding.UTF8))
            {
                string baseLayer = line.ToUpper();
                arranjos.allBaseLayer.Remove(baseLayer);
                arranjos.allBaseLayer.Add(baseLayer);
            }

            arranjos.allBaseLayer.Sort();
            File.Delete(filetxt);
        }

        private void RefreshLayers(string selectedLayer)
        {
            BaseLayerComboBox.ItemsSource = null;
            BaseLayerComboBox.ItemsSource = arranjos.allBaseLayer;
            BaseLayerComboBox.Text = selectedLayer;
        }
    }
}
