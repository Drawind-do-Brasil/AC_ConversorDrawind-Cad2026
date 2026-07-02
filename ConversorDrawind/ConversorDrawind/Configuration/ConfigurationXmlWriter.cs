using System.Collections.Generic;

namespace ConversorDrawind
{
    internal static class ConfigurationXmlWriter
    {
        public static void Save(
            Configuration configuration,
            string file,
            Arranjos arranjos,
            List<Block> blocks,
            List<Block> blocosi,
            List<Block> blocoso,
            StatusConversorItem statusConversorItem)
        {
            configuration.SaveXMLCore(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }
    }
}
