using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ConversorDrawind
{
    public class Arranjos
    {
        private static string[] colors = new string[] { "ALL", "BYLAYER", "BYBLOCK", "BLUE", "CYAN", "GREEN", "MAGENTA", "RED", "WHITE", "YELLOW" };
        private static string[] objects = new string[] { "ALL", "TEXT", "LINE", "ARC", "CIRCLE", "HATCH", "DIMENSION", "MTEXT", "LWPOLYLINE", "SPLINE", "ATTDEF" , "SOLID", "POINT"};
       
        public static string defaultTextStyle = "TEXTO:RomanS:false:false:2.5:1:0";

        public static List<string> DefaultColors()
        {
            List<string> result = colors.ToList();
            for (int i = 8; i < 256; i++)
            {
                result.Add(Convert.ToString(i));
            }

            return result;
        }

        public static List<string> DefaultObjectTypes()
        {
            return objects.ToList();
        }

        public static List<string> DefaultFilterLineTypes()
        {
            return new ArranjosDefaultValues().LineType1.ToList();
        }

        public static List<string> DefaultLayerLineTypes()
        {
            return new ArranjosDefaultValues().LineType2.ToList();
        }

        public static List<string> DefaultRemovedLineTypes()
        {
            return new ArranjosDefaultValues().LineTypeRemove.ToList();
        }

        public static List<string> DefaultBaseLayers()
        {
            return new ArranjosDefaultValues().LinesTekla.ToList();
        }

        [XmlIgnore]
        public static Arranjos Arrj = new Arranjos();

        [XmlIgnore]
        public static List<Block> ListBlocks = new List<Block>();

        [XmlIgnore]
        public static List<Block> ListBlocksInv = new List<Block>();

        [XmlIgnore]
        public static List<Block> ListBlocksOrig = new List<Block>();

        public List<string> allBaseLayer = new List<string>();
        public List<string> allNewLayer = new List<string>();
        public List<string> allNewLayerComposition = new List<string>();
        public List<string> conversor = new List<string>();
        public List<Filter> layerRemove = new List<Filter>();
        public List<string> listLISPCommand = new List<string>();
        public List<string> listDLLCommand = new List<string>();
        public List<string> allExplodeLayers = new List<string>();
        public List<string> allTextSyles = new List<string>();

        [XmlIgnore]
        public List<string> AllNewLayerComposition { get { return allNewLayerComposition; } }

        [XmlIgnore]
        public List<string> Conversor { get { return conversor; } }

        [XmlIgnore]
        public List<Filter> LayerRemove { get { return layerRemove; } }

        [XmlIgnore]
        public List<string> ListLISPCommand { get { return listLISPCommand; } }

        [XmlIgnore]
        public List<string> AllExplodeLayers { get { return allExplodeLayers; } }

        [XmlIgnore]
        public List<string> AllTextSyles { get { return allTextSyles; } }

        public Arranjos()
        {
            Load();
        }

        private void Load()
        {
            allTextSyles.Add(defaultTextStyle);
            allNewLayer.Add("0");
            allBaseLayer.AddRange(DefaultBaseLayers());
        }

        private sealed class ArranjosDefaultValues
        {
            public string[] LineType1 = new string[] { "ALL", "BYLAYER", "BYBLOCK", "BORDER", "CENTER", "CONTINUOUS", "DASHDOT", "DIVIDE", "DOT", "HIDDEN", "PHANTOM", "XKITLINE00", "XKITLINE01", "XKITLINE02", "XKITLINE03", "XKITLINE04", "XKITLINE05", "XKITLINE06", "DXK_LINE_DOT1", "DXK_LINE_DOT2", "DXK_LINE_DOT3", "DXK_LINE_DOT4", "DXK_LINE_DOT5" };
            public string[] LineType2 = new string[] { "BYLAYER", "BYBLOCK", "CONTINUOUS,Continuous ________________________________" };
            public string[] LineTypeRemove = new string[] { "BYLAYER", "BYBLOCK" };
            public string[] LinesTekla = new string[] { "0",
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
        }
    }
}





