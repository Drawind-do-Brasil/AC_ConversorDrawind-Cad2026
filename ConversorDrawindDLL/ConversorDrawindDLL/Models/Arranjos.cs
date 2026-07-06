using System.Collections.Generic;

namespace ConversorDrawindDLL
{
    public class Arranjos
    {
        public static string defaultTextStyle = "TEXTO:RomanS:false:false:2.5:1:0";

        public static Arranjos Arrj = new Arranjos();
        public List<string> AllNewLayerComposition = new List<string>();
        public List<string> Conversor = new List<string>();

        public List<Filter> LayerRemove = new List<Filter>();
        public List<string> ListLISPCommand = new List<string>();

        public List<string> AllExplodeLayers = new List<string>();
 
        public static List<BlockClass> ListBlocks = new List<BlockClass>();
        public static List<BlockClass> ListBlocksInv = new List<BlockClass>();
        public static List<BlockClass> ListBlocksOrig = new List<BlockClass>();
        public List<string> AllTextSyles = new List<string>();
    }

    
}
