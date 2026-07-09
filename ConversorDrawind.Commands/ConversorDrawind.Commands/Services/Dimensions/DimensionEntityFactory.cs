using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawind.Commands
{
    internal static class DimensionEntityFactory
    {
        internal static Line CreateLine(Point3d p1, Point3d p2, string layer, Color color)
        {
            Line line = new Line(p1, p2);
            line.SetDatabaseDefaults();
            DimensionStyleApplier.ApplyLineStyle(line, layer, color);
            return line;
        }

        internal static RotatedDimension CreateRotatedDimension(
            DimensionProperties dimensionProperties,
            ObjectId dimStyle,
            string layer,
            Color color)
        {
            RotatedDimension rotatedDimension = new RotatedDimension();
            rotatedDimension.SetDatabaseDefaults();
            rotatedDimension.XLine1Point = dimensionProperties.XLine1Point;
            rotatedDimension.XLine2Point = dimensionProperties.XLine2Point;
            rotatedDimension.DimLinePoint = dimensionProperties.DimLinePoint;
            rotatedDimension.Rotation = dimensionProperties.Rotation;
            rotatedDimension.DimensionText = dimensionProperties.Text;
            DimensionStyleApplier.ApplyRotatedDimensionStyle(
                rotatedDimension,
                dimensionProperties,
                dimStyle,
                layer,
                color);
            return rotatedDimension;
        }

        internal static Point3AngularDimension CreateAngularDimension(
            DimensionProperties dimensionProperties,
            ObjectId dimStyle,
            string layer,
            Color color)
        {
            Point3AngularDimension dimension = new Point3AngularDimension();
            dimension.SetDatabaseDefaults();
            dimension.XLine1Point = dimensionProperties.XLine1Start;
            dimension.XLine2Point = dimensionProperties.XLine2Start;
            dimension.CenterPoint = dimensionProperties.Center;
            dimension.ArcPoint = dimensionProperties.ArcPoint;
            dimension.DimensionText = dimensionProperties.Text;
            DimensionStyleApplier.ApplyAngularDimensionStyle(
                dimension,
                dimensionProperties,
                dimStyle,
                layer,
                color);
            return dimension;
        }

        internal static Point3AngularDimension CreateAngularDimensionWithLargeGap(
            DimensionProperties dimensionProperties,
            ObjectId dimStyle,
            string layer,
            Color color,
            double arrowSize)
        {
            Point3AngularDimension dimension = CreateAngularDimension(
                dimensionProperties,
                dimStyle,
                layer,
                color);
            DimensionStyleApplier.ApplyLargeGap(dimension, layer, arrowSize);
            return dimension;
        }
    }
}
