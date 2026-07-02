using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind
{
    public partial class Form_4_LayersLayerBase : Form
    {
        private Arranjos arranjos = new Arranjos();
        public string layerBase;

        public Form_4_LayersLayerBase(string valor, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            layerBase = valor;
            InitializeComponent();
            if (arranjos.allBaseLayer.Count > 0)
            {
                LayerBaseLayers.Items.AddRange(arranjos.allBaseLayer.ToArray());
                LayerBaseLayers.Text = layerBase;
            }
            else
            {
                CarregarLayerBaseLayer();
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
                MessageBox.Show(new Form() { TopMost = true }, 
                                 e.Message,
                                 "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning,
                                 MessageBoxDefaultButton.Button1);
            }
        }


        private void CarregarLayerBaseLayer()
        {
            try
            {
                openFileDialog.Filter = "Drawing (*.dwg)|*.dwg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Thread newThread = new Thread(new ParameterizedThreadStart(GetLayerDrawing));
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start(openFileDialog.FileName);

                    Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAnalisando));
                    newThread2.SetApartmentState(ApartmentState.STA);
                    newThread2.Start();

                    newThread.Join();
                    Form_0_JanelaPrincipal.StopStatusThread(newThread2);

                    string filetxt = new Configuration().GetPROGRAMDirectoryTemp() + "TempImporLayer.Temp";
                    if (File.Exists(filetxt))
                    {
                        StreamReader streamReader = new StreamReader(filetxt, Encoding.UTF8, true);
                        while (!streamReader.EndOfStream)
                        {
                            string baselayer = streamReader.ReadLine().ToUpper();
                            arranjos.allBaseLayer.Remove(baselayer);
                            arranjos.allBaseLayer.Add(baselayer);
                        }
                        arranjos.allBaseLayer.Sort();
                        LayerBaseLayers.Items.Clear();
                        LayerBaseLayers.Items.AddRange(arranjos.allBaseLayer.ToArray());
                        LayerBaseLayers.Text = arranjos.allBaseLayer.First();
                        streamReader.Close();
                        File.Delete(filetxt);
                    }
                    this.Activate();
                }
            }
            catch (Exception)
            {
               
            }

        }

        private void LayerBaseProcurar_Click(object sender, EventArgs e)
        {
            CarregarLayerBaseLayer();
        }

        private void LayerBaseContinuar_Click(object sender, EventArgs e)
        {
            if (LayerBaseLayers.Items.Contains(LayerBaseLayers.Text))
                layerBase = LayerBaseLayers.Text;
            this.Close();
        }

        private void LayerBaseCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlLayerBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
