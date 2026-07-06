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
    }
}



