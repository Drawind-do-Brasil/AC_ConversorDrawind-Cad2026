using System;
using System.Windows.Forms;
using System.IO;

namespace ConversorDrawind
{
    public partial class Form_1_CaminhoLin : Form
    {
        public string file = "";
        public Form_1_CaminhoLin(string arquivo)
        {
            InitializeComponent();
            textBox1.Text = file = arquivo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Arquivo Lin (*.lin)|*.lin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = textBox1.Text = openFileDialog.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(file))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, especifique o arquivo correto.",
                                "Atenção",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
