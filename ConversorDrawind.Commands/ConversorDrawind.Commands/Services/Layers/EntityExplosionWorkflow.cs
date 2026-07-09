using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Linq;

namespace ConversorDrawind.Commands
{
    static class EntityExplosionWorkflow
    {
        public static void ConvertLayersNewRecursive(ObjectId id, Transaction trans)
        {
            Entity obj = (Entity)trans.GetObject(id, OpenMode.ForRead);
            if (obj == null)
                return;

            if (Configuration.Config.General.ConverterType == 1 &&
                string.Equals(obj.Id.ObjectClass.DxfName, "MTEXT", StringComparison.OrdinalIgnoreCase))
            {
                ExplodeObjects(new ObjectId[] { id });
            }

            if (string.Equals(obj.Id.ObjectClass.DxfName, "INSERT", StringComparison.OrdinalIgnoreCase))
            {
                BlockReference bref = (BlockReference)obj;
                BlockTableRecord block = (BlockTableRecord)trans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                BlockTableRecordEnumerator benum = block.GetEnumerator();

                while (benum.MoveNext())
                {
                    ConvertLayersNewRecursive(benum.Current, trans);
                }
            }

            InstanciaConversor.ConvertInstance(obj);
        }

        public static void ConvertLayersNew()
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
                            ConvertLayersNewRecursive(id, transaction);
                        }
                    }
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        public static void ExplodeRadialDimenstionLarge()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, ConversionLog.Write).ExplodeRadialDimensionLarge();
        }

        public static void ExplodeObjects()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, ConversionLog.Write).ExplodeAllBlockReferences();
        }

        public static void ExplodeObjects(ObjectId[] mtexts)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, ConversionLog.Write).ExplodeMTextAndBlocks(mtexts);
        }

        public static void ExplodeObjectsInv()
        {
            ObjectId[] myMtexts = LayerSelectionQueries.FilterLayers(RuntimeConfigurationState.ExplodeLayers.ToArray());
            ExplodeObjectsInv1(myMtexts);
            myMtexts = LayerSelectionQueries.FilterLayers(RuntimeConfigurationState.ExplodeLayers.ToArray());
            ExplodeObjectsInv1(myMtexts);
            myMtexts = LayerSelectionQueries.Filter("ALL", "DIMENSION", "ALL", "ALL");
            ExplodeObjectsInv1(myMtexts);
            myMtexts = LayerSelectionQueries.Filter("ALL", "ALL", "ALL", "ALL");
            ExplodeObjectsInv2(myMtexts);
            DrawingStyleOperations.UpdateDimensionPrecision();
        }

        public static void ExplodeObjectsInv1(ObjectId[] mtexts)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, ConversionLog.Write).ExplodeInverseKnownTypes(mtexts);
        }

        public static void ExplodeObjectsInv2(ObjectId[] mtexts)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityExplodeService(documentContext, ConversionLog.Write).ExplodeImpDimensions(mtexts);
        }
    }
}
