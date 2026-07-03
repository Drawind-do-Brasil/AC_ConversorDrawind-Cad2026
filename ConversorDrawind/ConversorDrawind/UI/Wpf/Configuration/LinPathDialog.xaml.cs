using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ConversorDrawind.UI.Wpf.Configuration
{
    public partial class LinPathDialog : Window
    {
        public LinPathDialog(string file)
        {
            InitializeComponent();
            SelectedFile = file;
            PathTextBox.Text = file;
        }

        public string SelectedFile { get; private set; }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                FileName = "acad.lin",
                Filter = "Arquivo Lin (*.lin)|*.lin",
                Multiselect = false
            };

            if (dialog.ShowDialog(this) == true)
            {
                SelectedFile = dialog.FileName;
                PathTextBox.Text = dialog.FileName;
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile = PathTextBox.Text;
            if (File.Exists(SelectedFile))
            {
                DialogResult = true;
                Close();
                return;
            }

            MessageBox.Show(
                this,
                "Por favor, especifique o arquivo correto.",
                "Atencao",
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
