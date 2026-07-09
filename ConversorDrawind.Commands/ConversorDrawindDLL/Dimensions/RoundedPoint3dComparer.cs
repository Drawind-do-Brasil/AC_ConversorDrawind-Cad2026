using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    internal sealed class RoundedPoint3dComparer : EqualityComparer<Point3d>
    {
        private readonly int precision;

        internal RoundedPoint3dComparer(int precision)
        {
            this.precision = precision;
        }

        public override bool Equals(Point3d p1, Point3d p2)
        {
            return Math.Round(p1.X, precision) == Math.Round(p2.X, precision) &&
                   Math.Round(p1.Y, precision) == Math.Round(p2.Y, precision) &&
                   Math.Round(p1.Z, precision) == Math.Round(p2.Z, precision);
        }

        public override int GetHashCode(Point3d obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }
}
