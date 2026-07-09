using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawind.Commands
{
    internal sealed class BlockAttributeReader
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly Func<string, ObjectId[]> filterBlock;
        private readonly Action<string, string> logError;

        internal BlockAttributeReader(
            IAcadDocumentContext documentContext,
            Func<string, ObjectId[]> filterBlock,
            Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.filterBlock = filterBlock ?? throw new ArgumentNullException(nameof(filterBlock));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void CaptureAttributesFromBlocks(List<Block> blocks)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (Block block in blocks)
                    {
                        try
                        {
                            ObjectId[] objectIds = filterBlock(block.blockName);
                            if (objectIds.Count() > 0)
                            {
                                ObjectId objectId = filterBlock(block.blockName).First();
                                if (objectId != null)
                                {
                                    BlockReference blockReference = (BlockReference)transaction.GetObject(objectId, OpenMode.ForRead);
                                    BlockTableRecord blockTableRecord = (BlockTableRecord)transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead);
                                    for (int i = 0; i < blockReference.AttributeCollection.Count; i++)
                                    {
                                        Entity entity = transaction.GetObject(blockReference.AttributeCollection[i], OpenMode.ForRead) as Entity;
                                        if (entity.GetType() == typeof(AttributeReference))
                                        {
                                            AttributeReference attributeReference = entity as AttributeReference;
                                            block.listTags[i].text = attributeReference.TextString;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            logError(LogContext.CapturarAtributosDosBlocos, e.Message);
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
    }
}
