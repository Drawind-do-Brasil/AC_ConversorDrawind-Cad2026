using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawindDLL
{
    internal static class DimensionGeometry
    {
        internal static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        internal static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        internal static bool IsOnLine(Point3d endPoint1, Point3d endPoint2, Point3d checkPoint)
        {
            if (Math.Round(endPoint1.Y, 4) == Math.Round(endPoint2.Y, 4) && Math.Round(endPoint1.Y, 4) == Math.Round(checkPoint.Y, 4))
                return true;
            else if (Math.Round(endPoint1.X, 4) == Math.Round(endPoint2.X, 4) && Math.Round(endPoint1.X, 4) == Math.Round(checkPoint.X, 4))
                return true;
            else
                return Math.Round((double)(checkPoint.Y - endPoint1.Y) / (endPoint2.Y - endPoint1.Y), 4)
                    == Math.Round((double)(checkPoint.X - endPoint1.X) / (endPoint2.X - endPoint1.X), 4);
        }

        internal static bool IsPointEqual(Point3d p1, Point3d p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        internal static Point3d RoundPoint(Point3d point, int precision)
        {
            return new Point3d(
                Math.Round(point.X, precision),
                Math.Round(point.Y, precision),
                Math.Round(point.Z, precision));
        }

        internal static bool CheckParallelLine(double radian1, double radian2)
        {
            double slope1 = Math.Tan(radian1);
            double slope2 = Math.Tan(radian2);

            if ((Math.Round(slope1, 1) == Math.Round(slope2, 1)) || (Math.Round(radian1, 2) == Math.Round(radian2, 2)))
                return true;

            double angle1 = Math.Round(RadianToDegree(radian1), 2);
            double angle2 = Math.Round(RadianToDegree(radian2), 2);

            if (angle1 - 180 == angle2 || angle1 + 180 == angle2)
                return true;
            return false;
        }

        internal static double SlopeTwoPoints(Point3d p1, Point3d p2)
        {
            return Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
        }

        internal static PointEspecial1 CheckIntersectionLines(
            Point3d pp1,
            Point3d pp2,
            Point3d pp3,
            Point3d pp4,
            int pointPrecision,
            double intersectionTolerance)
        {
            Point3d p1 = RoundPoint(pp1, pointPrecision);
            Point3d p2 = RoundPoint(pp2, pointPrecision);
            Point3d p3 = RoundPoint(pp3, pointPrecision);
            Point3d p4 = RoundPoint(pp4, pointPrecision);

            double xD1, yD1, xD2, yD2, xD3, yD3;
            double dot, deg, len1, len2;
            double segmentLen1, segmentLen2;
            double ua, ub, div;

            xD1 = p2.X - p1.X;
            xD2 = p4.X - p3.X;
            yD1 = p2.Y - p1.Y;
            yD2 = p4.Y - p3.Y;
            xD3 = p1.X - p3.X;
            yD3 = p1.Y - p3.Y;

            len1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            len2 = Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            dot = (xD1 * xD2 + yD1 * yD2);
            deg = dot / (len1 * len2);

            if (Math.Abs(deg) == 1)
                return null;

            PointEspecial1 pt = new PointEspecial1();
            div = yD2 * xD1 - xD2 * yD1;
            ua = (xD2 * yD3 - yD2 * xD3) / div;
            ub = (xD1 * yD3 - yD1 * xD3) / div;
            pt.X = p1.X + ua * xD1;
            pt.Y = p1.Y + ua * yD1;

            xD1 = pt.X - p1.X;
            xD2 = pt.X - p2.X;
            yD1 = pt.Y - p1.Y;
            yD2 = pt.Y - p2.Y;
            segmentLen1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            xD1 = pt.X - p3.X;
            xD2 = pt.X - p4.X;
            yD1 = pt.Y - p3.Y;
            yD2 = pt.Y - p4.Y;
            segmentLen2 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            if (Math.Abs(len1 - segmentLen1) > intersectionTolerance || Math.Abs(len2 - segmentLen2) > intersectionTolerance)
                return null;

            return pt;
        }

        internal static bool IsTextPerpendicularToLine(DBText text, Line line, BlockReference blockReference)
        {
            Matrix3d blockTransform = blockReference.BlockTransform;
            Vector3d lineDirection = line.EndPoint.TransformBy(blockTransform) - line.StartPoint.TransformBy(blockTransform);
            Vector3d textDirection = text.AlignmentPoint.TransformBy(blockTransform) - text.Position.TransformBy(blockTransform);
            double angle = lineDirection.GetAngleTo(textDirection);

            const double tolerance = 1e-10;
            return Math.Abs(angle - Math.PI / 2) < tolerance;
        }

        internal static bool CheckPerpendicularLines(
            Point3d lineAPointStart,
            Point3d lineAPointEnd,
            Point3d lineBPointStart,
            Point3d lineBPointEnd,
            BlockReference blockReference,
            double tolerance)
        {
            Matrix3d blockTransform = blockReference.BlockTransform;
            Vector3d lineADirection = lineAPointEnd.TransformBy(blockTransform) - lineAPointStart.TransformBy(blockTransform);
            Vector3d lineBDirection = lineBPointEnd.TransformBy(blockTransform) - lineBPointStart.TransformBy(blockTransform);

            double angle = lineADirection.GetAngleTo(lineBDirection);
            double degreeAngle = RadianToDegree(angle);

            return Math.Abs(90 - degreeAngle) < tolerance;
        }

        internal static Point3d GetPointLine(Point3d p1, double angle)
        {
            double slope = Math.Tan(angle);
            Random rnd = new Random(DateTime.Now.Millisecond);
            double x = 0;
            do
            {
                x = rnd.Next(0, 500);
            } while (x == p1.X);

            double y = slope * (x - p1.X) + p1.Y;
            return new Point3d(x, y, 0);
        }
    }
}
