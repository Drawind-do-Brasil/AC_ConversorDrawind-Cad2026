using ConversorDrawind.UI.Wpf.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class NewLayerDialog : Window
    {
        private readonly global::ConversorDrawind.Configuration configuration;
        private readonly NewLayer newLayer;
        private readonly List<string> layers;
        private readonly List<string> colors;
        private readonly List<string> lineTypes;
        private readonly List<string> textStyles;

        public NewLayerDialog(NewLayer newLayer, global::ConversorDrawind.Configuration configuration)
        {
            InitializeComponent();

            this.newLayer = newLayer;
            this.configuration = configuration ?? new global::ConversorDrawind.Configuration();
            this.configuration.EnsureDefaults();
            layers = this.configuration.Layers.NewLayers.Select(layer => layer.Name).ToList();
            colors = BuildColorList(this.configuration.Catalogs);
            lineTypes = this.configuration.Catalogs.LayerLineTypes.ToList();
            textStyles = this.configuration.Text.Styles.Select(textStyle => textStyle.Name).ToList();

            LayerComboBox.ItemsSource = layers;
            ColorComboBox.ItemsSource = colors;
            LineTypeComboBox.ItemsSource = lineTypes;
            TextStyleComboBox.ItemsSource = textStyles;

            LoadCurrentValues();
        }

        public NewLayer NewLayer => newLayer;

        private void LoadCurrentValues()
        {
            LayerComboBox.Text = newLayer.layer;
            ColorComboBox.Text = newLayer.cor;
            TextHeightTextBox.Text = newLayer.alturaTexto;
            TextWidthTextBox.Text = newLayer.larguraTexto;
            TextStyleComboBox.Text = newLayer.estiloTexto;

            foreach (string lineType in lineTypes)
            {
                if (lineType.Split(',').First().ToUpper() == newLayer.tipoLinha)
                {
                    LineTypeComboBox.Text = lineType;
                    break;
                }
            }
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (layers.Contains(LayerComboBox.Text))
            {
                newLayer.layer = LayerComboBox.Text;
            }

            if (colors.Contains(ColorComboBox.Text))
            {
                newLayer.cor = ColorComboBox.Text;
            }

            if (lineTypes.Contains(LineTypeComboBox.Text))
            {
                newLayer.tipoLinha = LineTypeComboBox.Text.Split(',').First().ToUpper();
            }

            if (textStyles.Contains(TextStyleComboBox.Text))
            {
                newLayer.estiloTexto = TextStyleComboBox.Text;
            }

            newLayer.alturaTexto = TextHeightTextBox.Text;
            newLayer.larguraTexto = TextWidthTextBox.Text;
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OtherColorsButtonClick(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog dialog = new ColorPickerDialog
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (!configuration.Catalogs.Colors.Contains(dialog.ColorValue))
            {
                configuration.Catalogs.Colors.Add(dialog.ColorValue);
                colors.Add(dialog.ColorValue);
                ColorComboBox.Items.Refresh();
            }

            ColorComboBox.Text = dialog.ColorValue;
        }

        private void DecimalTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, e.Text);

            e.Handled = !IsDecimalLikeText(proposed);
        }

        private static bool IsDecimalLikeText(string text)
        {
            return text.All(character => char.IsDigit(character) || character == ',')
                && text.Count(character => character == ',') <= 1;
        }

        private static List<string> BuildColorList(CatalogConfiguration catalogs)
        {
            List<string> availableColors = catalogs.Colors.ToList();

            if (catalogs.Colors.Count > 0)
            {
                availableColors.Remove(catalogs.Colors.First());
            }

            return availableColors;
        }
    }
}



