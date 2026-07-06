using System;
using System.Linq;

namespace ConversorDrawind
{
    public class Filter
    {
        private Arranjos arranjos = new Arranjos();
        public string layerBase;
        public string tipoObjeto;
        public string cor;
        public string tipoLinha;
        public string conteudoTexto;
        public string alturaTexto;
        public string orientacao = "ALL";

        public Filter(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public Filter(Filter filter)
        {
            this.arranjos = filter.arranjos;
            this.layerBase = filter.layerBase;
            this.tipoObjeto = filter.tipoObjeto;
            this.cor = filter.cor;
            this.tipoLinha = filter.tipoLinha;
            this.conteudoTexto = filter.conteudoTexto;
            this.alturaTexto = filter.alturaTexto;
            this.orientacao = filter.orientacao;
        }

        public string GetConjunto()
        {
            return tipoObjeto + ":" +
                   cor + ":" +
                   tipoLinha + ":" +
                   conteudoTexto + ":" +
                   alturaTexto + ":" +
                   orientacao;
        }

        public void SetConjunto()
        {
            tipoObjeto = arranjos.allobjects.First();
            cor = arranjos.allcolor.First();
            tipoLinha = arranjos.allLineType1.First();
            conteudoTexto = string.Empty;
            alturaTexto = string.Empty;
            orientacao = "ALL";
        }

        public void SetConjunto2()
        {
            tipoObjeto = "ALL";
            cor = "ALL";
            tipoLinha = "ALL";
            conteudoTexto = string.Empty;
            alturaTexto = string.Empty;
            orientacao = "ALL";
        }

        public void SetConjunto3()
        {
            layerBase = "ALL";
            tipoObjeto = "TEXT";
            cor = "ALL";
            tipoLinha = "ALL";
            conteudoTexto = string.Empty;
            alturaTexto = string.Empty;
            orientacao = "ALL";
        }

        public void SetConjunto(string conjunto)
        {
            try
            {
                string[] split = conjunto.Split(':');
                tipoObjeto = split[0];
                cor = split[1];
                tipoLinha = split[2];
                conteudoTexto = split[3];
                alturaTexto = split[4];
                orientacao = split[5];
            }
            catch (Exception)
            {
                tipoObjeto = arranjos.allobjects.First();
                cor = arranjos.allcolor.First();
                tipoLinha = arranjos.allLineType1.First();
                conteudoTexto = string.Empty;
                alturaTexto = string.Empty;
                orientacao = "ALL";
            }
        }
    }
}




