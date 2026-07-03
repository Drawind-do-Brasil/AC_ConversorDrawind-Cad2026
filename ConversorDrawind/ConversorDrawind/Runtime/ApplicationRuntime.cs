using System;
using System.IO;
using System.Threading;

namespace ConversorDrawind
{
    public static class ApplicationRuntime
    {
        public static string ExtensaoGeral = "DWG";
        public static string LOGdirConvertidos = Path.Combine(Path.GetTempPath(), "ConversorDrawindTemp");
        public static string LOGarqConvertidos = Path.Combine(LOGdirConvertidos, "lastconvertedfiles.log");

        private static volatile bool controladorT = true;
        private static volatile bool controladorT2 = true;

        public static bool ControladorT { get => controladorT; set => controladorT = value; }
        public static bool ControladorT2 { get => controladorT2; set => controladorT2 = value; }

        public static double[] GetPoint(double[] ponto) => ponto;

        public static void CloseAllInstance()
        {
            try
            {
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("acad"))
                {
                    process.Kill();
                }
            }
            catch
            {
            }
        }

        public static void StopStatusThread(Thread thread)
        {
            try
            {
                ControladorT = false;
                ControladorT2 = false;
                if (thread != null && thread.IsAlive)
                {
                    thread.Join(1000);
                }
            }
            catch
            {
            }
            finally
            {
                ControladorT = true;
                ControladorT2 = true;
            }
        }

        public static void ThreadMethodAnalisando()
        {
            Informacao info = new Informacao();
            info.AtualizarStatus("Analisando");
            info.SetTopLevelInfUser(true);
            info.Show();
            while (ControladorT)
            {
                info.Update();
                Thread.Sleep(100);
            }
            info.Close();
        }

        public static void ThreadMethodAbrindoCad()
        {
            Informacao info = new Informacao();
            info.AtualizarStatus("Abrindo CAD");
            info.SetTopLevelInfUser(true);
            info.Show();
            while (ControladorT2)
            {
                info.Update();
                Thread.Sleep(100);
            }
            info.Close();
        }
    }
}




