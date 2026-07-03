using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_3_LinhasErradas : IDisposable
    {
        private readonly List<CorrecaoLinhas> linhasErradas;
        private readonly Arranjos arranjos;

        public Form_3_LinhasErradas(List<CorrecaoLinhas> linhasErradas, Arranjos arranjos)
        {
            this.linhasErradas = linhasErradas;
            this.arranjos = arranjos;
        }

        public bool CheckLines()
        {
            return new WrongLineTypesDialog(linhasErradas, arranjos).CheckLines();
        }

        public DialogResult ShowDialog()
        {
            WrongLineTypesDialog dialog = new WrongLineTypesDialog(linhasErradas, arranjos);
            bool? result = dialog.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
