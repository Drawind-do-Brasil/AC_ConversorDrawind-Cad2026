using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawind.Commands
{
    internal static class ConversionSession
    {
        private static ConversorDrawind.Point drawingMinPoint = CreateEmptyMinPoint();
        private static ConversorDrawind.Point drawingMaxPoint = CreateEmptyMaxPoint();

        internal static ConversorDrawind.Point DrawingMinPoint => drawingMinPoint;

        internal static ConversorDrawind.Point DrawingMaxPoint => drawingMaxPoint;

        internal static string LogDirectory { get; set; } = string.Empty;

        internal static string LogFileName { get; set; } = string.Empty;

        internal static double CapturedScale { get; set; } = -1;

        internal static double AppliedScale { get; set; } = 1;

        internal static DateTime StartedAt { get; set; }

        internal static string ConverterName { get; set; } = string.Empty;

        internal static Point3d MinPoint3d => new Point3d(drawingMinPoint.X, drawingMinPoint.Y, drawingMinPoint.Z);

        internal static Point3d MaxPoint3d => new Point3d(drawingMaxPoint.X, drawingMaxPoint.Y, drawingMaxPoint.Z);

        internal static void Reset()
        {
            drawingMinPoint = CreateEmptyMinPoint();
            drawingMaxPoint = CreateEmptyMaxPoint();
            CapturedScale = -1;
            AppliedScale = 1;
            StartedAt = DateTime.Now;
            ConverterName = string.Empty;
        }

        internal static void SetMinPoint(ConversorDrawind.Point point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            drawingMinPoint = point;
        }

        internal static void SetMaxPoint(double x, double y, double z)
        {
            drawingMaxPoint.X = x;
            drawingMaxPoint.Y = y;
            drawingMaxPoint.Z = z;
        }

        internal static void SetLogFile(string directory, string fileName)
        {
            LogDirectory = directory ?? string.Empty;
            LogFileName = fileName ?? string.Empty;
        }

        private static ConversorDrawind.Point CreateEmptyMinPoint()
        {
            return new ConversorDrawind.Point(double.MaxValue, double.MaxValue, double.MaxValue);
        }

        private static ConversorDrawind.Point CreateEmptyMaxPoint()
        {
            return new ConversorDrawind.Point(double.MinValue, double.MinValue, double.MinValue);
        }
    }
}
