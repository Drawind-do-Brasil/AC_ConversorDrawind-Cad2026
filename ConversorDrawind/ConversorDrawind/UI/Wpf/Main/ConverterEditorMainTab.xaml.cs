using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorMainTab : UserControl
    {
        public ConverterEditorMainTab()
        {
            InitializeComponent();
        }

        public TextBox CommentsTextBox => AddCommentsTextBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void FeatureCheckChanged(object sender, RoutedEventArgs e) => Forward(nameof(FeatureCheckChanged), sender, e);
        private void OriginChanged(object sender, RoutedEventArgs e) => Forward(nameof(OriginChanged), sender, e);
    }
}
