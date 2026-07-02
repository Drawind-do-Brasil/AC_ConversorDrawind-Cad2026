using Autodesk.AutoCAD.Interop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ACAD = Autodesk.AutoCAD.Interop;


namespace ConversorDrawind
{
    public partial class Form_3_ConfigurarLayers : Form
    {
        public Class_Arranjos arranjos = new Class_Arranjos();

        public Form_3_ConfigurarLayers(Class_Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
        }

        private void NewLayerConfiguration_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dbNewLayersDataSet.Tabela1' table. You can move, or remove it, as needed.
            CarregardGVNewLayers();
            dGVNewLayers.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dGVNewLayers.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dGVNewLayers.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void CarregardGVNewLayers()
        {
            dGVNewLayers.Rows.Clear();
            for (int i = 0; i < this.arranjos.allNewLayerComposition.Count; i++)
            {
                string[] listTemp = this.arranjos.allNewLayerComposition[i].Split(':');

                dGVNewLayers.Rows.Add(listTemp[0], listTemp[1], listTemp[2]);

            }
            if (dGVNewLayers.Rows.Count == 0)
            {
                Class_NewLayer novoLayer = new Class_NewLayer(this.arranjos);
                novoLayer.SetConjuntoEspecial();

                dGVNewLayers.Rows.Add(novoLayer.layer, novoLayer.cor, novoLayer.tipoLinha.Split(',').First().ToUpper());
            }
        }


        private void CarregarNewLayer(object fileName)
        {
            try
            {
                string file = (string)fileName;
                ACAD.AcadApplication acadApplication;
                ACAD.AcadDocument acadDocument;
                using (Class_MessageFilter.ScopedRegistration())
                {
                    acadApplication = new ACAD.AcadApplication();
                    acadDocument = Class_ComRetry.Invoke(() => acadApplication.Documents.Open(file, false), 120, 100);
                }
                using (Class_MessageFilter.ScopedRegistration())
                {
                    LoadFiles.LoadFile(Class_DrawingProcess.DLLPath1, acadDocument);
                }
                using (Class_MessageFilter.ScopedRegistration())
                {
                    LoadFiles.SendCommand("DRAWINDCAD_NewLayer\n", acadDocument);

                    /*acadDocument.Close(false);
                    acadApplication.Quit();*/

                }
            }
            catch (Exception e)
            {
                Form_0_JanelaPrincipal.ControladorT = false;
                MessageBox.Show(new Form() { TopMost = true}, 
                                  e.Message,
                                 "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning,
                                 MessageBoxDefaultButton.Button1);
            }
        }


        public void CheckLines()
        {
            List<Class_CorrecaoLinhas> linhasErradas = new List<Class_CorrecaoLinhas>();
            for (int i = 0; i <  arranjos.allNewLayerComposition.Count; i++)
            {
                string[] listTemp = arranjos.allNewLayerComposition[i].Split(':');
                bool lineOK = false;
                foreach(var line in arranjos.allLineType2)
                {
                    string[] lineTemp = line.Split(',');
                    if (listTemp.Last().ToUpper() == lineTemp.First().ToUpper())
                        lineOK = true;
                }
                if (!lineOK)
                {
                    Class_CorrecaoLinhas linhaerrada = new Class_CorrecaoLinhas();
                    linhaerrada.linha = arranjos.allNewLayerComposition[i];
                    linhaerrada.nomeLayer = listTemp[0];
                    linhaerrada.oldLinha = listTemp[2];
                    linhaerrada.posLinha = i;
                    linhasErradas.Add(linhaerrada);
                }
            }

            if (linhasErradas.Count > 0)
            {
                Form_3_LinhasErradas formLinhasErradas = new Form_3_LinhasErradas(linhasErradas, arranjos);
                formLinhasErradas.ShowDialog();
            }
        }

        public void OpenAcadLoadLayer()
        {
            AtualizarAllNewLayerComposition();
            AtualizarAllNewLayer();
            bool IsAddAll = true;
            bool IsCheck = false;
            bool IsUpdate = false;
            try
            {
                openFileDialog.Filter = "Drawing (*.dwg)|*.dwg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Thread newThread = new Thread(new ParameterizedThreadStart(CarregarNewLayer));
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start(openFileDialog.FileName);

                    Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAnalisando));
                    newThread2.SetApartmentState(ApartmentState.STA);
                    newThread2.Start();

                    newThread.Join();
                    Form_0_JanelaPrincipal.StopStatusThread(newThread2);

                    string filetxt = new Class_Configuration().GetPROGRAMDirectoryTemp() + "TempImporNewLayer.Temp";
                    if (File.Exists(filetxt))
                    {
                        StreamReader streamReader = new StreamReader(filetxt, Encoding.UTF8, true);
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

                                for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
                                {
                                     string[] lineLayerArray2 = arranjos.allNewLayerComposition[i].Split(':');
                                     if (lineLayerArray2.First() == "0")
                                     {
                                         arranjos.allNewLayerComposition[i] = lineLayer;
                                         break;
                                     }
                                }
                            }
                            else
                            {
                                if (!IsCheck)
                                {
                                    if (MessageBox.Show("Deseja atualizar os layer que já existem?\nObservação: O layer '0' sempre é atualizado.",
                                                        "Atenção",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                    {
                                        IsUpdate = true;
                                    }

                                    IsCheck = true;
                                }

                                if (IsUpdate)
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
                                    IsAddAll = false;
                                }
                            }
                        }

