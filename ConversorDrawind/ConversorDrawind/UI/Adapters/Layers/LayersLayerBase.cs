using ConversorDrawind.UI.Wpf.Layers;
using System;

namespace ConversorDrawind
{
    public sealed class LayersLayerBase : IDisposable
    {
        private readonly Configuration configuration;

        public LayersLayerBase(string valor, Configuration configuration)
        {
            this.configuration = configuration ?? new Configuration();
            layerBase = valor;
        }

        public string layerBase;

        public UiDialogResult ShowDialog()
        {
            LayerBaseDialog dialog = new LayerBaseDialog(layerBase, configuration);
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
