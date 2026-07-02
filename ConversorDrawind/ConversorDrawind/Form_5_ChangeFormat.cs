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
    public partial class Form_5_ChangeFormat : Form
    {
        public Form_5_ChangeFormat() 
        {
            InitializeComponent();
        }

        static string Button_id; 
        static Form_5_ChangeFormat newMessageBox;

        public static string Show(string txtMessage, string txtTitle) 
        {
            Button_id = "0"; 
            newMessageBox = new Form_5_ChangeFormat();
            newMessageBox.label1.Text = txtMessage;
            newMessageBox.Text = txtTitle;
            newMessageBox.ShowDialog();
            return Button_id; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button_id = "1";
            newMessageBox.Dispose(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Button_id = "2"; 
            newMessageBox.Dispose();
        }
    }
}
