using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawindDLL
{
    internal static class BlockMatcher
    {
        internal static bool CheckPoint(Point3d cp, Point3d p1, Point3d p2)
        {
            Point3d pp1 = new Point3d(
                (p1.X <= p2.X) ? p1.X : p2.X,
                (p1.Y <= p2.Y) ? p1.Y : p2.Y,
                (p1.Z <= p2.Z) ? p1.Z : p2.Z);

            Point3d pp2 = new Point3d(
                (p1.X > p2.X) ? p1.X : p2.X,
                (p1.Y > p2.Y) ? p1.Y : p2.Y,
                (p1.Z > p2.Z) ? p1.Z : p2.Z);

            if (cp.X >= pp1.X && cp.Y >= pp1.Y && cp.X <= pp2.X && cp.Y <= pp2.Y)
                return true;
            return false;
        }
    }
}
