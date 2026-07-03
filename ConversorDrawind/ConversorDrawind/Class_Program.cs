using System;
using System.Threading;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Main;

namespace ConversorDrawind
{
    static class Class_Program
    {
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
            System.Windows.Application application = new System.Windows.Application();
            application.Run(new MainWindow());
        }
    }
}
