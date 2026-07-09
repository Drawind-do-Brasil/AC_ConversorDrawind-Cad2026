using System;
using System.Globalization;
using System.Windows.Data;

namespace ConversorDrawind.UI.Wpf.Main.ViewModels
{
    public sealed class SourceModeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mode = value is int intValue ? intValue : 0;
            int expected = ReadExpectedMode(parameter);
            return mode == expected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return ReadExpectedMode(parameter);
            }

            return Binding.DoNothing;
        }

        private static int ReadExpectedMode(object parameter)
        {
            return int.TryParse(parameter?.ToString(), out int result) ? result : 0;
        }
    }
}
