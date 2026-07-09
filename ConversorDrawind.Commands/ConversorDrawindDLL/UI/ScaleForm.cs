using System;
using System.Windows.Forms;

namespace ConversorDrawind.Commands
{
    public partial class ScaleForm : Form
    {
        public ScaleForm()
        {
            InitializeComponent();
            scaleB.SelectAll();

            //scaleB.Focus();
        }

        public double scale = 1;

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                OK_Click(null, null);
            }
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (scaleB.Text.Contains(',') || scaleB.Text.Contains('.'))
                    e.Handled = true;
            }
            else if (e.KeyChar == '.')
            {
                if (scaleB.Text.Contains(',') || scaleB.Text.Contains('.'))
                    e.Handled = true;
            }

        }

        private void Cancela_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            scale = Convert.ToDouble(scaleB.Text.ReplaceComma());
            this.Close();
        }

        private void scaleB_Enter(object sender, EventArgs e)
        {

        }
    }
}
