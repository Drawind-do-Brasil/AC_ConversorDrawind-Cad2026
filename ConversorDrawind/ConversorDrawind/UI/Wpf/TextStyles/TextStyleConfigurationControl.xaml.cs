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
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();

        public TextStyleConfigurationControl()
        {
            InitializeComponent();
            TextStylesDataGrid.ItemsSource = textStyles;
        }

        public event EventHandler ConfigurationChanged;

        public void LoadConfiguration(global::ConversorDrawind.Configuration configuration)
        {
            this.configuration = configuration ?? new global::ConversorDrawind.Configuration();
            this.configuration.EnsureDefaults();
            LoadRows();
        }

        public bool ApplyRowsToConfiguration()
        {
            configuration.Text.Styles.Clear();
            foreach (TextStyleRow row in textStyles)
            {
                configuration.Text.Styles.Add(row.ToDefinition());
            }

            if (configuration.Text.Styles.Count == 0)
            {
                configuration.Text.Styles.Add(DefaultTextStyle());
            }

            NotifyConfigurationChanged();
            return true;
        }

        private void LoadRows()
        {
            textStyles.Clear();
            if (configuration.Text.Styles.Count == 0)
            {
                configuration.Text.Styles.Add(DefaultTextStyle());
            }

            foreach (TextStyleDefinition textStyle in configuration.Text.Styles)
            {
                textStyles.Add(TextStyleRow.FromDefinition(textStyle));
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
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
            ApplyRowsToConfiguration();
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
                ApplyRowsToConfiguration();
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
                ApplyRowsToConfiguration();
            }
        }

        private void NotifyConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        private static TextStyleDefinition DefaultTextStyle()
        {
            return new TextStyleDefinition
            {
                Name = "TEXTO",
                Font = "RomanS",
                Italic = false,
                Bold = false,
                Size = 2.5,
                WidthFactor = 1,
                ObliqueAngle = 0
            };
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

            public static TextStyleRow FromDefinition(TextStyleDefinition definition)
            {
                return FromValues(LegacyConfigurationParsers.FormatTextStyleDefinition(definition).Split(':'));
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

            public TextStyleDefinition ToDefinition()
            {
                return LegacyConfigurationParsers.ParseTextStyleDefinition(string.Join(":", ToValues()));
            }
        }
    }
}
