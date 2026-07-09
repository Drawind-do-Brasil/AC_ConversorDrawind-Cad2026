using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawindDLL
{
    internal sealed class ConversionExtentsWorkflow
    {
        private readonly Action refreshExtents;
        private readonly Func<Point3d> getMinPoint;
        private readonly Func<Point3d> getMaxPoint;
        private readonly Action moveToOrigin;
        private readonly Action<ConversorDrawind.Point> setMinPoint;
        private readonly Action<double, double, double> setMaxPoint;

        internal ConversionExtentsWorkflow(
            Action refreshExtents,
            Func<Point3d> getMinPoint,
            Func<Point3d> getMaxPoint,
            Action moveToOrigin,
            Action<ConversorDrawind.Point> setMinPoint,
            Action<double, double, double> setMaxPoint)
        {
            this.refreshExtents = refreshExtents ?? throw new ArgumentNullException(nameof(refreshExtents));
            this.getMinPoint = getMinPoint ?? throw new ArgumentNullException(nameof(getMinPoint));
            this.getMaxPoint = getMaxPoint ?? throw new ArgumentNullException(nameof(getMaxPoint));
            this.moveToOrigin = moveToOrigin ?? throw new ArgumentNullException(nameof(moveToOrigin));
            this.setMinPoint = setMinPoint ?? throw new ArgumentNullException(nameof(setMinPoint));
            this.setMaxPoint = setMaxPoint ?? throw new ArgumentNullException(nameof(setMaxPoint));
        }

        internal void RefreshAndZoom()
        {
            refreshExtents();
            ZoomToCurrentExtents();
        }

        internal void MoveToOriginAndRefreshZoom()
        {
            Point3d minPoint = getMinPoint();
            Point3d maxPoint = getMaxPoint();

            moveToOrigin();

            setMaxPoint(
                maxPoint.X - minPoint.X,
                maxPoint.Y - minPoint.Y,
                maxPoint.Z - minPoint.Z);
            setMinPoint(new ConversorDrawind.Point(0, 0, 0));

            ZoomToCurrentExtents();
        }

        private void ZoomToCurrentExtents()
        {
            ConvertLayer.Zoom(getMaxPoint(), getMinPoint());
        }
    }
}
