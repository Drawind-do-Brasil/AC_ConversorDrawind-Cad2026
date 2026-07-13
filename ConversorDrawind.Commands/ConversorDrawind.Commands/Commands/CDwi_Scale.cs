using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_Scale")]
        public static void CDwi_ConvertToScale()
        {

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            ScaleWorkflow scaleWorkflow = new ScaleWorkflow(systemVariables);
            Configuration configuration = Configuration.Config;
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                new AcadEditorMessenger(editor),
                ConversionLog.Write,
                ConversionMessages.ShowWarningIfEnabled);

            stepRunner.Run(
                Localization.StartScalingDrawing,
                () =>
                {
                    ConversionSession.AppliedScale = DrawingTransformOperations.GetScaleDrawing(ConversionSession.CapturedScale);

                    DrawingTransformOperations.ScaleDrawing(ConversionSession.AppliedScale);
                    if (configuration.General.ConverterType == 1)
                        new DimensionScaleService(LayerSelectionQueries.Filter, documentContext, ConversionLog.Write)
                            .ApplyScale(ConversionSession.AppliedScale);
                    scaleWorkflow.ApplyDrawingScale(configuration.Lines.LineTypeScale, configuration.Dimensions.Scale, ConversionSession.AppliedScale);
                    Point3d ptMax = ConversionSession.MaxPoint3d;
                    Point3d ptMin = ConversionSession.MinPoint3d;
                    database.Limmax = new Point2d(ptMax.X * ConversionSession.AppliedScale, ptMax.Y * ConversionSession.AppliedScale);
                    database.Limmin = new Point2d(ptMin.X, ptMin.Y);

                    using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
                    {
                        try
                        {
                            ViewportTableRecord acVportTblRec = acTrans.GetObject(document.Editor.ActiveViewportId, OpenMode.ForWrite) as ViewportTableRecord;
                            acVportTblRec.GridEnabled = true;
                            acVportTblRec.GridIncrements = new Point2d(ConversionSession.AppliedScale * 10, ConversionSession.AppliedScale * 10);
                            document.Editor.UpdateTiledViewportsFromDatabase();
                            if (configuration.General.ConverterType == 0)
                            {
                                DimStyleTable dimStyleTable = (DimStyleTable)acTrans.GetObject(database.DimStyleTableId, OpenMode.ForRead);
                                DimStyleTableRecord dimStyleTableRecord = null;
                                if (dimStyleTable.Has(configuration.Dimensions.StyleName) == true)
                                {
                                    dimStyleTableRecord = acTrans.GetObject(dimStyleTable[configuration.Dimensions.StyleName],
                                                          OpenMode.ForWrite) as DimStyleTableRecord;
                                    dimStyleTableRecord.Dimscale = configuration.Dimensions.Scale * ConversionSession.AppliedScale;
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            ConversionLog.Write(LogContext.CapturarEscalaDoDesenho, e.Message);
                        }
                        finally
                        {
                            acTrans.MyCommit();
                        }
                    }

                    DrawingTransformOperations.Zoom(ptMin, new Point3d(ptMax.X * ConversionSession.AppliedScale, ptMax.Y * ConversionSession.AppliedScale, ptMax.Z * ConversionSession.AppliedScale));
                },
                LogContext.CapturarEscalaDoDesenho,
                Localization.WarningCouldNotScaleDrawing,
                Localization.ErrorScalingDrawing + "\n",
                Localization.MessageCompleted + "\n");


        }
    }
}
