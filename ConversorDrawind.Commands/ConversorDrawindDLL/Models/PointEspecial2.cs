using System;

namespace ConversorDrawindDLL
{
    public class PointEspecial2 : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        public PointEspecial2()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public PointEspecial2(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public PointEspecial2(PointEspecial2 point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public PointEspecial2(global::ConversorDrawind.Point point)
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
