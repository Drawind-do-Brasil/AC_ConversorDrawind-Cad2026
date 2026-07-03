using System.Windows;

namespace ConversorDrawind.UI.Wpf.Status
{
    public partial class StatusWindow : Window
    {
        public StatusWindow()
        {
            InitializeComponent();
            VersionText.Text = Properties.Settings.Default.Versao;
        }

        public void UpdateStatus(string status)
        {
            StatusText.Text = status;
        }
    }
}



