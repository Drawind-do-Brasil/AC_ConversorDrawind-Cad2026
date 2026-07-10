using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConversorDrawind
{
    partial class DrawingProcess
    {
        public static void GoProcess(object parameter)
        {
            PrepareBatchLog();

            parametros = parameter as Param1;
            if (parametros == null || parametros.configuration == null || parametros.desenhosName == null || !parametros.desenhosName.Any())
            {
                Debug.WriteLine("Parâmetros inválidos para iniciar a conversão.");
                return;
            }

            WriteTemporaryConfigurationPath();

            bool convertedAnyFile = false;
            bool deleteLogAfterClose = false;
            List<string> failedFiles = new List<string>();

            using (FileStream logStream = new FileStream(ApplicationRuntime.LOGarqConvertidos, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter convertedFilesLog = new StreamWriter(logStream))
            {
                try
                {
                    IsACADOpen = false;
                    using (MessageFilter.ScopedRegistration())
                    {
                        OpenACAD();
                        LoadFile(DLLPath1);
                    }

                    ProcessDrawings(convertedFilesLog, failedFiles, ref convertedAnyFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    deleteLogAfterClose = !convertedAnyFile;
                    ShowAutoCADCommandLine();
                    CloseACAD();
                    Valor = 100;
                }
            }

            if (deleteLogAfterClose)
                TryDeleteFile(ApplicationRuntime.LOGarqConvertidos);

            ShowBatchFailures(failedFiles);
        }

        private static void PrepareBatchLog()
        {
            try
            {
                DrawingProcessPaths.EnsureConvertedLogDirectory();
                TryDeleteFile(ApplicationRuntime.LOGarqConvertidos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static void WriteTemporaryConfigurationPath()
        {
            string temporaryCommandFile = DrawingProcessPaths.TempCommandFile;
            TryDeleteFile(temporaryCommandFile);

            using (StreamWriter writer = new StreamWriter(temporaryCommandFile))
                writer.WriteLine(DrawingProcessPaths.GetConverterTxmlPath(parametros));
        }

        private static void ProcessDrawings(StreamWriter convertedFilesLog, List<string> failedFiles, ref bool convertedAnyFile)
        {
            int drawingIndex = 1;
            int drawingCount = parametros.desenhosName.Count();

            foreach (string file in parametros.desenhosName)
            {
                if (Processo.IsCanceled)
                    return;

                FileOpen = Path.GetFileName(file);
                bool convertedFile = RunCommand(file, drawingIndex == drawingCount);
                Valor = (int)((double)drawingIndex / drawingCount * 100);
                Index = drawingIndex++;

                if (convertedFile)
                {
                    convertedFilesLog.WriteLine(file);
                    convertedAnyFile = true;
                }
                else
                {
                    failedFiles.Add(file);
                }
            }
        }

        private static void ShowBatchFailures(List<string> failedFiles)
        {
            if (failedFiles.Count == 0)
                return;

            System.Windows.MessageBox.Show(
                "Não foi possível concluir a conversão de " + failedFiles.Count + " desenho(s). Os arquivos com falha não foram registrados como convertidos.",
                Localization.TitleWarningNoExclamation,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }

        private static void TryDeleteFile(string path)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);

                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
