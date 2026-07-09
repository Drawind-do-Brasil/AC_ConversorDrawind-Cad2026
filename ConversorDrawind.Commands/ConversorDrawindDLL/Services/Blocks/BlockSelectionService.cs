using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace ConversorDrawindDLL
{
    internal sealed class BlockSelectionService
    {
        private readonly IEntitySelector entitySelector;

        internal BlockSelectionService(IEntitySelector entitySelector)
        {
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
        }

        internal ObjectId[] SelectBlockReferences()
        {
            try
            {
                PromptSelectionResult result = entitySelector.SelectAll(new SelectionFilter(LayerFilterFactory.InsertOnly()));
                return result.Status == PromptStatus.OK ? result.Value.GetObjectIds() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
