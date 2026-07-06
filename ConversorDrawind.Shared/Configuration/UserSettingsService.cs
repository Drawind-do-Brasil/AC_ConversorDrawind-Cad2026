using System;
using System.IO;

namespace ConversorDrawind
{
    internal static class UserSettingsService
    {
        private const string LinPackFileName = "LinPack.nfj";

        public static string LinPackPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LinPackFileName); }
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
    }
}



