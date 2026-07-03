using System;
using ConversorDrawind.UI.Wpf.Configuration;

namespace ConversorDrawind
{
    public class Form_1_CaminhoLin : IDisposable
    {
        public string file = string.Empty;

        public Form_1_CaminhoLin(string arquivo)
        {
            file = arquivo;
        }

        public System.Windows.Forms.DialogResult ShowDialog()
        {
            LinPathDialog dialog = new LinPathDialog(file);
            bool? result = dialog.ShowDialog();
            file = dialog.SelectedFile;

            return result == true
                ? System.Windows.Forms.DialogResult.OK
                : System.Windows.Forms.DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
