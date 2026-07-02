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
    public partial class Form_5_AttFormatWidthFactor : Form
    {
        public string WicthFactor = "1";

        public Form_5_AttFormatWidthFactor(string wicthFactor)
        {
            InitializeComponent();
            textBox1.Text = WicthFactor = wicthFactor;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (textBox1.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            WicthFactor = textBox1.Text;
            this.Close();
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
