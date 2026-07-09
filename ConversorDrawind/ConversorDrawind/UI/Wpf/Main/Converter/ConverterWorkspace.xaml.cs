using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterWorkspace : UserControl
    {
        public ConverterWorkspace()
        {
            InitializeComponent();
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;
        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddDrawingsClick(object sender, RoutedEventArgs e) => Forward(nameof(AddDrawingsClick), sender, e);
        private void ClearDrawingsClick(object sender, RoutedEventArgs e) => Forward(nameof(ClearDrawingsClick), sender, e);
        private void RestoreLastConvertedClick(object sender, RoutedEventArgs e) => Forward(nameof(RestoreLastConvertedClick), sender, e);
        private void StatusComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => Forward(nameof(StatusComboBoxSelectionChanged), sender, e);
        private void DrawingsDrop(object sender, DragEventArgs e) => Forward(nameof(DrawingsDrop), sender, e);
        private void DrawingsDragEnter(object sender, DragEventArgs e) => Forward(nameof(DrawingsDragEnter), sender, e);
        private void ConvertersSelectionChanged(object sender, SelectionChangedEventArgs e) => Forward(nameof(ConvertersSelectionChanged), sender, e);
        private void ConvertClick(object sender, RoutedEventArgs e) => Forward(nameof(ConvertClick), sender, e);
    }
}



