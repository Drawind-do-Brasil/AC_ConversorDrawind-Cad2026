using Autodesk.AutoCAD.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ConversorDrawind
{
   public  static class LoadFiles
    {
        private static bool _RunCommand = false;

        public  static void LoadFile(string file, AcadDocument acadDocument)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    ComRetry.Invoke(() => acadDocument.SetVariable("FILEDIA", 0));
                    try
                    {
                        string ext = "";
                        if (!String.IsNullOrEmpty(file))
                        {
                            ext = Path.GetExtension(file);
                            if (ext.ToUpper() == ".DLL")
                               SendCommand("NETLOAD " + file + "\n", acadDocument);
                            else
                                SendCommand("(load  \"" + file.Replace("\\", "\\\\") + "\")\n", acadDocument);
                        }
                    }
                    finally
                    {
                        ComRetry.Invoke(() => acadDocument.SetVariable("FILEDIA", 1));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static void SendCommand(string CommandName, AcadDocument acadDocument)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    _RunCommand = true;

                    ComRetry.Invoke(() => acadDocument.SendCommand(CommandName));
                    WaitCommandFinished(CommandName, acadDocument);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private const int CommandTimeoutMs = 900000;
        private const int CommandPollMs = 50;
        private static void WaitCommandFinished(string commandName, AcadDocument acadDocument)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int idleReadCount = 0;

            while (sw.ElapsedMilliseconds < CommandTimeoutMs)
            {
                if (GetCommandActive(acadDocument) == 0)
                {
                    idleReadCount++;

                    if (idleReadCount >= 3)
                    {
                        _RunCommand = false;
                        return;
                    }
                }
                else
                {
                    idleReadCount = 0;
                }

                Thread.Sleep(CommandPollMs);
            }

            throw new TimeoutException("Tempo limite aguardando execução do comando: " + commandName);
        }
        private static int GetCommandActive(AcadDocument acadDocument)
        {
            try
            {
                if (acadDocument == null)
                    return 0;

                object cmdActive = ComRetry.Invoke(() => acadDocument.GetVariable("CMDACTIVE"));
                return Convert.ToInt32(cmdActive);
            }
            catch (Exception)
            {
                return _RunCommand ? 1 : 0;
            }
        }


    }
}




