using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class ConfigurarLayersLinha : IDisposable
    {
        public string linha;
        private readonly Arranjos arranjos;

        public ConfigurarLayersLinha(string valor, Arranjos arranjos)
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

        public UiDialogResult ShowDialog()
        {
            LayerLineTypeDialog dialog = new LayerLineTypeDialog(linha.Split(',')[0].ToUpper(), arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                linha = dialog.LineType;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



