using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConversorDrawind
{
    public class CorrecaoLinhas
    {
        public bool status = false;
        public string linha = "";
        public string nomeLayer = "";
        public int posLinha = 0;
        public string newLinha = "";
        public string oldLinha = "";

        public string GetNewLinha()
        {
            string[] newLinhaSplit = linha.Split(':');
            return newLinhaSplit[0] + ":" + newLinhaSplit[1] + ":" + newLinha;
        }
    }
}

