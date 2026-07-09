using System;
using System.Linq;
using System.Xml.Serialization;

namespace ConversorDrawind
{
    public class Filter
    {
        private CatalogConfiguration catalogs = new Configuration().Catalogs;
        public string layerBase;
        public string tipoObjeto;
        public string cor;
        public string tipoLinha;
        public string conteudoTexto;
        public string alturaTexto;
        public string orientacao = "ALL";

        [XmlIgnore]
        public int alturaTextoRound;

        public Filter()
        {
        }

        public Filter(CatalogConfiguration catalogs)
        {
            this.catalogs = catalogs ?? new Configuration().Catalogs;
        }

        public Filter(Filter filter)
        {
            this.catalogs = filter.catalogs;
            this.layerBase = filter.layerBase;
            this.tipoObjeto = filter.tipoObjeto;
            this.cor = filter.cor;
            this.tipoLinha = filter.tipoLinha;
            this.conteudoTexto = filter.conteudoTexto;
            this.alturaTexto = filter.alturaTexto;
            this.orientacao = filter.orientacao;
            this.alturaTextoRound = filter.alturaTextoRound;
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
            tipoObjeto = catalogs.ObjectTypes.FirstOrDefault() ?? "ALL";
            cor = catalogs.Colors.FirstOrDefault() ?? "ALL";
            tipoLinha = catalogs.FilterLineTypes.FirstOrDefault() ?? "ALL";
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

                int qtde = 0;
                int.TryParse((alturaTexto ?? string.Empty).Replace('.', ',').Split(',').Last(), out qtde);
                alturaTextoRound = qtde;
            }
            catch (Exception)
            {
                tipoObjeto = catalogs.ObjectTypes.FirstOrDefault() ?? "ALL";
                cor = catalogs.Colors.FirstOrDefault() ?? "ALL";
                tipoLinha = catalogs.FilterLineTypes.FirstOrDefault() ?? "ALL";
                conteudoTexto = string.Empty;
                alturaTexto = string.Empty;
                orientacao = "ALL";
            }
        }
    }
}




