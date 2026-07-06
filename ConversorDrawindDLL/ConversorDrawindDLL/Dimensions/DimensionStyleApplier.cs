using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawindDLL
{
    internal static class DimensionStyleApplier
    {
        internal static void ApplyLineStyle(Line line, string layer, Color color)
        {
            line.Layer = layer;
            line.Color = color;
        }

        internal static void ApplyRotatedDimensionStyle(
            RotatedDimension dimension,
            DimensionProperties dimensionProperties,
            ObjectId dimStyle,
            string layer,
            Color color)
        {
            dimension.DimensionStyle = dimStyle;
            dimension.UsingDefaultTextPosition = false;
            dimension.TextPosition = dimensionProperties.TextPosition;
            dimension.TextRotation = dimensionProperties.TextRotation;
            if (dimension.Layer != layer)
                dimension.Layer = layer;
            dimension.Color = color;
        }

        internal static void ApplyAngularDimensionStyle(
            Point3AngularDimension dimension,
            DimensionProperties dimensionProperties,
            ObjectId dimStyle,
            string layer,
            Color color)
        {
            if (dimension.Layer != layer)
                dimension.Layer = layer;
            dimension.Color = color;
            dimension.DimensionStyle = dimStyle;
            dimension.TextPosition = dimensionProperties.TextPosition;
            dimension.UsingDefaultTextPosition = false;
        }

        internal static void ApplyLargeGap(Point3AngularDimension dimension, string layer, double arrowSize)
        {
            dimension.Layer = layer;
            dimension.Dimgap = 10 * arrowSize * 2;
        }
    }
}
