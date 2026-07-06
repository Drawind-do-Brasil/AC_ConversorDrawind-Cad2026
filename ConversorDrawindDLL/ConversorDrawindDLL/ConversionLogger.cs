using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.IO;

namespace ConversorDrawindDLL
{
    internal static class ConversionLogger
    {
        internal static void InitializeForDocument(Document document, ref string directory, ref string fileName)
        {
            if (directory == "")
            {
                directory = Path.GetDirectoryName(document.Name);
                fileName = Path.Combine(directory, "Conversor.log");
                AppendLine(fileName, "Log de erros internos da conversão: " + Environment.UserDomainName + " " + Environment.UserName + " " + DateTime.Now);
            }

            AppendLine(fileName, "Drawing: " + document.Name);
        }

        internal static void Write(string directory, string fileName, string log, string erro)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            AppendLine(fileName, log + " : " + erro);
        }

        private static void AppendLine(string fileName, string text)
        {
            StreamWriter sw = File.AppendText(fileName);
            sw.WriteLine(text);
            sw.Close();
        }
    }
}
