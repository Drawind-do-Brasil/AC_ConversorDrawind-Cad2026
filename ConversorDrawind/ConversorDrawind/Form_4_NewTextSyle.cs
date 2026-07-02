using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public partial class Form_4_NewTextSyle : Form
    {
        private Class_Arranjos arranjos = new Class_Arranjos();
        public Class_NewLayer novoLayer;
        DataGridViewRow row = null;
        public string[] valores = new string[7];
        public bool createNew = false;

        public Form_4_NewTextSyle(DataGridViewRow row, Class_Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
            this.row = row;

            foreach (FontFamily font in FontFamily.Families)
                Fonte.Items.Add(font.Name);
            if (!Fonte.Items.Contains("RomanS"))
                Fonte.Items.Add("RomanS");
            Fonte.Text = "RomanS";

            bool valor = false;

            Nome.Text = row.Cells[0].Value. ToString();
            Fonte.Text = row.Cells[1].Value.ToString();
            Boolean.TryParse(row.Cells[2].Value.ToString(), out valor);
            Oblique.Checked = valor;
            Boolean.TryParse(row.Cells[3].Value.ToString(), out valor);
            Negrito.Checked = valor;
            Tamanho.Text = row.Cells[4].Value.ToString();
            WidthFactor.Text = row.Cells[5].Value.ToString();
            Angulo.Text = row.Cells[6].Value.ToString();

        }
        public Form_4_NewTextSyle(string line, Class_Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;

            foreach (FontFamily font in FontFamily.Families)
                Fonte.Items.Add(font.Name);
            if (!Fonte.Items.Contains("RomanS"))
                Fonte.Items.Add("RomanS");
            Fonte.Text = "RomanS";

            if (string.IsNullOrEmpty(line))
            {
                Nome.Text = "";
                Tamanho.Text = "2.5";
                WidthFactor.Text = "1";
                Angulo.Text = "0";
            }
        }

        private void ControlNewLayerBContinuar_Click(object sender, EventArgs e)
        {
            if (row != null)
            {
                row.Cells[0].Value = Nome.Text;
                row.Cells[1].Value = Fonte.Text;
                row.Cells[2].Value = Oblique.Checked;
                row.Cells[3].Value = Negrito.Checked;
                row.Cells[4].Value = Tamanho.Text;
                row.Cells[5].Value = WidthFactor.Text;
                row.Cells[6].Value = Angulo.Text;
            }

            else
            {
                createNew = true;
                valores[0] = Nome.Text;
                valores[1] = Fonte.Text;
                valores[2] = Oblique.Checked.ToString();
                valores[3] = Negrito.Checked.ToString();
                valores[4] = Tamanho.Text;
                valores[5] = WidthFactor.Text;
                valores[6] = Angulo.Text;
            }

            this.Close();
        }

        private void ControlNewLayerBCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlNewLayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void Fonte_SelectedIndexChanged(object sender, EventArgs e)
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (File.Exists( Path.Combine(roaming, @"\Autodesk\AutoCAD 2026\R25.1\enu\support\") + Fonte.Text + ".shx"))
            {
                Oblique.Enabled = false;
                Negrito.Enabled = false;
            }
            else
            {
                Oblique.Enabled = true;
                Negrito.Enabled = true;
            }
        }

        private void Tamanho_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (Tamanho.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void WidthFactor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (WidthFactor.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void Angulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)8)
                return;
            if (e.KeyChar == ',' || e.KeyChar == '.')
                e.Handled = true;
            try
            {
                string t = Angulo.Text + Convert.ToString(e.KeyChar);
                if (t == "-")
                    return;
                double valor = Convert.ToDouble(t);
                if (!(valor >= -85 && valor <= 85))
                    e.Handled = true;

            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }

        private void Tamanho_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "2,5";
        }

        private void WidthFactor_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "1";
        }

        private void Angulo_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "0";
        }
    }
}
