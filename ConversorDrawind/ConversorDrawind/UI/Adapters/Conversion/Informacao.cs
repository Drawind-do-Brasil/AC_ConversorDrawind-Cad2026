using ConversorDrawind.UI.Wpf.Status;
using System;
using System.Windows.Threading;

namespace ConversorDrawind
{
    public class Informacao : IDisposable
    {
        private StatusWindow statusWindow;
        private bool disposed;

        public Informacao()
        {
            Show();
            SetTopLevelInfUser(true);
        }

        public bool TopLevel { get; private set; }

        public void Show()
        {
            if (statusWindow != null)
                return;

            statusWindow = new StatusWindow();
            statusWindow.Show();
            Update();
        }

        public void Update()
        {
            statusWindow?.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
        }

        public void SetTopLevelInfUser(bool valor)
        {
            TopLevel = valor;
            if (statusWindow != null)
            {
                statusWindow.Topmost = valor;
            }
        }

        public void AtualizarStatus(string status)
        {
            statusWindow?.UpdateStatus(status);
        }

        public void Close()
        {
            if (statusWindow == null)
                return;

            statusWindow.Close();
            statusWindow = null;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Close();
            disposed = true;
        }
    }
}



