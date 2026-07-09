using System.Linq;

namespace ConversorDrawindDLL
{
    internal static class RuntimeConfigurationLoader
    {
        internal static void Load(string file, Configuration configuration)
        {
            Configuration loadedConfiguration = global::ConversorDrawind.ConverterConfigurationReader.Load(file);
            configuration.Apply(loadedConfiguration);
            Configuration.Config = configuration;
            Configuration.Config.Text.DefaultSize = ResolveTextSize(Configuration.Config.Text.DefaultStyleName);
            RuntimeConfigurationState.ResetWorkingStateFromConfiguration();
            RuntimeConfigurationState.RebuildConverterInstances();
        }

        private static double ResolveTextSize(string styleName)
        {
            string textStyle = RuntimeConfigurationState.TextStyles.FirstOrDefault(style =>
                string.Equals(style.Split(':').First(), styleName, System.StringComparison.OrdinalIgnoreCase))
                ?? RuntimeConfigurationState.TextStyles.First();

            string[] textStyleSplit = textStyle.Split(':');
            return textStyleSplit.Length > 4 ? textStyleSplit[4].ToDouble() : 2.5;
        }
    }
}
