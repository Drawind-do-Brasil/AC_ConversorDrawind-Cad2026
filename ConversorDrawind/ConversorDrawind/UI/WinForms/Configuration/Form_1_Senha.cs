using ConversorDrawind.UI.Wpf.Configuration;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_1_Senha : IDisposable
    {
        public bool ok = false;

        public bool CheckSerial()
        {
            return new LicenseDialog().CheckSerial();
        }

        public DialogResult ShowDialog()
        {
            LicenseDialog dialog = new LicenseDialog();
            bool? result = dialog.ShowDialog();
            ok = dialog.Ok;

            if (!ok)
            {
                Application.Exit();
            }

            return result == true || ok ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
