using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawind.Commands
{
    class DimensionProperties
    {
        public string Text = "XXX";
        public Point3d XLine1Point = new Point3d(0, 15, 0);
        public Point3d XLine2Point = new Point3d(20, 20, 0);
        public Point3d DimLinePoint = new Point3d(55, 20, 0);
        public Point3d Center = new Point3d(0, 0, 0);
        public Point3d ChordPoint = new Point3d(5, 5, 0);
        public Point3d XLine1Start = new Point3d(0, 5, 0);
        public Point3d XLine1End = new Point3d(1, 7, 0);
        public Point3d XLine2Start = new Point3d(0, 5, 0);
        public Point3d XLine2End = new Point3d(1, 3, 0);
        public Point3d ArcPoint = new Point3d(3, 5, 0);
        public Point3d OverrideCenter = new Point3d(0, 2, 0);
        public Point3d JogPoint = new Point3d(1, 4.5, 0);
        public Point3d TextPosition = new Point3d(0, 0, 0);
        public double Rotation = 0;
        public double TextRotation = 0;
        public double LeaderLength = 5;
        public double JogAngle = 0.707;

    }
}
