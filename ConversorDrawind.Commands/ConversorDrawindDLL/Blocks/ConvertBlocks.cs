using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{
    class ConvertBlocks
    {
        private static double EscalaAtual = 1;
        private static double RefTamFormato = Configuration.INTREFTamFormato;
        private static bool HasStartPointOverride = false;
        private static Point3d StartPointOverride = Point3d.Origin;

        internal static void ResetForTests()
        {
            EscalaAtual = 1;
            RefTamFormato = Configuration.INTREFTamFormato;
            HasStartPointOverride = false;
            StartPointOverride = Point3d.Origin;
        }

        public static bool CheckPoint(Point3d cp, Point3d p1, Point3d p2)
        {
            return BlockMatcher.CheckPoint(cp, p1, p2);
        }


        public static void GeTTextNew(string layer)
        {

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            Database acCurDb = documentContext.Database;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnLayer(layer));
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);
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
                            Matrix3d blockTransform = bref.BlockTransform;
                            while (benum.MoveNext())
                            {
                                Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                if (obj.GetType() == typeof(DBText))
                                {
                                    DBText text = obj as DBText;
                                    Point3d textPositionInModelSpace = text.Position.TransformBy(blockTransform);
                                    List<TagBlock> listTags = GetTagBlocksAtPoint(textPositionInModelSpace);
                                    foreach (TagBlock tag in listTags)
                                    {
                                        int qtde = 0;
                                        int.TryParse(tag.filtro.alturaTexto.ReplaceComma().Split(',').Last(), out qtde);

                                        if ((tag.filtro.cor == "ALL" || ConvertLayer.GetColorForName(tag.filtro.cor).ColorNameForDisplay == text.Color.ColorNameForDisplay)
                                        && (String.IsNullOrEmpty(tag.filtro.conteudoTexto) || string.Equals(text.TextString, tag.filtro.conteudoTexto, StringComparison.OrdinalIgnoreCase))
                                        && (String.IsNullOrEmpty(tag.filtro.alturaTexto) || Math.Round(text.Height, qtde) == Convert.ToDouble(tag.filtro.alturaTexto.ReplaceComma()))
                                            && string.Equals(tag.filtro.layerBase, text.Layer, StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.text = text.TextString;
                                            tag.verifiqued = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 87", e.Message);
                    }
                    finally
                    {
                        acTrans.MyCommit();
                    }
                }
            }
        }

        public static void GeTText()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            RefTamFormato = Configuration.INTREFTamFormato;
            ConvertLayer.Zoom();

            Application.UpdateScreen();
            editor.UpdateScreen();
            editor.Regen();

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (Block block in RuntimeConfigurationState.TeklaBlocks)
                    {
                        foreach (TagBlock tag in block.listTags)
                        {
                            if (tag.verifiqued)
                                continue;
                            List<ObjectId> myIDs = new List<ObjectId>();
                            if (tag.modify)
                            {
                                Point3d p1 = GetPTReal(new Point3d(tag.p1.X, tag.p1.Y, tag.p1.Z));
                                Point3d p2 = GetPTReal(new Point3d(tag.p2.X, tag.p2.Y, tag.p2.Z));

                                PromptSelectionResult psr = entitySelector.SelectCrossingWindow(p2,
                                                    p1,
                                                    FilterText(tag.filtro.layerBase));
                                if (psr.Status == PromptStatus.OK)
                                    myIDs.AddRange(psr.Value.GetObjectIds());
                                int cont = 0;
                                foreach (ObjectId id in myIDs)
                                {
                                    DBText text = transaction.GetObject(id, OpenMode.ForRead) as DBText;
                                    if (tag.filtro.cor == "ALL" || ConvertLayer.GetColorForName(tag.filtro.cor).ColorNameForDisplay == text.Color.ColorNameForDisplay)
                                    {
                                        if (String.IsNullOrEmpty(tag.filtro.conteudoTexto) || string.Equals(text.TextString, tag.filtro.conteudoTexto, StringComparison.OrdinalIgnoreCase))
                                        {

                                            int qtde = 0;
                                            int.TryParse(tag.filtro.alturaTexto.ReplaceComma().Split(',').Last(), out qtde);


                                            if (String.IsNullOrEmpty(tag.filtro.alturaTexto) || Math.Round(text.Height, qtde) == Convert.ToDouble(tag.filtro.alturaTexto.ReplaceComma()))
                                            {


                                                if (cont == 0)
                                                    tag.text = text.TextString;
                                                else if (CheckPoint(text.Position, p1, p2))
                                                {
                                                    tag.text = text.TextString;
                                                    break;
                                                }

                                                cont++;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 2", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }

        }

        private static List<TagBlock> GetTagBlocksAtPoint(Point3d point)
        {
            List<TagBlock> list = new List<TagBlock>();
            foreach (Block block in RuntimeConfigurationState.TeklaBlocks)
            {
                list.AddRange(block.listTags.Where(tag => !tag.verifiqued && CheckPoint(point,
                    GetPTReal(new Point3d(tag.p1.X, tag.p1.Y, tag.p1.Z)),
                    GetPTReal(new Point3d(tag.p2.X, tag.p2.Y, tag.p2.Z)))).ToList());
            }

            return list;
        }

        public static void GeTTextInv(List<Block> Block)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new BlockAttributeReader(documentContext, FilterBlock, Conversor.EscreverLog)
                .CaptureAttributesFromBlocks(Block);
        }

        public static void SetText(List<Block> Block)
        {
            foreach (Block block in Block)
            {
                ChangingAttibutes(FilterBlock(block.blockName), block);
            }

        }

        public static void SetText2(List<Block> blockClassi, List<Block> blockClasso)
        {
            foreach (Block block in blockClasso)
            {
                foreach (Block item in blockClassi)
                {
                    if (block.blockNameRelacao == item.blockName)
                    {
                        ChangingAttibutes2(FilterBlock(block.blockName), block, item);
                        break;
                    }
                }

            }

        }

        public static void ChangingAttibutes(ObjectId[] objectIdList, Block block)
        {
            BlockAttributeWriter.ChangeAttributes(objectIdList, block, Conversor.EscreverLog);
        }


        public static void ChangingAttibutes2(ObjectId[] objectIdList, Block block, Block blockClassi)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            BlockAttributeWriter.ChangeRelatedAttributes(
                documentContext,
                objectIdList,
                block,
                blockClassi,
                Conversor.EscreverLog);
        }


        public static Point3d GetStartPoint()
        {
            if (HasStartPointOverride)
                return StartPointOverride;

            return Conversor.GetNewMin();
        }

        public static void SetStartPointOverride(Point3d startPoint)
        {
            StartPointOverride = startPoint;
            HasStartPointOverride = true;
        }

        public static void ClearStartPointOverride()
        {
            HasStartPointOverride = false;
            StartPointOverride = Point3d.Origin;
        }

        public static Point3d GetFormatStartPoint(string layerName)
        {
            Point3d startPoint;

            if (TryGetMinimumLinePointFromLayer(layerName, out startPoint))
                return startPoint;

            if (TryGetMinimumEntityPointFromLayer(layerName, out startPoint))
                return startPoint;

            startPoint = Conversor.GetNewMin();
            if (FormatStartPointService.IsValidPoint(startPoint))
                return startPoint;

            return Point3d.Origin;
        }

        private static bool TryGetMinimumLinePointFromLayer(string layerName, out Point3d minPoint)
        {
            minPoint = Point3d.Origin;

            if (string.IsNullOrWhiteSpace(layerName))
                return false;

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            bool found = false;
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord modelSpace = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId objectId in modelSpace)
                    {
                        Entity entity = transaction.GetObject(objectId, OpenMode.ForRead) as Entity;
                        if (entity == null)
                            continue;

                        Line line = entity as Line;
                        if (line != null)
                        {
                            if (FormatStartPointService.IsSameLayer(line.Layer, layerName))
                            {
                                FormatStartPointService.UpdateMinimumPoint(line.StartPoint, ref minX, ref minY, ref minZ);
                                FormatStartPointService.UpdateMinimumPoint(line.EndPoint, ref minX, ref minY, ref minZ);
                                found = true;
                            }

                            continue;
                        }

                        BlockReference blockReference = entity as BlockReference;
                        if (blockReference == null)
                            continue;

                        bool useAllBlockLines = FormatStartPointService.IsSameLayer(blockReference.Layer, layerName);
                        BlockTableRecord blockDefinition = transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                        foreach (ObjectId blockObjectId in blockDefinition)
                        {
                            Line blockLine = transaction.GetObject(blockObjectId, OpenMode.ForRead) as Line;
                            if (blockLine == null)
                                continue;

                            if (!useAllBlockLines && !FormatStartPointService.IsSameLayer(blockLine.Layer, layerName))
                                continue;

                            FormatStartPointService.UpdateMinimumPoint(blockLine.StartPoint.TransformBy(blockReference.BlockTransform), ref minX, ref minY, ref minZ);
                            FormatStartPointService.UpdateMinimumPoint(blockLine.EndPoint.TransformBy(blockReference.BlockTransform), ref minX, ref minY, ref minZ);
                            found = true;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 88", e.Message);
                    found = false;
                }
                finally
                {
                    transaction.MyCommit();
                }
            }

            if (!found)
                return false;

            minPoint = new Point3d(minX, minY, minZ);
            return true;
        }

        private static bool TryGetMinimumEntityPointFromLayer(string layerName, out Point3d minPoint)
        {
            minPoint = Point3d.Origin;

            if (string.IsNullOrWhiteSpace(layerName))
                return false;

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            bool found = false;
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord modelSpace = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId objectId in modelSpace)
                    {
                        Entity entity = transaction.GetObject(objectId, OpenMode.ForRead) as Entity;
                        if (entity == null || !FormatStartPointService.IsSameLayer(entity.Layer, layerName))
                            continue;

                        try
                        {
                            Extents3d extents = (Extents3d)entity.Bounds;
                            FormatStartPointService.UpdateMinimumPoint(extents.MinPoint, ref minX, ref minY, ref minZ);
                            found = true;
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 89", e.Message);
                    found = false;
                }
                finally
                {
                    transaction.MyCommit();
                }
            }

            if (!found)
                return false;

            minPoint = new Point3d(minX, minY, minZ);
            return true;
        }

        public static Point3d GetPTReal(Point3d pt)
        {
            return GetPTReal(pt, GetCurrentScaleReference());
        }

        public static Point3d GetPTReal(Point3d pt, BlockScaleReference scaleReference)
        {
            Point3d start = scaleReference.StartPoint;
            double X = (pt.X * scaleReference.Scale) + start.X;
            double Y = (pt.Y * scaleReference.Scale) + start.Y;
            double Z = (pt.Z * scaleReference.Scale) + start.Z;
            return new Point3d(X, Y, Z);
        }

        internal static BlockScaleReference GetCurrentScaleReference()
        {
            return new BlockScaleReference(EscalaAtual, RefTamFormato, GetStartPoint());
        }

        private static SelectionFilter FilterText(string layerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextOnOptionalLayer(layerName));
        }

        public static ObjectId[] FilterBlock(string blockName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertBlockByName(blockName));
            ObjectId[] objectIdList = entitySelector.SelectAll(selectionFilter).Value.GetObjectIds();
            return objectIdList;
        }

        public static Dictionary<string, string> FillDictionary(string[] tags, string[] values)
        {
            return BlockAttributeWriter.FillDictionary(tags, values);
        }

        public static void AttUpdFromDict(Transaction tr, ObjectId[] objectIdList, Dictionary<string, string> dict)
        {
            BlockAttributeWriter.UpdateFromDictionary(tr, objectIdList, dict);
        }

        public static void DeleteLayerNew(List<Filter> layers)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new BlockDeletionService(documentContext, FilterBlock, ConvertLayer.Filter, Conversor.EscreverLog)
                .DeleteLayerObjectsInBlocks(layers);
        }

        public static void DeleteLayer(List<Filter> layers)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new BlockDeletionService(documentContext, FilterBlock, ConvertLayer.Filter, Conversor.EscreverLog)
                .DeleteLayerObjects(layers);
        }


        public static void DeleteBlocks(List<Block> blockClassi)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new BlockDeletionService(documentContext, FilterBlock, ConvertLayer.Filter, Conversor.EscreverLog)
                .DeleteRelatedBlocks(blockClassi);
        }


    }
}
