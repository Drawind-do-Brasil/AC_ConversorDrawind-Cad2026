using Autodesk.AutoCAD.Interop;
using System;
using System.IO;
using System.Linq;

namespace ConversorDrawind
{
    public static class CadInteropPaths
    {
        public static string[] GetSupportFolders(AcadApplication acadApp)
        {
            string supportPath = acadApp.Preferences.Files.SupportPath ?? "";

            return supportPath
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static string FindFile(AcadApplication acadApp, string fileName)
        {
            foreach (var folder in GetSupportFolders(acadApp))
            {
                string full = Path.Combine(folder, fileName);
                if (File.Exists(full))
                    return full;
            }
            return null;
        }
    }
}



