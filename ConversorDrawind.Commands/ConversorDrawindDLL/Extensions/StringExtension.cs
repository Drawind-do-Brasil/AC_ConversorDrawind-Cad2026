using System.Globalization;

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
