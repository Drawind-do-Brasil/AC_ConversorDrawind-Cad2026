using ConversorDrawind.UI.Wpf.Configuration;
using System;
namespace ConversorDrawind
{
    public sealed class Senha : IDisposable
    {
        public bool ok = false;

        public bool CheckSerial()
        {
            return new LicenseDialog().CheckSerial();
        }

        public UiDialogResult ShowDialog()
        {
            LicenseDialog dialog = new LicenseDialog();
            bool? result = dialog.ShowDialog();
            ok = dialog.Ok;

            if (!ok)
            {
                System.Windows.Application.Current?.Shutdown();
            }

            return result == true || ok ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}




