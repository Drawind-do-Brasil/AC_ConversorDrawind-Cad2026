using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace ConversorDrawind.Commands
{
    static class LayerConversionWorkflow
    {
        public static void ConvertLayers()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database acCurDb = documentContext.Database;
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
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
                        foreach (ObjectId id in objectIdList)
                        {
                            ConvertLayerRecursively(id, transaction);
                        }
                    }
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private static void ConvertLayerRecursively(ObjectId id, Transaction trans)
        {
            Entity obj = (Entity)trans.GetObject(id, OpenMode.ForRead);
            if (obj == null)
                return;

            if (Configuration.Config.General.ConverterType == 1 &&
                string.Equals(obj.Id.ObjectClass.DxfName, "MTEXT", StringComparison.OrdinalIgnoreCase))
            {
                EntityExplosionWorkflow.ExplodeObjects(new ObjectId[] { id });
            }

            if (string.Equals(obj.Id.ObjectClass.DxfName, "INSERT", StringComparison.OrdinalIgnoreCase))
            {
                BlockReference bref = (BlockReference)obj;
                BlockTableRecord block = (BlockTableRecord)trans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                BlockTableRecordEnumerator benum = block.GetEnumerator();

                while (benum.MoveNext())
                {
                    ConvertLayerRecursively(benum.Current, trans);
                }
            }

            InstanciaConversor.ConvertInstance(obj);
        }
    }
}
