using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_3_ConfigurarLayersNome : Form
    {
        public string nome;
        private Arranjos arranjos;
        public Form_3_ConfigurarLayersNome(string nome, Arranjos arranjos)
        {
            InitializeComponent();
            this.nome = nome;
            NLCLNome.Text = nome;
            this.arranjos = arranjos;
        }

        private void NLCBContinuar_Click(object sender, EventArgs e)
        {
            if (!arranjos.allNewLayer.Contains(NLCLNome.Text) || NLCLNome.Text == this.nome)
            {
                this.nome = NLCLNome.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("O layer já existe!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1);
            }
        }

        private void NLCBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
