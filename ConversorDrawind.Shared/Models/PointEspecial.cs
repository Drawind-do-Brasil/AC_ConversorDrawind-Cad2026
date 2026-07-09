namespace ConversorDrawind
{
    public sealed class PointEspecial : Point
    {
        public PointEspecial()
        {
        }

        public PointEspecial(double x, double y, double z)
            : base(x, y, z)
        {
        }

        public PointEspecial(Point point)
            : base(point)
        {
        }
    }
}
