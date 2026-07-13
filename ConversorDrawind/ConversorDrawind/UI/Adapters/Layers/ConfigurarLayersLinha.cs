using ConversorDrawind.UI.Wpf.Layers;
using System;
namespace ConversorDrawind
{
    public sealed class ConfigurarLayersLinha : IDisposable
    {
        public string linha;
        private readonly CatalogConfiguration catalogs;

        public ConfigurarLayersLinha(string valor, Configuration configuration)
        {
            catalogs = (configuration ?? new Configuration()).Catalogs;
            linha = string.Empty;

            foreach (string lineType in catalogs.LayerLineTypes)
            {
                if (lineType.Split(',')[0].ToUpper() == valor)
                {
                    linha = lineType;
                    break;
                }
            }
        }

        public UiDialogResult ShowDialog()
        {
            LayerLineTypeDialog dialog = new LayerLineTypeDialog(linha.Split(',')[0].ToUpper(), catalogs);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                linha = dialog.LineType;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



