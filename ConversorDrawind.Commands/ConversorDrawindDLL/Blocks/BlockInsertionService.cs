using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    internal sealed class BlockInsertionService
    {
        private readonly Database database;
        private readonly Transaction transaction;

        internal BlockInsertionService(Database database, Transaction transaction)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
            this.transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        internal ObjectId Insert(string blockName, Point3d insertionPoint, IDictionary<string, string> attributeValues)
        {
            BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForWrite) as BlockTable;
            if (!blockTable.Has(blockName))
                return ObjectId.Null;

            ObjectId blockDefinitionId = blockTable[blockName];
            BlockTableRecord modelSpace = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            BlockReference blockReference = new BlockReference(insertionPoint, blockDefinitionId);

            ObjectId blockReferenceId = modelSpace.AppendEntity(blockReference);
            transaction.AddNewlyCreatedDBObject(blockReference, true);

            AddAttributes(blockDefinitionId, blockReference, attributeValues);
            return blockReferenceId;
        }

        private void AddAttributes(ObjectId blockDefinitionId, BlockReference blockReference, IDictionary<string, string> attributeValues)
        {
            BlockTableRecord blockDefinition = transaction.GetObject(blockDefinitionId, OpenMode.ForRead) as BlockTableRecord;

            foreach (ObjectId objectId in blockDefinition)
            {
                Entity entity = transaction.GetObject(objectId, OpenMode.ForRead, false) as Entity;
                AttributeDefinition attributeDefinition = entity as AttributeDefinition;
                if (attributeDefinition == null || attributeDefinition.Constant)
                    continue;

                AttributeReference attributeReference = new AttributeReference();
                attributeReference.SetDatabaseDefaults(database);
                attributeReference.SetAttributeFromBlock(attributeDefinition, blockReference.BlockTransform);
                attributeReference.TextString = GetAttributeText(attributeDefinition, attributeValues);
                attributeReference.AdjustAlignment(HostApplicationServices.WorkingDatabase);

                blockReference.AttributeCollection.AppendAttribute(attributeReference);
                transaction.AddNewlyCreatedDBObject(attributeReference, true);
            }
        }

        private static string GetAttributeText(AttributeDefinition attributeDefinition, IDictionary<string, string> attributeValues)
        {
            if (attributeValues != null && attributeValues.ContainsKey(attributeDefinition.Tag))
                return attributeValues[attributeDefinition.Tag];

            return attributeDefinition.TextString;
        }
    }
}
