using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConversorDrawind.Commands
{
    internal sealed class DimensionPrecisionService
    {
        private readonly Func<string, string, string, string, ObjectId[]> filter;
        private readonly IAcadDocumentContext documentContext;
        private readonly Action<string, string> logError;

        internal DimensionPrecisionService(
            Func<string, string, string, string, ObjectId[]> filter,
            IAcadDocumentContext documentContext,
            Action<string, string> logError)
        {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void UpdateDimensionPrecision()
        {
            Regex regex = new Regex(@"\d+(\,\d*)?");

            ObjectId[] ids = filter("ALL", "DIMENSION", "ALL", "ALL");
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (ObjectId item in ids)
                    {
                        Entity entity = transaction.GetObject(item, OpenMode.ForWrite) as Entity;
                        Dimension dimension = entity as Dimension;
                        DBObjectCollection objects = new DBObjectCollection();
                        dimension.Explode(objects);
                        List<string> texts = new List<string>();
                        dimension.TextPosition = dimension.TextPosition;
                        foreach (DBObject explodedObject in objects)
                        {
                            if (explodedObject.GetType() == typeof(MText))
                            {
                                MText mText = explodedObject as MText;
                                Match match = regex.Match(mText.Text);
                                if (match.Success)
                                    texts.Add(match.Value.ReplaceComma());
                            }
                        }

                        List<string> orderedTexts = texts.OrderBy(p => Math.Abs(Convert.ToDouble(p) - dimension.Measurement)).ToList();
                        if (dimension != null)
                        {
                            double measure = Math.Round(dimension.Measurement, dimension.Dimdec);
                            if (dimension.GetType() == typeof(Point3AngularDimension) ||
                                dimension.GetType() == typeof(LineAngularDimension2))
                                measure = Math.Round(dimension.Measurement, dimension.Dimadec);

                            int precision = orderedTexts.First().Split(',').Last().Length;
                            if (!orderedTexts.First().Contains(','))
                                precision = 0;

                            dimension.Dimadec = precision;
                            dimension.Dimdec = precision;
                        }
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.AtualizarPrecisaoDaCota, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
