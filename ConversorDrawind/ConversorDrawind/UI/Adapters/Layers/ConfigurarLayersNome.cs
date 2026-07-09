using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Linq;

namespace ConversorDrawind
{
    public sealed class ConfigurarLayersNome : IDisposable
    {
        private readonly Configuration configuration;

        public ConfigurarLayersNome(string nome, Configuration configuration)
        {
            this.nome = nome;
            this.configuration = configuration ?? new Configuration();
        }

        public string nome;

        public UiDialogResult ShowDialog()
        {
            LayerNameDialog dialog = new LayerNameDialog(nome, configuration.Layers.NewLayers.Select(layer => layer.Name));
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
