using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class LayerStateService
    {
        private readonly Database database;
        private readonly Transaction transaction;

        internal LayerStateService(Database database, Transaction transaction)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
            this.transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        internal void UpsertAndFreeze(string name, string linetypeName, Color color)
        {
            LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForWrite) as LayerTable;
            LayerTableRecord layerRecord;

            if (layerTable.Has(name))
            {
                layerRecord = transaction.GetObject(layerTable[name], OpenMode.ForWrite) as LayerTableRecord;
            }
            else
            {
                layerRecord = new LayerTableRecord { Name = name };
                layerTable.Add(layerRecord);
                transaction.AddNewlyCreatedDBObject(layerRecord, true);
            }

            layerRecord.Color = color;
                layerRecord.LinetypeObjectId = LayerSetupOperations.LoadLinetype(linetypeName);
            layerRecord.IsFrozen = true;
        }
    }
}
