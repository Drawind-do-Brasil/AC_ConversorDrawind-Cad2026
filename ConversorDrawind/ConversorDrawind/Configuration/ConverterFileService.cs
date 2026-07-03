using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConversorDrawind
{
    internal static class ConverterFileService
    {
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
            configuration.LoadXML(converterName, arranjos, blocks, blocksInv, blocksOrig, statusConversorItem);
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
            configuration.SaveXML(converterName, arranjos, blocks, blocksInv, blocksOrig, statusConversorItem);
        }

    }
}



