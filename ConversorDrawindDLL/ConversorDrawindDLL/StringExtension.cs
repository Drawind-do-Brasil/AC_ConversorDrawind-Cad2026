using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversorDrawindDLL
{
    public static class StringExtension
    {
        public static string ReplaceComma(this string valor)
        {
            var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (separator == ",")
                return valor.Replace('.', ',');
            else
                return valor.Replace(',', '.');
        }
    }
}
