using System.Windows;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow : Window
    {
        private readonly Form_0_JanelaPrincipal mainControl;

        public MainWindow()
        {
            InitializeComponent();
            mainControl = new Form_0_JanelaPrincipal();
            Host.Child = mainControl;
            Closed += MainWindowClosed;
        }

        private void MainWindowClosed(object sender, System.EventArgs e)
        {
            mainControl.Dispose();
        }
    }
}
