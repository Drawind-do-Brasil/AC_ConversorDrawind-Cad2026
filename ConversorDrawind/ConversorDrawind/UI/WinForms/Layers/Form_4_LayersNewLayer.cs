using System;
using System.Linq;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_4_LayersNewLayer : Form
    {
        private Arranjos arranjos = new Arranjos();
        public NewLayer novoLayer;

        public Form_4_LayersNewLayer( string line, Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
            novoLayer = new NewLayer(this.arranjos);
            ControlNewLayerCBLayer.Items.AddRange(this.arranjos.allNewLayer.ToArray());
            ControlNewLayerCBCor.Items.AddRange(this.arranjos.allcolor.ToArray());
            ControlNewLayerCBCor.Items.Remove(this.arranjos.allcolor.First());
            ControlNewLayerCBTipoLinha.Items.AddRange(this.arranjos.allLineType2.ToArray());
            EstiloTexto.Items.AddRange(this.arranjos.allTextSyles.Select(a => a.Split(':').First()).ToArray());

            novoLayer.SetConjunto(line);
            for (int i = 0; i < ControlNewLayerCBTipoLinha.Items.Count; i++)
            {
                if(ControlNewLayerCBTipoLinha.Items[i].ToString().Split(',').First().ToUpper() == novoLayer.tipoLinha)
                {
                    ControlNewLayerCBTipoLinha.Text = ControlNewLayerCBTipoLinha.Items[i].ToString();
                    break;
                }
            }
            ControlNewLayerCBLayer.Text = novoLayer.layer;
            ControlNewLayerCBCor.Text = novoLayer.cor;
            ControlNewLayerCBTextoAltura.Text = novoLayer.alturaTexto;
            ControlNewLayerCBTextoLargura.Text = novoLayer.larguraTexto;
            EstiloTexto.Text = novoLayer.estiloTexto;
        }

        private void ControlNewLayerBContinuar_Click(object sender, EventArgs e)
        {
            if (ControlNewLayerCBLayer.Items.Contains(ControlNewLayerCBLayer.Text))
                novoLayer.layer = ControlNewLayerCBLayer.Text;
            if (ControlNewLayerCBCor.Items.Contains(ControlNewLayerCBCor.Text))
                novoLayer.cor = ControlNewLayerCBCor.Text;
            if (ControlNewLayerCBTipoLinha.Items.Contains(ControlNewLayerCBTipoLinha.Text))
                novoLayer.tipoLinha = ControlNewLayerCBTipoLinha.Text.Split(',').First().ToUpper();
             if (EstiloTexto.Items.Contains(EstiloTexto.Text))
                novoLayer.estiloTexto = EstiloTexto.Text;
            novoLayer.alturaTexto = ControlNewLayerCBTextoAltura.Text;
            novoLayer.larguraTexto = ControlNewLayerCBTextoLargura.Text;
         
            this.Close();
        }

        private void ControlNewLayerBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlNewLayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void ControlNewLayerBCores_Click(object sender, EventArgs e)
        {
            string color = string.Empty;
            Form_8_GenericNewColor newColor = new Form_8_GenericNewColor(ControlNewLayerCBCor.Text);
            newColor.ShowDialog();

            if (!arranjos.allcolor.Contains(newColor.colorClass))
            {
                arranjos.allcolor.Add(newColor.colorClass);
                ControlNewLayerCBCor.Items.Add(newColor.colorClass);
            }

            ControlNewLayerCBCor.Text = newColor.colorClass;
            newColor.Dispose();
        }

        private void ControlNewLayerCBTextoAltura_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ControlNewLayerCBTextoAltura.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ControlNewLayerCBTextoLargura_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ControlNewLayerCBTextoLargura.Text.Contains(','))
                    e.Handled = true;
            }
        }
    }
}
