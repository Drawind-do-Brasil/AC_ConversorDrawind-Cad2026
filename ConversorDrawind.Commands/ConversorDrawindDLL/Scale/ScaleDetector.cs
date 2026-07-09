using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawindDLL
{
    internal static class ScaleDetector
    {
        internal static Point3d GetPointDifference(Point3d pontoIni, Point3d pontoRef, double scale)
        {
            return new Point3d(
                (pontoRef.X * scale) + pontoIni.X,
                (pontoRef.Y * scale) + pontoIni.Y,
                (pontoRef.Z * scale) + pontoIni.Z);
        }

        internal static bool IsOrientation(Point3d p1, Point3d p2, string orientacao)
        {
            if (orientacao == "HORIZONTAL" && Math.Round(p1.Y, 3) == Math.Round(p2.Y, 3))
                return true;
            if (orientacao == "VERTICAL" && Math.Round(p1.X, 3) == Math.Round(p2.X, 3))
                return true;
            return false;
        }
    }
}
