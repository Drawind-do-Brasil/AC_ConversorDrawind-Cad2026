using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    internal sealed class DmBlockService
    {
        private const string BlockName = "QUADRO_DRAWIND";
        private const string LayerName = "DrawindDM";
        private const string WeightAttributeTag = "PESO";

        private readonly IAcadDocumentContext documentContext;
        private readonly IEntitySelector entitySelector;

        internal DmBlockService(IAcadDocumentContext documentContext, IEntitySelector entitySelector)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
        }

        internal void AddOrUpdate()
        {
            Database database = documentContext.Database;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    ObjectId[] existingBlocks = SelectDmBlocks();
                    if (existingBlocks.Length == 0)
                    {
                        new BlockDefinitionImportService(database)
                            .ImportFromDrawing(BlockName, Configuration.LoadConfigDLL());
                    }

                    string weight = new DmWeightTagService()
                        .ExtractFirstWeightAndNormalize(transaction, SelectWeightSourceObjects());

                    if (existingBlocks.Length == 0)
                        InsertDmBlock(database, transaction, weight);
                    else
                        UpdateDmBlocks(transaction, existingBlocks, weight);
                }
                catch (Exception e)
                {
                    ConversionLog.Write(LogContext.CriarBloco, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private void InsertDmBlock(Database database, Transaction transaction, string weight)
        {
            Dictionary<string, string> attributeValues = new Dictionary<string, string>
            {
                { WeightAttributeTag, weight ?? string.Empty }
            };

            ObjectId blockId = new BlockInsertionService(database, transaction)
                .Insert(BlockName, new Point3d(1, 1, 0), attributeValues);

            if (blockId == ObjectId.Null)
                return;

            new LayerStateService(database, transaction).UpsertAndFreeze(
                LayerName,
                "Continuos",
                Color.FromColorIndex(ColorMethod.ByLayer, 0));

            Entity entity = transaction.GetObject(blockId, OpenMode.ForWrite) as Entity;
            if (entity != null)
                entity.Layer = LayerName;
        }

        private void UpdateDmBlocks(Transaction transaction, ObjectId[] blockIds, string weight)
        {
            foreach (ObjectId blockId in blockIds)
            {
                BlockReference blockReference = transaction.GetObject(blockId, OpenMode.ForRead) as BlockReference;
                if (blockReference == null)
                    continue;

                UpdateWeightAttribute(transaction, blockReference, weight ?? string.Empty);
            }
        }

        private static void UpdateWeightAttribute(Transaction transaction, BlockReference blockReference, string weight)
        {
            foreach (ObjectId attributeId in blockReference.AttributeCollection)
            {
                AttributeReference attribute = transaction.GetObject(attributeId, OpenMode.ForWrite, false) as AttributeReference;
                if (attribute != null && string.Equals(attribute.Tag, WeightAttributeTag, StringComparison.OrdinalIgnoreCase))
                {
                    attribute.TextString = weight;
                    return;
                }
            }
        }

        private ObjectId[] SelectDmBlocks()
        {
            return SelectAll(new SelectionFilter(LayerFilterFactory.InsertBlockByNameAnd(BlockName)));
        }

        private ObjectId[] SelectWeightSourceObjects()
        {
            return SelectAll(new SelectionFilter(LayerFilterFactory.TextOrInsert()));
        }

        private ObjectId[] SelectAll(SelectionFilter selectionFilter)
        {
            PromptSelectionResult selectionResult = entitySelector.SelectAll(selectionFilter);
            if (selectionResult.Status == PromptStatus.OK)
                return selectionResult.Value.GetObjectIds();

            return Array.Empty<ObjectId>();
        }
    }
}
