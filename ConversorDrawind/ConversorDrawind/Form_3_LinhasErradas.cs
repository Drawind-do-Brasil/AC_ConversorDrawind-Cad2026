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
    public partial class Form_3_LinhasErradas : Form
    {
        List<Class_CorrecaoLinhas> LinhasErradas = new List<Class_CorrecaoLinhas>();
        Class_Arranjos Arranjos = new Class_Arranjos();
        public Form_3_LinhasErradas(List<Class_CorrecaoLinhas> linhasErradas, Class_Arranjos arranjos)
        {
            InitializeComponent();
            LinhasErradas = linhasErradas;
            Arranjos = arranjos;
        }

        private void FormLinhasErradas_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dbNewLayersDataSet2.Tabela2' table. You can move, or remove it, as needed.
            CarregarDataGridView1();
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;  
        }

        private void CarregarDataGridView1()
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < this.LinhasErradas.Count; i++)
            {
                dataGridView1.Rows.Add(LinhasErradas[i].nomeLayer, LinhasErradas[i].oldLinha);
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (e.ColumnIndex == 1)
                {
                    Form_3_ConfigurarLayersLinha myLinha = new Form_3_ConfigurarLayersLinha(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString(), Arranjos);
                    myLinha.ShowDialog();
                    string linha = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString();
                    try
                    {
                        linha = myLinha.linha.Split(',').First().ToUpper();
                    }
                    catch (Exception)
                    {

                    }
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value = linha;
                    myLinha.Dispose();
                    LinhasErradas[e.RowIndex].newLinha = linha;
                    if (CheckLines())
                        button1.Enabled = true;
                }
            }
        }

        public bool CheckLines()
        {
            for (int i = 0; i < LinhasErradas.Count; i++)
            {
                bool linhaOK = false;
                foreach (var line in Arranjos.allLineType2)
                {
                    string[] lineTemp = line.Split(',');
                    if (LinhasErradas[i].newLinha.ToUpper() == lineTemp.First().ToUpper())
                        linhaOK = true;
                }
                if (!linhaOK)
                    return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Class_CorrecaoLinhas item in LinhasErradas)
            {
                Arranjos.allNewLayerComposition[item.posLinha] = item.GetNewLinha();
            }

            this.Close();
        }
    }
}
