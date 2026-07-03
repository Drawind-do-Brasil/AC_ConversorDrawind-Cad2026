using ConversorDrawind;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Configuration
{
    public partial class LicenseDialog : Window
    {
        private readonly string senhaCP = string.Empty;
        private readonly string local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConversorDrawind", "access.lic");
        private readonly string localLegacy = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConversorDrawind", "Acess.dll");

        public LicenseDialog()
        {
            InitializeComponent();
        }

        public bool Ok { get; private set; }

        public bool CheckSerial()
        {
            if (Directory.Exists(@"\\192.168.7.10\xdraw$\Aplicativos\ConversorDrawind"))
            {
                return true;
            }

            return false;
        }

        private void ActivateButtonClick(object sender, RoutedEventArgs e)
        {
            if (SerialTextBox.Text != senhaCP)
            {
                ShowInvalidPassword();
                return;
            }

            string tc = string.Empty;
            try
            {
                string readPath = File.Exists(local) ? local : localLegacy;
                if (File.Exists(readPath))
                {
                    using (StreamReader sr = new StreamReader(readPath))
                    {
                        sr.ReadLine();
                        tc = sr.ReadLine();
                    }
                }
            }
            catch (Exception)
            {
            }

            using (StreamWriter sw = new StreamWriter(local))
            {
                sw.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(SerialTextBox.Text)));
                sw.WriteLine(tc);
            }

            if (CheckSerial())
            {
                Ok = true;
                Close();
                System.Windows.MessageBox.Show(
                    Localization.MessageActivationSuccess,
                    Localization.TitleAttentionPlain,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            ShowInvalidPassword();
        }

        private static void ShowInvalidPassword()
        {
            System.Windows.MessageBox.Show(
                Localization.MessageInvalidPassword,
                Localization.TitleAttentionPlain,
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
