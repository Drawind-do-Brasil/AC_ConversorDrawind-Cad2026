using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorScaleTab : UserControl
    {
        public ConverterEditorScaleTab()
        {
            InitializeComponent();
        }

        public RadioButton ManualRadio => ManualScaleRadio;
        public RadioButton AutoRadio => AutoScaleRadio;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void ScaleModeChanged(object sender, RoutedEventArgs e) => Forward(nameof(ScaleModeChanged), sender, e);
    }
}
