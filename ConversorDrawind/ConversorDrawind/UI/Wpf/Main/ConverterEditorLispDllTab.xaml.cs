using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorLispDllTab : UserControl
    {
        public ConverterEditorLispDllTab()
        {
            InitializeComponent();
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddLispClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLispClick), sender, e);
        private void AddDllClick(object sender, RoutedEventArgs e) => Forward(nameof(AddDllClick), sender, e);
    }
}
