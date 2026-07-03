using ConversorDrawind.UI.Wpf.TextStyles;
using System;

namespace ConversorDrawind
{
    public sealed class NewTextSyle : IDisposable
    {
        public string[] valores = new string[7];
        public bool createNew = false;

        public NewTextSyle(string line, Arranjos arranjos)
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
                return;
            }

            string[] parts = line.Split(':');
            for (int i = 0; i < valores.Length; i++)
            {
                valores[i] = i < parts.Length ? parts[i] : string.Empty;
            }
        }

        public UiDialogResult ShowDialog()
        {
            TextStyleDialog dialog = new TextStyleDialog(valores);
            bool? result = dialog.ShowDialog();

            if (result != true)
            {
                return UiDialogResult.Cancel;
            }

            valores = dialog.Values;
            createNew = true;
            return UiDialogResult.OK;
        }

        public void Dispose()
        {
        }
    }
}
