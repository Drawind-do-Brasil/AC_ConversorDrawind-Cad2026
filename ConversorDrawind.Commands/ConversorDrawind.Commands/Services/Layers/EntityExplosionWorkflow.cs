using Autodesk.AutoCAD.DatabaseServices;
using ConversorDrawind.Commands.Services.Styles;
using System.Linq;

namespace ConversorDrawind.Commands
{
    static class EntityExplosionWorkflow
    {
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
            StyleOperations.UpdateDimensionPrecision();
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
