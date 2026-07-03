using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class ConfigurarLayersCor : IDisposable
    {
        private readonly Arranjos arranjos;
        public string cor;

        public ConfigurarLayersCor(string valor, Arranjos arranjos)
        {
            cor = valor;
            this.arranjos = arranjos;
        }

        public UiDialogResult ShowDialog()
        {
            LayerColorDialog dialog = new LayerColorDialog(cor, arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                cor = dialog.Color;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



