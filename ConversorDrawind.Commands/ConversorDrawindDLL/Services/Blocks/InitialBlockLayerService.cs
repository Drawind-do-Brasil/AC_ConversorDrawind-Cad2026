using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class InitialBlockLayerService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly IEntitySelector entitySelector;
        private readonly Action<string, string> logError;

        internal InitialBlockLayerService(
            IAcadDocumentContext documentContext,
            IEntitySelector entitySelector,
            Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void ConvertBlockLayers()
        {
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(new SelectionFilter(LayerFilterFactory.InsertOnly()));
            ObjectId[] objectIds = promptSelectionResult.Status == PromptStatus.OK
                ? promptSelectionResult.Value.GetObjectIds()
                : null;

            if (objectIds == null)
            {
                return;
            }

            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (ObjectId item in objectIds)
                    {
                        ApplyFirstEntityLayer((BlockReference)transaction.GetObject(item, OpenMode.ForWrite));
                    }
                }
                catch (Exception e)
                {
                    logError(LogContext.ConverterCamadasIniciais, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private static void ApplyFirstEntityLayer(BlockReference blockReference)
        {
            if (blockReference.Layer != "0")
            {
                return;
            }

            BlockTableRecord block = (BlockTableRecord)blockReference.BlockTableRecord.GetObject(OpenMode.ForRead);
            BlockTableRecordEnumerator entities = block.GetEnumerator();
            if (!entities.MoveNext())
            {
                return;
            }

            Entity firstEntity = (Entity)entities.Current.GetObject(OpenMode.ForRead);
            blockReference.Layer = firstEntity.Layer;
        }
    }
}
