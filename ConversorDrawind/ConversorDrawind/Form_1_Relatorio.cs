using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ConversorDrawind
{
    public partial class Form_1_Relatorio : Form
    {
        List<string> list = new List<string>();
        public Form_1_Relatorio(List<string> x, string message)
        {
            InitializeComponent();
            label1.Text = message;
            list = x;
            for (int i = 0; i < x.Count; i++)
            {
                listBox1.Items.Add(x[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DNC_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {

                    if (File.Exists(saveFileDialog1.FileName))
                    {
                        File.Delete(saveFileDialog1.FileName);
                    }

                    StreamWriter streamWriter = new StreamWriter(saveFileDialog1.FileName, true);
                    for (int i = 0; i < list.Count; i++)
                    {
                        streamWriter.WriteLine(list[i]);
                    }
                    streamWriter.Close();
                }
            }
        }
    }
}