                        streamReader.Close();
                        File.Delete(filetxt);

                        CheckLines();
                    }
                }
            }
            catch (Exception)
            {

            }
            CarregardGVNewLayers();
            if (!IsAddAll)
            {
                MessageBox.Show("Alguns layers não foram adicionados, porque existem layers com o mesmo nome na lista.",
                          "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning,
                          MessageBoxDefaultButton.Button1);
            }
        }

        public void OpenAcadLoadLayerExterno()
        {
            bool IsAddAll = true;
            bool IsCheck = false;
            bool IsUpdate = false;
            try
            {
                openFileDialog.Filter = "Drawing (*.dwg)|*.dwg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Thread newThread = new Thread(new ParameterizedThreadStart(CarregarNewLayer));
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start(openFileDialog.FileName);

                    Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAnalisando));
                    newThread2.SetApartmentState(ApartmentState.STA);
                    newThread2.Start();

                    newThread.Join();
                    Form_0_JanelaPrincipal.StopStatusThread(newThread2);

                    string filetxt = new Class_Configuration().GetPROGRAMDirectoryTemp() + "TempImporNewLayer.Temp";
                    if (File.Exists(filetxt))
                    {
                        StreamReader streamReader = new StreamReader(filetxt, Encoding.UTF8, true);
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
                                bool estaEmAllNewLayerComposition = false;
                                for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
                                {
                                    string[] lineLayerArray2 = arranjos.allNewLayerComposition[i].Split(':');
                                    if (lineLayerArray2.First() == "0")
                                    {
                                        arranjos.allNewLayerComposition[i] = lineLayer;
                                        estaEmAllNewLayerComposition = true;
                                        break;
                                    }
                                }
                                if (!estaEmAllNewLayerComposition)
                                    arranjos.allNewLayerComposition.Add(lineLayer);
                            }
                            else
                            {
                                if (!IsCheck)
                                {
                                    if (MessageBox.Show("Deseja atualizar os layer que já existem?\nObservação: O layer '0' sempre é atualizado.",
                                                        "Atenção",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                    {
                                        IsUpdate = true;
                                    }

                                    IsCheck = true;
                                }

                                if (IsUpdate)
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
                                    IsAddAll = false;
                                }
                            }
                        }

                        streamReader.Close();
                        File.Delete(filetxt);

                        CheckLines();
                    }
                }
            }
            catch (Exception)
            {

            }
            CarregardGVNewLayers();
            if (!IsAddAll)
            {
                MessageBox.Show("Alguns layers não foram adicionados, porque existem layers com o mesmo nome na lista.",
                          "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning,
                          MessageBoxDefaultButton.Button1);
            }
        }

        private void AtualizarAllNewLayerComposition()
        {
            this.arranjos.allNewLayerComposition.Clear();
            for (int i = 0; i < dGVNewLayers.Rows.Count; i++)
            {
                string temp = dGVNewLayers.Rows[i].Cells[0].Value.ToString() + ":" +
                                 dGVNewLayers.Rows[i].Cells[1].Value.ToString() + ":" +
                                 dGVNewLayers.Rows[i].Cells[2].Value.ToString();
                this.arranjos.allNewLayerComposition.Add(temp);
            }
        }

        private void AtualizarAllNewLayer()
        {
            this.arranjos.allNewLayer.Clear();
            for (int i = 0; i < dGVNewLayers.Rows.Count; i++)
            {
                this.arranjos.allNewLayer.Add(dGVNewLayers.Rows[i].Cells[0].Value.ToString());
            }
        }

        private void NewLayerConfiguration_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void bCarregar_Click(object sender, EventArgs e)
        {
            OpenAcadLoadLayer();
        }

        private void bLimparTudo_Click(object sender, EventArgs e)
        {
            if (dGVNewLayers.RowCount > 0 && dGVNewLayers.CurrentRow.Index != -1)
            {
                if (MessageBox.Show("Deseja realmente limpar tudo?",
                                    "Atenção",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    arranjos.allNewLayerComposition.Clear();
                    arranjos.allNewLayer.Clear();
                    arranjos.allNewLayer.Add("0");
                    dGVNewLayers.Rows.Clear();
                }
            }
        }

        private void bContinuar_Click(object sender, EventArgs e)
        {
            bool IsRemove = false;
            this.arranjos.allNewLayerComposition.Clear();
            this.arranjos.allNewLayer.Clear();
            for (int i = 0; i < dGVNewLayers.Rows.Count; i++)
            {
                if (!this.arranjos.allNewLayer.Contains(dGVNewLayers.Rows[i].Cells[0].Value.ToString()) ||
                    dGVNewLayers.Rows[i].Cells[0].Value.ToString() == "")
                {
                    string temp = dGVNewLayers.Rows[i].Cells[0].Value.ToString() + ":" +
                                  dGVNewLayers.Rows[i].Cells[1].Value.ToString() + ":" +
                                  dGVNewLayers.Rows[i].Cells[2].Value.ToString();
                    this.arranjos.allNewLayerComposition.Add(temp);
                    this.arranjos.allNewLayer.Add(dGVNewLayers.Rows[i].Cells[0].Value.ToString());
                }
                else
                {
                    IsRemove = true;
                }
            }
            if (!this.arranjos.allNewLayer.Contains("0"))
            {
                Class_NewLayer novoLayer = new Class_NewLayer(this.arranjos);
                novoLayer.SetConjuntoEspecial();
                arranjos.allNewLayerComposition.Add(novoLayer.layer + ":" +
                                                    novoLayer.cor + ":" +
                                                    novoLayer.tipoLinha.Split(',').First().ToUpper());
                arranjos.allNewLayer.Add(novoLayer.layer);
                MessageBox.Show("O layer 0 deve sempre existir, portanto ele foi adicionado a lista.",
                 "Error",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning,
                 MessageBoxDefaultButton.Button1);
            }
            /*
            if (this.arranjos.allNewLayer.Count == 0)
            {
                arranjos.allNewLayer.Add("0");
            }*/
            this.Close();
            if (IsRemove)
            {
                MessageBox.Show("Alguns layers repetidos ou sem nome foram removidos da lista",
                                 "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning,
                                 MessageBoxDefaultButton.Button1);
            }

            arranjos.allNewLayer.Sort();
            arranjos.allNewLayerComposition.Sort();
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bExcluirLinha_Click(object sender, EventArgs e)
        {
            if (dGVNewLayers.RowCount > 0 && dGVNewLayers.CurrentRow.Index != -1)
            {
                if (MessageBox.Show("Deseja realmente excluir a linha selecionada?",
                                    "Atenção",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    dGVNewLayers.Rows.RemoveAt(dGVNewLayers.CurrentRow.Index);
                }
            }
        }

        private void bincluirLinha_Click(object sender, EventArgs e)
        {
            int novosNomes = 0;
            bool IsOK = false;
            string name = "";
            while (!IsOK)
            {
                ++novosNomes;
                AtualizarAllNewLayer();
                if (!arranjos.allNewLayer.Contains("0"))
                {
                    IsOK = true;
                    name = "0";
                }
                else if (!arranjos.allNewLayer.Contains("NovoLayer_" + novosNomes))
                {
                    IsOK = true;
                    name = "NovoLayer_" + novosNomes;
                }
            }

            if (dGVNewLayers.Rows.Count == 0)
            {
                Class_NewLayer novoLayer = new Class_NewLayer(this.arranjos);
                novoLayer.SetConjuntoEspecial();
                dGVNewLayers.Rows.Add(name, novoLayer.cor, novoLayer.tipoLinha.Split(',').First().ToUpper());
    
            }
            else if (dGVNewLayers.Rows[dGVNewLayers.CurrentRow.Index].Cells[0].Value.ToString() != "")
            {
                dGVNewLayers.Rows.Add(name, 
                    dGVNewLayers.Rows[dGVNewLayers.CurrentRow.Index].Cells[1].Value.ToString(),
                    dGVNewLayers.Rows[dGVNewLayers.CurrentRow.Index].Cells[2].Value.ToString());

            }
            dGVNewLayers.CurrentCell = dGVNewLayers.Rows[dGVNewLayers.Rows.Count - 1].Cells[0];

            AtualizarAllNewLayer();
        }

        private void dGVNewLayers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (e.ColumnIndex == 0)
                {
                    string valor = string.Empty;
                    if (dGVNewLayers.Rows.Count != 0)
                        valor = dGVNewLayers.Rows[dGVNewLayers.CurrentRow.Index].Cells[0].Value.ToString();
                    Form_3_ConfigurarLayersNome mynome = new Form_3_ConfigurarLayersNome(valor, this.arranjos);
                    mynome.ShowDialog();
                    dGVNewLayers.Rows[e.RowIndex].Cells[0].Value = mynome.nome;
                    AtualizarAllNewLayer();
                    mynome.Dispose();
                }
                if (e.ColumnIndex == 1)
                {
                    Form_3_ConfigurarLayersCor myCor = new Form_3_ConfigurarLayersCor(dGVNewLayers.Rows[dGVNewLayers.CurrentRow.Index].Cells[1].Value.ToString(), arranjos);
                    myCor.ShowDialog();
                    dGVNewLayers.Rows[e.RowIndex].Cells[1].Value = myCor.cor;
                    myCor.Dispose();
                }
                if (e.ColumnIndex == 2)
                {
                    Form_3_ConfigurarLayersLinha myLinha = new Form_3_ConfigurarLayersLinha(dGVNewLayers.Rows[dGVNewLayers.CurrentRow.Index].Cells[2].Value.ToString(), arranjos);
                    myLinha.ShowDialog();
                    dGVNewLayers.Rows[e.RowIndex].Cells[2].Value = myLinha.linha.Split(',').First().ToUpper();
                    myLinha.Dispose();
                }
            }
        }
    }
}
