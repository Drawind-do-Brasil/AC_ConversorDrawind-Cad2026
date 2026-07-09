using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{
    class FixArrow
    {
        private static double distFactor = 0.2;
        private static double distFactor2 = 0.08;
        private static double scale = 1;
        private static double distMin = 7.23;

        internal static void ResetForTests()
        {
            distFactor = 0.2;
            distFactor2 = 0.08;
            scale = 1;
            distMin = 7.23;
        }

        public static void ConsetaSetaSeta(string tipoSeta, double escala, double distanciaMinima)
        {
            scale = escala;
            distMin = distanciaMinima;
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database dataBase = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            using (Transaction transaction = dataBase.TransactionManager.MyStartTransaction())
            {
                try
                {
                    ObjectId[] objectsID = new ObjectId[0];
                    PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(FilterByType(new string[] { "DIMENSION" }));
                    if (promptSelectionResult.Status == PromptStatus.OK)
                        objectsID = promptSelectionResult.Value.GetObjectIds();

                    foreach (ObjectId id in objectsID)
                    {
                        Entity entity = transaction.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (entity.GetType() == typeof(RotatedDimension))
                        {
                            RotatedDimension rotatedDimension = entity as RotatedDimension;

                            Point3d point = ArrowFixService.ProjectFirstExtensionPoint(
                                rotatedDimension.XLine1Point,
                                rotatedDimension.XLine2Point,
                                rotatedDimension.DimLinePoint);

                            if (rotatedDimension.DimLinePoint.DistanceTo(point) < distMin * scale)
                            {
                                if (AnalisePoint(rotatedDimension, rotatedDimension.DimLinePoint))
                                {
                                    rotatedDimension.Dimsah = true;
                                    rotatedDimension.Dimblk2 = ConvertLayer.GetArrowObjectId(tipoSeta);
                                }
                                if (AnalisePoint(rotatedDimension, point))
                                {
                                    ObjectId ids = rotatedDimension.Dimblk2;
                                    rotatedDimension.Dimsah = true;
                                    rotatedDimension.Dimblk1 = ConvertLayer.GetArrowObjectId(tipoSeta);
                                    rotatedDimension.Dimblk2 = ids;

                                }
                            }
                        }
                    }
                    editor.Regen();
                }
                catch (Exception e)
                {
                    ConversionLog.Write(LogContext.FixarSetaDaCota, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        public static bool AnalisePoint(RotatedDimension rotatedDimension, Point3d pt)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database dataBase = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            try
            {

                List<ObjectId> listObjectId = new List<ObjectId>();
                Point3d P1 = new Point3d(pt.X + (scale * distFactor), pt.Y + (scale * distFactor), pt.Z);
                Point3d P2 = new Point3d(pt.X - (scale * distFactor), pt.Y - (scale * distFactor), pt.Z);
                PromptSelectionResult psr = entitySelector.SelectCrossingWindow(P1, P2, FilterByType(new string[] { "DIMENSION" }));
                if (psr.Status == PromptStatus.OK)
                    listObjectId.AddRange(psr.Value.GetObjectIds());

                for (int i = 0; i < listObjectId.Count; i++)
                {
                    if (listObjectId[i].ToString() == rotatedDimension.Id.ToString())
                    {
                        listObjectId.RemoveAt(i);
                        break;
                    }
                }

                for (int i = 0; i < listObjectId.Count; i++)
                {
                    Entity entity = listObjectId[i].GetObject(OpenMode.ForRead) as Entity;
                    if (entity.GetType() == typeof(RotatedDimension))
                    {
                        RotatedDimension dimension = entity as RotatedDimension;
                        Point3d point = ArrowFixService.ProjectFirstExtensionPoint(
                            dimension.XLine1Point,
                            dimension.XLine2Point,
                            dimension.DimLinePoint);
                        double t1 = pt.DistanceTo(dimension.DimLinePoint);
                        double t2 = pt.DistanceTo(point);
                        if (pt.DistanceTo(dimension.DimLinePoint) > (distFactor2 * scale) && pt.DistanceTo(point) > (distFactor2 * scale))
                        {
                            listObjectId.RemoveAt(i);
                            i--;
                        }
                    }
                }

                if (listObjectId.Count == 1)
                {
                    Entity entity = listObjectId.First().GetObject(OpenMode.ForRead) as Entity;
                    if (entity.GetType() == typeof(RotatedDimension))
                    {
                        RotatedDimension dimension = entity as RotatedDimension;
                        if (GetDistDimension(dimension) < distMin * scale)
                            return true;
                    }
                }

            }
            catch (Exception e)
            {
                ConversionLog.Write(LogContext.FixarSetaDaCota, e.Message);
            }



            return false;
        }

        public static double GetDistDimension(RotatedDimension rotatedDimension)
        {
            return ArrowFixService.GetDimensionDistance(rotatedDimension);
        }

        public static SelectionFilter FilterByType(string[] tiposDeObjetos)
        {
            return new SelectionFilter(LayerFilterFactory.ObjectTypes(tiposDeObjetos));
        }
    }
}

