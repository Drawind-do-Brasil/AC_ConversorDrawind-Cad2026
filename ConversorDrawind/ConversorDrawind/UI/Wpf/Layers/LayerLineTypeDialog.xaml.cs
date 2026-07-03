using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerLineTypeDialog : Window
    {
        private readonly List<string> lineTypes;

        public LayerLineTypeDialog(string currentLineType, Arranjos arranjos)
        {
            InitializeComponent();

            lineTypes = arranjos.allLineType2
                .Where(lineType => lineType != arranjos.lineTypeRemove.First() && lineType != arranjos.lineTypeRemove.Last())
                .ToList();

            LineType = lineTypes.FirstOrDefault(lineType => lineType.Split(',').First().ToUpper() == currentLineType);
            LineTypeComboBox.ItemsSource = lineTypes;
            LineTypeComboBox.Text = LineType;
        }

        public string LineType { get; private set; }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (lineTypes.Contains(LineTypeComboBox.Text))
            {
                LineType = LineTypeComboBox.Text;
            }

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}



