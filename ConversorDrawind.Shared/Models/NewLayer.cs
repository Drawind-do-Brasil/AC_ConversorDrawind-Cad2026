using System;
using System.Linq;

namespace ConversorDrawind
{
    public class NewLayer
    {
        private readonly Configuration configuration;

        public string layer;
        public string cor;
        public string tipoLinha;
        public string alturaTexto;
        public string larguraTexto;
        public string estiloTexto;

        public NewLayer(Configuration configuration)
        {
            this.configuration = configuration ?? new Configuration();
            this.configuration.EnsureDefaults();
        }

        public string GetConjunto()
        {
            return layer + ":" +
                   cor + ":" +
                   tipoLinha + ":" +
                   alturaTexto + ":" +
                   larguraTexto + ":" +
                   estiloTexto;
        }

        public void SetConjunto()
        {
            layer = "0";
            cor = configuration.Catalogs.Colors.Skip(1).FirstOrDefault() ?? "BYLAYER";
            tipoLinha = configuration.Catalogs.LayerLineTypes.FirstOrDefault() ?? "BYLAYER";
            alturaTexto = string.Empty;
            larguraTexto = string.Empty;
            estiloTexto = configuration.Text.Styles.FirstOrDefault()?.Name ?? configuration.Text.DefaultStyleName;
        }

        public void SetConjuntoEspecial()
        {
            string layertemp = "";
            for (int i = 0; i < configuration.Catalogs.LayerLineTypes.Count; i++)
            {
                string lineType = configuration.Catalogs.LayerLineTypes[i];
                if (!configuration.Catalogs.RemovedLineTypes.Contains(lineType))
                {
                    layertemp = lineType;
                    break;
                }
            }

            layer = "0";
            cor = "WHITE";
            tipoLinha = layertemp;
            alturaTexto = string.Empty;
            larguraTexto = string.Empty;
            estiloTexto = configuration.Text.Styles.FirstOrDefault()?.Name ?? configuration.Text.DefaultStyleName;
        }

        public void SetConjunto(string conjunto)
        {
            try
            {
                string[] split = conjunto.Split(':');
                layer = split[0];
                cor = split[1];
                tipoLinha = split[2];
                alturaTexto = split[3];
                larguraTexto = split[4];
                estiloTexto = split.Count() >= 6
                    ? split[5]
                    : configuration.Text.Styles.FirstOrDefault()?.Name ?? configuration.Text.DefaultStyleName;
            }
            catch (Exception)
            {
                layer = configuration.Layers.NewLayers.FirstOrDefault()?.Name ?? "0";
                cor = configuration.Catalogs.Colors.FirstOrDefault() ?? "ALL";
                tipoLinha = configuration.Catalogs.LayerLineTypes.FirstOrDefault() ?? "BYLAYER";
                alturaTexto = string.Empty;
                larguraTexto = string.Empty;
                estiloTexto = configuration.Text.Styles.FirstOrDefault()?.Name ?? configuration.Text.DefaultStyleName;
            }
        }
    }
}
