using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{
    internal static class TextStyleResolver
    {
        internal static string Resolve(IEnumerable<string> textStyles, string styleName)
        {
            if (styleName == null)
                throw new NullReferenceException();

            string textStyle = textStyles.FirstOrDefault(style =>
                string.Equals(GetStyleName(style), styleName, StringComparison.OrdinalIgnoreCase));

            return textStyle ?? "TEXTO:RomanS:false:false:2.5:1:0";
        }

        internal static double ResolveTextSize(IEnumerable<string> textStyles, string styleName)
        {
            string[] textStyleSplit = Resolve(textStyles, styleName).Split(':');
            return textStyleSplit[4].ToDouble();
        }

        internal static double ResolveOblique(IEnumerable<string> textStyles, string styleName)
        {
            string[] textStyleSplit = Resolve(textStyles, styleName).Split(':');
            return DimensionGeometry.DegreeToRadian(textStyleSplit[6].ToDouble());
        }

        private static string GetStyleName(string textStyle)
        {
            return textStyle.Split(':').First();
        }
    }
}
