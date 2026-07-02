using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_4_LayersNewEntity : Form
    {
        public string entidade;

        public Form_4_LayersNewEntity(string entidadeAtual)
        {
            entidade = entidadeAtual;
            InitializeComponent();
        }

        private void bContinuar_Click(object sender, EventArgs e)
        {
            entidade = tBEntity.Text.ToUpper();
            this.Close();
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NewEntity_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
