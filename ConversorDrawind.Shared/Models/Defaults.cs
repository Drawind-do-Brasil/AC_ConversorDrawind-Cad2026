using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawind
{
    public static class Defaults
    {
        private static readonly string[] Colors =
        {
            "ALL", "BYLAYER", "BYBLOCK", "BLUE", "CYAN", "GREEN", "MAGENTA", "RED", "WHITE", "YELLOW"
        };

        private static readonly string[] ObjectTypes =
        {
            "ALL", "TEXT", "LINE", "ARC", "CIRCLE", "HATCH", "DIMENSION", "MTEXT", "LWPOLYLINE", "SPLINE",
            "ATTDEF", "SOLID", "POINT"
        };

        private static readonly string[] FilterLineTypes =
        {
            "ALL", "BYLAYER", "BYBLOCK", "BORDER", "CENTER", "CONTINUOUS", "DASHDOT", "DIVIDE", "DOT",
            "HIDDEN", "PHANTOM", "XKITLINE00", "XKITLINE01", "XKITLINE02", "XKITLINE03", "XKITLINE04",
            "XKITLINE05", "XKITLINE06", "DXK_LINE_DOT1", "DXK_LINE_DOT2", "DXK_LINE_DOT3", "DXK_LINE_DOT4",
            "DXK_LINE_DOT5"
        };

        private static readonly string[] LayerLineTypes =
        {
            "BYLAYER", "BYBLOCK", "CONTINUOUS,Continuous ________________________________"
        };

        private static readonly string[] RemovedLineTypes =
        {
            "BYLAYER", "BYBLOCK"
        };

        private static readonly string[] BaseLayers =
        {
            "0",
            "BOLT",
            "BOLT MARK",
            "BOLT_MARK",
            "BOLTS",
            "COMPONENT",
            "COMPONENT MARK",
            "CONNECTION MARK",
            "DETAIL_MARKS",
            "DIMENSION",
            "DRAWING SHEET",
            "DRAWING_TABLE",
            "GRAPHIC OBJECT",
            "GRAPHICAL_OBJECT",
            "GRID",
            "GRIDS",
            "LEVEL TEXT",
            "LINKED OBJECT",
            "MARK",
            "MARKS",
            "MODEL OBJECT",
            "NEIGHBOUR PART MARK",
            "OTHER OBJECT TYPE",
            "PART",
            "PART MARK",
            "REINFORCEMENT MARK",
            "REINFORCING BAR",
            "REVISION TEXT",
            "SECTION",
            "SECTION_MARKS",
            "SYMBOL TEXT",
            "TEXT",
            "VIEW_LABEL",
            "VIEWPORT LAYER",
            "WELD",
            "WELD_MARKS",
            "LINKED OBJECT",
            "NEIGHBOUR PART MARK"
        };

        public static TextStyleDefinition TextStyle()
        {
            return new TextStyleDefinition
            {
                Name = "TEXTO",
                Font = "RomanS",
                Italic = false,
                Bold = false,
                Size = 2.5,
                WidthFactor = 1,
                ObliqueAngle = 0
            };
        }

        public static string LegacyTextStyle()
        {
            return LegacyConfigurationParsers.FormatTextStyleDefinition(TextStyle());
        }

        public static List<string> DefaultColors()
        {
            List<string> result = Colors.ToList();
            for (int i = 8; i < 256; i++)
                result.Add(Convert.ToString(i));

            return result;
        }

        public static List<string> DefaultObjectTypes()
        {
            return ObjectTypes.ToList();
        }

        public static List<string> DefaultFilterLineTypes()
        {
            return FilterLineTypes.ToList();
        }

        public static List<string> DefaultLayerLineTypes()
        {
            return LayerLineTypes.ToList();
        }

        public static List<string> DefaultRemovedLineTypes()
        {
            return RemovedLineTypes.ToList();
        }

        public static List<string> DefaultBaseLayers()
        {
            return BaseLayers.ToList();
        }

        public static List<LayerDefinition> DefaultNewLayers()
        {
            return new List<LayerDefinition>
            {
                new LayerDefinition
                {
                    Name = "0",
                    Color = "WHITE",
                    LineType = "CONTINUOUS"
                }
            };
        }

        public static List<string> DefaultNewLayerNames()
        {
            return DefaultNewLayers().Select(layer => layer.Name).ToList();
        }

        public static List<string> DefaultLegacyNewLayers()
        {
            return DefaultNewLayers().Select(LegacyConfigurationParsers.FormatLayerDefinition).ToList();
        }
    }
}
