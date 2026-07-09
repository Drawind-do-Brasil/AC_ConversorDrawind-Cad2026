using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class DimensionScaleService
    {
        private readonly Func<string, string, string, string, ObjectId[]> filter;
        private readonly IAcadDocumentContext documentContext;
        private readonly Action<string, string> logError;

        internal DimensionScaleService(
            Func<string, string, string, string, ObjectId[]> filter,
            IAcadDocumentContext documentContext,
            Action<string, string> logError)
        {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void ApplyScale(double scale)
        {
            ObjectId[] ids = filter("ALL", "DIMENSION", "ALL", "ALL");
            Database database = documentContext.Database;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (ObjectId item in ids)
                    {
                        try
                        {
                            Entity entity = transaction.GetObject(item, OpenMode.ForWrite) as Entity;
                            Dimension dimension = entity as Dimension;
                            if (dimension != null)
                            {
                                dimension.Dimscale = scale;
                                dimension.Dimlfac /= scale;
                            }
                        }
                        catch (Exception e)
                        {
                            logError(LogContext.AtualizarConfiguracaoDaDimensao, e.Message);
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
