using ConversorDrawind.UI.Wpf.TextStyles;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_4_NewTextSyle : IDisposable
    {
        private readonly DataGridViewRow row;
        public string[] valores = new string[7];
        public bool createNew = false;

        public Form_4_NewTextSyle(DataGridViewRow row, Arranjos arranjos)
        {
            this.row = row;
            valores[0] = row.Cells[0].Value.ToString();
            valores[1] = row.Cells[1].Value.ToString();
            valores[2] = row.Cells[2].Value.ToString();
            valores[3] = row.Cells[3].Value.ToString();
            valores[4] = row.Cells[4].Value.ToString();
            valores[5] = row.Cells[5].Value.ToString();
            valores[6] = row.Cells[6].Value.ToString();
        }

        public Form_4_NewTextSyle(string line, Arranjos arranjos)
        {
            if (string.IsNullOrEmpty(line))
            {
                valores[0] = string.Empty;
                valores[1] = "RomanS";
                valores[2] = bool.FalseString;
                valores[3] = bool.FalseString;
                valores[4] = "2.5";
                valores[5] = "1";
                valores[6] = "0";
            }
        }

        public DialogResult ShowDialog()
        {
            TextStyleDialog dialog = new TextStyleDialog(valores);
            bool? result = dialog.ShowDialog();

            if (result != true)
            {
                return DialogResult.Cancel;
            }

            valores = dialog.Values;
            if (row != null)
            {
                for (int i = 0; i < valores.Length; i++)
                {
                    row.Cells[i].Value = valores[i];
                }
            }
            else
            {
                createNew = true;
            }

            return DialogResult.OK;
        }

        public void Dispose()
        {
        }
    }
}
