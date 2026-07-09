using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersNewLayer : IDisposable
    {
        private readonly Configuration configuration;
        private NewLayer novoLayer;

        public LayersNewLayer(LayerOutput value, Configuration configuration)
        {
            this.configuration = configuration ?? new Configuration();
            novoLayer = ToNewLayer(value, this.configuration);
        }

        public LayerOutput LayerOutput
        {
            get { return ToLayerOutput(novoLayer); }
        }

        public UiDialogResult ShowDialog()
        {
            NewLayerDialog dialog = new NewLayerDialog(novoLayer, configuration);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                novoLayer = dialog.NewLayer;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }

        private static NewLayer ToNewLayer(LayerOutput value, Configuration configuration)
        {
            value = value ?? new LayerOutput();
            return new NewLayer(configuration)
            {
                layer = value.LayerName,
                cor = value.Color,
                tipoLinha = value.LineType,
                alturaTexto = value.TextContent,
                larguraTexto = value.TextHeight,
                estiloTexto = value.TextStyle
            };
        }

        private static LayerOutput ToLayerOutput(NewLayer value)
        {
            value = value ?? new NewLayer(new Configuration());
            return new LayerOutput
            {
                LayerName = value.layer,
                Color = value.cor,
                LineType = value.tipoLinha,
                TextContent = value.alturaTexto,
                TextHeight = value.larguraTexto,
                TextStyle = value.estiloTexto
            };
        }
    }
}



