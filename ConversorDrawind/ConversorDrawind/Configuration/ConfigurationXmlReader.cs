using System.Collections.Generic;

namespace ConversorDrawind
{
    internal static class ConfigurationXmlReader
    {
        public static void Load(
            Configuration configuration,
            string file,
            Arranjos arranjos,
            List<Block> blocks,
            List<Block> blocosi,
            List<Block> blocoso,
            StatusConversorItem statusConversorItem)
        {
            configuration.LoadXMLCore(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }
    }
}
