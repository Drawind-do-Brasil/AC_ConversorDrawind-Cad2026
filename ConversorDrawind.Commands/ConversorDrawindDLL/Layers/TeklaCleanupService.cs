using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace ConversorDrawindDLL
{
    internal static class TeklaCleanupService
    {
        private const string TeklaStructuresText = "Tekla structures";

        internal static void DeleteFromBlockLayer(
            IAcadDocumentContext documentContext,
            IEntitySelector entitySelector,
            string layer)
        {
            Database database = documentContext.Database;

            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnLayer(layer));
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);
            ObjectId[] objectIdList = null;
            if (promptSelectionResult.Status.ToString() == "OK")
                objectIdList = promptSelectionResult.Value.GetObjectIds();

            if (objectIdList == null)
                return;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (ObjectId item in objectIdList)
                    {
                        BlockReference blockReference = (BlockReference)transaction.GetObject(item, OpenMode.ForRead);
                        BlockTableRecord block = (BlockTableRecord)transaction.GetObject(
                            blockReference.BlockTableRecord,
                            OpenMode.ForRead);

                        var enumerator = block.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Entity entity = (Entity)transaction.GetObject(enumerator.Current, OpenMode.ForRead);
                            if (entity.GetType() != typeof(DBText))
                                continue;

                            DBText text = entity as DBText;
                            if (text.TextString == TeklaStructuresText)
                                EraseText(text);
                        }
                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 90", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        internal static void DeleteDrawingSheetTexts(
            IAcadDocumentContext documentContext,
            IEntitySelector entitySelector)
        {
            Database database = documentContext.Database;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(
                        ConvertLayer.FilterText2("Drawing sheet", "ALL", "DrawingSheet", "Drawing_Sheet"));

                    ObjectId[] objectIdList = null;
                    if (promptSelectionResult.Status.ToString() == "OK")
                        objectIdList = promptSelectionResult.Value.GetObjectIds();

                    if (objectIdList == null)
                        return;

                    foreach (ObjectId item in objectIdList)
                    {
                        Entity entity = transaction.GetObject(item, OpenMode.ForRead) as Entity;
                        if (entity.GetType() != typeof(DBText))
                            continue;

                        DBText text = entity as DBText;
                        if (string.Equals(text.TextString, TeklaStructuresText, StringComparison.OrdinalIgnoreCase))
                            EraseText(text);
                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 80", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private static void EraseText(DBText text)
        {
            text.UpgradeOpen();
            text.Erase(true);
            text.DowngradeOpen();
        }
    }
}
