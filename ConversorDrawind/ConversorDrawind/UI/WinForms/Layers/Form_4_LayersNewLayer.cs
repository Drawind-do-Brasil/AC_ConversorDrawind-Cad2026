using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_4_LayersNewLayer : IDisposable
    {
        private readonly Arranjos arranjos;
        public NewLayer novoLayer;

        public Form_4_LayersNewLayer(string line, Arranjos arranjos)
        {
            this.arranjos = arranjos;
            novoLayer = new NewLayer(this.arranjos);
            novoLayer.SetConjunto(line);
        }

        public DialogResult ShowDialog()
        {
            NewLayerDialog dialog = new NewLayerDialog(novoLayer, arranjos);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                novoLayer = dialog.NewLayer;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
