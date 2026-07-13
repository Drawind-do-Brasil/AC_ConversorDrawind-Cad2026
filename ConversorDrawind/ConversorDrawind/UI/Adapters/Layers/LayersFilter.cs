using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class LayersFilter : IDisposable
    {
        private readonly Configuration configuration;
        private bool disableOrientation;
        private bool disableText;
        private string lineType2Source;

        private Filter filtro;

        public LayersFilter(EntityFilter value, Configuration configuration)
        {
            this.configuration = configuration ?? new Configuration();
            filtro = ToFilter(value, this.configuration.Catalogs);
        }

        public EntityFilter EntityFilter
        {
            get { return ToEntityFilter(filtro); }
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

        public UiDialogResult ShowDialog()
        {
            LayerFilterDialog dialog = new LayerFilterDialog(filtro, configuration.Catalogs);
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
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }

        private static Filter ToFilter(EntityFilter value, CatalogConfiguration catalogs)
        {
            value = value ?? new EntityFilter();
            return new Filter(catalogs)
            {
                layerBase = value.BaseLayer,
                tipoObjeto = value.ObjectType,
                cor = value.Color,
                tipoLinha = value.LineType,
                conteudoTexto = value.TextContent,
                alturaTexto = value.TextHeight,
                orientacao = string.IsNullOrWhiteSpace(value.Orientation) ? "ALL" : value.Orientation
            };
        }

        private static EntityFilter ToEntityFilter(Filter value)
        {
            value = value ?? new Filter();
            return new EntityFilter
            {
                BaseLayer = value.layerBase,
                ObjectType = value.tipoObjeto,
                Color = value.cor,
                LineType = value.tipoLinha,
                TextContent = value.conteudoTexto,
                TextHeight = value.alturaTexto,
                Orientation = string.IsNullOrWhiteSpace(value.orientacao) ? "ALL" : value.orientacao
            };
        }
    }
}



