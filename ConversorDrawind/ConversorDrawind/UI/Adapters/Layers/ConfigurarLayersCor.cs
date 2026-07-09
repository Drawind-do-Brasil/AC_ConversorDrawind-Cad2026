using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class ConfigurarLayersCor : IDisposable
    {
        private readonly CatalogConfiguration catalogs;
        public string cor;

        public ConfigurarLayersCor(string valor, Configuration configuration)
        {
            cor = valor;
            catalogs = (configuration ?? new Configuration()).Catalogs;
        }

        public UiDialogResult ShowDialog()
        {
            LayerColorDialog dialog = new LayerColorDialog(cor, catalogs);
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



