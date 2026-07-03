using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_4_LayersLayerBase : IDisposable
    {
        private readonly Arranjos arranjos;
        public string layerBase;

        public Form_4_LayersLayerBase(string valor, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            layerBase = valor;
        }

        public DialogResult ShowDialog()
        {
            LayerBaseDialog dialog = new LayerBaseDialog(layerBase, arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                layerBase = dialog.LayerBase;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
