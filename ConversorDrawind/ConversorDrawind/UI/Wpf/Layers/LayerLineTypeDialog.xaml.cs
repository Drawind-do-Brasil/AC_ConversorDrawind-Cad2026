using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerLineTypeDialog : Window
    {
        private readonly List<string> lineTypes;

        public LayerLineTypeDialog(string currentLineType, CatalogConfiguration catalogs)
        {
            InitializeComponent();

            catalogs = catalogs ?? new global::ConversorDrawind.Configuration().Catalogs;
            lineTypes = catalogs.LayerLineTypes
                .Where(lineType => !catalogs.RemovedLineTypes.Contains(lineType))
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



