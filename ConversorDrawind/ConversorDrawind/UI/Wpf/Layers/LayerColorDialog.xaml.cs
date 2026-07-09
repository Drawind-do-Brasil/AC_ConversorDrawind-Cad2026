using ConversorDrawind.UI.Wpf.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerColorDialog : Window
    {
        private readonly CatalogConfiguration catalogs;
        private readonly List<string> colors;

        public LayerColorDialog(string currentColor, CatalogConfiguration catalogs)
        {
            InitializeComponent();

            this.catalogs = catalogs ?? new global::ConversorDrawind.Configuration().Catalogs;
            Color = currentColor;
            colors = BuildColorList(this.catalogs);

            ColorComboBox.ItemsSource = colors;
            ColorComboBox.Text = currentColor;
        }

        public string Color { get; private set; }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (colors.Contains(ColorComboBox.Text))
            {
                Color = ColorComboBox.Text;
            }

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OtherColorButtonClick(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog dialog = new ColorPickerDialog
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (!catalogs.Colors.Contains(dialog.ColorValue))
            {
                catalogs.Colors.Add(dialog.ColorValue);
                colors.Add(dialog.ColorValue);
                ColorComboBox.Items.Refresh();
            }

            ColorComboBox.Text = dialog.ColorValue;
        }

        private static List<string> BuildColorList(CatalogConfiguration catalogs)
        {
            List<string> availableColors = catalogs.Colors.ToList();

            if (catalogs.Colors.Count > 0)
            {
                availableColors.Remove(catalogs.Colors.First());
            }

            foreach (string removedLineType in catalogs.RemovedLineTypes)
            {
                availableColors.Remove(removedLineType);
            }

            return availableColors;
        }
    }
}



