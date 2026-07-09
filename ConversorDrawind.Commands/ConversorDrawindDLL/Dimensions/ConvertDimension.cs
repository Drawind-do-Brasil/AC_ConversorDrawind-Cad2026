using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ConversorDrawindDLL
{
    class ConvertDimension
    {
        private const int DefaultPointPrecision = 3;
        private const int ArcCenterPrecision = 1;
        private const double IntersectionTolerance = 0.01;
        private const double PerpendicularTolerance = 0.01;

        private Document document;
        private Database database;
        private Editor editor;
        private Transaction transaction;
        private DimensionEntityWriter entityWriter;
        private DimensionTextEntityService textEntityService;


 
        public void ConvertByInventor()
        {
            ConvertLayer.CreateDimstyle2();
            ObjectId[] ids = ConvertLayer.Filter("ALL", "DIMENSION", "ALL", "ALL");
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database acCurDb = documentContext.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (ObjectId item in ids)
                    {

                        Entity myEntity = acTrans.GetObject(item, OpenMode.ForWrite) as Entity;
                        Dimension d = myEntity as Dimension;
                        if (d != null)
                        {
                            d.Layer = Configuration.Config.Dimensions.Layer;
                        }


                    }
                }
                catch (Exception e)
                {

                    ConversionLog.Write(LogContext.ConverterCotas, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }


        public void ConvertByTekla()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            document = documentContext.Document;
            database = documentContext.Database;
            editor = documentContext.Editor;
            ObjectId ds = ConvertLayer.CreateDimstyle();

            using (transaction = database.TransactionManager.MyStartTransaction())
            {
                entityWriter = new DimensionEntityWriter(database, transaction);
                textEntityService = new DimensionTextEntityService(database, transaction);

                try
                {
                    List<ObjectId> oID = new List<ObjectId>();

                    try
                    {
                        oID.AddRange(FilterDimension());
                    }
                    catch (System.Exception e)
                    {
                        ConversionLog.Write(LogContext.ConverterCotaRadial, e.Message);
                    }

                    if (oID.Count > 0)
                        ConvertFactory(oID.ToArray(), ds);
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.ConverterCotaLinear, e.Message);
                    throw new System.InvalidOperationException(Localization.MessageInvalidLinearDimensionEntity);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }


        private ObjectId[] FilterDimension()
        {
            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnLayer(Configuration.Config.Dimensions.BaseLayer));
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            ObjectId[] objectIdList = entitySelector.SelectAll(selectionFilter).Value.GetObjectIds();
            return objectIdList;
        }

        private void ConvertFactory(ObjectId[] objectIdList, ObjectId ds)
        {
            for (int i = 0; i < objectIdList.Length; i++)
            {
                ObjectId objectId = objectIdList[i];
                BlockReference blockReference = (BlockReference)objectId.GetObject(OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)blockReference.BlockTableRecord.GetObject(OpenMode.ForWrite);

                ObjectsInBlock objectsInBlock = DimensionBlockReader.Read(blockTableRecord, blockReference, ds);

                switch (DimensionBlockClassifier.Classify(objectsInBlock))
                {
                    case DimensionBlockKind.Arc:
                        ConvertArcDimension(objectsInBlock, ref blockTableRecord);
                        break;
                    case DimensionBlockKind.LinearOrElevation:
                        if (!ConvertLineDimension(objectsInBlock, blockReference, ref blockTableRecord))
                        {
                            //Tratar cotas picadas do tipo 3 x XXX = XXXX
                            //ConvertLineDimensionBreak(objectsInBlock, ref blockTableRecord, ref objectIdList, i + 1);
                        }
                        break;
                }
            }
        }



        private bool ConvertLineDimension(ObjectsInBlock objectsInBlock, BlockReference blockReference, ref BlockTableRecord blockTableRecord)
        {
            List<Line> lineListParallel = new List<Line>();
            List<Line> lineListPerpendicular = new List<Line>();
            List<Line> lineListPerpendicularTemp = new List<Line>();
            List<Line> lineListOther = new List<Line>();
            List<Line> lineListPerpendicularBase = new List<Line>();
            List<Point3d> ListPoinsIntersection = new List<Point3d>();
            for (int j = 0; j < objectsInBlock.lineList.Count; j++)
            {
                if (DimensionGeometry.CheckParallelLine(objectsInBlock.dBTextList[0].Rotation,
                    objectsInBlock.lineList[j].Angle))
                {
                    lineListParallel.Add(objectsInBlock.lineList[j]);
                }
                /**/
                else if (DimensionGeometry.CheckPerpendicularLines(DimensionGeometry.RoundPoint(objectsInBlock.dBTextList.First().Position, DefaultPointPrecision),
                         DimensionGeometry.RoundPoint(DimensionGeometry.GetPointLine(objectsInBlock.dBTextList.First().Position,
                         objectsInBlock.dBTextList.First().Rotation), DefaultPointPrecision),
                         DimensionGeometry.RoundPoint(objectsInBlock.lineList[j].StartPoint, DefaultPointPrecision),
                         DimensionGeometry.RoundPoint(objectsInBlock.lineList[j].EndPoint, DefaultPointPrecision), blockReference, PerpendicularTolerance))
                {
                    lineListPerpendicular.Add(objectsInBlock.lineList[j]);
                    lineListPerpendicularTemp.Add(objectsInBlock.lineList[j]);
                }
                else
                {

                    lineListOther.Add(objectsInBlock.lineList[j]);
                }
            }

            List<Line> verdadeiros = new List<Line>();

            for (int j = 0; j < lineListOther.Count; j++)
            {
                bool falso = true;
                for (int k = 0; k < lineListPerpendicular.Count; k++)
                {
                    PointEspecial1 PointTemp = DimensionGeometry.CheckIntersectionLines(lineListOther[j].StartPoint,
                                      lineListOther[j].EndPoint,
                                      lineListPerpendicular[k].StartPoint,
                                      lineListPerpendicular[k].EndPoint,
                                      DefaultPointPrecision,
                                      IntersectionTolerance);
                    if (PointTemp != null)
                    {
                        for (int l = 0; l < lineListParallel.Count; l++)
                        {
                            PointEspecial1 PointTemp2 = DimensionGeometry.CheckIntersectionLines(lineListOther[j].StartPoint,
                                              lineListOther[j].EndPoint,
                                              lineListParallel[l].StartPoint,
                                              lineListParallel[l].EndPoint,
                                              DefaultPointPrecision,
                                              IntersectionTolerance);
                            if (PointTemp2 != null)
                            {
                                verdadeiros.Add(lineListOther[j]);
                                ListPoinsIntersection.Add(new Point3d(PointTemp.X, PointTemp.Y, 0));
                                falso = false;
                            }
                        }
                    }
                }
                if (falso)
                    entityWriter.CreateLine(lineListOther[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                   lineListOther[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
            }
            verdadeiros = verdadeiros.Distinct().ToList();
            for (int j = 0; j < verdadeiros.Count; j++)
            {
                for (int k = 0; k < lineListPerpendicularTemp.Count; k++)
                {
                    if (DimensionGeometry.CheckIntersectionLines(verdadeiros[j].StartPoint,
                        verdadeiros[j].EndPoint, lineListPerpendicularTemp[k].StartPoint,
                        lineListPerpendicularTemp[k].EndPoint,
                        DefaultPointPrecision,
                        IntersectionTolerance) != null)
                    {
                        lineListPerpendicularBase.Add(lineListPerpendicularTemp[k]);
                        lineListPerpendicularTemp.RemoveAt(k);
                        k--;
                    }
                }
            }

            ListPoinsIntersection = ListPoinsIntersection.Distinct(new RoundedPoint3dComparer(DefaultPointPrecision)).ToList();
            bool asLinePerpendicular = false;

            if (lineListPerpendicular.Count == 1)
                asLinePerpendicular = true;

            else
            {
                try
                {
                    if (DimensionGeometry.IsOnLine(lineListPerpendicular.First().StartPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().EndPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().StartPoint.TransformBy(objectsInBlock.matrix3d)) ||
                                 DimensionGeometry.IsOnLine(lineListPerpendicular.First().EndPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().StartPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().EndPoint.TransformBy(objectsInBlock.matrix3d)))
                        asLinePerpendicular = true;
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.ConverterCotaAngular, e.Message);
                }
            }


            if (!asLinePerpendicular)
            {
                try
                {
                    DimensionProperties dimensionProperties = new DimensionProperties();

                    if (lineListPerpendicularBase.Count == 2)
                    {
                        dimensionProperties.XLine1Point = lineListPerpendicularBase.First().StartPoint.TransformBy(objectsInBlock.matrix3d);
                        dimensionProperties.XLine2Point = lineListPerpendicularBase.Last().StartPoint.TransformBy(objectsInBlock.matrix3d);
                        for (int j = 0; j < lineListPerpendicularTemp.Count; j++)
                        {
                            entityWriter.CreateLine(lineListPerpendicularTemp[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                       lineListPerpendicularTemp[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                        }
                    }
                    else
                    {
                        dimensionProperties.XLine1Point = lineListPerpendicular.First().StartPoint.TransformBy(objectsInBlock.matrix3d);
                        dimensionProperties.XLine2Point = lineListPerpendicular.Last().StartPoint.TransformBy(objectsInBlock.matrix3d);
                    }

                    dimensionProperties.Text = objectsInBlock.dBTextList.First().TextString;
                    dimensionProperties.DimLinePoint = lineListParallel.First().StartPoint.TransformBy(objectsInBlock.matrix3d);
                    double rotation2 = DimensionGeometry.SlopeTwoPoints(lineListParallel.First().StartPoint.TransformBy(objectsInBlock.matrix3d),
                                                      lineListParallel.First().EndPoint.TransformBy(objectsInBlock.matrix3d));
                    dimensionProperties.Rotation = rotation2;

                    BlockTableRecord acBlkTblRec = entityWriter.GetModelSpaceForWrite();

                    dimensionProperties.TextPosition = textEntityService.CalculateAlignedTextPosition(
                        objectsInBlock.dBTextList.First(),
                        objectsInBlock.matrix3d,
                        acBlkTblRec);
                    dimensionProperties.TextRotation = objectsInBlock.dBTextList.First().Rotation;
                    entityWriter.CreateRotatedDimension(dimensionProperties,
                                           objectsInBlock.dimStyle);


                    if (lineListParallel.Count > 1)
                    {
                        if (ListPoinsIntersection.Count == 2)
                        {
                            for (int j = 0; j < lineListParallel.Count; j++)
                            {
                                bool t1 = false;
                                bool t2 = false;

                                if (DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), ArcCenterPrecision).X < DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), ArcCenterPrecision).X)
                                {
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).X >= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).X &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).X <= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).X)
                                        t1 = true;
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).X >= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).X &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).X <= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).X)
                                        t2 = true;
                                }
                                else if (DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), ArcCenterPrecision).X > DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), ArcCenterPrecision).X)
                                {
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).X >= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).X &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).X <= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).X)
                                        t1 = true;
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).X >= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).X &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).X <= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).X)
                                        t2 = true;
                                }
                                else if (DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), ArcCenterPrecision).Y < DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), ArcCenterPrecision).Y)
                                {
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).Y >= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).Y &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).Y <= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).Y)
                                        t1 = true;
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).Y >= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).Y &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).Y <= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).Y)
                                        t2 = true;
                                }
                                else if (DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), ArcCenterPrecision).Y > DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), ArcCenterPrecision).Y)
                                {
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).Y >= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).Y &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].StartPoint, DefaultPointPrecision).Y <= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).Y)
                                        t1 = true;
                                    if (DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).Y >= DimensionGeometry.RoundPoint(ListPoinsIntersection.Last(), DefaultPointPrecision).Y &&
                                       DimensionGeometry.RoundPoint(lineListParallel[j].EndPoint, DefaultPointPrecision).Y <= DimensionGeometry.RoundPoint(ListPoinsIntersection.First(), DefaultPointPrecision).Y)
                                        t2 = true;
                                }

                                if (!t1 || !t2)
                                {
                                    entityWriter.CreateLine(lineListParallel[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                               lineListParallel[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                                }

                            }
                        }
                        else
                        {
                            for (int j = 1; j < lineListParallel.Count; j++)
                            {
                                entityWriter.CreateLine(lineListParallel[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                           lineListParallel[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                            }
                        }
                    }

                    textEntityService.CopyAdditionalTextEntities(objectsInBlock, rotation2, acBlkTblRec);


                    entityWriter.EraseBlockReferences(blockTableRecord);
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.ConverterCotaDiametro, e.Message);
                }

                return true;
            }
            return false;
        }

        private void ConvertArcDimension(ObjectsInBlock objectsInBlock, ref BlockTableRecord blockTableRecord)
        {
            bool type1 = true;
            Point3d pCenter = DimensionGeometry.RoundPoint(objectsInBlock.arcList.First().Center, ArcCenterPrecision);
            foreach (Line item in objectsInBlock.lineList)
            {
                if (DimensionGeometry.IsPointEqual(DimensionGeometry.RoundPoint(item.EndPoint, ArcCenterPrecision), pCenter) || DimensionGeometry.IsPointEqual(DimensionGeometry.RoundPoint(item.StartPoint, ArcCenterPrecision), pCenter))
                {
                    type1 = false;
                    break;
                }
            }

            if (type1)
            {
                try
                {
                    DimensionProperties dimensionProperties = new DimensionProperties();
                    List<Line> line1 = new List<Line>();
                    List<Line> line2 = new List<Line>();
                    List<Line> lineOther = new List<Line>();

                    line1.Add(objectsInBlock.lineList.First());
                    int i;
                    for (i = 1; i < objectsInBlock.lineList.Count; i++)
                    {
                        if (DimensionGeometry.IsOnLine(objectsInBlock.lineList.First().StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[i].EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[i].StartPoint.TransformBy(objectsInBlock.matrix3d)) ||
                         DimensionGeometry.IsOnLine(objectsInBlock.lineList.First().EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[i].StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[i].EndPoint.TransformBy(objectsInBlock.matrix3d)))
                            line1.Add(objectsInBlock.lineList[i]);
                        else
                            break;
                    }
                    lineOther.Add(objectsInBlock.lineList[i]);
                    i++;
                    line2.Add(objectsInBlock.lineList[i]);
                    for (int j = i + 1; j < objectsInBlock.lineList.Count; j++)
                    {
                        if (DimensionGeometry.IsOnLine(objectsInBlock.lineList[i].StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].StartPoint.TransformBy(objectsInBlock.matrix3d)) ||
                         DimensionGeometry.IsOnLine(objectsInBlock.lineList[i].EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].EndPoint.TransformBy(objectsInBlock.matrix3d)))
                            line2.Add(objectsInBlock.lineList[j]);
                        else
                        {
                            lineOther.Add(objectsInBlock.lineList[j]);
                            break;
                        }
                    }

                    dimensionProperties.Text = objectsInBlock.dBTextList.First().TextString;
                    dimensionProperties.XLine1Start = line2.Last().StartPoint.TransformBy(objectsInBlock.matrix3d);
                    try
                    {
                        dimensionProperties.XLine1End = line2.Last().GetPointAtParameter(0.5).TransformBy(objectsInBlock.matrix3d);
                    }
                    catch (System.Exception e)
                    {
                        ConversionLog.Write(LogContext.AtualizarTextoDeCota, e.Message);
                        dimensionProperties.XLine1End = line2.Last().EndPoint.TransformBy(objectsInBlock.matrix3d);
                    }
                    dimensionProperties.XLine2Start = line1.Last().StartPoint.TransformBy(objectsInBlock.matrix3d);
                    try
                    {
                        dimensionProperties.XLine2End = line1.Last().GetPointAtParameter(0.5).TransformBy(objectsInBlock.matrix3d);
                    }
                    catch (System.Exception e)
                    {
                        ConversionLog.Write(LogContext.ConverterCotaAlinhada, e.Message);
                        dimensionProperties.XLine2End = line1.Last().EndPoint.TransformBy(objectsInBlock.matrix3d);
                    }
                    dimensionProperties.Center = objectsInBlock.arcList.First().Center.TransformBy(objectsInBlock.matrix3d);
                    dimensionProperties.ArcPoint = objectsInBlock.arcList.First().GetPointAtParameter(objectsInBlock.arcList.First().StartAngle + (objectsInBlock.arcList.First().TotalAngle / 2)).TransformBy(objectsInBlock.matrix3d);
                    dimensionProperties.TextPosition = textEntityService.CalculateAlignedTextPosition(
                        objectsInBlock.dBTextList.First(),
                        objectsInBlock.matrix3d,
                        entityWriter.GetModelSpaceForWrite());

                    for (int j = 0; j < line1.Count - 1; j++)
                    {
                        entityWriter.CreateLine(line1[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                   line1[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                    }
                    for (int j = 0; j < line2.Count - 1; j++)
                    {
                        entityWriter.CreateLine(line2[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                   line2[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                    }

                    /* CreateAngularDimension(dimensionProperties,
                                            objectsInBlock.dimStyle);
                     */
                    double distX = 0;
                    for (int j = 0; j < objectsInBlock.arcList.Count; j++)
                    {
                        if (objectsInBlock.arcList[j].StartPoint.DistanceTo(objectsInBlock.arcList[j].EndPoint) > distX)
                            distX = objectsInBlock.arcList[j].StartPoint.DistanceTo(objectsInBlock.arcList[j].EndPoint);
                    }
                    if (distX < Configuration.Config.Dimensions.ArrowSize * 2)
                    {
                        entityWriter.CreateAngularDimensionWithLargeGap(dimensionProperties,
                                              objectsInBlock.dimStyle);
                    }
                    else
                    {
                        entityWriter.CreateAngularDimension(dimensionProperties,
                                               objectsInBlock.dimStyle);

                    }

                    entityWriter.EraseBlockReferences(blockTableRecord);

                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.ConverterCotas, e.Message);
                }
            }

            else
            {
                try
                {

                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.ConverterCotas, e.Message);
                }
            }
        }

    }
}

