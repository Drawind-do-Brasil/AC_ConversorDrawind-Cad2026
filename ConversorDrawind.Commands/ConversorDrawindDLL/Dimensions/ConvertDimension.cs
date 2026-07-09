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

        internal class MyComparer : EqualityComparer<Point3d>
        {
            int ared = 3;
            public override bool Equals(Point3d p1, Point3d p2)
            {
                return Math.Round(p1.X, ared) == Math.Round(p2.X, ared) &&
                       Math.Round(p1.Y, ared) == Math.Round(p2.Y, ared) &&
                       Math.Round(p1.Z, ared) == Math.Round(p2.Z, ared);
            }

            public override int GetHashCode(Point3d obj)
            {
                return obj == null ? 0 : obj.GetHashCode();
            }
        }


        private Document document;
        private Database database;
        private Editor editor;
        private Transaction transaction;


        /// <summary>
        /// Constructor
        /// </summary>
        public ConvertDimension()
        {
    
        }

        public void ConvertDInv()
        {
            ConvertLayer.CreateDimstyle2();
            ObjectId[] ids = ConvertLayer.Filter("ALL","DIMENSION", "ALL", "ALL");
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
                            /*
                            if (d.Dimblk1s.ToUpper() == "DOT")
                            {
                                d.Dimblk1s = "DotSmall";
                            }
                            if (d.Dimblk2s.ToUpper() == "DOT")
                            {
                                d.Dimblk2s = "DotSmall";
                            }*/
                        }


                    }
                }
                catch (Exception e)
                {

                    Conversor.EscreverLog("Erro 41", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        /// <summary>
        /// Main Class
        /// </summary>
        public void ConvertD()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            document = documentContext.Document;
            database = documentContext.Database;
            editor = documentContext.Editor;
            ObjectId ds = ConvertLayer.CreateDimstyle();

            using (transaction = database.TransactionManager.MyStartTransaction())
            {

                try
                {
                    List<ObjectId> oID = new List<ObjectId>();

                    try
                    {
                        oID.AddRange(FilterDimension());
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 42", e.Message);
                    }

                    if (oID.Count > 0)
                        ConvertDim(oID.ToArray(), ds);
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 43", e.Message);
                    throw new System.InvalidOperationException("Erro - NFJ1");
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        /// <summary>
        /// FilterDimension by Robson Januario Martins
        /// </summary>
        /// <returns></returns>
        private ObjectId[] FilterDimension()
        {
            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnLayer(Configuration.Config.Dimensions.BaseLayer));
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            ObjectId[] objectIdList = entitySelector.SelectAll(selectionFilter).Value.GetObjectIds();
            return objectIdList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectIdList"></param>
        private void ConvertDim(ObjectId[] objectIdList, ObjectId ds)
        {

        

            for (int i = 0; i < objectIdList.Length; i++)
            {
                ObjectId objectId = objectIdList[i];
                BlockReference blockReference = (BlockReference)objectId.GetObject(OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)blockReference.BlockTableRecord.GetObject(OpenMode.ForWrite);

                ObjectsInBlock objectsInBlock = GetObjectsInBlock(blockTableRecord);
                objectsInBlock.matrix3d = blockReference.BlockTransform;
                objectsInBlock.textStyle = ConvertLayer.GetTextSyleByName(Configuration.Config.Text.DefaultStyleName);
                objectsInBlock.dimStyle = ds;

                bool IsDimensionTangent = false;

                if (objectsInBlock.dBTextList.Count > 0)
                {
                    IsDimensionTangent = DimensionTextAnalyzer.HasDifferentTextRotations(
                        objectsInBlock.dBTextList.Select(text => text.Rotation));

                    if (IsDimensionTangent)
                    {
                        //É uma cota tangante
                    }

                    else if (objectsInBlock.hatchList.Count > 0)
                    {
                        //É uma cota em raio 
                    }

                    else if (objectsInBlock.arcList.Count > 0)
                    {
                        //É uma cota angular ou em arco]
                        ConvertArcDimension(objectsInBlock, ref blockTableRecord);
                    }

                    else
                    {
                        //É uma cota linear ou elevação
                        /*
                        ConvertToLayer.Zoom(blockReference.GeometricExtents.MinPoint.TransformBy(objectsInBlock.matrix3d), 
                            blockReference.GeometricExtents.MaxPoint.TransformBy(objectsInBlock.matrix3d));
                        */
                        if (!ConvertLineDimension(objectsInBlock, blockReference, ref blockTableRecord))
                        {
                            //Tratar cotas picadas do tipo 3 x XXX = XXXX
                            //ConvertLineDimensionBreak(objectsInBlock, ref blockTableRecord, ref objectIdList, i + 1);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectsInBlock"></param>
        /// <param name="blockTableRecord"></param>
        /// <param name="objectIdList"></param>
        /// <param name="position"></param>
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
                if (CheckParallelLine(objectsInBlock.dBTextList[0].Rotation,
                    objectsInBlock.lineList[j].Angle))
                {
                    lineListParallel.Add(objectsInBlock.lineList[j]);
                }
                /**/
                else if (CkeckPerpendicularLines(RoundPoint(objectsInBlock.dBTextList.First().Position),
                         RoundPoint(GetPointLine(objectsInBlock.dBTextList.First().Position,
                         objectsInBlock.dBTextList.First().Rotation)),
                         RoundPoint(objectsInBlock.lineList[j].StartPoint),
                         RoundPoint(objectsInBlock.lineList[j].EndPoint), blockReference))
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
                    PointEspecial1 PointTemp = CkeckIntersectionLines(lineListOther[j].StartPoint,
                                      lineListOther[j].EndPoint,
                                      lineListPerpendicular[k].StartPoint,
                                      lineListPerpendicular[k].EndPoint);
                    if (PointTemp != null)
                    {
                        for (int l = 0; l < lineListParallel.Count; l++)
                        {
                            PointEspecial1 PointTemp2 = CkeckIntersectionLines(lineListOther[j].StartPoint,
                                              lineListOther[j].EndPoint,
                                              lineListParallel[l].StartPoint,
                                              lineListParallel[l].EndPoint);
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
                    CreateLine(lineListOther[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                   lineListOther[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
            }
            verdadeiros = verdadeiros.Distinct().ToList();
            for (int j = 0; j < verdadeiros.Count; j++)
            {
                for (int k = 0; k < lineListPerpendicularTemp.Count; k++)
                {
                    if (CkeckIntersectionLines(verdadeiros[j].StartPoint,
                        verdadeiros[j].EndPoint, lineListPerpendicularTemp[k].StartPoint,
                        lineListPerpendicularTemp[k].EndPoint) != null)
                    {
                        lineListPerpendicularBase.Add(lineListPerpendicularTemp[k]);
                        lineListPerpendicularTemp.RemoveAt(k);
                        k--;
                    }
                }
            }

            ListPoinsIntersection = ListPoinsIntersection.Distinct(new MyComparer()).ToList();
            bool asLinePerpendicular = false;

            if (lineListPerpendicular.Count == 1)
                asLinePerpendicular = true;

            else
            {
                try
                {
                    if (IsOnLine(lineListPerpendicular.First().StartPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().EndPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().StartPoint.TransformBy(objectsInBlock.matrix3d)) ||
                                 IsOnLine(lineListPerpendicular.First().EndPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().StartPoint.TransformBy(objectsInBlock.matrix3d),
                                 lineListPerpendicular.Last().EndPoint.TransformBy(objectsInBlock.matrix3d)))
                        asLinePerpendicular = true;
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 44", e.Message);
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
                            CreateLine(lineListPerpendicularTemp[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
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
                    double rotation2 = SlopeTwoPoints(lineListParallel.First().StartPoint.TransformBy(objectsInBlock.matrix3d),
                                                      lineListParallel.First().EndPoint.TransformBy(objectsInBlock.matrix3d));
                    dimensionProperties.Rotation = rotation2;

                    BlockTable acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTblRec = transaction.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Point3d textPositionPoint = objectsInBlock.dBTextList.First().Position.TransformBy(objectsInBlock.matrix3d);
                    DBText positionText = new DBText();

                    positionText.SetDatabaseDefaults();
                    positionText.Justify = objectsInBlock.dBTextList.First().Justify;
                    positionText.TextString = objectsInBlock.dBTextList.First().TextString;
                    positionText.TextStyleId = objectsInBlock.dBTextList.First().TextStyleId;
                    positionText.Height = objectsInBlock.dBTextList.First().Height;
                    positionText.Rotation = objectsInBlock.dBTextList.First().Rotation;
                    positionText.WidthFactor = objectsInBlock.dBTextList.First().WidthFactor;
                    positionText.HorizontalMode = TextHorizontalMode.TextCenter;
                    positionText.VerticalMode = TextVerticalMode.TextVerticalMid;
                    positionText.AlignmentPoint = textPositionPoint;
                    positionText.Position = textPositionPoint;
                    positionText.AdjustAlignment(database);
                    double difX = positionText.AlignmentPoint.X - positionText.Position.X;
                    double difY = positionText.AlignmentPoint.Y - positionText.Position.Y;
                    acBlkTblRec.AppendEntity(positionText);
                    transaction.AddNewlyCreatedDBObject(positionText, true);

                    dimensionProperties.TextPosition = new Point3d((textPositionPoint.X + difX), (textPositionPoint.Y + difY), textPositionPoint.Z);
                    dimensionProperties.TextRotation = objectsInBlock.dBTextList.First().Rotation;
                    positionText.Erase();
                    CreateRotatedDimension(dimensionProperties,
                                           objectsInBlock.dimStyle);


                    if (lineListParallel.Count > 1)
                    {
                        if (ListPoinsIntersection.Count == 2)
                        {
                            for (int j = 0; j < lineListParallel.Count; j++)
                            {
                                bool t1 = false;
                                bool t2 = false;

                                if (RoundPoint(ListPoinsIntersection.First(), 1).X < RoundPoint(ListPoinsIntersection.Last(), 1).X)
                                {
                                    if (RoundPoint(lineListParallel[j].StartPoint).X >= RoundPoint(ListPoinsIntersection.First()).X &&
                                       RoundPoint(lineListParallel[j].StartPoint).X <= RoundPoint(ListPoinsIntersection.Last()).X)
                                        t1 = true;
                                    if (RoundPoint(lineListParallel[j].EndPoint).X >= RoundPoint(ListPoinsIntersection.First()).X &&
                                       RoundPoint(lineListParallel[j].EndPoint).X <= RoundPoint(ListPoinsIntersection.Last()).X)
                                        t2 = true;
                                }
                                else if (RoundPoint(ListPoinsIntersection.First(), 1).X > RoundPoint(ListPoinsIntersection.Last(), 1).X)
                                {
                                    if (RoundPoint(lineListParallel[j].StartPoint).X >= RoundPoint(ListPoinsIntersection.Last()).X &&
                                       RoundPoint(lineListParallel[j].StartPoint).X <= RoundPoint(ListPoinsIntersection.First()).X)
                                        t1 = true;
                                    if (RoundPoint(lineListParallel[j].EndPoint).X >= RoundPoint(ListPoinsIntersection.Last()).X &&
                                       RoundPoint(lineListParallel[j].EndPoint).X <= RoundPoint(ListPoinsIntersection.First()).X)
                                        t2 = true;
                                }
                                else if (RoundPoint(ListPoinsIntersection.First(), 1).Y < RoundPoint(ListPoinsIntersection.Last(), 1).Y)
                                {
                                    if (RoundPoint(lineListParallel[j].StartPoint).Y >= RoundPoint(ListPoinsIntersection.First()).Y &&
                                       RoundPoint(lineListParallel[j].StartPoint).Y <= RoundPoint(ListPoinsIntersection.Last()).Y)
                                        t1 = true;
                                    if (RoundPoint(lineListParallel[j].EndPoint).Y >= RoundPoint(ListPoinsIntersection.First()).Y &&
                                       RoundPoint(lineListParallel[j].EndPoint).Y <= RoundPoint(ListPoinsIntersection.Last()).Y)
                                        t2 = true;
                                }
                                else if (RoundPoint(ListPoinsIntersection.First(), 1).Y > RoundPoint(ListPoinsIntersection.Last(), 1).Y)
                                {
                                    if (RoundPoint(lineListParallel[j].StartPoint).Y >= RoundPoint(ListPoinsIntersection.Last()).Y &&
                                       RoundPoint(lineListParallel[j].StartPoint).Y <= RoundPoint(ListPoinsIntersection.First()).Y)
                                        t1 = true;
                                    if (RoundPoint(lineListParallel[j].EndPoint).Y >= RoundPoint(ListPoinsIntersection.Last()).Y &&
                                       RoundPoint(lineListParallel[j].EndPoint).Y <= RoundPoint(ListPoinsIntersection.First()).Y)
                                        t2 = true;
                                }

                                if (!t1 || !t2)
                                {
                                    CreateLine(lineListParallel[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                               lineListParallel[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                                }

                            }
                        }
                        else
                        {
                            for (int j = 1; j < lineListParallel.Count; j++)
                            {
                                CreateLine(lineListParallel[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                           lineListParallel[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                            }
                        }
                    }

                    for (int i = 1; i < objectsInBlock.dBTextList.Count; i++)
                    {
                        Point3d tPPInternal = objectsInBlock.dBTextList[i].Position.TransformBy(objectsInBlock.matrix3d);
                        DBText pTInternal = new DBText();

                        pTInternal.SetDatabaseDefaults();
                        pTInternal.Justify = objectsInBlock.dBTextList[i].Justify;
                        pTInternal.TextString = objectsInBlock.dBTextList[i].TextString;
                        pTInternal.TextStyleId = objectsInBlock.dBTextList[i].TextStyleId;
                        pTInternal.Height = objectsInBlock.dBTextList[i].Height;
                        pTInternal.Rotation = rotation2;
                        pTInternal.WidthFactor = objectsInBlock.dBTextList[i].WidthFactor;
                        pTInternal.TextStyleId = objectsInBlock.textStyle;
                        pTInternal.Layer = Configuration.Config.Dimensions.Layer;
                        pTInternal.Color = ConvertLayer.GetColorForName( Configuration.Config.Dimensions.TextColor);
                        pTInternal.Position = tPPInternal;
                       
                        pTInternal.AdjustAlignment(database);
                        acBlkTblRec.AppendEntity(pTInternal);
                        transaction.AddNewlyCreatedDBObject(pTInternal, true);
                    }


                    ObjectIdCollection objIdColl = blockTableRecord.GetBlockReferenceIds(true, true);
                    foreach (ObjectId item in objIdColl)
                    {
                        BlockReference acBref = item.GetObject(OpenMode.ForWrite) as BlockReference;
                        acBref.Erase();
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 45", e.Message);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectsInBlock"></param>
        /// <param name="blockTableRecord"></param>
        private void ConvertArcDimension(ObjectsInBlock objectsInBlock, ref BlockTableRecord blockTableRecord)
        {
            bool type1 = true;
            Point3d pCenter = RoundPoint(objectsInBlock.arcList.First().Center, 1);
            foreach (Line item in objectsInBlock.lineList)
            {
                if (IsPointEqual(RoundPoint(item.EndPoint, 1), pCenter) || IsPointEqual(RoundPoint(item.StartPoint, 1), pCenter))
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
                        if (IsOnLine(objectsInBlock.lineList.First().StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[i].EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[i].StartPoint.TransformBy(objectsInBlock.matrix3d)) ||
                         IsOnLine(objectsInBlock.lineList.First().EndPoint.TransformBy(objectsInBlock.matrix3d),
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
                        if (IsOnLine(objectsInBlock.lineList[i].StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].StartPoint.TransformBy(objectsInBlock.matrix3d)) ||
                         IsOnLine(objectsInBlock.lineList[i].EndPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                         objectsInBlock.lineList[j].EndPoint.TransformBy(objectsInBlock.matrix3d)))
                            line2.Add(objectsInBlock.lineList[j]);
                        else
                        {
                            lineOther.Add(objectsInBlock.lineList[j]);
                            break;
                        }
                    }

                    BlockTable acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTblRec = transaction.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Point3d textPositionPoint = objectsInBlock.dBTextList.First().Position.TransformBy(objectsInBlock.matrix3d);
                    DBText positionText = new DBText();

                    positionText.SetDatabaseDefaults();
                    positionText.Justify = objectsInBlock.dBTextList.First().Justify;
                    positionText.TextString = objectsInBlock.dBTextList.First().TextString;
                    positionText.TextStyleId = objectsInBlock.dBTextList.First().TextStyleId;
                    positionText.Height = objectsInBlock.dBTextList.First().Height;
                    positionText.Rotation = objectsInBlock.dBTextList.First().Rotation;
                    positionText.WidthFactor = objectsInBlock.dBTextList.First().WidthFactor;
                    positionText.HorizontalMode = TextHorizontalMode.TextCenter;
                    positionText.VerticalMode = TextVerticalMode.TextVerticalMid;
                    positionText.AlignmentPoint = textPositionPoint;
                    positionText.Position = textPositionPoint;
                    positionText.AdjustAlignment(database);
                    double difX = positionText.AlignmentPoint.X - positionText.Position.X;
                    double difY = positionText.AlignmentPoint.Y - positionText.Position.Y;
                    acBlkTblRec.AppendEntity(positionText);
                    transaction.AddNewlyCreatedDBObject(positionText, true);

                    dimensionProperties.Text = objectsInBlock.dBTextList.First().TextString;
                    dimensionProperties.XLine1Start = line2.Last().StartPoint.TransformBy(objectsInBlock.matrix3d);
                    try
                    {
                        dimensionProperties.XLine1End = line2.Last().GetPointAtParameter(0.5).TransformBy(objectsInBlock.matrix3d);
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 46", e.Message);
                        dimensionProperties.XLine1End = line2.Last().EndPoint.TransformBy(objectsInBlock.matrix3d);
                    }
                    dimensionProperties.XLine2Start = line1.Last().StartPoint.TransformBy(objectsInBlock.matrix3d);
                    try
                    {
                        dimensionProperties.XLine2End = line1.Last().GetPointAtParameter(0.5).TransformBy(objectsInBlock.matrix3d);
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 47", e.Message);
                        dimensionProperties.XLine2End = line1.Last().EndPoint.TransformBy(objectsInBlock.matrix3d);
                    }
                    dimensionProperties.Center = objectsInBlock.arcList.First().Center.TransformBy(objectsInBlock.matrix3d);
                    dimensionProperties.ArcPoint = objectsInBlock.arcList.First().GetPointAtParameter(objectsInBlock.arcList.First().StartAngle + (objectsInBlock.arcList.First().TotalAngle / 2)).TransformBy(objectsInBlock.matrix3d);
                    dimensionProperties.TextPosition = new Point3d((textPositionPoint.X + difX), (textPositionPoint.Y + difY), textPositionPoint.Z);

                    positionText.Erase();

                    for (int j = 0; j < line1.Count - 1; j++)
                    {
                        CreateLine(line1[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
                                   line1[j].EndPoint.TransformBy(objectsInBlock.matrix3d));
                    }
                    for (int j = 0; j < line2.Count - 1; j++)
                    {
                        CreateLine(line2[j].StartPoint.TransformBy(objectsInBlock.matrix3d),
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
                    if (distX <  Configuration.Config.Dimensions.ArrowSize * 2)
                    {
                        CreateAngularDimension3(dimensionProperties,
                                              objectsInBlock.dimStyle);
                    }
                    else
                    {
                        CreateAngularDimension2(dimensionProperties,
                                               objectsInBlock.dimStyle);

                    }

                    ObjectIdCollection objIdColl = blockTableRecord.GetBlockReferenceIds(true, true);
                    foreach (ObjectId item in objIdColl)
                    {
                        BlockReference acBref = item.GetObject(OpenMode.ForWrite) as BlockReference;
                        acBref.Erase();
                    }

                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 48", e.Message);
                }
            }

            else
            {
                try
                {

                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 49", e.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockTableRecord"></param>
        /// <returns></returns>
        private ObjectsInBlock GetObjectsInBlock(BlockTableRecord blockTableRecord)
        {
            ObjectsInBlock objectsInBlock = new ObjectsInBlock();
            foreach (ObjectId item in blockTableRecord)
            {
                DBObject dBObject = (DBObject)item.GetObject(OpenMode.ForRead);
                if (dBObject.GetType() == typeof(Line))
                {
                    objectsInBlock.lineList.Add((Line)dBObject);
                }
                else if (dBObject.GetType() == typeof(DBText))
                {
                    objectsInBlock.dBTextList.Add((DBText)dBObject);
                }
                else if (dBObject.GetType() == typeof(Arc))
                {
                    objectsInBlock.arcList.Add((Arc)dBObject);
                }
                else if (dBObject.GetType() == typeof(Hatch))
                {
                    objectsInBlock.hatchList.Add((Hatch)dBObject);
                }
            }
            return objectsInBlock;
        }


        /// <summary>
        /// http://docs.autodesk.com/ACD/2010/ENU/AutoCAD%20.NET%20Developer%27s%20Guide/
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void CreateLine(Point3d p1, Point3d p2)
        {
            BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            Line line = DimensionEntityFactory.CreateLine(
                p1,
                p2,
                Configuration.Config.Dimensions.Layer,
                ConvertLayer.GetColorForName(Configuration.Config.Dimensions.LineColor));
            blockTableRecord.AppendEntity(line);
            transaction.AddNewlyCreatedDBObject(line, true);
        }

        /// <summary>
        /// Creating Dimensions Linear
        /// </summary>
        /// <param name="dimensionProperties"></param>
        private void CreateRotatedDimension(DimensionProperties dimensionProperties, ObjectId dimStyle)
        {
            BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            RotatedDimension rotatedDimension = DimensionEntityFactory.CreateRotatedDimension(
                dimensionProperties,
                dimStyle,
                Configuration.Config.Dimensions.Layer,
                ConvertLayer.GetColorForName(Configuration.Config.Dimensions.LineColor));
            blockTableRecord.AppendEntity(rotatedDimension);
            transaction.AddNewlyCreatedDBObject(rotatedDimension, true);
          

        }





        private void CreateAngularDimension2(DimensionProperties dimensionProperties, ObjectId dimStyle)
        {

            BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            Point3AngularDimension lineAngularDimension2 = DimensionEntityFactory.CreateAngularDimension(
                dimensionProperties,
                dimStyle,
                Configuration.Config.Dimensions.Layer,
                ConvertLayer.GetColorForName(Configuration.Config.Dimensions.LineColor));
            blockTableRecord.AppendEntity(lineAngularDimension2);
            transaction.AddNewlyCreatedDBObject(lineAngularDimension2, true);


        }

        private void CreateAngularDimension3(DimensionProperties dimensionProperties, ObjectId dimStyle)
        {

            BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            Point3AngularDimension lineAngularDimension2 = DimensionEntityFactory.CreateAngularDimensionWithLargeGap(
                dimensionProperties,
                dimStyle,
                Configuration.Config.Dimensions.Layer,
                ConvertLayer.GetColorForName(Configuration.Config.Dimensions.LineColor),
                Configuration.Config.Dimensions.ArrowSize);
            blockTableRecord.AppendEntity(lineAngularDimension2);
            transaction.AddNewlyCreatedDBObject(lineAngularDimension2, true);


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point3d RoundPoint(Point3d point)
        {
            return new Point3d(Math.Round(point.X, 3), Math.Round(point.Y, 3), Math.Round(point.Z, 3));
        }

        private Point3d RoundPoint(Point3d point, int round)
        {
            return new Point3d(Math.Round(point.X, round), Math.Round(point.Y, round), Math.Round(point.Z, round));
        }

        /// <summary>
        /// http://www.vcskicks.com/csharp_net_angles.php
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegreeToRadian(double angle)
        {
            return DimensionGeometry.DegreeToRadian(angle);
        }

        /// <summary>
        /// http://www.vcskicks.com/csharp_net_angles.php
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double RadianToDegree(double angle)
        {
            return DimensionGeometry.RadianToDegree(angle);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/907390/how-can-i-tell-if-a-point-belongs-to-a-certain-line
        /// </summary>
        /// <param name="endPoint1"></param>
        /// <param name="endPoint2"></param>
        /// <param name="checkPoint"></param>
        /// <returns></returns>
        private bool IsOnLine(Point3d endPoint1, Point3d endPoint2, Point3d checkPoint)
        {
            return DimensionGeometry.IsOnLine(endPoint1, endPoint2, checkPoint);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        private PointEspecial1 CkeckIntersectionLines(Point3d pp1, Point3d pp2, Point3d pp3, Point3d pp4)
        {
            Point3d p1 = RoundPoint(pp1);
            Point3d p2 = RoundPoint(pp2);
            Point3d p3 = RoundPoint(pp3);
            Point3d p4 = RoundPoint(pp4);

            double xD1, yD1, xD2, yD2, xD3, yD3;
            double dot, deg, len1, len2;
            double segmentLen1, segmentLen2;
            double ua, ub, div;

            // calculate differences  
            xD1 = p2.X - p1.X;
            xD2 = p4.X - p3.X;
            yD1 = p2.Y - p1.Y;
            yD2 = p4.Y - p3.Y;
            xD3 = p1.X - p3.X;
            yD3 = p1.Y - p3.Y;

            // calculate the lengths of the two lines  
            len1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            len2 = Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate angle between the two lines.  
            dot = (xD1 * xD2 + yD1 * yD2); // dot product  
            deg = dot / (len1 * len2);

            // if abs(angle)==1 then the lines are parallell,  
            // so no intersection is possible  
            if (Math.Abs(deg) == 1) return null;

            // find intersection Pt between two lines  
            PointEspecial1 pt = new PointEspecial1();
            div = yD2 * xD1 - xD2 * yD1;
            ua = (xD2 * yD3 - yD2 * xD3) / div;
            ub = (xD1 * yD3 - yD1 * xD3) / div;
            pt.X = p1.X + ua * xD1;
            pt.Y = p1.Y + ua * yD1;

            // calculate the combined length of the two segments  
            // between Pt-p1 and Pt-p2  
            xD1 = pt.X - p1.X;
            xD2 = pt.X - p2.X;
            yD1 = pt.Y - p1.Y;
            yD2 = pt.Y - p2.Y;
            segmentLen1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate the combined length of the two segments  
            // between Pt-p3 and Pt-p4  
            xD1 = pt.X - p3.X;
            xD2 = pt.X - p4.X;
            yD1 = pt.Y - p3.Y;
            yD2 = pt.Y - p4.Y;
            segmentLen2 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // if the lengths of both sets of segments are the same as  
            // the lenghts of the two lines the point is actually  
            // on the line segment.  

            // if the point isn’t on the line, return null  
            if (Math.Abs(len1 - segmentLen1) > 0.01 || Math.Abs(len2 - segmentLen2) > 0.01)
                return null;

            // return the valid intersection  
            return pt;
        }


        public static bool IsTextPerpendicularToLine(DBText text, Line line, BlockReference blockReference)
        {
            Matrix3d blockTransform = blockReference.BlockTransform;

            // Obter o vetor direção da linha
            Vector3d lineDirection = line.EndPoint.TransformBy(blockTransform) - line.StartPoint.TransformBy(blockTransform);

            // Obter o vetor direção do texto
            Vector3d textDirection = text.AlignmentPoint.TransformBy(blockTransform) - text.Position.TransformBy(blockTransform);

            // Verificar se o vetor direção do texto é perpendicular ao vetor direção da linha
            double angle = lineDirection.GetAngleTo(textDirection);

            // Verificar se o ângulo entre as linhas é próximo de 90 graus
            const double tolerance = 1e-10;
            var resultado = angle - Math.PI / 2;
            var resultado2 = lineDirection.IsParallelTo(textDirection);
            var resultado3 = DegreeToRadian(angle);
            var resultado4 = RadianToDegree(angle);
            return Math.Abs(resultado) < tolerance;
        }
        private bool CkeckPerpendicularLines(Point3d lineAPointStart, Point3d lineAPointEnd, Point3d lineBPointStart, Point3d lineBPointEnd, BlockReference blockReference)
        {
            Matrix3d blockTransform = blockReference.BlockTransform;
            Vector3d lineADirection = lineAPointEnd.TransformBy(blockTransform) - lineAPointStart.TransformBy(blockTransform);
            Vector3d lineBDirection = lineBPointEnd.TransformBy(blockTransform) - lineBPointStart.TransformBy(blockTransform);


            double angle = lineADirection.GetAngleTo(lineBDirection);

            // Verificar se o ângulo entre as linhas é próximo de 90 graus
            const double tolerance = 0.01;

            var degreeAngle = RadianToDegree(angle);

            var resultado =  Math.Abs(90 - degreeAngle)  < tolerance;
         

            return resultado;
        }
        /// <summary>
        /// http://www.mathopenref.com/coordequationps.html
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Point3d GetPointLine(Point3d p1, double angle)
        {
            double slope = Math.Tan(angle);
            Random rnd = new Random(DateTime.Now.Millisecond);
            double x = 0;
            do
            {
                x = rnd.Next(0, 500);
            } while (x == p1.X);
            double y = slope * (x - p1.X) + p1.Y;
            Point3d point = new Point3d(x, y, 0);
            return point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radian1"></param>
        /// <param name="radian2"></param>
        /// <returns></returns>
        private bool CheckParallelLine(double radian1, double radian2)
        {
            return DimensionGeometry.CheckParallelLine(radian1, radian2);
        }



        /// <summary>
        /// http://www.learningwave.com/lwonline/algebra_section2/slope3.html
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double SlopeTwoPoints(Point3d p1, Point3d p2)
        {
            return DimensionGeometry.SlopeTwoPoints(p1, p2);
        }

        private bool IsPointEqual(Point3d p1, Point3d p2)
        {
            if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
                return true;
            return false;
        }
    }
}
