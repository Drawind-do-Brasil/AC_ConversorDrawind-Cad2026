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
                List<FileInfo> allFiles = directoryInfo.GetFiles("*.template").ToList();
                allFiles.AddRange(directoryInfo.GetFiles("*.txml"));
                return allFiles.Select(file => Path.GetFileNameWithoutExtension(file.Name)).ToList();
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

        public static string GetTemplatePath(string converterName, StatusConversorItem statusConversorItem)
        {
            return ConfigurationPaths.TemplatePath(converterName, statusConversorItem);
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
            if (configuration.CheckFileTxmlExist(converterName, statusConversorItem))
                configuration.LoadXML(converterName, arranjos, blocks, blocksInv, blocksOrig, statusConversorItem);
            else
                configuration.Load(converterName, arranjos, blocks, blocksInv, blocksOrig, statusConversorItem);
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

        public static void MigrateTemplateToTxml(
            Configuration configuration,
            string converterName,
            Arranjos arranjos,
            List<Block> blocks,
            List<Block> blocksInv,
            List<Block> blocksOrig,
            StatusConversorItem statusConversorItem)
        {
            string txmlPath = GetTxmlPath(converterName, statusConversorItem);
            string templatePath = GetTemplatePath(converterName, statusConversorItem);

            if (!File.Exists(txmlPath) && File.Exists(templatePath))
            {
                configuration.Load(converterName, arranjos, blocks, blocksInv, blocksOrig, statusConversorItem);
                configuration.SaveXML(converterName, arranjos, blocks, blocksInv, blocksOrig, statusConversorItem);
                File.Delete(templatePath);
            }
        }
    }
}
