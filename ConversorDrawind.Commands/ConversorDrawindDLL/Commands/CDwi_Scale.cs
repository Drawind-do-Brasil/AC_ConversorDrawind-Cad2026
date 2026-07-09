using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FORMS = System.Windows.Forms;

namespace ConversorDrawindDLL
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
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                new AcadEditorMessenger(editor),
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);

            stepRunner.Run(
                "Colocando o desenho na escala real... ",
                () =>
                {
                    escalaFinal = ConvertLayer.GetScaleDrawing(escalaCapiturada);

                    ConvertLayer.ScaleDrawing(escalaFinal);
                    if (Configuration.Config.General.ConverterType == 1)
                        UPDATE_DIMENSTION(escalaFinal);
                    scaleWorkflow.ApplyDrawingScale(Configuration.Config.Lines.LineTypeScale, Configuration.Config.Dimensions.Scale, escalaFinal);
                    Point3d ptMax = GetNewMax();
                    Point3d ptMin = GetNewMin();
                    database.Limmax = new Point2d(ptMax.X * escalaFinal, ptMax.Y * escalaFinal);
                    database.Limmin = new Point2d(ptMin.X, ptMin.Y);

                    using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
                    {
                        try
                        {
                            ViewportTableRecord acVportTblRec = acTrans.GetObject(document.Editor.ActiveViewportId, OpenMode.ForWrite) as ViewportTableRecord;
                            acVportTblRec.GridEnabled = true;
                            acVportTblRec.GridIncrements = new Point2d(escalaFinal * 10, escalaFinal * 10);
                            document.Editor.UpdateTiledViewportsFromDatabase();
                            if (Configuration.Config.General.ConverterType == 0)
                            {
                                DimStyleTable dimStyleTable = (DimStyleTable)acTrans.GetObject(database.DimStyleTableId, OpenMode.ForRead);
                                DimStyleTableRecord dimStyleTableRecord = null;
                                if (dimStyleTable.Has(Configuration.Config.Dimensions.StyleName) == true)
                                {
                                    dimStyleTableRecord = acTrans.GetObject(dimStyleTable[Configuration.Config.Dimensions.StyleName],
                                                          OpenMode.ForWrite) as DimStyleTableRecord;
                                    dimStyleTableRecord.Dimscale = Configuration.Config.Dimensions.Scale * escalaFinal;
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Conversor.EscreverLog(LogContext.CapturarEscalaDoDesenho, e.Message);
                        }
                        finally
                        {
                            acTrans.MyCommit();
                        }
                    }

                    ConvertLayer.Zoom(ptMin, new Point3d(ptMax.X * escalaFinal, ptMax.Y * escalaFinal, ptMax.Z * escalaFinal));
                },
                LogContext.CapturarEscalaDoDesenho,
                "Não foi possível colocar o desenho na escala real!",
                "Descrição: Erro ao tentar colocar o desenho na escala real...\n",
                "... Completado.\n");


        }
    }
}