using System;

namespace ConversorDrawind
{
    internal static class NumericTextParser
    {
        public static double ToDouble(string value)
        {
            return Convert.ToDouble(value.Replace('.', ','));
        }
    }
}



