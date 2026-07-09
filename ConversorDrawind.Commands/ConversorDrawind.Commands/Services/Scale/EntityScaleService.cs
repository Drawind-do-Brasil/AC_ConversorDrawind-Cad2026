using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class EntityScaleService
    {
        private readonly Action<string, string> logError;

        internal EntityScaleService(Action<string, string> logError)
        {
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void Scale(ObjectId id, Point3d basePoint, double scale)
        {
            Matrix3d transform = Matrix3d.Scaling(scale, basePoint);

            Database database = id.Database;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite);

                    if (entity != null)
                    {
                        entity.TransformBy(transform);
                    }

                    if (string.Equals(id.ObjectClass.DxfName, "DIMENSION", StringComparison.OrdinalIgnoreCase))
                    {
                        Dimension dimension = entity as Dimension;
                        dimension.Dimscale = scale;
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    logError(LogContext.DefinirEscalaDoBloco, ex.Message);
                    Application.ShowAlertDialog(ex.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
