using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawindDLL
{
    class ConvertBlocks
    {
        private static double EscalaAtual = 1;
        private static double RefTamFormato = Configuration.INTREFTamFormato;
        private static bool HasStartPointOverride = false;
        private static Point3d StartPointOverride = Point3d.Origin;

        public static bool CheckPoint(Point3d cp, Point3d p1, Point3d p2)
        {
            Point3d pp1 = new Point3d(((p1.X <= p2.X) ? p1.X : p2.X), ((p1.Y <= p2.Y) ? p1.Y : p2.Y), ((p1.Z <= p2.Z) ? p1.Z : p2.Z));
            Point3d pp2 = new Point3d(((p1.X > p2.X) ? p1.X : p2.X), ((p1.Y > p2.Y) ? p1.Y : p2.Y), ((p1.Z > p2.Z) ? p1.Z : p2.Z));
            if (cp.X >= pp1.X && cp.Y >= pp1.Y && cp.X <= pp2.X && cp.Y <= pp2.Y)
                return true;
            return false;
        }


        public static void GeTTextNew(string layer)
        {

            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            Database acCurDb = document.Database;
     
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
                            Matrix3d blockTransform = bref.BlockTransform;
                            while (benum.MoveNext())
                            {
                                Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                if (obj.GetType() == typeof(DBText))
                                {
                                    DBText text = obj as DBText;
                                    Point3d textPositionInModelSpace = text.Position.TransformBy(blockTransform);
                                    List<TagBlockClass> listTags = TagBlockClass.GetTagBlock(textPositionInModelSpace);
                                    foreach (TagBlockClass tag in listTags)
                                    {
                                        int qtde = 0;
                                        int.TryParse(tag.filtro.alturaTexto.ReplaceComma().Split(',').Last(), out qtde);

                                        if ((tag.filtro.cor == "ALL" || ConvertLayer.GetColorForName(tag.filtro.cor).ColorNameForDisplay == text.Color.ColorNameForDisplay)
                                        && (String.IsNullOrEmpty(tag.filtro.conteudoTexto) || text.TextString.ToUpper() == tag.filtro.conteudoTexto.ToUpper())
                                        && (String.IsNullOrEmpty(tag.filtro.alturaTexto) || Math.Round(text.Height, qtde) == Convert.ToDouble(tag.filtro.alturaTexto.ReplaceComma()))
                                            && tag.filtro.layerBase.ToUpper() == text.Layer.ToUpper())
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            RefTamFormato = Configuration.INTREFTamFormato;
            ConvertLayer.Zoom();

            Application.UpdateScreen();
            Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen();
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (BlockClass block in Arranjos.ListBlocks)
                    {
                        foreach (TagBlockClass tag in block.listTags)
                        {
                            if (tag.verifiqued)
                                continue;
                            List<ObjectId> myIDs = new List<ObjectId>();
                            if (tag.modify)
                            {
                                Point3d p1 = GetPTReal(new Point3d(tag.p1.X, tag.p1.Y, tag.p1.Z));
                                Point3d p2 = GetPTReal(new Point3d(tag.p2.X, tag.p2.Y, tag.p2.Z));

                                PromptSelectionResult psr = editor.SelectCrossingWindow(p2,
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
                                        if (String.IsNullOrEmpty(tag.filtro.conteudoTexto ) || text.TextString.ToUpper() == tag.filtro.conteudoTexto.ToUpper())
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

        public static void GeTTextInv(List<BlockClass> blockClass)
        {

            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {


                    foreach (BlockClass block in blockClass)
                    {

                        try
                        {
                            ObjectId[] l = FilterBlock(block.blockName);
                            if (l.Count() > 0)
                            {
                                ObjectId objectIdList = FilterBlock(block.blockName).First();
                                if (objectIdList != null)
                                {

                                    BlockReference blockReference = (BlockReference)transaction.GetObject(objectIdList, OpenMode.ForRead);
                                    BlockTableRecord blockTableRecord = (BlockTableRecord)transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead);
                                    for (int i = 0; i < blockReference.AttributeCollection.Count; i++)
                                    {
                                        Entity Entity = transaction.GetObject(blockReference.AttributeCollection[i], OpenMode.ForRead) as Entity;
                                        if (Entity.GetType() == typeof(AttributeReference))
                                        {
                                            AttributeReference attributeReference = Entity as AttributeReference;
                                            block.listTags[i].text = attributeReference.TextString;
                                        }
                                    }
                                }
                            }

                        }
                        catch (System.Exception e)
                        {
                            Conversor.EscreverLog("Erro 3", e.Message);
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        public static void SetText(List<BlockClass> blockClass)
        {
            foreach (BlockClass block in blockClass)
            {
                ChangingAttibutes(FilterBlock(block.blockName), block);
            }

        }

        public static void SetText2(List<BlockClass> blockClassi, List<BlockClass> blockClasso)
        {
            foreach (BlockClass block in blockClasso)
            {
                foreach (BlockClass item in blockClassi)
                {
                    if (block.blockNameRelacao == item.blockName)
                    {
                        ChangingAttibutes2(FilterBlock(block.blockName), block, item);
                        break;
                    }
                }

            }

        }

        public static void ChangingAttibutes(ObjectId[] objectIdList, BlockClass block)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
           
                try
                {
                    if (objectIdList != null)
                    {
                        foreach (ObjectId id in objectIdList)
                        {
                            BlockReference blockReference = (BlockReference)id.GetObject( OpenMode.ForRead);
                            BlockTableRecord blockTableRecord = (BlockTableRecord)blockReference.BlockTableRecord.GetObject( OpenMode.ForRead);
                            for (int i = 0; i < blockReference.AttributeCollection.Count; i++)
                            {
                                if (block.listTags[i].modify)
                                {
                                    Entity Entity = blockReference.AttributeCollection[i].GetObject(OpenMode.ForWrite) as Entity;
                                    if (Entity.GetType() == typeof(AttributeReference))
                                    {
                                        AttributeReference attributeReference = Entity as AttributeReference;
                                        attributeReference.WidthFactor = block.listTags[i].widthfactor;
                                        attributeReference.TextString = block.listTags[i].text;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 4", e.Message);
                }
                finally
                {
                  
                }
            
        }


        public static void ChangingAttibutes2(ObjectId[] objectIdList, BlockClass block, BlockClass blockClassi)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (objectIdList != null)
                    {
                        foreach (ObjectId id in objectIdList)
                        {
                            BlockReference blockReference = (BlockReference)transaction.GetObject(id, OpenMode.ForRead);
                            BlockTableRecord blockTableRecord = (BlockTableRecord)transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead);
                            for (int i = 0; i < blockReference.AttributeCollection.Count; i++)
                            {
                                if (block.listTags[i].indiceRelacao != -1)
                                {
                                    Entity Entity = transaction.GetObject(blockReference.AttributeCollection[i], OpenMode.ForWrite) as Entity;
                                    if (Entity.GetType() == typeof(AttributeReference))
                                    {
                                        AttributeReference attributeReference = Entity as AttributeReference;
                                        attributeReference.WidthFactor = block.listTags[i].widthfactor;
                                        attributeReference.TextString = blockClassi.listTags[block.listTags[i].indiceRelacao].text;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 5", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
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
            if (IsValidPoint(startPoint))
                return startPoint;

            return Point3d.Origin;
        }

        private static bool TryGetMinimumLinePointFromLayer(string layerName, out Point3d minPoint)
        {
            minPoint = Point3d.Origin;

            if (string.IsNullOrWhiteSpace(layerName))
                return false;

            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
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
                            if (IsSameLayer(line.Layer, layerName))
                            {
                                UpdateMinimumPoint(line.StartPoint, ref minX, ref minY, ref minZ);
                                UpdateMinimumPoint(line.EndPoint, ref minX, ref minY, ref minZ);
                                found = true;
                            }

                            continue;
                        }

                        BlockReference blockReference = entity as BlockReference;
                        if (blockReference == null)
                            continue;

                        bool useAllBlockLines = IsSameLayer(blockReference.Layer, layerName);
                        BlockTableRecord blockDefinition = transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                        foreach (ObjectId blockObjectId in blockDefinition)
                        {
                            Line blockLine = transaction.GetObject(blockObjectId, OpenMode.ForRead) as Line;
                            if (blockLine == null)
                                continue;

                            if (!useAllBlockLines && !IsSameLayer(blockLine.Layer, layerName))
                                continue;

                            UpdateMinimumPoint(blockLine.StartPoint.TransformBy(blockReference.BlockTransform), ref minX, ref minY, ref minZ);
                            UpdateMinimumPoint(blockLine.EndPoint.TransformBy(blockReference.BlockTransform), ref minX, ref minY, ref minZ);
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

            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
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
                        if (entity == null || !IsSameLayer(entity.Layer, layerName))
                            continue;

                        try
                        {
                            Extents3d extents = (Extents3d)entity.Bounds;
                            UpdateMinimumPoint(extents.MinPoint, ref minX, ref minY, ref minZ);
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

        private static void UpdateMinimumPoint(Point3d point, ref double minX, ref double minY, ref double minZ)
        {
            if (point.X < minX)
                minX = point.X;
            if (point.Y < minY)
                minY = point.Y;
            if (point.Z < minZ)
                minZ = point.Z;
        }

        private static bool IsSameLayer(string entityLayer, string layerName)
        {
            return string.Equals(entityLayer, layerName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValidPoint(Point3d point)
        {
            return !double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z) &&
                   !double.IsInfinity(point.X) && !double.IsInfinity(point.Y) && !double.IsInfinity(point.Z) &&
                   point.X != double.MaxValue && point.Y != double.MaxValue && point.Z != double.MaxValue;
        }

        public static Point3d GetPTReal(Point3d pt)
        {
            Point3d start = GetStartPoint();
            double X = (pt.X * EscalaAtual) + start.X;
            double Y = (pt.Y * EscalaAtual) + start.Y;
            double Z = (pt.Z * EscalaAtual) + start.Z;
            return new Point3d(X, Y, Z);
        }

        private static SelectionFilter FilterText(string layerName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = acDoc.Editor;

            List<TypedValue> typedValuelist = new List<TypedValue>();
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<and"));
            if (layerName != "ALL")
                typedValuelist.Add(new TypedValue((int)DxfCode.LayerName, layerName));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return new SelectionFilter(typedValuelist.ToArray());
        }

        public static ObjectId[] FilterBlock(string blockName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = acDoc.Editor;

            TypedValue[] typedValue = new TypedValue[2];
            typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            typedValue.SetValue(new TypedValue((int)DxfCode.BlockName, blockName), 1);
            SelectionFilter selectionFilter = new SelectionFilter(typedValue);
            ObjectId[] objectIdList = editor.SelectAll(selectionFilter).Value.GetObjectIds();
            return objectIdList;
        }

        public static Dictionary<string, string> FillDictionary(string[] tags, string[] values)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (tags.Length != values.Length)
            {
                throw new System.Exception("A lista contem um número diferente de elementos. Erro...");

            }
            for (int i = 0; i < tags.Length; i++)
            {
                dict.Add(tags[i], values[i]);
            }
            return dict;
        }

        public static void AttUpdFromDict(Transaction tr, ObjectId[] objectIdList, Dictionary<string, string> dict)
        {
            for (int i = 0; i < objectIdList.Length; i++)
            {
                ObjectId objectId = objectIdList[i];

                BlockReference blockReference = (BlockReference)tr.GetObject(objectId, OpenMode.ForRead);
                
                foreach (ObjectId item in blockReference.AttributeCollection)
                {
                    Entity ent = tr.GetObject(item, OpenMode.ForWrite, false) as Entity;

                    if (ent.GetType() == typeof(AttributeReference))
                    {
                        AttributeReference attRef = ent as AttributeReference;

                        if (dict.ContainsKey(attRef.Tag))
                            attRef.TextString = dict[attRef.Tag];
                    }
                }
            }
        }

        public static void DeleteLayerNew(List<Filter> layers)
        {
        
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            Database acCurDb = document.Database;

            TypedValue[] typedValue = new TypedValue[1];
            typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);

            SelectionFilter selectionFilter = new SelectionFilter(typedValue);
            PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
            ObjectId[] objectIdList = null;
            if (promptSelectionResult.Status.ToString() == "OK")
                objectIdList = promptSelectionResult.Value.GetObjectIds();
            if (objectIdList != null)
            {
             
                    try
                    {

                        foreach (var item in objectIdList)
                        {
                            BlockReference bref = (BlockReference)item.GetObject( OpenMode.ForRead);

                            var block = (BlockTableRecord)bref.BlockTableRecord.GetObject( OpenMode.ForRead);
                            var benum = block.GetEnumerator();

                            while (benum.MoveNext())
                            {
                                Entity obj = (Entity)benum.Current.GetObject( OpenMode.ForWrite);


                                List<Filter> filtro =
                                     layers.Where(p => p.layerBase == obj.Layer.ToUpper() &&
                                     (p.tipoObjeto == "ALL" || p.tipoObjeto == obj.Id.ObjectClass.DxfName.ToUpper()) &&
                                     (p.cor == "ALL" || obj.Color.ColorNameForDisplay == ConvertLayer.GetColorForName(p.cor).ColorNameForDisplay) &&
                                     (p.tipoLinha.ToUpper() == "ALL" || p.tipoLinha.ToUpper() == obj.Linetype.ToUpper()) &&
                                     (obj.GetType() != typeof(DBText) || (obj.GetType() == typeof(DBText) &&
                                                                            (String.IsNullOrEmpty(p.conteudoTexto) || p.conteudoTexto == ((DBText)obj).TextString) &&
                                                                            (String.IsNullOrEmpty(p.alturaTexto) || Math.Round(((DBText)obj).Height, p.alturaTextoRound) == Convert.ToDouble(p.alturaTexto.ReplaceComma()))))).ToList();

                                if (filtro.Count > 0)
                                    obj.Erase();

                            }
                            /* benum = block.GetEnumerator();
                             if (!benum.MoveNext())
                                 block.Erase();

                             */
                        }
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 88", e.Message);
                    }
                    finally
                    {

                    }
                
            }
        }

        public static void DeleteLayer(List<Filter> layers)
        {
            try
            {
                foreach (Filter filtro in layers)
                {
                    ObjectId[] myIds = ConvertLayer.Filter(filtro.layerBase, filtro.tipoObjeto, filtro.cor, filtro.tipoLinha);
                    Document acDoc = Application.DocumentManager.MdiActiveDocument;
                    Database acCurDb = acDoc.Database;
                    
                        try
                        {
                            if (myIds != null)
                            {
                                foreach (ObjectId id in myIds)
                                {
                                    Entity myEntity = id.GetObject( OpenMode.ForWrite) as Entity;
                                    if (filtro.cor == "ALL" || myEntity.Color.ColorNameForDisplay == ConvertLayer.GetColorForName(filtro.cor).ColorNameForDisplay)
                                    {
                                        if (myEntity.GetType() != typeof(DBText) && (myEntity.Id.ObjectClass.DxfName.ToUpper() != "INSERT" || filtro.tipoObjeto.ToUpper() == "INSERT"))
                                        {
                                            myEntity.Erase();
                                        }

                                        else if (myEntity.GetType() == typeof(DBText))
                                        {
                                            DBText myText = (DBText)myEntity;
                                            double myHeight = 0;
                                            int qtde = 0;
                                            int.TryParse(filtro.alturaTexto.ReplaceComma().Split(',').Last(), out qtde);

                                            if (!String.IsNullOrEmpty(filtro.alturaTexto))
                                                myHeight = Convert.ToDouble(filtro.alturaTexto.ReplaceComma());

                                            if ((String.IsNullOrEmpty(filtro.conteudoTexto) || myText.TextString.ToUpper() == filtro.conteudoTexto.ToUpper()) &&
                                                (String.IsNullOrEmpty(filtro.alturaTexto) || Math.Round(myText.Height, qtde) == myHeight))
                                            {
                                                myText.Erase();

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Conversor.EscreverLog("Erro 6", e.Message);
                        }
                        finally
                        {

                        }
                    
                }
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 7", e.Message);
            }
        }


        public static void DeleteBlocks(List<BlockClass> blockClassi)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (BlockClass filtro in blockClassi)
                    {
                        if (filtro.blockNameRelacao != "")
                        {
                            ObjectId[] myIds = FilterBlock(filtro.blockNameRelacao);

                            if (myIds != null)
                            {
                                foreach (ObjectId id in myIds)
                                {
                                    Entity myEntity = acTrans.GetObject(id, OpenMode.ForWrite) as Entity;
                                    myEntity.Erase();
                                }
                            }

                        }
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 8", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

   
    }
}
