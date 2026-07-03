using ConversorDrawind.UI.Wpf.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class LayerColorDialog : Window
    {
        private readonly Arranjos arranjos;
        private readonly List<string> colors;

        public LayerColorDialog(string currentColor, Arranjos arranjos)
        {
            InitializeComponent();

            this.arranjos = arranjos;
            Color = currentColor;
            colors = BuildColorList(arranjos);

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

            if (!arranjos.allcolor.Contains(dialog.ColorValue))
            {
                arranjos.allcolor.Add(dialog.ColorValue);
                colors.Add(dialog.ColorValue);
                ColorComboBox.Items.Refresh();
            }

            ColorComboBox.Text = dialog.ColorValue;
        }

        private static List<string> BuildColorList(Arranjos arranjos)
        {
            List<string> availableColors = arranjos.allcolor.ToList();

            if (arranjos.allcolor.Count > 0)
            {
                availableColors.Remove(arranjos.allcolor.First());
            }

            availableColors.Remove(arranjos.lineTypeRemove.First());
            availableColors.Remove(arranjos.lineTypeRemove.Last());

            return availableColors;
        }
    }
}



