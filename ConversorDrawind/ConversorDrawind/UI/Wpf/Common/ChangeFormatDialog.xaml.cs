using System.Windows;

namespace ConversorDrawind.UI.Wpf.Common
{
    public partial class ChangeFormatDialog : Window
    {
        public ChangeFormatDialog(string message, string title)
        {
            InitializeComponent();
            Title = title;
            MessageTextBlock.Text = message;
            ButtonId = "0";
        }

        public string ButtonId { get; private set; }

        private void ChangePathButtonClick(object sender, RoutedEventArgs e)
        {
            ButtonId = "1";
            Close();
        }

        private void UpdateBlocksButtonClick(object sender, RoutedEventArgs e)
        {
            ButtonId = "2";
            Close();
        }
    }
}



