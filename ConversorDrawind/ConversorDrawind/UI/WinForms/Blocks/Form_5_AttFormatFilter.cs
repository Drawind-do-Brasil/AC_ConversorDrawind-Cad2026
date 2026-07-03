using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class Form_5_AttFormatFilter : IDisposable
    {
        public Filter filtro;

        private readonly Arranjos arranjos;

        public Form_5_AttFormatFilter(string valor, Arranjos arranjos, string layer)
        {
            this.arranjos = arranjos;
            filtro = new Filter(arranjos)
            {
                layerBase = layer
            };
            filtro.SetConjunto(valor);
        }

        public DialogResult ShowDialog()
        {
            BlockFilterDialog dialog = new BlockFilterDialog(arranjos, filtro);
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
