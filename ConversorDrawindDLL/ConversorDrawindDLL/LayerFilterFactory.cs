using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    internal static class LayerFilterFactory
    {
        internal static TypedValue[] TextAndMTextOnLayer(string layerName)
        {
            return TextTypesOnLayer(layerName, "TEXT", "MTEXT");
        }

        internal static TypedValue[] TextAndMTextOnLayers(params string[] layerNames)
        {
            List<TypedValue> typedValues = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.Operator, "<and"),
                new TypedValue((int)DxfCode.Operator, "<or")
            };

            foreach (string layerName in layerNames)
            {
                typedValues.Add(new TypedValue((int)DxfCode.LayerName, layerName));
            }

            typedValues.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "<or"));
            typedValues.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            typedValues.Add(new TypedValue((int)DxfCode.Start, "MTEXT"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return typedValues.ToArray();
        }

        internal static TypedValue[] TextAndInsertOnLayer(string layerName)
        {
            return TextTypesOnLayer(layerName, "TEXT", "INSERT");
        }

        internal static TypedValue[] TextOnOptionalLayer(string layerName)
        {
            List<TypedValue> typedValues = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.Operator, "<and")
            };

            if (layerName != "ALL")
            {
                typedValues.Add(new TypedValue((int)DxfCode.LayerName, layerName));
            }

            typedValues.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return typedValues.ToArray();
        }

        internal static TypedValue[] InsertOnLayer(string layerName)
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT"),
                new TypedValue((int)DxfCode.LayerName, layerName)
            };
        }

        internal static TypedValue[] InsertOnly()
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT")
            };
        }

        internal static TypedValue[] InsertBlockByName(string blockName)
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT"),
                new TypedValue((int)DxfCode.BlockName, blockName)
            };
        }

        internal static TypedValue[] InsertBlockByNameAnd(string blockName)
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Operator, "<and"),
                new TypedValue((int)DxfCode.Start, "INSERT"),
                new TypedValue((int)DxfCode.BlockName, blockName),
                new TypedValue((int)DxfCode.Operator, "and>")
            };
        }

        internal static TypedValue[] TextOrInsert()
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Operator, "<and"),
                new TypedValue((int)DxfCode.Operator, "<or"),
                new TypedValue((int)DxfCode.Start, "TEXT"),
                new TypedValue((int)DxfCode.Start, "INSERT"),
                new TypedValue((int)DxfCode.Operator, "or>"),
                new TypedValue((int)DxfCode.Operator, "and>")
            };
        }

        internal static TypedValue[] ObjectTypes(params string[] objectTypes)
        {
            List<TypedValue> typedValues = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.Operator, "<or")
            };

            foreach (string objectType in objectTypes)
            {
                typedValues.Add(new TypedValue((int)DxfCode.Start, objectType));
            }

            typedValues.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            return typedValues.ToArray();
        }

        private static TypedValue[] TextTypesOnLayer(string layerName, string firstType, string secondType)
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Operator, "<and"),
                new TypedValue((int)DxfCode.LayerName, layerName),
                new TypedValue((int)DxfCode.Operator, "<or"),
                new TypedValue((int)DxfCode.Start, firstType),
                new TypedValue((int)DxfCode.Start, secondType),
                new TypedValue((int)DxfCode.Operator, "or>"),
                new TypedValue((int)DxfCode.Operator, "and>")
            };
        }
    }
}
