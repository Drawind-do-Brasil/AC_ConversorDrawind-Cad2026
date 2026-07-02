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
    public partial class Form_6_AttFormatInventor : Form
    {
        Block inventor = new Block();
        Block original = new Block();
        Block inventorCopy = new Block();
        Block originalrCopy = new Block();

        public Block Original
        {
            get { return originalrCopy; }
        }

        public Block Inventor
        {
            get { return inventorCopy; }
        }

        public Form_6_AttFormatInventor(Block inventor, Block original)
        {
            InitializeComponent();
            this.inventor = inventor;
            this.original = original;
            this.inventorCopy = inventor.DeepCopy();
            this.originalrCopy = original.DeepCopy();

            foreach (TagBlock item in original.listTags)
            {
                if (item.indiceRelacao != -1)
                {
                    DataAssociacao.Rows.Add(item.tag, item.indiceRelacao + "  -  " + inventor.listTags[item.indiceRelacao].tag, item.widthfactor);
                    //original.listTags[item.indiceRelacao].isSociate = true;
                    item.isSociate = true;
                }

                else
                    DataAssociacao.Rows.Add(item.tag, "", item.widthfactor);
            }
        }

        private void DataAssociacao_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (e.ColumnIndex == 1)
                {
                    Form_6_TagFormatInventor myBlock = new Form_6_TagFormatInventor(inventor.listTags, original.listTags[e.RowIndex]);
                    myBlock.ShowDialog();

                    if (myBlock.indice != -1)
                    {
                        DataAssociacao.Rows[e.RowIndex].Cells[1].Value = myBlock.indice + "  -  " + inventor.listTags[myBlock.indice].tag;
                    }
                    else
                    {
                        DataAssociacao.Rows[e.RowIndex].Cells[1].Value = "";
                    }
                    myBlock.Dispose();
                }
                else if (e.ColumnIndex == 2)
                {
                    string wf = DataAssociacao.Rows[DataAssociacao.CurrentRow.Index].Cells[2].Value.ToString();
                    Form_5_AttFormatWidthFactor wff = new Form_5_AttFormatWidthFactor(wf);
                    wff.ShowDialog();
                    DataAssociacao.Rows[e.RowIndex].Cells[2].Value = wff.WicthFactor;
                    original.listTags[e.RowIndex].widthfactor = wff.WicthFactor;
                    wff.Dispose();
                }

              
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            inventorCopy = inventor;
            originalrCopy = original;
            this.Close();
        }

        private void RecalculateTags()
        {
            inventor.ResetTagReference();
            original.ResetTagReference();
            int ids = 0;
            foreach (DataGridViewRow item in DataAssociacao.Rows)
            {
                int index = -1;
                try
                {
                     index  = Convert.ToInt32(item.Cells[1].Value.ToString().Split('-').First().Trim());
                }
                catch (Exception)
                {
                    
                }
                original.listTags[ids].indiceRelacao = index;
                if (index != -1)
                    inventor.listTags[index].isSociate = true;
                ids++;
            }
        }
        private void AssociarPTags_Click(object sender, EventArgs e)
        {

            inventor.ResetTagReference();
            original.ResetTagReference();
            for (int i = 0; i < DataAssociacao.Rows.Count; i++)
            {
                DataAssociacao.Rows[i].Cells[1].Value = "";
            }
            for (int i = 0;i < original.listTags.Count; i++)
            {
                for (int j = 0; j < inventor.listTags.Count; j++)
                {
                    if (!inventor.listTags[j].isSociate && inventor.listTags[j].tag.ToUpper() == original.listTags[i].tag.ToUpper())
                    {
                        original.listTags[i].indiceRelacao = j;
                        inventor.listTags[j].isSociate = true;
                        DataAssociacao.Rows[i].Cells[1].Value = j + "  -  " + inventor.listTags[j].tag;
                    }
                }
            }
        }

        private void AssociarPOrdem_Click(object sender, EventArgs e)
        {
            inventor.ResetTagReference();
            original.ResetTagReference();
            for (int i = 0; i < DataAssociacao.Rows.Count; i++)
            {
                DataAssociacao.Rows[i].Cells[1].Value = "";
            }
            for (int i = 0; i < inventor.listTags.Count && i < original.listTags.Count; i++)
            {
                original.listTags[i].indiceRelacao = i;
                inventor.listTags[i].isSociate = true;
                DataAssociacao.Rows[i].Cells[1].Value = i + "  -  " + inventor.listTags[i].tag;
            }
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DataAssociacao.SelectedCells.Count > 1)
                if (DataAssociacao.SelectedCells[0].ColumnIndex != 1)
                    return;
            List<DataGridViewCell> list = new List<DataGridViewCell>();
            foreach (DataGridViewCell item in DataAssociacao.SelectedCells)
            {
                list.Add(item);
            }
            list = list.OrderByDescending(p => p.RowIndex).ToList();
            foreach (DataGridViewCell item in list)
            {
                if (item.RowIndex + 1 > DataAssociacao.RowCount - 1)
                    continue;

                DataAssociacao.Rows[item.RowIndex + 1].Cells[1].Value = DataAssociacao.Rows[item.RowIndex].Cells[1].Value;
                DataAssociacao.Rows[item.RowIndex + 1].Cells[1].Selected = true;
                DataAssociacao.Rows[item.RowIndex].Cells[1].Value = "";
                DataAssociacao.Rows[item.RowIndex].Cells[1].Selected = false;
            }


            RecalculateTags();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DataAssociacao.SelectedCells.Count > 1)
                if (DataAssociacao.SelectedCells[0].ColumnIndex != 1)
                    return;
            List<DataGridViewCell> list = new List<DataGridViewCell>();
            foreach (DataGridViewCell item in DataAssociacao.SelectedCells)
            {
                list.Add(item);
            }
            list = list.OrderBy(p => p.RowIndex).ToList();
            foreach (DataGridViewCell item in list)
            {
                if (item.RowIndex - 1 == -1)
                    continue;

                DataAssociacao.Rows[item.RowIndex - 1].Cells[1].Value = DataAssociacao.Rows[item.RowIndex].Cells[1].Value;
                DataAssociacao.Rows[item.RowIndex - 1].Cells[1].Selected = true;
                DataAssociacao.Rows[item.RowIndex].Cells[1].Value = "";
                DataAssociacao.Rows[item.RowIndex].Cells[1].Selected = false;
            }

            RecalculateTags();
        }

 
        private void DataAssociacao_SelectionChanged(object sender, EventArgs e)
        {
            if (DataAssociacao.SelectedCells[0].Selected == true)
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void DataAssociacao_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            System.Windows.Forms.DataGridView.HitTestInfo hitTestInfo = dataGridView.HitTest(e.X, e.Y);
            if (hitTestInfo.ColumnIndex == 2 && hitTestInfo.RowIndex != -1)
            {
   
                if (e.Button == MouseButtons.Right)
                {
                    string wf = DataAssociacao.SelectedCells[0].Value.ToString();
                    Form_5_AttFormatWidthFactor wff = new Form_5_AttFormatWidthFactor(wf);
                    wff.ShowDialog();
                    wff.Dispose();
                    foreach (DataGridViewCell cell in DataAssociacao.SelectedCells)
                    {
                        cell.Value = wff.WicthFactor;
                        original.listTags[cell.RowIndex].widthfactor = wff.WicthFactor;
                    }
                }
            }
        }  
    }
}
