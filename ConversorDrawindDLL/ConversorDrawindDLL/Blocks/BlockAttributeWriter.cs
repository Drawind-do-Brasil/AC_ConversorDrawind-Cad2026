using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    internal static class BlockAttributeWriter
    {
        internal static Dictionary<string, string> FillDictionary(string[] tags, string[] values)
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

        internal static void ChangeAttributes(ObjectId[] objectIdList, BlockClass block, Action<string, string> logError)
        {
            try
            {
                if (objectIdList != null)
                {
                    foreach (ObjectId id in objectIdList)
                    {
                        BlockReference blockReference = (BlockReference)id.GetObject(OpenMode.ForRead);
                        BlockTableRecord blockTableRecord = (BlockTableRecord)blockReference.BlockTableRecord.GetObject(OpenMode.ForRead);
                        for (int i = 0; i < blockReference.AttributeCollection.Count; i++)
                        {
                            if (block.listTags[i].modify)
                            {
                                Entity entity = blockReference.AttributeCollection[i].GetObject(OpenMode.ForWrite) as Entity;
                                if (entity.GetType() == typeof(AttributeReference))
                                {
                                    AttributeReference attributeReference = entity as AttributeReference;
                                    attributeReference.WidthFactor = block.listTags[i].widthfactor;
                                    attributeReference.TextString = block.listTags[i].text;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logError("Erro 4", e.Message);
            }
        }

        internal static void ChangeRelatedAttributes(
            IAcadDocumentContext documentContext,
            ObjectId[] objectIdList,
            BlockClass block,
            BlockClass relatedBlock,
            Action<string, string> logError)
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
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
                                    Entity entity = transaction.GetObject(blockReference.AttributeCollection[i], OpenMode.ForWrite) as Entity;
                                    if (entity.GetType() == typeof(AttributeReference))
                                    {
                                        AttributeReference attributeReference = entity as AttributeReference;
                                        attributeReference.WidthFactor = block.listTags[i].widthfactor;
                                        attributeReference.TextString = relatedBlock.listTags[block.listTags[i].indiceRelacao].text;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logError("Erro 5", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        internal static void UpdateFromDictionary(
            Transaction transaction,
            ObjectId[] objectIdList,
            Dictionary<string, string> dictionary)
        {
            for (int i = 0; i < objectIdList.Length; i++)
            {
                ObjectId objectId = objectIdList[i];

                BlockReference blockReference = (BlockReference)transaction.GetObject(objectId, OpenMode.ForRead);

                foreach (ObjectId item in blockReference.AttributeCollection)
                {
                    Entity entity = transaction.GetObject(item, OpenMode.ForWrite, false) as Entity;

                    if (entity.GetType() == typeof(AttributeReference))
                    {
                        AttributeReference attributeReference = entity as AttributeReference;

                        if (dictionary.ContainsKey(attributeReference.Tag))
                            attributeReference.TextString = dictionary[attributeReference.Tag];
                    }
                }
            }
        }
    }
}
