using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawindDLL
{
    internal static class ArrowFixService
    {
        internal static double GetDimensionDistance(RotatedDimension rotatedDimension)
        {
            return GetDimensionDistance(
                rotatedDimension.XLine1Point,
                rotatedDimension.XLine2Point,
                rotatedDimension.DimLinePoint);
        }

        internal static double GetDimensionDistance(Point3d xLine1Point, Point3d xLine2Point, Point3d dimLinePoint)
        {
            Line3d line = new Line3d(xLine2Point, dimLinePoint);
            Plane plane = line.GetPerpendicularPlane(dimLinePoint);
            Point3d projectedPoint = xLine1Point.OrthoProject(plane);
            return dimLinePoint.DistanceTo(projectedPoint);
        }

        internal static Point3d ProjectFirstExtensionPoint(Point3d xLine1Point, Point3d xLine2Point, Point3d dimLinePoint)
        {
            Line3d line = new Line3d(xLine2Point, dimLinePoint);
            Plane plane = line.GetPerpendicularPlane(dimLinePoint);
            return xLine1Point.OrthoProject(plane);
        }
    }
}
