using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_3_ConfigurarLayersLinha : IDisposable
    {
        public string linha;
        private readonly Arranjos arranjos;

        public Form_3_ConfigurarLayersLinha(string valor, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            linha = string.Empty;

            foreach (string lineType in arranjos.allLineType2)
            {
                if (lineType.Split(',')[0].ToUpper() == valor)
                {
                    linha = lineType;
                    break;
                }
            }
        }

        public DialogResult ShowDialog()
        {
            LayerLineTypeDialog dialog = new LayerLineTypeDialog(linha.Split(',')[0].ToUpper(), arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                linha = dialog.LineType;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
