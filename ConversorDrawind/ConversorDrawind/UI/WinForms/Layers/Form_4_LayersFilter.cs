using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_4_LayersFilter : IDisposable
    {
        private readonly Arranjos arranjos;
        private bool disableOrientation;
        private bool disableText;
        private string lineType2Source;

        public Filter filtro;

        public Form_4_LayersFilter(string valor, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            filtro = new Filter(arranjos);
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

        public DialogResult ShowDialog()
        {
            LayerFilterDialog dialog = new LayerFilterDialog(filtro, arranjos);
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
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
