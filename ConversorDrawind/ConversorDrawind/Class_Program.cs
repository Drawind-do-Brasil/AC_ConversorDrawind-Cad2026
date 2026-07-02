using System;
using System.Threading;
using System.Windows.Forms;

namespace ConversorDrawind
{
    static class Class_Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread newThread = new Thread(new ThreadStart(ThreadMethod));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();     
        }
        static void ThreadMethod()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_0_JanelaPrincipal());
        }
    }
}
