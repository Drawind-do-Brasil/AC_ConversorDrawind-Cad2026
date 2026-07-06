using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{
    internal static class DimensionTextAnalyzer
    {
        internal static bool HasDifferentTextRotations(IEnumerable<double> rotationsInRadians)
        {
            double[] rotations = rotationsInRadians.ToArray();
            if (rotations.Length == 0)
                return false;

            double firstRotation = Math.Round(DimensionGeometry.RadianToDegree(rotations.First()), 2);

            for (int i = 0; i < rotations.Length; i++)
            {
                if (firstRotation != Math.Round(DimensionGeometry.RadianToDegree(rotations[i]), 2))
                    return true;
            }

            return false;
        }
    }
}
