using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.IO;
using System.Text;

namespace ConversorDrawindDLL
{
    internal static class ConversionLogger
    {
        private const string DefaultLogFileName = "Conversor.log";

        internal static void InitializeForDocument(Document document, ref string directory, ref string fileName)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Path.GetDirectoryName(document.Name);
                fileName = ResolveLogFileName(directory, fileName);
                AppendLine(fileName, Localization.MessageInternalConversionLogHeader + ": " + Environment.UserDomainName + " " + Environment.UserName + " " + DateTime.Now);
            }
            else
            {
                fileName = ResolveLogFileName(directory, fileName);
            }

            AppendLine(fileName, Localization.MessageDrawingPrefix + " " + document.Name);
        }

        internal static void Write(string directory, string fileName, string context, string detail)
        {
            string resolvedFileName = ResolveLogFileName(directory, fileName);
            AppendLine(resolvedFileName, FormatLine(context, detail));
        }

        private static string FormatLine(string context, string detail)
        {
            string safeContext = string.IsNullOrWhiteSpace(context) ? Localization.MessageNoLogContext : context.Trim();
            string safeDetail = string.IsNullOrWhiteSpace(detail) ? Localization.MessageNoLogDetails : detail.Trim();

            return safeContext + " : " + safeDetail;
        }

        private static string ResolveLogFileName(string directory, string fileName)
        {
            string resolvedDirectory = string.IsNullOrWhiteSpace(directory)
                ? Path.Combine(Path.GetTempPath(), "ConversorDrawind")
                : directory;

            string resolvedFileName = string.IsNullOrWhiteSpace(fileName)
                ? Path.Combine(resolvedDirectory, DefaultLogFileName)
                : fileName;

            if (!Path.IsPathRooted(resolvedFileName))
                resolvedFileName = Path.Combine(resolvedDirectory, resolvedFileName);

            string logDirectory = Path.GetDirectoryName(resolvedFileName);
            if (!string.IsNullOrWhiteSpace(logDirectory))
                Directory.CreateDirectory(logDirectory);

            return resolvedFileName;
        }

        private static void AppendLine(string fileName, string text)
        {
            File.AppendAllText(fileName, text + Environment.NewLine, Encoding.UTF8);
        }
    }
}