using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace ConversorDrawind
{
    public partial class Form_5_AttFormat : Form
    {
        Block Block = null;
        Arranjos arranjos = null;
        public GetInfo myDrawingBlock = null;

        public Form_5_AttFormat(Block BlockTemp, Arranjos ArranjosTemp, GetInfo DrawingBlock)
        {
            InitializeComponent();
            Block = BlockTemp;
            arranjos = ArranjosTemp;
            myDrawingBlock = DrawingBlock;
        }

        private void BlockConf_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dbBlocksDataSet.blocks' table. You can move, or remove it, as needed.
            foreach (TagBlock item in Block.listTags)
            {
                dataGridView.Rows.Add(item.tag,
                                      item.modify,
                                      item.p1.X.ToString().Replace(',', '.') + "," +
                                      item.p1.Y.ToString().Replace(',', '.') + "," +
                                      item.p1.Z.ToString().Replace(',', '.') + ";" +
                                      item.p2.X.ToString().Replace(',', '.') + "," +
                                      item.p2.Y.ToString().Replace(',', '.') + "," +
                                      item.p2.Z.ToString().Replace(',', '.'),
                                      item.filtro.layerBase + ";" + item.filtro.GetConjunto(),
                                      item.widthfactor);

            }
            dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private bool IsContinueOp1 = true;

        private void CarregarMyDrawingBlock()
        {
            if (myDrawingBlock != null)
                myDrawingBlock.UpdateStatus();
            if (myDrawingBlock == null || myDrawingBlock.Status() == "ERROR")
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAbrindoCad));
                        newThread2.SetApartmentState(ApartmentState.STA);
                        newThread2.Start();;
                        myDrawingBlock = new GetInfo(openFileDialog.FileName);
                        Form_0_JanelaPrincipal.StopStatusThread(newThread2);
                        IsContinueOp1 = true;
                    }
                    openFileDialog.Dispose();
                }
                else
                    IsContinueOp1 = false;
            }
            else
                IsContinueOp1 = true;
        }


        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellDoubleClick(sender, e.RowIndex, e.ColumnIndex);
        }
        private void dataGridView_CellDoubleClick(object sender,  int rowIndex, int columIndex)
        {
            if (columIndex!= -1 && rowIndex != -1)
            {

                if (columIndex == 2)
                {
                    CarregarMyDrawingBlock();
                    if (IsContinueOp1)
                    {
                        if (myDrawingBlock != null)
                        {
                            PointEspecial p1 = new PointEspecial();
                            PointEspecial p2 = new PointEspecial();
                            myDrawingBlock.Get2Point(ref p1, ref p2);
                            foreach (DataGridViewRow row in dataGridView.SelectedRows)
                            {
                                row.Cells[2].Value = p1.X.ToString().Replace(',', '.') + "," +
                                           p1.Y.ToString().Replace(',', '.') + "," +
                                           p1.Z.ToString().Replace(',', '.') + ";" +
                                           p2.X.ToString().Replace(',', '.') + "," +
                                           p2.Y.ToString().Replace(',', '.') + "," +
                                           p2.Z.ToString().Replace(',', '.');
                            }
                            System.Threading.Thread.Sleep(5);
                            SetForegroundWindow(this.Handle);
                            this.Activate();
                        }
                    }
                }
                else if (columIndex == 3)
                {
                    string[] layer = dataGridView.Rows[dataGridView.CurrentRow.Index].Cells[3].Value.ToString().Split(';');
                    Form_5_AttFormatFilter myFilter = new Form_5_AttFormatFilter(layer[1], arranjos, layer[0]);
                    myFilter.ShowDialog();
                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        row.Cells[3].Value = myFilter.filtro.layerBase + ";" + myFilter.filtro.GetConjunto();
                    }
                    myFilter.Dispose();
                }

                else if (columIndex == 4)
                {
                    string wf = dataGridView.Rows[dataGridView.CurrentRow.Index].Cells[4].Value.ToString();
                    Form_5_AttFormatWidthFactor wff = new Form_5_AttFormatWidthFactor(wf);
                    wff.ShowDialog();
                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        row.Cells[4].Value = wff.WicthFactor;
                    }
                    wff.Dispose();
                }
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            Block.listTags.Clear();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
			{
                TagBlock Tag = new TagBlock();
			    Tag.tag = dataGridView.Rows[i].Cells[0].Value.ToString();
                Tag.modify = Convert.ToBoolean(dataGridView.Rows[i].Cells[1].Value.ToString());
                string[] pts = dataGridView.Rows[i].Cells[2].Value.ToString().Split(';');
                string[] pts1 = pts[0].Split(',');
                string[] pts2 = pts[1].Split(',');
                Tag.p1 = new PointEspecial(Convert.ToDouble(pts1[0].Replace('.', ',')), Convert.ToDouble(pts1[1].Replace('.', ',')), Convert.ToDouble(pts1[2].Replace('.', ',')));
                Tag.p2 = new PointEspecial(Convert.ToDouble(pts2[0].Replace('.', ',')), Convert.ToDouble(pts2[1].Replace('.', ',')), Convert.ToDouble(pts2[2].Replace('.', ',')));
                string[] layer = dataGridView.Rows[i].Cells[3].Value.ToString().Split(';');
                Tag.filtro.SetConjunto(layer[1]);
                Tag.filtro.layerBase = layer[0];
                Tag.widthfactor = dataGridView.Rows[i].Cells[4].Value.ToString();
                Block.listTags.Add(Tag);
			}
            this.Close();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (e.ColumnIndex == 1)
                {
                    if (dataGridView.Rows[dataGridView.CurrentRow.Index].Cells[1].Value.ToString().ToUpper() == "TRUE")
                        dataGridView.Rows[e.RowIndex].Cells[1].Value = false;
                    else
                        dataGridView.Rows[e.RowIndex].Cells[1].Value = true;
                }
            }
        }

        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            System.Windows.Forms.DataGridView.HitTestInfo hitTestInfo = dataGridView.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (hitTestInfo.ColumnIndex != -1 && hitTestInfo.RowIndex != -1)
                {
                    if (hitTestInfo.ColumnIndex == 1)
                    {
                        Form_5_AttFormatChangeCheckBox mcb = new Form_5_AttFormatChangeCheckBox();
                        mcb.ShowDialog();
                        foreach (DataGridViewRow row in dataGridView.SelectedRows)
                        {
                            row.Cells[1].Value = mcb.modificar;
                        }
                    }
                    else
                        dataGridView_CellDoubleClick(sender, hitTestInfo.RowIndex, hitTestInfo.ColumnIndex);
                }
                

            }
        }
    }
}
