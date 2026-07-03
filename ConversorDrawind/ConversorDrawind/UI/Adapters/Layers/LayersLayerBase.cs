using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersLayerBase : IDisposable
    {
        private readonly Arranjos arranjos;
        public string layerBase;

        public LayersLayerBase(string valor, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            layerBase = valor;
        }

        public UiDialogResult ShowDialog()
        {
            LayerBaseDialog dialog = new LayerBaseDialog(layerBase, arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                layerBase = dialog.LayerBase;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



