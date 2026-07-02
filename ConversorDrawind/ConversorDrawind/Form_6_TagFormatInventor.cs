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
    public partial class Form_6_TagFormatInventor : Form
    {
        List<Color> cores = new List<Color>();
        public int indice = -1;
        List<Class_TagBlockClass> tags = new List<Class_TagBlockClass>();
        List<Class_TagBlockClass> tagsCopy = new List<Class_TagBlockClass>();
        Class_TagBlockClass original = new Class_TagBlockClass();
        public Form_6_TagFormatInventor(List<Class_TagBlockClass> tags, Class_TagBlockClass original)
        {
            InitializeComponent();
            comboBox1.Items.Add(" ");
            int index = 0;
            foreach (Class_TagBlockClass item in tags)
            {
                comboBox1.Items.Add(index + "  -  " + item.tag);
                if (item.isSociate)
                    cores.Add(Color.LightGray);
                else
                    cores.Add(Color.Black);
                index++;
            }
            this.tagsCopy = tags;
            foreach (Class_TagBlockClass item in tags)
            {
                this.tags.Add(item.DeepCopy());
            }
            this.original = original;
            if (original.indiceRelacao != -1)
            {
                comboBox1.Text = original.indiceRelacao + "  -  " + tags[original.indiceRelacao].tag;
            }

        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
           
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index > 0)
            {
                e.DrawBackground();
                string n = ((ComboBox)sender).Items[e.Index].ToString();
                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = cores[e.Index - 1];
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, b, rect.X, rect.Top);
                e.DrawFocusRectangle();

            }
        }
        bool saltLoop = false;
      
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!saltLoop)
            {
                if (original.indiceRelacao != -1)
                {
                    tags[original.indiceRelacao].isSociate = false;
                }
                if (comboBox1.SelectedIndex == 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                    indice = -1;
                }
                else if (comboBox1.SelectedIndex > 0)
                {
                    if (tags[comboBox1.SelectedIndex - 1].isSociate == true)
                    {

                        //tags[comboBox1.SelectedIndex - 1].isSociate = false;
                        indice = -1;
                        saltLoop = true;
                        comboBox1.Text = comboBox1.Items[0].ToString();
                        saltLoop = false;

                    }
                    else
                    {
                        tags[comboBox1.SelectedIndex - 1].isSociate = true;
                        indice = comboBox1.SelectedIndex - 1;

                    }
                }
            }
        }

        bool save = false;
        private void ControlFilterBContinuar_Click(object sender, EventArgs e)
        {
            save = true;

            this.Close();
        }

        private void ControlFilterBCancelar_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void Form_AssTag_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (save)
            {
                original.indiceRelacao = indice;
                this.tagsCopy.Clear();
                foreach (Class_TagBlockClass item in this.tags)
                {
                    this.tagsCopy.Add(item.DeepCopy());
                }
            }
            else
            {
                indice = original.indiceRelacao;
            }
        }
    }
}
