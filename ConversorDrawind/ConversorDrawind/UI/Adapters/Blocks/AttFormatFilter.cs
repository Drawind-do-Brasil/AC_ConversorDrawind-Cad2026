using System;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class AttFormatFilter : IDisposable
    {
        public Filter filtro;

        private readonly Arranjos arranjos;

        public AttFormatFilter(string valor, Arranjos arranjos, string layer)
        {
            this.arranjos = arranjos;
            filtro = new Filter(arranjos)
            {
                layerBase = layer
            };
            filtro.SetConjunto(valor);
        }

        public UiDialogResult ShowDialog()
        {
            BlockFilterDialog dialog = new BlockFilterDialog(arranjos, filtro);
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



