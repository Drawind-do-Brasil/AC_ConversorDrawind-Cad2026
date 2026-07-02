using System;
using System.Linq;

namespace ConversorDrawindDLL
{
    public class Filter
    {

        public string layerBase;
        public string tipoObjeto;
        public string cor;
        public string tipoLinha;
        public string conteudoTexto;
        public string alturaTexto;
        public string orientacao = "ALL";
        public int alturaTextoRound;

        public Filter()
        {

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

        public void SetConjunto3()
        {
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
                int.TryParse(alturaTexto.ReplaceComma().Split(',').Last(), out qtde);
                alturaTextoRound = qtde; 
            }
            catch (Exception e)
            {
                Conversor.EscreverLog("Erro 84", e.Message);
                tipoObjeto = "ALL";
                cor = "ALL";
                tipoLinha = "ALL";
                conteudoTexto = string.Empty;
                alturaTexto = string.Empty;
                orientacao = "ALL";
            }
        }
    }
}
