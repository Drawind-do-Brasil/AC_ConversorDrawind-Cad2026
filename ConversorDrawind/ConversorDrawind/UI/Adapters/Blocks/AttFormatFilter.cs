using System;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class AttFormatFilter : IDisposable
    {
        public Filter filtro;

        private readonly Configuration configuration;

        public AttFormatFilter(string valor, Configuration configuration, string layer)
        {
            this.configuration = configuration ?? new Configuration();
            filtro = new Filter(this.configuration.Catalogs)
            {
                layerBase = layer
            };
            filtro.SetConjunto(valor);
        }

        public UiDialogResult ShowDialog()
        {
            BlockFilterDialog dialog = new BlockFilterDialog(configuration, filtro);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                filtro = dialog.Filter;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



