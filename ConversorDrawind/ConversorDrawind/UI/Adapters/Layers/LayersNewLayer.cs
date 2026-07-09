using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersNewLayer : IDisposable
    {
        private readonly Configuration configuration;
        public NewLayer novoLayer;

        public LayersNewLayer(string line, Configuration configuration)
        {
            this.configuration = configuration ?? new Configuration();
            novoLayer = new NewLayer(this.configuration);
            novoLayer.SetConjunto(line);
        }

        public UiDialogResult ShowDialog()
        {
            NewLayerDialog dialog = new NewLayerDialog(novoLayer, configuration);
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



