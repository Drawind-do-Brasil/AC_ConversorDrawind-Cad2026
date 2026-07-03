using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class ConfigurarLayersNome : IDisposable
    {
        public string nome;
        private readonly Arranjos arranjos;

        public ConfigurarLayersNome(string nome, Arranjos arranjos)
        {
            this.nome = nome;
            this.arranjos = arranjos;
        }

        public UiDialogResult ShowDialog()
        {
            LayerNameDialog dialog = new LayerNameDialog(nome, arranjos.allNewLayer);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                nome = dialog.LayerName;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



