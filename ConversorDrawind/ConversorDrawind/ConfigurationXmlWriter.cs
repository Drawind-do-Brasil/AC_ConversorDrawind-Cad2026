using System.Collections.Generic;

namespace ConversorDrawind
{
    internal static class ConfigurationXmlWriter
    {
        public static void Save(
            Class_Configuration configuration,
            string file,
            Class_Arranjos arranjos,
            List<Class_BlockClass> blocks,
            List<Class_BlockClass> blocosi,
            List<Class_BlockClass> blocoso,
            StatusConversorItem statusConversorItem)
        {
            configuration.SaveXMLCore(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }
    }
}
