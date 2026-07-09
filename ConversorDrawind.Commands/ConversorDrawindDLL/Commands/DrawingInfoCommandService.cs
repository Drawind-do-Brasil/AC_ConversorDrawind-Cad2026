using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.IO;

namespace ConversorDrawindDLL
{
    internal sealed class DrawingInfoCommandService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly Action<string, string> logError;

        internal DrawingInfoCommandService(IAcadDocumentContext documentContext, Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void CapturePoint()
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptPointOptions promptPointOptions = new PromptPointOptions(Localization.PromptSelectPoint);
                    promptPointOptions.AllowNone = false;

                    PromptPointResult promptPointResult = editor.GetPoint(promptPointOptions);

                    if (promptPointResult.Status.ToString() == "OK")
                    {
                        Point3d myPoint = promptPointResult.Value;
                        WriteTempFile("ConvertTo.PointInfo", myPoint.X + ";" + myPoint.Y + ";" + myPoint.Z);
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.CapturarInformacaoDoDesenho, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        internal void CaptureTwoPoints()
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    Point3d firstPoint = editor.GetPoint(Localization.PromptSelectFirstPoint).Value;
                    Point3d secondPoint = editor.GetCorner(Localization.PromptSelectSecondPoint, firstPoint).Value;

                    WriteTempFile(
                        "ConvertTo.Point2Info",
                        firstPoint.X + ";" + firstPoint.Y + ";" + firstPoint.Z,
                        secondPoint.X + ";" + secondPoint.Y + ";" + secondPoint.Z);
                }
                catch (Exception e)
                {
                    logError(LogContext.CapturarPonto, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        internal void CaptureLayer()
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptEntityOptions promptEntityOptions = new PromptEntityOptions(Localization.PromptSelectObject);
                    promptEntityOptions.AllowNone = false;
                    promptEntityOptions.SetRejectMessage("");
                    PromptEntityResult promptEntityResult = editor.GetEntity(promptEntityOptions);
                    Entity dBObject = acTrans.GetObject(promptEntityResult.ObjectId, OpenMode.ForRead) as Entity;
                    if (dBObject.Layer != "")
                    {
                        WriteTempFile("ConvertTo.LayerInfo", dBObject.Layer);
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.CapturarPontos, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        internal void CaptureTextHeight()
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptEntityOptions promptEntityOptions = new PromptEntityOptions(Localization.PromptSelectObject);
                    promptEntityOptions.AllowNone = false;
                    promptEntityOptions.SetRejectMessage("");
                    PromptEntityResult promptEntityResult = editor.GetEntity(promptEntityOptions);
                    Entity dBObject = acTrans.GetObject(promptEntityResult.ObjectId, OpenMode.ForRead) as Entity;
                    if (dBObject.GetType() == typeof(DBText))
                    {
                        DBText myText = dBObject as DBText;
                        WriteTempFile("ConvertTo.TextHeightInfo", myText.Height.ToString());
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.CapturarDistanciaVertical, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        internal void CaptureHorizontalDistance()
        {
            CaptureDistance(LogContext.CapturarDistanciaHorizontal, point => point.X);
        }

        internal void CaptureVerticalDistance()
        {
            CaptureDistance(null, point => point.Y);
        }

        private void CaptureDistance(string logContext, Func<Point3d, double> coordinateSelector)
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptPointOptions promptPointOptions = new PromptPointOptions(Localization.PromptSelectFirstDistancePoint);
                    promptPointOptions.AllowNone = false;

                    PromptPointResult promptPointResult = editor.GetPoint(promptPointOptions);

                    PromptPointOptions promptPointOptions2 = new PromptPointOptions(Localization.PromptSelectSecondDistancePoint);
                    promptPointOptions2.AllowNone = false;

                    PromptPointResult promptPointResult2 = editor.GetPoint(promptPointOptions2);

                    if (promptPointResult.Status.ToString() == "OK" && promptPointResult2.Status.ToString() == "OK")
                    {
                        double firstCoordinate = coordinateSelector(promptPointResult.Value);
                        double secondCoordinate = coordinateSelector(promptPointResult2.Value);
                        double distance = firstCoordinate < secondCoordinate
                            ? secondCoordinate - firstCoordinate
                            : firstCoordinate - secondCoordinate;

                        WriteTempFile("ConvertTo.DistInfo", distance.ToString());
                    }
                }
                catch (Exception e)
                {
                    if (logContext != null)
                    {
                        logError(logContext, e.Message);
                    }
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        private static void WriteTempFile(string fileName, params string[] lines)
        {
            string path = Path.GetTempPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, fileName);

            if (File.Exists(path))
                File.Delete(path);

            StreamWriter streamWriter = new StreamWriter(path);
            foreach (string line in lines)
            {
                streamWriter.WriteLine(line);
            }
            streamWriter.Close();
        }
    }
}
