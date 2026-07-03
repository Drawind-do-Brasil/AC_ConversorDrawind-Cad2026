using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersNewLayer : IDisposable
    {
        private readonly Arranjos arranjos;
        public NewLayer novoLayer;

        public LayersNewLayer(string line, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            novoLayer = new NewLayer(this.arranjos);
            novoLayer.SetConjunto(line);
        }

        public UiDialogResult ShowDialog()
        {
            NewLayerDialog dialog = new NewLayerDialog(novoLayer, arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                novoLayer = dialog.NewLayer;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



