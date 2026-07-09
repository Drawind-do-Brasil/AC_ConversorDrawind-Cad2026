using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{
    class ConvertLayer
    {
        public static ObjectId[] Filter(string LayerName, string Start, string ColorName, string LinetypeName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            return new LayerSelectionService(entitySelector, Conversor.EscreverLog).Filter(LayerName, Start, ColorName, LinetypeName);
        }

        public static ObjectId[] FilterLayers(params string[] layers)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            return new LayerSelectionService(entitySelector, Conversor.EscreverLog).FilterLayers(layers);
        }


        public static void ConvertLayersNewRecursive(ObjectId id, Transaction trans)
        {
            Entity obj = (Entity)trans.GetObject(id, OpenMode.ForRead);
            if (obj == null)
                return;
            if (Configuration.Config.General.ConverterType == 1 &&
                string.Equals(obj.Id.ObjectClass.DxfName, "MTEXT", StringComparison.OrdinalIgnoreCase))
            {
                ExplodeObjects(new ObjectId[] { id });
            }

            if (string.Equals(obj.Id.ObjectClass.DxfName, "INSERT", StringComparison.OrdinalIgnoreCase))
            {
                BlockReference bref = (BlockReference)obj;

                BlockTableRecord block = (BlockTableRecord)trans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                BlockTableRecordEnumerator benum = block.GetEnumerator();

                while (benum.MoveNext())
                {
                    ConvertLayersNewRecursive(benum.Current, trans);
                }

            }
            InstanciaConversor.ConvertInstance(obj);

        }

        public static void ConvertLayersNew()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            Database acCurDb = documentContext.Database;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll();
            ObjectId[] objectIdList = null;
            using (Transaction transaction = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (promptSelectionResult.Status == PromptStatus.OK)
                        objectIdList = promptSelectionResult.Value.GetObjectIds();
                    if (objectIdList != null)
                    {
                        foreach (var id in objectIdList)
                        {
                            ConvertLayersNewRecursive(id, transaction);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }



        public static void CreateAndAssignALayer()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            LayerRepository.CreateAndAssignAll(documentContext, RuntimeConfigurationState.NewLayerCompositions);
        }

        public static ObjectId CreateAndAssignALayer(string nome)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return LayerRepository.CreateAndAssignByName(documentContext, RuntimeConfigurationState.NewLayerCompositions, nome);
        }

        public static Color GetColorForName(string color)
        {
            return ColorResolver.Resolve(color);
        }

        public static ObjectId LoadLinetype(string sLineTypName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            ObjectId id = ObjectId.Null;

            try
            {
                id = LinetypeService.LoadLinetype(database, sLineTypName);
            }


            catch (Exception e)
            {
                Conversor.EscreverLog(LogContext.ConverterEntidadePorLayer, e.Message);
            }
            finally
            {

            }

            return id;
        }

        public static void ExplodeRadialDimenstionLarge()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, Conversor.EscreverLog).ExplodeRadialDimensionLarge();
        }

        public static void ExplodeObjects()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, Conversor.EscreverLog).ExplodeAllBlockReferences();
        }

        public static void ExplodeObjects(ObjectId[] mtexts)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, Conversor.EscreverLog).ExplodeMTextAndBlocks(mtexts);
        }

        public static void ExplodeObjectsInv()
        {
            ObjectId[] myMtexts = FilterLayers(RuntimeConfigurationState.ExplodeLayers.ToArray());
            ExplodeObjectsInv1(myMtexts);
            myMtexts = FilterLayers(RuntimeConfigurationState.ExplodeLayers.ToArray());
            ExplodeObjectsInv1(myMtexts);
            myMtexts = Filter("ALL", "DIMENSION", "ALL", "ALL");
            ExplodeObjectsInv1(myMtexts);
            myMtexts = Filter("ALL", "ALL", "ALL", "ALL");
            ExplodeObjectsInv2(myMtexts);
            UPDATE_DIMENSTION();
        }

        public static void ExplodeObjectsInv1(ObjectId[] mtexts)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, Conversor.EscreverLog).ExplodeInverseKnownTypes(mtexts);
        }

        public static void ExplodeObjectsInv2(ObjectId[] mtexts)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, Conversor.EscreverLog).ExplodeImpDimensions(mtexts);
        }

        public static void UPDATE_DIMENSTION()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new DimensionPrecisionService(ConvertLayer.Filter, documentContext, Conversor.EscreverLog).UpdateDimensionPrecision();
        }

        private static void PurgeSymbolTableRecords(
            Func<Database, ObjectId> getTableId,
            string message,
            string eraseLogContext,
            string purgeLogContext)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEditorMessenger messenger = new AcadEditorMessenger(documentContext.Editor);
            new SymbolTablePurgeService(documentContext, messenger, Conversor.EscreverLog)
                .PurgeSymbolTableRecords(getTableId, message, eraseLogContext, purgeLogContext);
        }

        public static void PurgeDimensionSyles()
        {
            PurgeSymbolTableRecords(
                database => database.DimStyleTableId,
                Localization.MessagePurgingDimensionStyles,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeTextSyles()
        {
            PurgeSymbolTableRecords(
                database => database.TextStyleTableId,
                Localization.MessagePurgingTextStyles,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeUnreferencedLineTypes()
        {
            PurgeSymbolTableRecords(
                database => database.LinetypeTableId,
                Localization.MessagePurgingLineTypes,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeUnreferencedLayers()
        {
            PurgeSymbolTableRecords(
                database => database.LayerTableId,
                Localization.MessagePurgingLayers,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeUnreferencedBlocks()
        {
            PurgeSymbolTableRecords(
                database => database.BlockTableId,
                Localization.MessagePurgingBlocks,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static Autodesk.AutoCAD.GraphicsInterface.FontDescriptor UpdateTextFont(string font, bool italic, bool negrito)
        {
            return TextStyleService.CreateFontDescriptor(font, italic, negrito);
        }

        public static ObjectId GetTextSyleByName(string name = null)
        {
            name = TextStyleService.ResolveStyleNameOrDefault(name, RuntimeConfigurationState.TextStyles);
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return TextStyleService.GetStyleIdByName(documentContext, name);
        }

        public static void CreateTextSyles()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            TextStyleService.CreateStyles(documentContext, RuntimeConfigurationState.TextStyles, Conversor.EscreverLog);
        }

        public static ObjectId CreateDimstyle()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return DimensionStyleService.CreateCurrentStyle(documentContext, Configuration.Config);
        }

        public static ObjectId CreateDimstyle2()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return DimensionStyleService.UpdateAllStyles(documentContext, Configuration.Config);
        }

        public static ObjectId GetArrowObjectId(string newArrName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            return ArrowBlockService.GetArrowObjectId(
                newArrName,
                "DIMBLK",
                documentContext,
                systemVariables,
                Conversor.EscreverLog,
                LogContext.CriarEstilosDeTexto);
        }

        public static ObjectId GetArrowObjectId(string newArrName, string tipo)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            return ArrowBlockService.GetArrowObjectId(
                newArrName,
                tipo,
                documentContext,
                systemVariables,
                Conversor.EscreverLog,
                LogContext.CriarEstiloDeCota);
        }

        public static string GetArrowBlockName(string name)
        {
            return ArrowBlockService.GetArrowBlockName(name);
        }

        public static string GetArrowBlockNameString(string name)
        {
            return ArrowBlockService.GetArrowBlockNameString(name);
        }

        public static void Zoom()
        {
            Point3d pontoMax = Conversor.GetNewMax();
            Point3d pontoMin = Conversor.GetNewMin();
            Zoom(pontoMin, pontoMax);
        }

        public static void Zoom(Point3d pMin, Point3d pMax)
        {
            new ZoomService(new AcadDocumentContext(), Conversor.EscreverLog).Zoom(pMin, pMax);
        }

        private static void Scale(ObjectId id, Point3d basept, double scale)
        {
            new EntityScaleService(Conversor.EscreverLog).Scale(id, basept, scale);
        }

        public static SelectionFilter FilterText(string LayerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextAndMTextOnLayer(LayerName));
        }


        public static SelectionFilter FilterText2(params string[] LayerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextAndMTextOnLayers(LayerName));
        }
        public static SelectionFilter FilterTextTeste(string LayerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextAndInsertOnLayer(LayerName));
        }


        public static void DeletingTekla(string layer)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            TeklaCleanupService.DeleteFromBlockLayer(documentContext, entitySelector, layer);
        }
        public static void DeletingTekla()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            TeklaCleanupService.DeleteDrawingSheetTexts(documentContext, entitySelector);
        }

        public static Point3d GetPointDiference(Point3d pontoIni, Point3d pontoRef, double scale)
        {
            return ScaleDetector.GetPointDifference(pontoIni, pontoRef, scale);
        }
        /*
         public static double ScaleDrawingCaptureText(string layertext)
         {
             double escala1p1 = 1;
             double escala = -1;

             IAcadDocumentContext documentContext = new AcadDocumentContext();
             Editor editor = documentContext.Editor;
             Database acCurDb = documentContext.Database;

             TypedValue[] typedValue = new TypedValue[2];
             typedValue.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

             SelectionFilter selectionFilter = new SelectionFilter(typedValue);
             PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
             ObjectId[] objectIdList = null;
             if (promptSelectionResult.Status.ToString() == "OK")
                 objectIdList = promptSelectionResult.Value.GetObjectIds();
             if (objectIdList != null)
             {
                 using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                 {
                     try
                     {
                         foreach (var item in objectIdList)
                         {
                            Entity entity = (Entity)acTrans.GetObject(item, OpenMode.ForRead);
                             if(entity.GetType() == typeof(BlockReference))
                             {
                                  BlockReference bref = (BlockReference)acTrans.GetObject(item, OpenMode.ForRead);

                             var block = (BlockTableRecord)acTrans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                             var benum = block.GetEnumerator();

                             while (benum.MoveNext())
                             {
                                 Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                 if (obj.GetType() == typeof(DBText))
                                 {
                                     DBText text = obj as DBText;
                                     if (string.Equals(Configuration.Config.Scale.Layer, text.Layer, StringComparison.OrdinalIgnoreCase) && ConvertBlocks.CheckPoint(text.Position,
                    ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp1.X, Configuration.Config.EXTSCALEAp1.Y, Configuration.Config.EXTSCALEAp1.Z)),
                    ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp2.X, Configuration.Config.EXTSCALEAp2.Y, Configuration.Config.EXTSCALEAp2.Z))))
                                     {
                                         int arredondamento = Configuration.Config.Scale.TextSize.ToString().Split(',').Last().Length;
                                         if (text.Height > Configuration.Config.Scale.TextSize - 0.2 &&
                                             text.Height < Configuration.Config.Scale.TextSize + 0.2)
                                         {
                                             string[] temp = text.TextString.Split(':');
                                             double escalaConvertida = 0;
                                             if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                                                 escala = escalaConvertida;
                                             if (escala1p1 == escala)
                                                 escala = -1;
                                         }
                                     }
                                 }
                             }
                         }
                         }
                     }
                     catch (System.Exception e)
                     {
                         Conversor.EscreverLog(LogContext.LimparCamadasTekla, e.Message);
                     }
                     finally
                     {
                         acTrans.MyCommit();
                     }
                 }
             }
             return escala;
         }*/
        /*
                public static double ScaleDrawingCapture(string layer)
                {
                    double escala1p1 = 1;
                    double escala = -1;

                    IAcadDocumentContext documentContext = new AcadDocumentContext();
                    Editor editor = documentContext.Editor;
                    IEntitySelector entitySelector = new AcadEntitySelector(editor);
                    Database acCurDb = documentContext.Database;

                    TypedValue[] typedValue = new TypedValue[2];
                    typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                    typedValue.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

                    SelectionFilter selectionFilter = new SelectionFilter(typedValue);
                    PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
                    ObjectId[] objectIdList = null;
                    if (promptSelectionResult.Status.ToString() == "OK")
                        objectIdList = promptSelectionResult.Value.GetObjectIds();
                    if (objectIdList != null)
                    {
                        using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                        {
                            try
                            {
                                foreach (var item in objectIdList)
                                {
                                    BlockReference bref = (BlockReference)acTrans.GetObject(item, OpenMode.ForRead);

                                    var block = (BlockTableRecord)acTrans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                                    var benum = block.GetEnumerator();

                                    while (benum.MoveNext())
                                    {
                                        Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                        if (obj.GetType() == typeof(DBText))
                                        {
                                            DBText text = obj as DBText;
                                            if (string.Equals(Configuration.Config.Scale.Layer, text.Layer, StringComparison.OrdinalIgnoreCase) && ConvertBlocks.CheckPoint(text.Position,
                           ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp1.X, Configuration.Config.EXTSCALEAp1.Y, Configuration.Config.EXTSCALEAp1.Z)),
                           ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp2.X, Configuration.Config.EXTSCALEAp2.Y, Configuration.Config.EXTSCALEAp2.Z))))
                                            {
                                                int arredondamento = Configuration.Config.Scale.TextSize.ToString().Split(',').Last().Length;
                                                if (text.Height > Configuration.Config.Scale.TextSize - 0.2 &&
                                                    text.Height < Configuration.Config.Scale.TextSize + 0.2)
                                                {
                                                    string[] temp = text.TextString.Split(':');
                                                    double escalaConvertida = 0;
                                                    if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                                                        escala = escalaConvertida;
                                                    if (escala1p1 == escala)
                                                        escala = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                Conversor.EscreverLog(LogContext.LimparCamadasTekla, e.Message);
                            }
                            finally
                            {
                                acTrans.MyCommit();
                            }
                        }
                    }
                    return escala;
                }
                public static double ScaleDrawingCapture()
                {
                    IAcadDocumentContext documentContext = new AcadDocumentContext();
                    Editor editor = documentContext.Editor;
                    Database acCurDb = documentContext.Database;

                    object ptMin = Conversor.GetNewMin();

                    Point3d pIni = ConvertBlocks.GetStartPoint();
                    double escala1p1 = 1;
                    double escala = -1;

                    Point3d pq1 = GetPointDiference(pIni, new Point3d(Configuration.Config.EXTSCALEAp1.X, Configuration.Config.EXTSCALEAp1.Y, Configuration.Config.EXTSCALEAp1.Z), escala1p1);
                    Point3d pq2 = GetPointDiference(pIni, new Point3d(Configuration.Config.EXTSCALEAp2.X, Configuration.Config.EXTSCALEAp2.Y, Configuration.Config.EXTSCALEAp2.Z), escala1p1);
                    ConvertLayer.Zoom(pq1, pq2);
                    editor.Regen();
                    editor.UpdateScreen();

                    using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                    {
                        try
                        {
                            if (!Configuration.Config.Scale.Manual)
                            {
                                List<ObjectId> myIDsScale = new List<ObjectId>();


                                PromptSelectionResult psr = entitySelector.SelectWindow(pq1,
                                                                                        pq2,
                                                                                        FilterTextTeste(Configuration.Config.Scale.Layer));
                                if (psr.Status == PromptStatus.OK)
                                    myIDsScale.AddRange(psr.Value.GetObjectIds());
                                if (myIDsScale.Count > 0)
                                {

                                    DBText dBObject = acTrans.GetObject(myIDsScale.First(), OpenMode.ForRead) as DBText;
                                    int arredondamento = Configuration.Config.Scale.TextSize.ToString().Split(',').Last().Length;

                                    if (dBObject.Height > Configuration.Config.Scale.TextSize - 0.2 &&
                                                  dBObject.Height < Configuration.Config.Scale.TextSize + 0.2)

                                    {
                                        string[] temp = dBObject.TextString.Split(':');
                                        double escalaConvertida = 0;
                                        if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                                            escala = escalaConvertida;
                                        if (escala1p1 == escala)
                                            escala = -1;
                                    }

                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Conversor.EscreverLog(LogContext.MoverDesenhoParaOrigem, e.Message);
                        }

                        finally
                        {
                            acTrans.MyCommit();
                        }
                    }
                    return escala;
                }
        */
        public static double GetScaleDrawing(double scale)
        {
            Point3d pIni = ConvertBlocks.GetStartPoint();
            double scaleDesenho = 1;

            if (Configuration.Config.Scale.Manual || scale <= 0)
            {
                Zoom(GetPointDiference(pIni, new Point3d(Configuration.Config.Scale.Point1.X, Configuration.Config.Scale.Point1.Y, Configuration.Config.Scale.Point1.Z), scaleDesenho),
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
            Point3d ptMax = Conversor.GetNewMax();
            Point3d ptMin = Conversor.GetNewMin();
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            Database acCurDb = documentContext.Database;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);
            Zoom(ptMin, ptMax);
            List<ObjectId> myIDs = new List<ObjectId>();
            PromptSelectionResult psr = entitySelector.SelectAll();
            if (psr.Status == PromptStatus.OK)
                myIDs.AddRange(psr.Value.GetObjectIds());

            foreach (ObjectId item in myIDs)
            {
                Scale(item, (Point3d)ptMin, scale);
            }
            return scale;
        }

        public static double ScaleDrawingInv(double scale, List<Block> blockClasso)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            Database acCurDb = documentContext.Database;

            List<ObjectId> myIDs = new List<ObjectId>();
            object ptMax = Conversor.GetNewMax();
            object ptMin = Conversor.GetNewMin();
            Point3d pIni = ConvertBlocks.GetStartPoint();


            Zoom((Point3d)ptMin, (Point3d)ptMax);

            try
            {
                foreach (Block item in blockClasso)
                {
                    myIDs.AddRange(ConvertBlocks.FilterBlock(item.blockName));
                }
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog(LogContext.ZoomNoDesenho, e.Message);
            }

            foreach (ObjectId item in myIDs)
            {
                Scale(item, (Point3d)ptMin, scale);

            }

            return scale;
        }

        public static bool WhatIsTheOrientation(Point3d p1, Point3d p2, string orientacao)
        {
            return ScaleDetector.IsOrientation(p1, p2, orientacao);
        }
    }
}
