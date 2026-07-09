using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ConversorDrawind.Commands
{
    static class DrawingTransformOperations
    {
        public static void Zoom()
        {
            Point3d pontoMax = ConversionSession.MaxPoint3d;
            Point3d pontoMin = ConversionSession.MinPoint3d;
            Zoom(pontoMin, pontoMax);
        }

        public static void Zoom(Point3d pMin, Point3d pMax)
        {
            new ZoomService(new AcadDocumentContext(), ConversionLog.Write).Zoom(pMin, pMax);
        }

        private static void Scale(ObjectId id, Point3d basept, double scale)
        {
            new EntityScaleService(ConversionLog.Write).Scale(id, basept, scale);
        }

        public static void DeletingTekla(string layer)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            TeklaCleanupService.DeleteFromBlockLayer(documentContext, entitySelector, layer);
        }

        public static void DeletingTekla()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            TeklaCleanupService.DeleteDrawingSheetTexts(documentContext, entitySelector);
        }

        public static Point3d GetPointDiference(Point3d pontoIni, Point3d pontoRef, double scale)
        {
            return ScaleDetector.GetPointDifference(pontoIni, pontoRef, scale);
        }

        public static double GetScaleDrawing(double scale)
        {
            Point3d pIni = ConvertBlocks.GetStartPoint();
            double scaleDesenho = 1;

            if (Configuration.Config.Scale.Manual || scale <= 0)
            {
                Zoom(
                    GetPointDiference(pIni, new Point3d(Configuration.Config.Scale.Point1.X, Configuration.Config.Scale.Point1.Y, Configuration.Config.Scale.Point1.Z), scaleDesenho),
                    GetPointDiference(pIni, new Point3d(Configuration.Config.Scale.Point2.X, Configuration.Config.Scale.Point2.Y, Configuration.Config.Scale.Point2.Z), scaleDesenho));

                ScaleForm scaleF = new ScaleForm();
                scaleF.TopMost = true;
                scaleF.ShowDialog();
                scale = scaleF.scale;
                scaleF.Dispose();
            }

            return scale;
        }

        public static double ScaleDrawing(double scale)
        {
            Point3d ptMax = ConversionSession.MaxPoint3d;
            Point3d ptMin = ConversionSession.MinPoint3d;
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);

            Zoom(ptMin, ptMax);
            List<ObjectId> myIDs = new List<ObjectId>();
            PromptSelectionResult psr = entitySelector.SelectAll();
            if (psr.Status == PromptStatus.OK)
                myIDs.AddRange(psr.Value.GetObjectIds());

            foreach (ObjectId item in myIDs)
            {
                Scale(item, ptMin, scale);
            }

            return scale;
        }

        public static double ScaleDrawingInv(double scale, List<Block> blockClasso)
        {
            List<ObjectId> myIDs = new List<ObjectId>();
            Point3d ptMax = ConversionSession.MaxPoint3d;
            Point3d ptMin = ConversionSession.MinPoint3d;

            Zoom(ptMin, ptMax);

            try
            {
                foreach (Block item in blockClasso)
                {
                    myIDs.AddRange(ConvertBlocks.FilterBlock(item.blockName));
                }
            }
            catch (System.Exception e)
            {
                ConversionLog.Write(LogContext.ZoomNoDesenho, e.Message);
            }

            foreach (ObjectId item in myIDs)
            {
                Scale(item, ptMin, scale);
            }

            return scale;
        }

        public static bool WhatIsTheOrientation(Point3d p1, Point3d p2, string orientacao)
        {
            return ScaleDetector.IsOrientation(p1, p2, orientacao);
        }
    }
}
