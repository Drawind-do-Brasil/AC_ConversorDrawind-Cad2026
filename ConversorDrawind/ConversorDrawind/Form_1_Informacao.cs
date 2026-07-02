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
    public partial class Form_1_Informacao : Form
    {
        public Form_1_Informacao()
        {
            InitializeComponent();
            this.Show();
            this.TopMost = true;
        }

        public void SetTopLevelInfUser(bool valor)
        {
            this.SetTopLevel(valor);
        }

        public void AtualizarStatus(string x)
        {
            label1.Text = x;
        }
    }
}
