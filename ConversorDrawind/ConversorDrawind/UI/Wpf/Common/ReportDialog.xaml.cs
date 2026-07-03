using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Common
{
    public partial class ReportDialog : Window
    {
        private readonly List<string> items;

        public ReportDialog(List<string> items, string message)
        {
            InitializeComponent();
            this.items = items;
            MessageTextBlock.Text = message;
            ItemsListBox.ItemsSource = items;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Texto (*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog(this) != true || saveFileDialog.FileName == string.Empty)
            {
                return;
            }

            if (File.Exists(saveFileDialog.FileName))
            {
                File.Delete(saveFileDialog.FileName);
            }

            using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, true))
            {
                foreach (string item in items)
                {
                    streamWriter.WriteLine(item);
                }
            }
        }
    }
}
