using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{
    internal sealed class BlockDeletionService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly Func<string, ObjectId[]> filterBlock;
        private readonly Func<string, string, string, string, ObjectId[]> filterEntities;
        private readonly Action<string, string> logError;

        internal BlockDeletionService(
            IAcadDocumentContext documentContext,
            Func<string, ObjectId[]> filterBlock,
            Func<string, string, string, string, ObjectId[]> filterEntities,
            Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.filterBlock = filterBlock ?? throw new ArgumentNullException(nameof(filterBlock));
            this.filterEntities = filterEntities ?? throw new ArgumentNullException(nameof(filterEntities));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void DeleteLayerObjectsInBlocks(List<Filter> layers)
        {
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);

            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnly());
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);
            ObjectId[] objectIds = null;
            if (promptSelectionResult.Status.ToString() == "OK")
                objectIds = promptSelectionResult.Value.GetObjectIds();

            if (objectIds != null)
            {
                try
                {
                    foreach (ObjectId item in objectIds)
                    {
                        BlockReference blockReference = (BlockReference)item.GetObject(OpenMode.ForRead);

                        BlockTableRecord block = (BlockTableRecord)blockReference.BlockTableRecord.GetObject(OpenMode.ForRead);
                        BlockTableRecordEnumerator enumerator = block.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            Entity entity = (Entity)enumerator.Current.GetObject(OpenMode.ForWrite);

                            List<Filter> filters =
                                 layers.Where(filter => string.Equals(filter.layerBase, entity.Layer, StringComparison.OrdinalIgnoreCase) &&
                                  (string.Equals(filter.tipoObjeto, "ALL", StringComparison.OrdinalIgnoreCase) || string.Equals(filter.tipoObjeto, entity.Id.ObjectClass.DxfName, StringComparison.OrdinalIgnoreCase)) &&
                                  (string.Equals(filter.cor, "ALL", StringComparison.OrdinalIgnoreCase) || entity.Color.ColorNameForDisplay == ConvertLayer.GetColorForName(filter.cor).ColorNameForDisplay) &&
                                  (string.Equals(filter.tipoLinha, "ALL", StringComparison.OrdinalIgnoreCase) || string.Equals(filter.tipoLinha, entity.Linetype, StringComparison.OrdinalIgnoreCase)) &&
                                  (entity.GetType() != typeof(DBText) || (entity.GetType() == typeof(DBText) &&
                                                                         (string.IsNullOrEmpty(filter.conteudoTexto) || filter.conteudoTexto == ((DBText)entity).TextString) &&
                                                                         (string.IsNullOrEmpty(filter.alturaTexto) || Math.Round(((DBText)entity).Height, filter.alturaTextoRound) == Convert.ToDouble(filter.alturaTexto.ReplaceComma()))))).ToList();

                            if (filters.Count > 0)
                                entity.Erase();
                        }
                    }
                }
                catch (Exception e)
                {
                    logError("Erro 88", e.Message);
                }
            }
        }

        internal void DeleteLayerObjects(List<Filter> layers)
        {
            try
            {
                foreach (Filter filter in layers)
                {
                    ObjectId[] objectIds = filterEntities(filter.layerBase, filter.tipoObjeto, filter.cor, filter.tipoLinha);

                    try
                    {
                        if (objectIds != null)
                        {
                            foreach (ObjectId id in objectIds)
                            {
                                Entity entity = id.GetObject(OpenMode.ForWrite) as Entity;
                                if (string.Equals(filter.cor, "ALL", StringComparison.OrdinalIgnoreCase) || entity.Color.ColorNameForDisplay == ConvertLayer.GetColorForName(filter.cor).ColorNameForDisplay)
                                {
                                    if (entity.GetType() != typeof(DBText) &&
                                        (!string.Equals(entity.Id.ObjectClass.DxfName, "INSERT", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(filter.tipoObjeto, "INSERT", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        entity.Erase();
                                    }
                                    else if (entity.GetType() == typeof(DBText))
                                    {
                                        DBText text = (DBText)entity;
                                        double height = 0;
                                        int decimals = 0;
                                        int.TryParse(filter.alturaTexto.ReplaceComma().Split(',').Last(), out decimals);

                                        if (!string.IsNullOrEmpty(filter.alturaTexto))
                                            height = Convert.ToDouble(filter.alturaTexto.ReplaceComma());

                                        if ((string.IsNullOrEmpty(filter.conteudoTexto) || string.Equals(text.TextString, filter.conteudoTexto, StringComparison.OrdinalIgnoreCase)) &&
                                            (string.IsNullOrEmpty(filter.alturaTexto) || Math.Round(text.Height, decimals) == height))
                                        {
                                            text.Erase();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logError("Erro 6", e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                logError("Erro 7", e.Message);
            }
        }

        internal void DeleteRelatedBlocks(List<BlockClass> blocks)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (BlockClass block in blocks)
                    {
                        if (block.blockNameRelacao != "")
                        {
                            ObjectId[] objectIds = filterBlock(block.blockNameRelacao);

                            if (objectIds != null)
                            {
                                foreach (ObjectId id in objectIds)
                                {
                                    Entity entity = transaction.GetObject(id, OpenMode.ForWrite) as Entity;
                                    entity.Erase();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logError("Erro 8", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
