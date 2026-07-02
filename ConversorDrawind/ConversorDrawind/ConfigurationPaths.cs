namespace ConversorDrawind
{
    internal static class ConfigurationPaths
    {
        public static string EnsureFolder(StatusConversorItem statusConversorItem)
        {
            return ConfigurationXmlContract.EnsureFolder(statusConversorItem);
        }

        public static string TxmlPath(string file, StatusConversorItem statusConversorItem)
        {
            return ConfigurationXmlContract.TxmlPath(file, statusConversorItem);
        }

        public static string TemplatePath(string file, StatusConversorItem statusConversorItem)
        {
            return ConfigurationXmlContract.TemplatePath(file, statusConversorItem);
        }

        public static string TemplatesPath(string file, StatusConversorItem statusConversorItem)
        {
            return ConfigurationXmlContract.TemplatesPath(file, statusConversorItem);
        }
    }
}
