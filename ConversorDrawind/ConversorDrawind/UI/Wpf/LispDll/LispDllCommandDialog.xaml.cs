using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.LispDll
{
    public partial class LispDllCommandDialog : Window
    {
        private const string AutoCadAppsFilter = "Autocad Apps File|*.arx;*.lsp;*.dvb;*.dbx;*.vlx;*.fas;*.dll";

        public LispDllCommandDialog(string commandEntry = null)
        {
            InitializeComponent();
            LoadCommandEntry(commandEntry);
        }

        public string CommandEntry { get; private set; }

        private void LoadCommandEntry(string commandEntry)
        {
            if (string.IsNullOrWhiteSpace(commandEntry))
            {
                return;
            }

            LispDllCommandEntry entry = LispDllCommandEntry.Parse(commandEntry);
            CommandNameTextBox.Text = entry.Name;
            CommandPathTextBox.Text = entry.Path;
            RunOnlyAtEndCheckBox.IsChecked = entry.RunOnlyAtEnd;
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = AutoCadAppsFilter,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) == true)
            {
                CommandPathTextBox.Text = dialog.FileName;
            }
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            CommandEntry = BuildCommandEntry();
            DialogResult = true;
            Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(CommandNameTextBox.Text))
            {
                MessageBox.Show(this, "Comando inválido", Localization.AppTitle);
                return false;
            }

            string path = CommandPathTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(path) && !File.Exists(path))
            {
                MessageBox.Show(this, "Arquivo inválido", Localization.AppTitle);
                return false;
            }

            return true;
        }

        private string BuildCommandEntry()
        {
            return new LispDllCommandEntry
            {
                Name = CommandNameTextBox.Text,
                Path = CommandPathTextBox.Text,
                RunOnlyAtEnd = RunOnlyAtEndCheckBox.IsChecked == true
            }.ToCommandEntry();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
