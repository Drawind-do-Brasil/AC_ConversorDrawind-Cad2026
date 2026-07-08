using System;
using System.IO;

namespace ConversorDrawind
{
    public static class UserSettingsService
    {
        private const string LinPackFileName = "LinPack.nfj";
        private const string LastConverterFileName = "LastConverter.nfj";

        public static string LinPackPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LinPackFileName); }
        }

        public static string LastConverterPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LastConverterFileName); }
        }

        public static string EnsureAndReadProgramDbLin(string defaultPath)
        {
            string directory = LinPackPath;

            if (!File.Exists(directory))
            {
                StreamWriter sw = new StreamWriter(directory);
                sw.WriteLine(defaultPath);
                sw.Close();
            }

            StreamReader sr = new StreamReader(directory);
            string programDbLin = sr.ReadLine();
            sr.Close();
            return programDbLin;
        }

        public static void SaveProgramDbLin(string path)
        {
            string directory = LinPackPath;
            if (File.Exists(directory))
                File.Delete(directory);

            StreamWriter sw = new StreamWriter(directory);
            sw.WriteLine(path);
            sw.Close();
        }

        public static void SaveLastConverter(StatusConversorItem statusConversorItem, string converterName)
        {
            if (statusConversorItem == null || string.IsNullOrWhiteSpace(converterName))
            {
                return;
            }

            using (StreamWriter sw = new StreamWriter(LastConverterPath, false))
            {
                sw.WriteLine(statusConversorItem.Pasta ?? string.Empty);
                sw.WriteLine(converterName);
            }
        }

        public static bool TryReadLastConverter(out string statusFolder, out string converterName)
        {
            statusFolder = string.Empty;
            converterName = string.Empty;

            if (!File.Exists(LastConverterPath))
            {
                return false;
            }

            string[] lines = File.ReadAllLines(LastConverterPath);
            if (lines.Length < 2)
            {
                return false;
            }

            statusFolder = lines[0] ?? string.Empty;
            converterName = lines[1] ?? string.Empty;
            return !string.IsNullOrWhiteSpace(statusFolder) && !string.IsNullOrWhiteSpace(converterName);
        }
    }
}



