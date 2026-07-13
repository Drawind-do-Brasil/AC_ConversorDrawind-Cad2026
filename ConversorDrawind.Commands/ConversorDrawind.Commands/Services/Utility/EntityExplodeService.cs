using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class EntityExplodeService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly Action<string, string> logError;

        internal EntityExplodeService(IAcadDocumentContext documentContext, Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void ExplodeMTextAndBlocks(ObjectId[] objectIds)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    DBObjectCollection objects = new DBObjectCollection();
                    foreach (ObjectId item in objectIds)
                    {
                        Entity entity = (Entity)transaction.GetObject(item, OpenMode.ForWrite);
                        MText mText = entity as MText;

                        if (mText != null)
                        {
                            MTextFragmentCallback callback =
                              new MTextFragmentCallback((frag, obj) => { return MTextFragmentCallbackStatus.Continue; });

                            mText.ExplodeFragments(callback);
                        }

                        try
                        {
                            if (entity.GetType() == typeof(BlockReference))
                            {
                                entity.Explode(objects);
                                entity.Erase();
                            }
                        }
                        catch (Exception e)
                        {
                            logError(LogContext.ExplodirEntidade, e.Message);
                        }
                    }

                    BlockTableRecord blockTableRecord = (BlockTableRecord)transaction.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (DBObject obj in objects)
                    {
                        Entity entity = (Entity)obj;

                        blockTableRecord.AppendEntity(entity);
                        transaction.AddNewlyCreatedDBObject(entity, true);
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.ExplodirBlocos, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        internal void ExplodeInverseKnownTypes(ObjectId[] objectIds)
        {
            ExplodeMatchingEntities(
                objectIds,
                entity => entity.GetType() == typeof(BlockReference) ||
                          entity.GetType() == typeof(MText) ||
                          entity.GetType() == typeof(Leader) ||
                          entity.GetType() == typeof(DiametricDimension) ||
                          entity.GetType() == typeof(RadialDimension) ||
                          entity.GetType() == typeof(RadialDimensionLarge),
                LogContext.ExplodirEntidade,
                LogContext.ExplodirBlocos);
        }

        internal void ExplodeImpDimensions(ObjectId[] objectIds)
        {
            ExplodeMatchingEntities(
                objectIds,
                entity => entity.GetType().ToString() == "Autodesk.AutoCAD.DatabaseServices.ImpDimension",
                LogContext.ExplodirCotaRadialGrande,
                LogContext.ExplodirCotaRadialGrande);
        }

        internal void ExplodeRadialDimensionLarge()
        {
            ExplodeBlockReferences(
                blockReference => blockReference.GetType() == typeof(RadialDimensionLarge),
                LogContext.ExplodirCotasImportadas,
                OpenMode.ForRead);
        }

        internal void ExplodeAllBlockReferences()
        {
            ExplodeBlockReferences(
                blockReference => true,
                LogContext.ExplodirTiposConhecidos,
                OpenMode.ForWrite);
        }

        private void ExplodeMatchingEntities(
            ObjectId[] objectIds,
            Func<Entity, bool> shouldExplode,
            string itemLogContext,
            string transactionLogContext)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    DBObjectCollection objects = new DBObjectCollection();
                    foreach (ObjectId item in objectIds)
                    {
                        Entity entity = (Entity)transaction.GetObject(item, OpenMode.ForWrite);

                        try
                        {
                            if (shouldExplode(entity))
                            {
                                entity.Explode(objects);
                                entity.Erase();
                            }
                        }
                        catch (Exception e)
                        {
                            logError(itemLogContext, e.Message);
                        }
                    }

                    AppendExplodedObjects(database, transaction, objects);
                }
                catch (Exception e)
                {
                    logError(transactionLogContext, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private static void AppendExplodedObjects(Database database, Transaction transaction, DBObjectCollection objects)
        {
            BlockTableRecord blockTableRecord = (BlockTableRecord)transaction.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);

            foreach (DBObject obj in objects)
            {
                Entity entity = (Entity)obj;

                blockTableRecord.AppendEntity(entity);
                transaction.AddNewlyCreatedDBObject(entity, true);
            }
        }

        private void ExplodeBlockReferences(
            Func<BlockReference, bool> shouldExplode,
            string transactionLogContext,
            OpenMode tableOpenMode)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, tableOpenMode) as BlockTable;

                    foreach (ObjectId objectId in blockTable)
                    {
                        BlockTableRecord blockTableRecord = transaction.GetObject(objectId, tableOpenMode) as BlockTableRecord;
                        ObjectIdCollection blockReferenceIds = blockTableRecord.GetBlockReferenceIds(true, true);
                        foreach (ObjectId item in blockReferenceIds)
                        {
                            BlockReference blockReference = transaction.GetObject(item, OpenMode.ForWrite) as BlockReference;
                            if (shouldExplode(blockReference))
                            {
                                blockReference.ExplodeToOwnerSpace();
                                blockReference.Erase();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logError(transactionLogContext, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
