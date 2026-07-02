using Autodesk.AutoCAD.Interop;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind
{
    public partial class Form_4_LayersFilter : Form
    {
        private Arranjos arranjos = new Arranjos();
        public Filter filtro;

        public Form_4_LayersFilter(string valor, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            filtro = new Filter(arranjos);
            InitializeComponent();
            CarregarControlFilterCBCor();
            CarregarControlFilterCBTipo();
            CarregarControlFilterCBLinha();
            filtro.SetConjunto(valor);
            CarregarConfiguracao();
        }

        private void CarregarControlFilterCBCor()
        {
            ControlFilterCBCor.Text = arranjos.allcolor.First();
            ControlFilterCBCor.Items.Clear();
            ControlFilterCBCor.Items.AddRange(arranjos.allcolor.ToArray());
        }

        private void CarregarControlFilterCBLinha()
        {
            ControlFilterCBLinha.Text = arranjos.allLineType1.First();
            ControlFilterCBLinha.Items.Clear();
            ControlFilterCBLinha.Items.AddRange(arranjos.allLineType1.ToArray());
        }

        public void CarregarControlFilterCBLinhaTipo2(string line)
        {
            ControlFilterCBLinha.Text = "ALL";
            ControlFilterCBLinha.Items.Clear();
            ControlFilterCBLinha.Items.Add("ALL");
            ControlFilterCBLinha.Items.AddRange(arranjos.allLineType2.ToArray());

            Filter filtro = new Filter(arranjos);
            filtro.SetConjunto(line);
            for (int i = 0; i < ControlFilterCBLinha.Items.Count; i++)
            {
                if (ControlFilterCBLinha.Items[i].ToString().Split(',').First().ToUpper() == filtro.tipoLinha)
                {
                    ControlFilterCBLinha.Text = ControlFilterCBLinha.Items[i].ToString();
                    break;
                }
            }
        }

        public void CarregarConfiguracao()
        {
            ControlFilterCBCor.Text = filtro.cor;
            ControlFilterCBTipo.Text = filtro.tipoObjeto;
            ControlFilterCBLinha.Text = filtro.tipoLinha;
            ControlFilterLTextoAltura.Text = filtro.alturaTexto;
            ControlFilterLTextoConteudo.Text = filtro.conteudoTexto;
            ControlFilterCBOrientacao.Text = filtro.orientacao;
        }

        private void CarregarControlFilterCBTipo()
        {
            ControlFilterCBTipo.Text = arranjos.allobjects.First();
            ControlFilterCBTipo.Items.AddRange(arranjos.allobjects.ToArray());
        }

        private void ControlFilterBCodigoCor_Click(object sender, EventArgs e)
        {
            string color = string.Empty;
            Form_8_GenericNewColor newColor = new Form_8_GenericNewColor(ControlFilterCBCor.Text);
            newColor.ShowDialog();
            if (!arranjos.allcolor.Contains(newColor.colorClass))
            {
                arranjos.allcolor.Add(newColor.colorClass);
                ControlFilterCBCor.Items.Add(newColor.colorClass);
            }
            ControlFilterCBCor.Text = newColor.colorClass;
            newColor.Dispose();
        }

        private void ControlFilterBContinuar_Click(object sender, EventArgs e)
        {
            if (ControlFilterCBCor.Items.Contains(ControlFilterCBCor.Text))
                filtro.cor = ControlFilterCBCor.Text;
            if (ControlFilterCBLinha.Items.Contains(ControlFilterCBLinha.Text))
                filtro.tipoLinha = ControlFilterCBLinha.Text.Split(',').First().ToUpper();
            if (ControlFilterCBTipo.Items.Contains(ControlFilterCBTipo.Text))
                filtro.tipoObjeto = ControlFilterCBTipo.Text;

            filtro.alturaTexto = ControlFilterLTextoAltura.Text;
            filtro.conteudoTexto = ControlFilterLTextoConteudo.Text;
            filtro.orientacao = ControlFilterCBOrientacao.Text;
            this.Close();
        }

        private void ControlFilterBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlFilterBAddOntrasEntidades_Click(object sender, EventArgs e)
        {
            Form_4_LayersNewEntity newEntity = new Form_4_LayersNewEntity(ControlFilterCBTipo.Text);
            newEntity.ShowDialog();
            arranjos.allobjects.Remove(newEntity.entidade);
            ControlFilterCBTipo.Items.Remove(newEntity.entidade);
            arranjos.allobjects.Add(newEntity.entidade);
            ControlFilterCBTipo.Items.Add(newEntity.entidade);
            ControlFilterCBTipo.Text = newEntity.entidade;
            newEntity.Dispose();
        }

        private void ControlFilterBLinhaCarregar_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog.Filter = "Drawing (*.dwg)|*.dwg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Thread newThread = new Thread(new ParameterizedThreadStart(GetLineType));
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start(openFileDialog.FileName);


                    Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAnalisando));
                    newThread2.SetApartmentState(ApartmentState.STA);
                    newThread2.Start();

                    newThread.Join();
                    Form_0_JanelaPrincipal.StopStatusThread(newThread2);

                    string filetxt = new Configuration().GetPROGRAMDirectoryTemp() + "TempImporLineType.Temp";
                    if (File.Exists(filetxt))
                    {
                        StreamReader streamReader = new StreamReader(filetxt, Encoding.UTF8, true);
                        while (!streamReader.EndOfStream)
                        {
                            string linetype = streamReader.ReadLine().ToUpper();
                            arranjos.allLineType1.Remove(linetype);
                            arranjos.allLineType1.Add(linetype);
                        }
                        arranjos.allLineType1.Sort();
                        ControlFilterCBLinha.Items.Clear();
                        ControlFilterCBLinha.Items.AddRange(arranjos.allLineType1.ToArray());
                        ControlFilterCBLinha.Text = arranjos.allLineType1.First();
                        streamReader.Close();
                        File.Delete(filetxt);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

     




       
        private  void GetLineType(object fileName)
        {
            try
            {
                string file = (string)fileName;
                ACAD.AcadApplication acadApplication;
                ACAD.AcadDocument acadDocument;
                using (MessageFilter.ScopedRegistration())
                {
                    acadApplication = new ACAD.AcadApplication();

                    acadDocument = acadApplication.Documents.Open(file, false);
                }
                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.LoadFile(DrawingProcess.DLLPath1, acadDocument);
                }
                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.SendCommand("DRAWINDCAD_LoadLineType\n", acadDocument);

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


        private void ControlFilter_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void ControlFilterLTextoAltura_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ControlFilterLTextoAltura.Text.Contains(','))
                    e.Handled = true;
            }
        }

        public void DisableText()
        {
            ControlFilterCBTipo.Text = "TEXT";
            ControlFilterGBTipo.Enabled = false;
            ControlFilterGBLinha.Enabled = false;
        }

        public void DisableOrientacao()
        {
            groupBox1.Enabled = false;
        }

        private void ControlFilterCBOrientacao_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void ControlFilterCBOrientacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ControlFilterCBOrientacao.Text == "ALL")
                ControlFilterGBTipo.Enabled = true;
            else
            {
                ControlFilterCBTipo.Text = "LINE";
                ControlFilterGBTipo.Enabled = false;
            }
        }
    }
}
