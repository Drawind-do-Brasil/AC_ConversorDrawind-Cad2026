namespace ConversorDrawind.Commands
{
    public static class ConfigurationRuntimeExtensions
    {
        public static void LoadXML(this Configuration configuration, string file)
        {
            RuntimeConfigurationLoader.Load(file, configuration);
        }
    }
}
