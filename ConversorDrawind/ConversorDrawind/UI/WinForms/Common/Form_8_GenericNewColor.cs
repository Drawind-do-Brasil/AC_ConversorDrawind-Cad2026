using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_8_GenericNewColor : Form
    {
        public string colorClass = string.Empty;
    
        public Form_8_GenericNewColor(string corAtual)
        {
            InitializeComponent();
            tbColor.Text = ",,";
            colorClass = corAtual;
        }

        private void tbColor_KeyPress(object sender, KeyPressEventArgs e)
        {
                e.Handled = true;
        }

        private void tbBlue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int soma = 0;
            try
            {
                soma = Convert.ToInt32(tbBlue.Text + e.KeyChar);
            }
            catch (Exception)
            {
                soma = 0;
            }

            if ((!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
                || soma > 255)
            {
                e.Handled = true;
            }
            else if (!Char.IsControl(e.KeyChar))
            {
                tbColor.Text = tBRed.Text + "," + tBGreen.Text + "," + tbBlue.Text + e.KeyChar;
            }
            else if (e.KeyChar == '\b')
            {
                try
                {
                    tbColor.Text = tBRed.Text + "," + tBGreen.Text + "," + tbBlue.Text.Remove(tbBlue.Text.Length - 1, 1);
                }
                catch (Exception)
                {

                    tbColor.Text = tBRed.Text + "," + tBGreen.Text + ",";
                }

            }
        }

        private void tBGreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            int soma = 0;
            try
            {
                soma = Convert.ToInt32(tBGreen.Text + e.KeyChar);
            }
            catch (Exception)
            {
                soma = 0;
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8
                || soma > 255)
            {
                e.Handled = true;
            }
            else if (!Char.IsControl(e.KeyChar))
            {
                tbColor.Text = tBRed.Text + "," + tBGreen.Text + e.KeyChar + "," + tbBlue.Text;
            }
            else if (e.KeyChar == '\b')
            {
                try
                {
                    tbColor.Text = tBRed.Text + "," + tBGreen.Text.Remove(tBGreen.Text.Length - 1, 1) + "," + tbBlue.Text;
                }
                catch (Exception)
                {
                    tbColor.Text = tBRed.Text + ",," + tbBlue.Text;
                }
            }
        }

        private void tBRed_KeyPress(object sender, KeyPressEventArgs e)
        {
            int soma = 0;
            try
            {
                soma = Convert.ToInt32(tBRed.Text + e.KeyChar);
            }
            catch (Exception)
            {
                soma = 0;
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8
                || soma > 255)
            {
                e.Handled = true;
            }
            else if (!Char.IsControl(e.KeyChar))
            {
                tbColor.Text = tBRed.Text + e.KeyChar + "," + tBGreen.Text + "," + tbBlue.Text;
            }
            else if (e.KeyChar == '\b')
            {
                try
                {
                    tbColor.Text = tBRed.Text.Remove(tBRed.Text.Length - 1, 1) + "," + tBGreen.Text + "," + tbBlue.Text;
                }
                catch (Exception)
                {
                    tbColor.Text = "," + tBGreen.Text + "," + tbBlue.Text;
                }
            }
        }

        private void bContinuar_Click(object sender, EventArgs e)
        {
            ConvertInt();
            if (tBRed.Text != "" && tBGreen.Text != "" && tbBlue.Text != "")
                colorClass = tBRed.Text + "," + tBGreen.Text + "," + tbBlue.Text;
            else
                colorClass = "BYLAYER";
            this.Close();
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ConfColor_MouseClick(object sender, MouseEventArgs e)
        {
            ConvertInt();
        }

        private void tBRed_MouseClick(object sender, MouseEventArgs e)
        {
            ConvertInt();
        }
        private void tBGreen_MouseClick(object sender, MouseEventArgs e)
        {
            ConvertInt();
        }

        private void tbBlue_MouseClick(object sender, MouseEventArgs e)
        {
            ConvertInt();
        }

        private void tbColor_MouseClick(object sender, MouseEventArgs e)
        {
            ConvertInt();
        }

        private void ConvertInt()
        {
            try
            {
                tBRed.Text = Convert.ToString(Convert.ToInt32(tBRed.Text));
                tbColor.Text = tBRed.Text + "," + tBGreen.Text + "," + tbBlue.Text;
            }
            catch (Exception)
            {

            }
            try
            {
                tBGreen.Text = Convert.ToString(Convert.ToInt32(tBGreen.Text));
                tbColor.Text = tBRed.Text + "," + tBGreen.Text + "," + tbBlue.Text;
            }
            catch (Exception)
            {

            }
            try
            {
                tbBlue.Text = Convert.ToString(Convert.ToInt32(tbBlue.Text));
                tbColor.Text = tBRed.Text + "," + tBGreen.Text + "," + tbBlue.Text;
            }
            catch (Exception)
            {

            }
        }

        private void ConfColor_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
