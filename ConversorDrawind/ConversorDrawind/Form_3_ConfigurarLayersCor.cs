using System;
using System.Linq;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_3_ConfigurarLayersCor : Form
    {
        Class_Arranjos arranjos = new Class_Arranjos();
        public string cor;

        public Form_3_ConfigurarLayersCor(string valor, Class_Arranjos arranjos)
        {
            InitializeComponent();
            NLCCBCor.Text = cor = valor;
            this.arranjos = arranjos;
            CarregarControlFilterCBCor();
        }

        private void CarregarControlFilterCBCor()
        {
            NLCCBCor.Items.Clear();
            NLCCBCor.Items.AddRange(arranjos.allcolor.ToArray());
            NLCCBCor.Items.Remove(this.arranjos.allcolor.First());
            NLCCBCor.Items.Remove(arranjos.lineTypeRemove.First());
            NLCCBCor.Items.Remove(arranjos.lineTypeRemove.Last());
        }

        private void NLCBContinuar_Click(object sender, EventArgs e)
        {
            if (NLCCBCor.Items.Contains(NLCCBCor.Text))
                cor = NLCCBCor.Text;
            this.Close();
        }

        private void NLCBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NLCBCor_Click(object sender, EventArgs e)
        {
            string color = string.Empty;
            Form_8_GenericNewColor newColor = new Form_8_GenericNewColor(NLCCBCor.Text);
            newColor.ShowDialog();

            if (!arranjos.allcolor.Contains(newColor.colorClass))
            {
                arranjos.allcolor.Add(newColor.colorClass);
                NLCCBCor.Items.Add(newColor.colorClass);
            }

            NLCCBCor.Text = newColor.colorClass;
            newColor.Dispose();
        }
    }
}
