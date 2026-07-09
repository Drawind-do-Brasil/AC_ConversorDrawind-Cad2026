using System.IO;

namespace ConversorDrawind.Commands
{
    internal static class ConversionPreflight
    {
        internal static string GetTempConfigurationPath()
        {
            string tempDirectory = Path.GetTempPath();
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            return Path.Combine(tempDirectory, "ConversorDrawind.Temp");
        }

        internal static void LoadTempConfiguration(Configuration configuration, ref string converterName)
        {
            string tempConfigurationPath = GetTempConfigurationPath();
            if (!File.Exists(tempConfigurationPath))
                return;

            try
            {
                StreamReader sr = new StreamReader(tempConfigurationPath);
                string file = sr.ReadLine();
                sr.Close();

                converterName = Path.GetFileNameWithoutExtension(file);
                RuntimeConfigurationLoader.Load(file, configuration);
            }
            catch (System.Exception e)
            {
                ConversionLog.Write(LogContext.CarregarConfiguracaoTemporaria, e.Message);
            }
        }
    }
}

