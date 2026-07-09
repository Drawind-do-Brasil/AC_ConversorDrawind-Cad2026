using Autodesk.AutoCAD.Colors;
using System;
using System.Collections.Generic;

namespace ConversorDrawind.Commands
{
    internal static class ColorResolver
    {
        private static readonly IReadOnlyDictionary<string, short> AciColorIndexes =
            new Dictionary<string, short>(StringComparer.OrdinalIgnoreCase)
            {
                { "RED", 1 },
                { "YELLOW", 2 },
                { "GREEN", 3 },
                { "CYAN", 4 },
                { "BLUE", 5 },
                { "MAGENTA", 6 },
                { "WHITE", 7 },
                { "BYLAYER", 256 },
                { "BYBLOCK", 0 },
            };

        internal static Color Resolve(string color)
        {
            short colorInt = 0;
            if (short.TryParse(color, out colorInt))
                return Color.FromColorIndex(ColorMethod.ByAci, colorInt);

            if (color == null)
                throw new NullReferenceException();

            if (AciColorIndexes.TryGetValue(color, out short aciIndex))
                return Color.FromColorIndex(ColorMethod.ByAci, aciIndex);
            if (string.Equals(color, "ALL", StringComparison.OrdinalIgnoreCase))
                return null;

            string[] stringRGB = color.Split(',');
            byte red = Convert.ToByte(stringRGB[0]);
            byte green = Convert.ToByte(stringRGB[1]);
            byte blue = Convert.ToByte(stringRGB[2]);
            return Color.FromRgb(red, green, blue);
        }
    }
}
