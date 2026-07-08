using ConversorDrawind;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.TextStyles
{
    public partial class TextStyleConfigurationControl : UserControl
    {
        private readonly ObservableCollection<TextStyleRow> textStyles = new ObservableCollection<TextStyleRow>();
        private Arranjos arranjos;

        public TextStyleConfigurationControl()
        {
            InitializeComponent();
            TextStylesDataGrid.ItemsSource = textStyles;
        }

        public TextStyleConfigurationControl(Arranjos arranjos)
            : this()
        {
            LoadArranjos(arranjos);
        }

        public event EventHandler ConfigurationChanged;

        public void LoadArranjos(Arranjos arranjos)
        {
            this.arranjos = arranjos;
            LoadRows();
        }

        public bool ApplyRowsToArranjos()
        {
            if (arranjos == null)
            {
                return false;
            }

            arranjos.allTextSyles.Clear();
            foreach (TextStyleRow row in textStyles)
            {
                arranjos.allTextSyles.Add(row.ToConjunto());
            }

            if (arranjos.allTextSyles.Count == 0)
            {
                arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);
            }

            NotifyConfigurationChanged();
            return true;
        }

        private void LoadRows()
        {
            textStyles.Clear();
            if (arranjos == null)
            {
                return;
            }

            if (arranjos.allTextSyles.Count == 0)
            {
                arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);
            }

            foreach (string textStyle in arranjos.allTextSyles)
            {
                string[] values = textStyle.Split(':');
                if (values.Length >= 7)
                {
                    textStyles.Add(TextStyleRow.FromValues(values));
                }
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (arranjos == null)
            {
                return;
            }

            TextStyleDialog dialog = new TextStyleDialog(new[] { string.Empty, "RomanS", bool.FalseString, bool.FalseString, "2.5", "1", "0" })
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (textStyles.Any(row => row.Nome.ToUpper() == dialog.Values[0].ToUpper()))
            {
                MessageBox.Show(
                    Localization.MessageCannotAddStyle,
                    Localization.TitleWarningNoExclamation,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            textStyles.Add(TextStyleRow.FromValues(dialog.Values));
            ApplyRowsToArranjos();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (textStyles.Count == 1)
            {
                MessageBox.Show(
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

            if (MessageBox.Show(
                    Localization.MessageDeleteSelectedRow,
                    Localization.TitleAttentionPlain,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                textStyles.Remove(selectedRow);
                ApplyRowsToArranjos();
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
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                selectedRow.ApplyValues(dialog.Values);
                TextStylesDataGrid.Items.Refresh();
                ApplyRowsToArranjos();
            }
        }

        private void NotifyConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
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
