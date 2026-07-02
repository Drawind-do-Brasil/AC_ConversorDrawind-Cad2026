using Autodesk.AutoCAD.Interop;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind
{
    public partial class Form_5_AttFormatFilter : Form
    {
        private Arranjos arranjos = new Arranjos();
        public Filter filtro;

        public Form_5_AttFormatFilter(string valor, Arranjos arranjos, string layer)
        {
            this.arranjos = arranjos;
            filtro = new Filter(arranjos);
            InitializeComponent();
            CarregarControlFilterCBCor();
            CarregarControlFilterCBLayer();

            filtro.layerBase = layer;
            filtro.SetConjunto(valor);
            CarregarConfiguracao();
        }

        private void CarregarControlFilterCBLayer()
        {
            ControlFilterCBLayer.Items.Clear();
            ControlFilterCBLayer.Items.Add("ALL");
            ControlFilterCBLayer.Items.AddRange(arranjos.allNewLayer.ToArray());
            ControlFilterCBLayer.Items.AddRange(arranjos.allBaseLayer.ToArray());
        }

        private void CarregarControlFilterCBCor()
        {
            ControlFilterCBCor.Text = arranjos.allcolor.First();
            ControlFilterCBCor.Items.Clear();
            ControlFilterCBCor.Items.AddRange(arranjos.allcolor.ToArray());
        }

        public void CarregarConfiguracao()
        {
            ControlFilterCBLayer.Text = filtro.layerBase;
            ControlFilterCBCor.Text = filtro.cor;
            ControlFilterLTextoAltura.Text = filtro.alturaTexto;
            ControlFilterLTextoConteudo.Text = filtro.conteudoTexto;
        }


        private void ControlFilterBCodigoCor_Click(object sender, EventArgs e)
        {
            string color = string.Empty;
            Form_8_GenericNewColor newColor = new Form_8_GenericNewColor(ControlFilterCBCor.Text);
            newColor.ShowDialog();
            arranjos.allcolor.Remove(newColor.colorClass);
            ControlFilterCBCor.Items.Remove(newColor.colorClass);
            arranjos.allcolor.Add(newColor.colorClass);
            ControlFilterCBCor.Items.Add(newColor.colorClass);
            ControlFilterCBCor.Text = newColor.colorClass;
            newColor.Dispose();
        }

        private void ControlFilterBContinuar_Click(object sender, EventArgs e)
        {
            if (ControlFilterCBCor.Items.Contains(ControlFilterCBCor.Text))
                filtro.cor = ControlFilterCBCor.Text;
            filtro.alturaTexto = ControlFilterLTextoAltura.Text;
            filtro.tipoObjeto = "TEXT";
            if (ControlFilterCBLayer.Items.Contains(ControlFilterCBLayer.Text))
                filtro.layerBase = ControlFilterCBLayer.Text;
            filtro.conteudoTexto = ControlFilterLTextoConteudo.Text;
            this.Close();
        }

        private void ControlFilterBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      



        private static void GetLineType(object fileName)
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
                MessageBox.Show(e.Message,
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

        private void ControlFilterBCodigoCor_Click_1(object sender, EventArgs e)
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
    }
}
