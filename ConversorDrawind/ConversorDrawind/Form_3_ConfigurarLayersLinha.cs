using System;
using System.Linq;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_3_ConfigurarLayersLinha : Form
    {
        public string linha;
        private Class_Arranjos arranjos = new Class_Arranjos();

        public Form_3_ConfigurarLayersLinha(string valor, Class_Arranjos arranjos)
        {
            InitializeComponent();
            NLCCBLinhas.Items.AddRange(arranjos.allLineType2.ToArray());
            NLCCBLinhas.Items.Remove(arranjos.lineTypeRemove.First());
            NLCCBLinhas.Items.Remove(arranjos.lineTypeRemove.Last());

            for (int i = 0; i <  NLCCBLinhas.Items.Count; i++)
            {
                if (NLCCBLinhas.Items[i].ToString().Split(',').First().ToUpper() == valor)
                {
                    NLCCBLinhas.Text = linha = NLCCBLinhas.Items[i].ToString();
                    break;
                }
            }

            this.arranjos = arranjos;
        }

        private void NLCBContinuar_Click(object sender, EventArgs e)
        {
            if(NLCCBLinhas.Items.Contains(NLCCBLinhas.Text))
                linha = NLCCBLinhas.Text;
            this.Close();
        }

        private void NLCBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
