using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_3_ConfigurarTextStyle : Form
    {
        public Class_Arranjos arranjos = new Class_Arranjos();

        public Form_3_ConfigurarTextStyle(Class_Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
        }

        private void Form_3_ConfigurarTextStyle_Load(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            for (int i = 0; i < this.arranjos.allTextSyles.Count; i++)
            {
                string[] listTemp = this.arranjos.allTextSyles[i].Split(':');

                dataGridView.Rows.Add(listTemp[0], listTemp[1], listTemp[2], listTemp[3], listTemp[4], listTemp[5], listTemp[6]);
            }
        }

        private void bincluirLinha_Click(object sender, EventArgs e)
        {
            Form_4_NewTextSyle form_4_NewTextSyle = new Form_4_NewTextSyle("", arranjos);
            form_4_NewTextSyle.ShowDialog();

            if (form_4_NewTextSyle.createNew)
            {
                if (dataGridView.Rows.Cast<DataGridViewRow>().Where(a => a.Cells[0].Value.ToString().ToUpper() == form_4_NewTextSyle.valores[0].ToUpper()).Count() > 0)
                {
                    MessageBox.Show("Não é possivel adiconar esse estilo, esse nome já existe!",
                                        "Erro",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                }
                dataGridView.Rows.Add(
                    form_4_NewTextSyle.valores[0],
                    form_4_NewTextSyle.valores[1],
                    form_4_NewTextSyle.valores[2],
                    form_4_NewTextSyle.valores[3],
                    form_4_NewTextSyle.valores[4],
                    form_4_NewTextSyle.valores[5],
                    form_4_NewTextSyle.valores[6]);
            }
        }

        private void bExcluirLinha_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 1)
            {
                MessageBox.Show("Não é possivel remover esse estilo, é necessário pelo menos um estilo!",
                                    "Erro",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("Deseja realmente excluir a linha selecionada?",
                                      "Atenção",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Exclamation,
                                      MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                dataGridView.Rows.RemoveAt(dataGridView.CurrentRow.Index);
            }
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                Form_4_NewTextSyle form_4_NewTextSyle = new Form_4_NewTextSyle(dataGridView.Rows[dataGridView.CurrentRow.Index], arranjos);
                form_4_NewTextSyle.ShowDialog();

            }
        }

        private void ControlNewLayerBContinuar_Click(object sender, EventArgs e)
        {
            arranjos.allTextSyles.Clear();
            foreach (DataGridViewRow item in dataGridView.Rows)
            {
                arranjos.allTextSyles.Add(item.Cells[0].Value + ":" +
                    item.Cells[1].Value + ":" +
                    item.Cells[2].Value + ":" +
                    item.Cells[3].Value + ":" +
                    item.Cells[4].Value + ":" +
                    item.Cells[5].Value + ":" +
                    item.Cells[6].Value);
            }
            this.Close();
        }

        private void ControlNewLayerBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
