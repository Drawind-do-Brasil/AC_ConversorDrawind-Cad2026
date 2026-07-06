using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    internal static class LayerRepository
    {
        internal static void CreateAndAssignAll(
            IAcadDocumentContext documentContext,
            IReadOnlyList<string> layerCompositions)
        {
            Database database = documentContext.Database;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    for (int i = 0; i < layerCompositions.Count; i++)
                    {
                        string[] layerDefinition = layerCompositions[i].Split(':');
                        UpsertLayer(transaction, layerTable, layerDefinition);
                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 55", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        internal static ObjectId CreateAndAssignByName(
            IAcadDocumentContext documentContext,
            IReadOnlyList<string> layerCompositions,
            string name)
        {
            Database database = documentContext.Database;
            ObjectId layerId = ObjectId.Null;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    for (int i = 0; i < layerCompositions.Count; i++)
                    {
                        string[] layerDefinition = layerCompositions[i].Split(':');
                        if (layerDefinition[0] == name)
                            layerId = UpsertLayer(transaction, layerTable, layerDefinition);
                    }

                    if (layerId == ObjectId.Null)
                    {
                        LayerTableRecord layerRecord = transaction.GetObject(layerTable["0"], OpenMode.ForRead) as LayerTableRecord;
                        layerId = layerRecord.Id;
                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 56", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }

            return layerId;
        }

        private static ObjectId UpsertLayer(Transaction transaction, LayerTable layerTable, string[] layerDefinition)
        {
            LayerTableRecord layerRecord;
            if (layerTable.Has(layerDefinition[0]) == false)
            {
                layerRecord = new LayerTableRecord();
                ApplyLayerDefinition(layerRecord, layerDefinition);
                layerTable.Add(layerRecord);
                transaction.AddNewlyCreatedDBObject(layerRecord, true);
            }
            else
            {
                layerRecord = transaction.GetObject(layerTable[layerDefinition[0]], OpenMode.ForWrite) as LayerTableRecord;
                ApplyLayerDefinition(layerRecord, layerDefinition);
            }

            return layerRecord.Id;
        }

        private static void ApplyLayerDefinition(LayerTableRecord layerRecord, string[] layerDefinition)
        {
            layerRecord.Color = ConvertLayer.GetColorForName(layerDefinition[1]);
            layerRecord.Name = layerDefinition[0];
            layerRecord.LinetypeObjectId = ConvertLayer.LoadLinetype(layerDefinition[2]);
        }
    }
}
