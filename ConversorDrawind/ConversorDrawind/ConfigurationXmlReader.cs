using System.Collections.Generic;

namespace ConversorDrawind
{
    internal static class ConfigurationXmlReader
    {
        public static void Load(
            Class_Configuration configuration,
            string file,
            Class_Arranjos arranjos,
            List<Class_BlockClass> blocks,
            List<Class_BlockClass> blocosi,
            List<Class_BlockClass> blocoso,
            StatusConversorItem statusConversorItem)
        {
            configuration.LoadXMLCore(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }
    }
}
