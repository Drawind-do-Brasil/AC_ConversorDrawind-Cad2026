using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawindDLL
{
    public static class CadPaths
    {
        public static string FindFileNetload(string fileName)
        {
            try
            {
                IAcadDocumentContext documentContext = new AcadDocumentContext();
                if (documentContext.Document == null)
                    return null;

                return HostApplicationServices.Current.FindFile(
                    fileName,
                    documentContext.Database,
                    FindFileHint.Default
                );
            }
            catch
            {
                return null;
            }
        }

        public static string FindShx(string fontName)
        {
            if (string.IsNullOrWhiteSpace(fontName))
                return null;

            if (!fontName.EndsWith(".shx", StringComparison.OrdinalIgnoreCase))
                fontName += ".shx";

            return FindFileNetload(fontName);
        }

        public static string FindLin(string linFileName)
        {
            if (string.IsNullOrWhiteSpace(linFileName))
                return null;

            if (!linFileName.EndsWith(".lin", StringComparison.OrdinalIgnoreCase))
                linFileName += ".lin";

            return FindFileNetload(linFileName);
        }
    }
}
