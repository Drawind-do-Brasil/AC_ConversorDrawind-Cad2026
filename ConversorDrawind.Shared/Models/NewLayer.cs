using System;
using System.Linq;

namespace ConversorDrawind
{
    public class NewLayer
    {
        private readonly Arranjos arranjos;

        public string layer;
        public string cor;
        public string tipoLinha;
        public string alturaTexto;
        public string larguraTexto;
        public string estiloTexto;

        public NewLayer(Arranjos arranjos)
        {
            this.arranjos = arranjos ?? new Arranjos();
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
            cor = arranjos.allcolor[1];
            tipoLinha = arranjos.allLineType2.First();
            alturaTexto = string.Empty;
            larguraTexto = string.Empty;
            estiloTexto = arranjos.allTextSyles.First().Split(':').First();
        }

        public void SetConjuntoEspecial()
        {
            string layertemp = "";
            for (int i = 0; i < arranjos.allLineType2.Count; i++)
            {
                if (arranjos.allLineType2[i] != arranjos.lineTypeRemove.First() &&
                    arranjos.allLineType2[i] != arranjos.lineTypeRemove.Last())
                {
                    layertemp = arranjos.allLineType2[i];
                    break;
                }
            }

            layer = "0";
            cor = "WHITE";
            tipoLinha = layertemp;
            alturaTexto = string.Empty;
            larguraTexto = string.Empty;
            estiloTexto = arranjos.allTextSyles.First().Split(':').First();
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
                    : arranjos.allTextSyles.First().Split(':').First();
            }
            catch (Exception)
            {
                layer = arranjos.allNewLayer.First();
                cor = arranjos.allcolor.First();
                tipoLinha = arranjos.allLineType2.First();
                alturaTexto = string.Empty;
                larguraTexto = string.Empty;
                estiloTexto = arranjos.allTextSyles.First().Split(':').First();
            }
        }
    }
}
