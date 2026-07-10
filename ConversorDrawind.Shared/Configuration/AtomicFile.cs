using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConversorDrawind
{
    internal static class AtomicFile
    {
        internal static void Write(string path, Action<string> writeTemporaryFile)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("O caminho do arquivo é obrigatório.", nameof(path));

            if (writeTemporaryFile == null)
                throw new ArgumentNullException(nameof(writeTemporaryFile));

            string directory = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(directory))
                directory = AppDomain.CurrentDomain.BaseDirectory;

            Directory.CreateDirectory(directory);

            string temporaryPath = Path.Combine(directory, Path.GetRandomFileName());
            try
            {
                writeTemporaryFile(temporaryPath);

                if (File.Exists(path))
                    File.Replace(temporaryPath, path, null);
                else
                    File.Move(temporaryPath, path);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }

        internal static void WriteAllLines(string path, IEnumerable<string> lines)
        {
            Write(path, temporaryPath =>
                File.WriteAllLines(temporaryPath, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)));
        }
    }
}
