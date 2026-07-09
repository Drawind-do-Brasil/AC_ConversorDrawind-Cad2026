using System;

namespace ConversorDrawind.Commands
{

    public static class ObjectExtension
    {
        public static bool ToBollean(this object objeto)
        {
            bool valor = true;
            if (objeto == null)
                return valor;
            bool.TryParse(objeto.ToString(), out valor);
            return valor;
        }

        public static int ToInt32(this object objeto)
        {
            int valor = 0;
            if (objeto == null)
                return valor;
            Int32.TryParse(objeto.ToString(), out valor);
            return valor;
        }

        public static double ToDouble(this object objeto)
        {
            double valor = 0;
            if (objeto == null)
                return valor;
            Double.TryParse(objeto.ToString().ReplaceComma(), out valor);
            return valor;
        }


        public static bool IsInt32(this object objeto)
        {
            int valor = 0;
            if (objeto == null)
                return false;
            return Int32.TryParse(objeto.ToString(), out valor);
        }
    }
}
