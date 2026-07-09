using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;

namespace ConversorDrawind.Commands
{
    internal sealed class LayerSelectionService
    {
        private readonly IEntitySelector entitySelector;
        private readonly Action<string, string> logError;

        internal LayerSelectionService(IEntitySelector entitySelector, Action<string, string> logError)
        {
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal ObjectId[] Filter(string layerName, string start, string colorName, string linetypeName)
        {
            try
            {
                List<TypedValue> typedValues = new List<TypedValue>();

                typedValues.Add(new TypedValue((int)DxfCode.Operator, "<and"));
                if (layerName != "ALL")
                    typedValues.Add(new TypedValue((int)DxfCode.LayerName, layerName));

                if (start != "ALL")
                    typedValues.Add(new TypedValue((int)DxfCode.Start, start));

                if (linetypeName != "ALL")
                    typedValues.Add(new TypedValue((int)DxfCode.LinetypeName, linetypeName));

                typedValues.Add(new TypedValue((int)DxfCode.Operator, "and>"));
                return SelectAll(new SelectionFilter(typedValues.ToArray()));
            }
            catch (Exception e)
            {
                logError(LogContext.SelecionarEntidadesPorLayer, e.Message);
                return null;
            }
        }

        internal ObjectId[] FilterLayers(params string[] layers)
        {
            try
            {
                List<TypedValue> typedValues = new List<TypedValue>();

                typedValues.Add(new TypedValue((int)DxfCode.Operator, "<and"));
                typedValues.Add(new TypedValue((int)DxfCode.Operator, "<or"));
                foreach (string item in layers)
                {
                    typedValues.Add(new TypedValue((int)DxfCode.LayerName, item));
                }
                typedValues.Add(new TypedValue((int)DxfCode.Operator, "or>"));
                typedValues.Add(new TypedValue((int)DxfCode.Operator, "and>"));
                return SelectAll(new SelectionFilter(typedValues.ToArray()));
            }
            catch (Exception e)
            {
                logError(LogContext.SelecionarLayers, e.Message);
                return null;
            }
        }

        private ObjectId[] SelectAll(SelectionFilter selectionFilter)
        {
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);
            if (promptSelectionResult.Status.ToString() == "OK")
                return promptSelectionResult.Value.GetObjectIds();

            return null;
        }
    }
}
