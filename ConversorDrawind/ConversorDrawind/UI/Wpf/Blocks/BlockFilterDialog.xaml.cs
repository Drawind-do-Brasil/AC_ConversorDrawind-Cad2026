using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Blocks
{
    public partial class BlockFilterDialog : Window
    {
        private readonly Arranjos arranjos;

        public BlockFilterDialog(Arranjos arranjos, Filter filter)
        {
            InitializeComponent();
            this.arranjos = arranjos;
            Filter = new Filter(filter);
            LoadControls();
            LoadConfiguration();
            DataObject.AddPastingHandler(HeightTextBox, HeightPasteHandler);
        }

        public Filter Filter { get; private set; }

        private void LoadControls()
        {
            LayerComboBox.Items.Clear();
            LayerComboBox.Items.Add("ALL");
            foreach (string layer in arranjos.allNewLayer.Concat(arranjos.allBaseLayer))
            {
                LayerComboBox.Items.Add(layer);
            }

            ColorComboBox.Items.Clear();
            foreach (string color in arranjos.allcolor)
            {
                ColorComboBox.Items.Add(color);
            }
        }

        private void LoadConfiguration()
        {
            LayerComboBox.Text = Filter.layerBase;
            ColorComboBox.Text = Filter.cor;
            HeightTextBox.Text = Filter.alturaTexto;
            ContentTextBox.Text = Filter.conteudoTexto;
        }

        private void OtherColorClick(object sender, RoutedEventArgs e)
        {
            GenericNewColor newColor = new GenericNewColor(ColorComboBox.Text);
            newColor.ShowDialog();
            if (!arranjos.allcolor.Contains(newColor.colorClass))
            {
                arranjos.allcolor.Add(newColor.colorClass);
                ColorComboBox.Items.Add(newColor.colorClass);
            }

            ColorComboBox.Text = newColor.colorClass;
            newColor.Dispose();
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            if (ColorComboBox.Items.Contains(ColorComboBox.Text))
            {
                Filter.cor = ColorComboBox.Text;
            }

            Filter.alturaTexto = HeightTextBox.Text;
            Filter.tipoObjeto = "TEXT";

            if (LayerComboBox.Items.Contains(LayerComboBox.Text))
            {
                Filter.layerBase = LayerComboBox.Text;
            }

            Filter.conteudoTexto = ContentTextBox.Text;
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void HeightPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsAllowedText(e.Text);
        }

        private void HeightPasteHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string text = e.DataObject.GetData(DataFormats.Text)?.ToString() ?? string.Empty;
            if (!IsAllowedPaste(text))
            {
                e.CancelCommand();
            }
        }

        private bool IsAllowedText(string text)
        {
            foreach (char character in text)
            {
                if (!char.IsDigit(character) && character != ',')
                {
                    return false;
                }
            }

            return text != "," || !HeightTextBox.Text.Contains(",");
        }

        private bool IsAllowedPaste(string text)
        {
            int commaCount = HeightTextBox.Text.Contains(",") ? 1 : 0;
            foreach (char character in text)
            {
                if (char.IsDigit(character))
                {
                    continue;
                }

                if (character == ',' && commaCount == 0)
                {
                    commaCount++;
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}



