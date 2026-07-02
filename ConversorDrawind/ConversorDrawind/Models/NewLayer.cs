using System;
using System.Linq;

namespace ConversorDrawind
{
    public class NewLayer
    {
        private Arranjos arranjos = new Arranjos();
        public string layer;
        public string cor;
        public string tipoLinha;
        public string alturaTexto;
        public string larguraTexto;
        public string estiloTexto;

        public NewLayer(Arranjos arranjos)
        {
            this.arranjos = arranjos;
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
            cor = this.arranjos.allcolor[1];
            tipoLinha = this.arranjos.allLineType2.First();
            alturaTexto = string.Empty;
            larguraTexto = string.Empty;
            estiloTexto = this.arranjos.allTextSyles.First().Split(':').First();
        }

        public void SetConjuntoEspecial()
        {
            string layertemp = "";
            for (int i = 0; i < this.arranjos.allLineType2.Count; i++)
            {
                if (this.arranjos.allLineType2[i] != this.arranjos.lineTypeRemove.First() &&
                    this.arranjos.allLineType2[i] != this.arranjos.lineTypeRemove.Last())
                {
                    layertemp = this.arranjos.allLineType2[i];
                    break;
                }
            }
            layer = "0";
            cor = "WHITE";
            tipoLinha = layertemp;
            alturaTexto = string.Empty;
            larguraTexto = string.Empty;
            estiloTexto = this.arranjos.allTextSyles.First().Split(':').First();
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
                if (split.Count() >= 6)
                    estiloTexto = split[5];
                else
                    estiloTexto = this.arranjos.allTextSyles.First().Split(':').First();
            }
            catch (Exception)
            {
                layer = this.arranjos.allNewLayer.First();
                cor = this.arranjos.allcolor.First();
                tipoLinha = this.arranjos.allLineType2.First();
                alturaTexto = string.Empty;
                larguraTexto = string.Empty;
                estiloTexto = this.arranjos.allTextSyles.First().Split(':').First();
            }
        }
    }
}

