using System.Collections.Generic;

namespace ConversorDrawind
{
    internal static class LegacyTemplateReader
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
            configuration.LoadTemplateCore(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }
    }
}
