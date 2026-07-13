using System;
using System.Globalization;

namespace ConversorDrawind
{
    public static class NumericTextParser
    {
        public static double ToDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            string normalized = value.Replace(',', '.');
            return Convert.ToDouble(normalized, CultureInfo.InvariantCulture);
        }
    }
}
