using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersNewEntity : IDisposable
    {
        public string entidade;

        public LayersNewEntity(string entidadeAtual)
        {
            entidade = entidadeAtual;
        }

        public UiDialogResult ShowDialog()
        {
            LayerEntityDialog dialog = new LayerEntityDialog(entidade);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                entidade = dialog.Entity;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



