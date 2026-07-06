using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawindDLL
{
    internal static class FormatStartPointService
    {
        internal static void UpdateMinimumPoint(Point3d point, ref double minX, ref double minY, ref double minZ)
        {
            if (point.X < minX)
                minX = point.X;
            if (point.Y < minY)
                minY = point.Y;
            if (point.Z < minZ)
                minZ = point.Z;
        }

        internal static bool IsSameLayer(string entityLayer, string layerName)
        {
            return string.Equals(entityLayer, layerName, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsValidPoint(Point3d point)
        {
            return !double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z) &&
                   !double.IsInfinity(point.X) && !double.IsInfinity(point.Y) && !double.IsInfinity(point.Z) &&
                   point.X != double.MaxValue && point.Y != double.MaxValue && point.Z != double.MaxValue;
        }
    }
}
