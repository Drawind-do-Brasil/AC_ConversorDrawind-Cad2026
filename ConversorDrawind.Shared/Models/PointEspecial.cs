using System;

namespace ConversorDrawind
{
    public class PointEspecial : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public PointEspecial()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public PointEspecial(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public PointEspecial(PointEspecial point)
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




