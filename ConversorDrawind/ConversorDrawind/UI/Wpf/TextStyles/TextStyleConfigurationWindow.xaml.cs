using ConversorDrawind;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.TextStyles
{
    public partial class TextStyleConfigurationWindow : Window
    {
        private readonly Arranjos arranjos;
        private readonly ObservableCollection<TextStyleRow> textStyles = new ObservableCollection<TextStyleRow>();

        public TextStyleConfigurationWindow(Arranjos arranjos)
        {
            InitializeComponent();
            this.arranjos = arranjos;
            LoadRows();
            TextStylesDataGrid.ItemsSource = textStyles;
        }

        private void LoadRows()
        {
            textStyles.Clear();
            foreach (string textStyle in arranjos.allTextSyles)
            {
                string[] values = textStyle.Split(':');
                textStyles.Add(TextStyleRow.FromValues(values));
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            TextStyleDialog dialog = new TextStyleDialog(new[] { string.Empty, "RomanS", bool.FalseString, bool.FalseString, "2.5", "1", "0" })
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (textStyles.Any(row => row.Nome.ToUpper() == dialog.Values[0].ToUpper()))
            {
                System.Windows.MessageBox.Show(
                    Localization.MessageCannotAddStyle,
                    Localization.TitleWarningNoExclamation,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            textStyles.Add(TextStyleRow.FromValues(dialog.Values));
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (textStyles.Count == 1)
            {
                System.Windows.MessageBox.Show(
                    Localization.MessageCannotRemoveStyle,
                    Localization.TitleWarningNoExclamation,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (TextStylesDataGrid.SelectedItem is not TextStyleRow selectedRow)
            {
                return;
            }

            if (System.Windows.MessageBox.Show(
                    Localization.MessageDeleteSelectedRow,
                    Localization.TitleAttentionPlain,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                textStyles.Remove(selectedRow);
            }
        }

        private void TextStylesDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TextStylesDataGrid.SelectedItem is not TextStyleRow selectedRow)
            {
                return;
            }

            TextStyleDialog dialog = new TextStyleDialog(selectedRow.ToValues())
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                selectedRow.ApplyValues(dialog.Values);
                TextStylesDataGrid.Items.Refresh();
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            arranjos.allTextSyles.Clear();
            foreach (TextStyleRow row in textStyles)
            {
                arranjos.allTextSyles.Add(row.ToConjunto());
            }

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private sealed class TextStyleRow
        {
            public string Nome { get; set; }
            public string Fonte { get; set; }
            public string Oblique { get; set; }
            public string Negrito { get; set; }
            public string Tamanho { get; set; }
            public string WidthFactor { get; set; }
            public string Angulo { get; set; }

            public static TextStyleRow FromValues(string[] values)
            {
                return new TextStyleRow
                {
                    Nome = values[0],
                    Fonte = values[1],
                    Oblique = values[2],
                    Negrito = values[3],
                    Tamanho = values[4],
                    WidthFactor = values[5],
                    Angulo = values[6]
                };
            }

            public void ApplyValues(string[] values)
            {
                Nome = values[0];
                Fonte = values[1];
                Oblique = values[2];
                Negrito = values[3];
                Tamanho = values[4];
                WidthFactor = values[5];
                Angulo = values[6];
            }

            public string[] ToValues()
            {
                return new[] { Nome, Fonte, Oblique, Negrito, Tamanho, WidthFactor, Angulo };
            }

            public string ToConjunto()
            {
                return string.Join(":", ToValues());
            }
        }
    }
}
