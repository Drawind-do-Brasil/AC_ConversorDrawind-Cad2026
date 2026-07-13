using System;
using System.IO;

namespace ConversorDrawind
{
    internal static class DrawingProcessPaths
    {
        public static string DllPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConversorDrawind.Commands.dll"); }
        }

        public static string TempCommandFile
        {
            get { return Path.Combine(Path.GetTempPath(), "ConversorDrawind.Temp"); }
        }

        public static string GetConverterTxmlPath(Param1 parametros)
        {
            return ConverterFileService.GetTxmlPath(parametros.conversorName, parametros.StatusConversorItem);
        }

        public static string GetExchangeFormatPath(Configuration configuration)
        {
            return configuration.General.SourceMode == 0
                ? configuration.Blocks.TeklaBlockPath
                : configuration.Blocks.CadBlockPath;
        }

        public static void EnsureConvertedLogDirectory()
        {
            if (!Directory.Exists(ApplicationRuntime.LOGdirConvertidos))
                Directory.CreateDirectory(ApplicationRuntime.LOGdirConvertidos);
        }

        public static string GetBackupPath(string drawingPath)
        {
            return Path.Combine(Path.GetDirectoryName(drawingPath), Path.GetFileNameWithoutExtension(drawingPath) + ".dwg.bak");
        }
    }
}




