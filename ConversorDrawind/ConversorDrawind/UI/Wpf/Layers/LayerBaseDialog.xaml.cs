using ConversorDrawind;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerBaseDialog : Window
    {
        private readonly global::ConversorDrawind.Configuration configuration;
        private readonly List<string> layers;

        public LayerBaseDialog(string currentLayer, global::ConversorDrawind.Configuration configuration)
        {
            InitializeComponent();
            this.configuration = configuration ?? new global::ConversorDrawind.Configuration();
            this.configuration.EnsureDefaults();
            layers = this.configuration.Layers.NewLayers.Select(layer => layer.Name).ToList();
            BaseLayerComboBox.ItemsSource = layers;
            BaseLayerComboBox.Text = currentLayer;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            BaseLayerComboBox.Focus();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            Thread loadThread = new Thread(LoadLayersFromAcad);
            loadThread.SetApartmentState(ApartmentState.STA);
            loadThread.Start(dialog.FileName);
            loadThread.Join();

            layers.Clear();
            layers.AddRange(configuration.Layers.NewLayers.Select(layer => layer.Name));
            BaseLayerComboBox.ItemsSource = null;
            BaseLayerComboBox.ItemsSource = layers;
        }

        private static void LoadLayersFromAcad(object state)
        {
            try
            {
                string file = (string)state;
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
                    LoadFiles.SendCommand("DRAWINDCAD_LoadLayer\n", acadDocument);
                }
            }
            catch (Exception e)
            {
                ApplicationRuntime.ControladorT = false;
                System.Windows.MessageBox.Show(e.Message, Localization.TitleWarningNoExclamation, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            LayerBase = BaseLayerComboBox.Text;
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public string LayerBase { get; private set; }
    }
}
