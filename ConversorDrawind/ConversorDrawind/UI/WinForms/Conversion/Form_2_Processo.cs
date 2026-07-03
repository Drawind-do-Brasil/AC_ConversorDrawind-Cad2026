using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Conversion;

namespace ConversorDrawind
{
    public class Form_2_Processo : IDisposable
    {
        private static bool isCanceled = false;
        public static string tempo = "";

        private readonly ConversionProgressWindow window;

        public Form_2_Processo(Param1 p)
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

        public DialogResult ShowDialog()
        {
            bool? result = window.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
