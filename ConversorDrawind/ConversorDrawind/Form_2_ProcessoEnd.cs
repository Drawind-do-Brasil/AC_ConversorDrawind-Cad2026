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
    public partial class Form_2_ProcessoEnd : Form
    {
        public Form_2_ProcessoEnd()
        {
            InitializeComponent();
            Label_Processo.Text = "Tempo de Conversão: " + Form_2_Processo.tempo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
