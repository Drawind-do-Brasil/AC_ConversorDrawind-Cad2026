using System;
using System.IO;

namespace ConversorDrawind
{
    public static class UserSettingsService
    {
        private const string LinPackFileName = "LinPack.nfj";
        private const string LastConverterFileName = "LastConverter.nfj";
        private const string SettingsFolderName = "ConversorDrawind";

        private static readonly object SyncRoot = new object();

        public static string LinPackPath
        {
            get { return Path.Combine(SettingsDirectory, LinPackFileName); }
        }

        public static string LastConverterPath
        {
            get { return Path.Combine(SettingsDirectory, LastConverterFileName); }
        }

        private static string SettingsDirectory
        {
            get
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localAppData, SettingsFolderName);
            }
        }

        public static string EnsureAndReadProgramDbLin(string defaultPath)
        {
            lock (SyncRoot)
            {
                MigrateLegacyFileIfNeeded(LinPackFileName);

                if (!File.Exists(LinPackPath))
                    AtomicFile.WriteAllLines(LinPackPath, new[] { defaultPath ?? string.Empty });

                using (StreamReader reader = new StreamReader(LinPackPath))
                    return reader.ReadLine() ?? string.Empty;
            }
        }

        public static void SaveProgramDbLin(string path)
        {
            lock (SyncRoot)
                AtomicFile.WriteAllLines(LinPackPath, new[] { path ?? string.Empty });
        }

        public static void SaveLastConverter(StatusConversorItem statusConversorItem, string converterName)
        {
            if (statusConversorItem == null || string.IsNullOrWhiteSpace(converterName))
            {
                return;
            }

            lock (SyncRoot)
                AtomicFile.WriteAllLines(
                    LastConverterPath,
                    new[] { statusConversorItem.Pasta ?? string.Empty, converterName });
        }

        public static bool TryReadLastConverter(out string statusFolder, out string converterName)
        {
            statusFolder = string.Empty;
            converterName = string.Empty;

            lock (SyncRoot)
            {
                MigrateLegacyFileIfNeeded(LastConverterFileName);

                if (!File.Exists(LastConverterPath))
                    return false;

                string[] lines = File.ReadAllLines(LastConverterPath);
                if (lines.Length < 2)
                    return false;

                statusFolder = lines[0] ?? string.Empty;
                converterName = lines[1] ?? string.Empty;
                return !string.IsNullOrWhiteSpace(statusFolder) && !string.IsNullOrWhiteSpace(converterName);
            }
        }

        private static void MigrateLegacyFileIfNeeded(string fileName)
        {
            string destination = Path.Combine(SettingsDirectory, fileName);
            if (File.Exists(destination))
                return;

            string legacyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(legacyPath))
                return;

            AtomicFile.WriteAllLines(destination, File.ReadAllLines(legacyPath));
        }
    }
}



