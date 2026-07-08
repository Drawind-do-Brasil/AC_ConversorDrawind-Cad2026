using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConversorDrawind
{
    public static class ConverterFileService
    {
        private static readonly IConfigurationRepository Repository = new TxmlConfigurationRepository();

        public static List<string> ListConverterNames(StatusConversorItem statusConversorItem)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(ConfigurationPaths.EnsureFolder(statusConversorItem));
                return directoryInfo
                    .GetFiles("*.txml")
                    .Select(file => Path.GetFileNameWithoutExtension(file.Name))
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static string GetTxmlPath(string converterName, StatusConversorItem statusConversorItem)
        {
            return ConfigurationPaths.TxmlPath(converterName, statusConversorItem);
        }

        public static void LoadConverter(
            Configuration configuration,
            string converterName,
            Arranjos arranjos,
            List<Block> blocks,
            List<Block> blocksInv,
            List<Block> blocksOrig,
            StatusConversorItem statusConversorItem)
        {
            Configuration loadedConfiguration = LoadConverter(converterName, statusConversorItem);
            configuration.Apply(loadedConfiguration);
            ConfigurationCompatibilityMapper.ApplyToLegacyState(loadedConfiguration.ToConverterConfiguration(), configuration, arranjos, blocks, blocksInv, blocksOrig);
        }

        public static void SaveConverter(
            Configuration configuration,
            string converterName,
            Arranjos arranjos,
            List<Block> blocks,
            List<Block> blocksInv,
            List<Block> blocksOrig,
            StatusConversorItem statusConversorItem)
        {
            ConverterConfiguration structuredConfiguration = ConfigurationCompatibilityMapper.FromLegacyState(configuration, arranjos, blocks, blocksInv, blocksOrig);
            SaveConverter(converterName, statusConversorItem, new Configuration(structuredConfiguration));
        }

        public static Configuration LoadConverter(string converterName, StatusConversorItem statusConversorItem)
        {
            return Repository.Load(converterName, statusConversorItem);
        }

        public static void SaveConverter(string converterName, StatusConversorItem statusConversorItem, Configuration configuration)
        {
            Repository.Save(converterName, statusConversorItem, configuration);
        }

        [System.Obsolete("Use SaveConverter(string, StatusConversorItem, Configuration).")]
        public static void SaveConverter(string converterName, StatusConversorItem statusConversorItem, ConverterConfiguration configuration)
        {
            Repository.Save(converterName, statusConversorItem, new Configuration(configuration));
        }

    }
}



