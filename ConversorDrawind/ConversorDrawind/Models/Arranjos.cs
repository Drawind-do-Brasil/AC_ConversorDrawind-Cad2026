using System;
using System.Collections.Generic;
using System.IO;

namespace ConversorDrawind
{
    public class Arranjos
    {
        private static string[] colors = new string[] { "ALL", "BYLAYER", "BYBLOCK", "BLUE", "CYAN", "GREEN", "MAGENTA", "RED", "WHITE", "YELLOW" };
        private static string[] objects = new string[] { "ALL", "TEXT", "LINE", "ARC", "CIRCLE", "HATCH", "DIMENSION", "MTEXT", "LWPOLYLINE", "SPLINE", "ATTDEF" , "SOLID", "POINT"};
        public string[] lineType1 = new string[] { "ALL", "BYLAYER", "BYBLOCK", "BORDER", "CENTER", "CONTINUOUS", "DASHDOT", "DIVIDE", "DOT", "HIDDEN", "PHANTOM",  "XKITLINE00", "XKITLINE01", "XKITLINE02", "XKITLINE03", "XKITLINE04", "XKITLINE05", "XKITLINE06", "DXK_LINE_DOT1", "DXK_LINE_DOT2", "DXK_LINE_DOT3", "DXK_LINE_DOT4", "DXK_LINE_DOT5" };
        public string[] lineType2 = new string[] { "BYLAYER", "BYBLOCK", "CONTINUOUS,Continuous ________________________________" };
        public string[] lineTypeRemove = new string[] { "BYLAYER", "BYBLOCK" };
        public string[] linesTekla = new string[] { "0", 
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
     
            "LINKED OBJECT" , 
            "NEIGHBOUR PART MARK"
};
        public static string defaultTextStyle = "TEXTO:RomanS:false:false:2.5:1:0";

        public List<string> allcolor = new List<string>();
        public List<string> allobjects = new List<string>();
        public List<string> allLineType1 = new List<string>();
        public List<string> allLineType2 = new List<string>();
        public List<string> allBaseLayer = new List<string>();
        public List<string> allNewLayer = new List<string>();
        public List<string> allNewLayerComposition = new List<string>();
        public List<string> conversor = new List<string>();
        public List<Filter> layerRemove = new List<Filter>();
        public List<string> listLISPCommand = new List<string>();
        public List<string> listDLLCommand = new List<string>();
        public List<string> allExplodeLayers = new List<string>();
        public List<string> allTextSyles = new List<string>();

        public Arranjos()
        {
            Carregar();
            allBaseLayer.AddRange(linesTekla);
            allLineType2.AddRange(lineType2);

            Configuration configuration = new Configuration();
        
            string directory = AppDomain.CurrentDomain.BaseDirectory + "LinPack.nfj";

            if (!File.Exists(directory))
            {
                StreamWriter sw = new StreamWriter(directory);
                sw.WriteLine(configuration.PROGRAMDbLin);
                sw.Close();
            }

            StreamReader sr = new StreamReader(directory);
            string PROGRAMDbLin = sr.ReadLine();
            sr.Close();

            if (File.Exists(PROGRAMDbLin))
            {
                StreamReader wr = new StreamReader(PROGRAMDbLin);
                string line = "";
                while (!wr.EndOfStream)
                {
                    line = wr.ReadLine();
                    if (line.Length > 0 && line.Substring(0,1) == "*")
                    {
                        allLineType2.Add(line.Remove(0,1));
                    }
                }
            }
        }

        private void Carregar()
        {
            allTextSyles.Add(defaultTextStyle);
            allcolor.AddRange(colors);
            for (int i = 8; i < 256; i++)
            {
                allcolor.Add(Convert.ToString(i));
            }
            allobjects.AddRange(objects);
            allLineType1.AddRange(lineType1);
            allNewLayer.Add("0");
        }
    }
}


