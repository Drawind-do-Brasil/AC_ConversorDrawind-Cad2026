using ConversorDrawind.UI.Wpf.Configuration;
using System;

namespace ConversorDrawind
{
    public class CaminhoLin : IDisposable
    {
        public string file = string.Empty;

        public CaminhoLin(string arquivo)
        {
            file = arquivo;
        }

        public UiDialogResult ShowDialog()
        {
            LinPathDialog dialog = new LinPathDialog(file);
            bool? result = dialog.ShowDialog();
            file = dialog.SelectedFile;

            return result == true
                ? UiDialogResult.OK
                : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



