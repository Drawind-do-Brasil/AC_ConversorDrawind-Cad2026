using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_3_ConfigurarLayersCor : IDisposable
    {
        private readonly Arranjos arranjos;
        public string cor;

        public Form_3_ConfigurarLayersCor(string valor, Arranjos arranjos)
        {
            cor = valor;
            this.arranjos = arranjos;
        }

        public DialogResult ShowDialog()
        {
            LayerColorDialog dialog = new LayerColorDialog(cor, arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                cor = dialog.Color;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
