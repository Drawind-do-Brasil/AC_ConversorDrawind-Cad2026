using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace ConversorDrawind.Commands
{
    internal static class LayerFilterFactory
    {
        private static readonly string[] GeneralEntityTypes =
        {
            "INSERT", "CIRCLE", "LINE", "TEXT", "ARC", "HATCH", "DIMENSION",
            "MTEXT", "LWPOLYLINE", "SPLINE", "ATTDEF", "SOLID", "POINT"
        };

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

        internal static TypedValue[] LayerProperties(string layerName, string start, string linetypeName)
        {
            List<TypedValue> typedValues = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.Operator, "<and")
            };

            if (layerName != "ALL")
            {
                typedValues.Add(new TypedValue((int)DxfCode.LayerName, layerName));
            }

            if (start != "ALL")
            {
                typedValues.Add(new TypedValue((int)DxfCode.Start, start));
            }

            if (linetypeName != "ALL")
            {
                typedValues.Add(new TypedValue((int)DxfCode.LinetypeName, linetypeName));
            }

            typedValues.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return typedValues.ToArray();
        }

        internal static TypedValue[] Layers(params string[] layers)
        {
            List<TypedValue> typedValues = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.Operator, "<and"),
                new TypedValue((int)DxfCode.Operator, "<or")
            };

            foreach (string layer in layers)
            {
                typedValues.Add(new TypedValue((int)DxfCode.LayerName, layer));
            }

            typedValues.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return typedValues.ToArray();
        }

        internal static TypedValue[] TeklaDrawingSheetInsert()
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Operator, "<and"),
                new TypedValue((int)DxfCode.Operator, "<or"),
                new TypedValue((int)DxfCode.LayerName, "ALL"),
                new TypedValue((int)DxfCode.LayerName, "DrawingSheet"),
                new TypedValue((int)DxfCode.LayerName, "Drawing Sheet"),
                new TypedValue((int)DxfCode.LayerName, "Drawing_Sheet"),
                new TypedValue((int)DxfCode.Operator, "or>"),
                new TypedValue((int)DxfCode.Start, "INSERT"),
                new TypedValue((int)DxfCode.Operator, "and>")
            };
        }

        internal static TypedValue[] GeneralDrawingEntities()
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Start, string.Join(", ", GeneralEntityTypes))
            };
        }

        internal static TypedValue[] ScaleDetectionEntities()
        {
            return new[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT, TEXT, MTEXT")
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
