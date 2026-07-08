using System;
using System.Threading;
using ConversorDrawind.UI.Wpf.Common;
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
            System.Windows.Application application = new System.Windows.Application();
            WpfApplicationStyles.Register(application);
            application.Run(new MainWindow());
        }
    }
}


