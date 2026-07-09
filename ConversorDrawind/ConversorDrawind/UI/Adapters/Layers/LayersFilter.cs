using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersFilter : IDisposable
    {
        private readonly Configuration configuration;
        private bool disableOrientation;
        private bool disableText;
        private string lineType2Source;

        public Filter filtro;

        public LayersFilter(string valor, Configuration configuration)
        {
            this.configuration = configuration ?? new Configuration();
            filtro = new Filter(this.configuration.Catalogs);
            filtro.SetConjunto(valor);
        }

        public void CarregarControlFilterCBLinhaTipo2(string line)
        {
            lineType2Source = line;
        }

        public void DisableText()
        {
            disableText = true;
        }

        public void DisableOrientacao()
        {
            disableOrientation = true;
        }

        public UiDialogResult ShowDialog()
        {
            LayerFilterDialog dialog = new LayerFilterDialog(filtro, configuration.Catalogs);
            if (lineType2Source != null)
            {
                dialog.LoadLineTypes2(lineType2Source);
            }

            if (disableText)
            {
                dialog.DisableText();
            }

            if (disableOrientation)
            {
                dialog.DisableOrientation();
            }

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



