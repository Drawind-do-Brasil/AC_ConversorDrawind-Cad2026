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
    public partial class Form_5_AttFormatChangeCheckBox : Form
    {
        public  bool modificar = true;

        public Form_5_AttFormatChangeCheckBox()
        {
            InitializeComponent();
            comboBox1.Text = "Modificar";
        }



        private void OK_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Modificar")
                modificar = true;
            else
                modificar = false;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
