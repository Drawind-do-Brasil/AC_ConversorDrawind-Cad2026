using System;

namespace ConversorDrawind
{
    public class Point : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Point(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}




