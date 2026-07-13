using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class ZoomService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly Action<string, string> logError;

        internal ZoomService(IAcadDocumentContext documentContext, Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void Zoom(Point3d pMin, Point3d pMax)
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    using (ViewTableRecord viewTableRecord = editor.GetCurrentView())
                    {
                        viewTableRecord.Target = new Point3d((pMin.X + pMax.X) / 2, (pMin.Y + pMax.Y) / 2, (pMin.Z + pMax.Z) / 2);
                        viewTableRecord.CenterPoint = Point2d.Origin;

                        Point3d pMinAux = new Point3d(pMin.X, pMin.Y, 0);
                        Point3d pAuxDeltaYHeith = new Point3d(pMin.X, pMax.Y, 0);
                        Point3d pAuxDeltaXWidth = new Point3d(pMax.X, pMin.Y, 0);

                        viewTableRecord.Height = pMinAux.DistanceTo(pAuxDeltaYHeith);
                        viewTableRecord.Width = pMinAux.DistanceTo(pAuxDeltaXWidth);
                        editor.SetCurrentView(viewTableRecord);
                        editor.Regen();
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.ZoomNoDesenho, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
