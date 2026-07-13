using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Common
{
    internal static class WpfApplicationStyles
    {
        public const double StandardInputHeight = 23;

        public static void Register(Application application)
        {
            if (application == null)
            {
                return;
            }

            application.Resources[typeof(TextBox)] = CreateTextBoxStyle();
            application.Resources[typeof(ComboBox)] = CreateComboBoxStyle();
            application.Resources[typeof(Button)] = CreateButtonStyle();
        }

        private static Style CreateTextBoxStyle()
        {
            Style style = new Style(typeof(TextBox));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, StandardInputHeight));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            return style;
        }

        private static Style CreateComboBoxStyle()
        {
            Style style = new Style(typeof(ComboBox));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, StandardInputHeight));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            return style;
        }

        private static Style CreateButtonStyle()
        {
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(10, 2, 10, 2)));
            return style;
        }
    }
}
