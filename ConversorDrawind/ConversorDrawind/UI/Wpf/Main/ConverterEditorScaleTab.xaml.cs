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
        public TextBox P1XTextBoxControl => P1XTextBox;
        public TextBox P1YTextBoxControl => P1YTextBox;
        public TextBox P1ZTextBoxControl => P1ZTextBox;
        public TextBox P2XTextBoxControl => P2XTextBox;
        public TextBox P2YTextBoxControl => P2YTextBox;
        public TextBox P2ZTextBoxControl => P2ZTextBox;
        public TextBox AP1XTextBoxControl => AP1XTextBox;
        public TextBox AP1YTextBoxControl => AP1YTextBox;
        public TextBox AP1ZTextBoxControl => AP1ZTextBox;
        public TextBox AP2XTextBoxControl => AP2XTextBox;
        public TextBox AP2YTextBoxControl => AP2YTextBox;
        public TextBox AP2ZTextBoxControl => AP2ZTextBox;
        public ComboBox ZoomLayerFilterComboBoxControl => ZoomLayerFilterComboBox;
        public TextBox ZoomTextSizeTextBoxControl => ZoomTextSizeTextBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void ScaleModeChanged(object sender, RoutedEventArgs e) => Forward(nameof(ScaleModeChanged), sender, e);
    }
}
