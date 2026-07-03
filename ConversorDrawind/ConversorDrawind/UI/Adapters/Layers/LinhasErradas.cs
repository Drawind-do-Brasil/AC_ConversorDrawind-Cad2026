using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Collections.Generic;
namespace ConversorDrawind
{
    public sealed class LinhasErradas : IDisposable
    {
        private readonly List<CorrecaoLinhas> linhasErradas;
        private readonly Arranjos arranjos;

        public LinhasErradas(List<CorrecaoLinhas> linhasErradas, Arranjos arranjos)
        {
            this.linhasErradas = linhasErradas;
            this.arranjos = arranjos;
        }

        public bool CheckLines()
        {
            return new WrongLineTypesDialog(linhasErradas, arranjos).CheckLines();
        }

        public UiDialogResult ShowDialog()
        {
            WrongLineTypesDialog dialog = new WrongLineTypesDialog(linhasErradas, arranjos);
            bool? result = dialog.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



