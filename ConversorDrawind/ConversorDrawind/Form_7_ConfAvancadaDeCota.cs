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
    public partial class Form_7_ConfAvaACADaDeCota : Form
    {
        public bool EXTDIMCorrigeSeta = false;
        public string EXTDIMCorrigeSetaTipoSeta = "Oblique";
        public double EXTDIMCorrigeSetaFactor = 7.23;

        public Form_7_ConfAvaACADaDeCota(bool concerta, string tipoSeta, double distancia)
        {
            InitializeComponent();
            checkBox1.Checked = EXTDIMCorrigeSeta = concerta;
            comboBox1.Text = EXTDIMCorrigeSetaTipoSeta = tipoSeta;
            EXTDIMCorrigeSetaFactor = distancia;
            textBox1.Text = Convert.ToString(EXTDIMCorrigeSetaFactor);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
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

        private void button3_Click(object sender, EventArgs e)
        {
            EXTDIMCorrigeSeta = checkBox1.Checked;
            EXTDIMCorrigeSetaTipoSeta = comboBox1.Text;
            EXTDIMCorrigeSetaFactor = Convert.ToDouble(textBox1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
