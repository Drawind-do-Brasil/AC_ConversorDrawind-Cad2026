using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_4_LayersNewEntity : IDisposable
    {
        public string entidade;

        public Form_4_LayersNewEntity(string entidadeAtual)
        {
            entidade = entidadeAtual;
        }

        public DialogResult ShowDialog()
        {
            LayerEntityDialog dialog = new LayerEntityDialog(entidade);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                entidade = dialog.Entity;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
