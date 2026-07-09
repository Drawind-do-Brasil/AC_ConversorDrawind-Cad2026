using ConversorDrawind.UI.Wpf.Conversion;
using System;

namespace ConversorDrawind
{
    public class Processo : IDisposable
    {
        private static bool isCanceled = false;
        public static string tempo = "";

        private readonly ConversionProgressWindow window;

        public Processo(Param1 p)
        {
            isCanceled = false;
            window = new ConversionProgressWindow(p);
        }

        public static bool IsCanceled
        {
            get { return isCanceled; }
        }

        internal static void SetCanceled(bool value)
        {
            isCanceled = value;
        }

        internal static void SetTempo(string value)
        {
            tempo = value;
        }

        public void SetTopLevelInfUser(bool valor)
        {
        }

        public void Update()
        {
        }

        public UiDialogResult ShowDialog()
        {
            bool? result = window.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



