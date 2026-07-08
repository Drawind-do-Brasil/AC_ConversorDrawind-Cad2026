using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConversorDrawind
{
    public static class ConverterFileService
    {
        private static readonly IConverterConfigurationRepository Repository = new TxmlConverterConfigurationRepository();

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
            ConverterConfiguration structuredConfiguration = LoadConverter(converterName, statusConversorItem);
            ConfigurationCompatibilityMapper.ApplyToLegacyState(structuredConfiguration, configuration, arranjos, blocks, blocksInv, blocksOrig);
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
            SaveConverter(converterName, statusConversorItem, structuredConfiguration);
        }

        public static ConverterConfiguration LoadConverter(string converterName, StatusConversorItem statusConversorItem)
        {
            return Repository.Load(converterName, statusConversorItem);
        }

        public static void SaveConverter(string converterName, StatusConversorItem statusConversorItem, ConverterConfiguration configuration)
        {
            Repository.Save(converterName, statusConversorItem, configuration);
        }

    }
}



