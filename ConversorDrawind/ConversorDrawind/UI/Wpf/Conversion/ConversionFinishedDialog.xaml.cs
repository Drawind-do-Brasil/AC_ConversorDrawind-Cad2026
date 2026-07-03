using System.Windows;

namespace ConversorDrawind.UI.Wpf.Conversion
{
    public partial class ConversionFinishedDialog : Window
    {
        public ConversionFinishedDialog(string conversionTime)
        {
            InitializeComponent();
            ProcessTimeTextBlock.Text = "Tempo de Conversão: " + conversionTime;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
