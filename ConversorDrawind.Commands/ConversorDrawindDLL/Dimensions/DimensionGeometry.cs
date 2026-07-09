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
    }
}
