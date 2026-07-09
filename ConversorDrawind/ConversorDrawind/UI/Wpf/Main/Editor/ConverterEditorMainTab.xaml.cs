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
        public CheckBox ConvertDimensionsCheckBoxControl => ConvertDimensionsCheckBox;
        public CheckBox ConvertLayersCheckBoxControl => ConvertLayersCheckBox;
        public CheckBox ScaleDrawingCheckBoxControl => ScaleDrawingCheckBox;
        public CheckBox AttributeFormatCheckBoxControl => AttributeFormatCheckBox;
        public CheckBox RunCommandsCheckBoxControl => RunCommandsCheckBox;
        public CheckBox ShowErrorMessagesCheckBoxControl => ShowErrorMessagesCheckBox;
        public CheckBox DeleteTeklaCheckBoxControl => DeleteTeklaCheckBox;
        public CheckBox PurgeCheckBoxControl => PurgeCheckBox;
        public CheckBox ExplodeCheckBoxControl => ExplodeCheckBox;
        public CheckBox ExplodeBlocksCheckBoxControl => ExplodeBlocksCheckBox;
        public CheckBox DmBlockCheckBoxControl => DmBlockCheckBox;
        public RadioButton TeklaOriginRadioControl => TeklaOriginRadio;
        public RadioButton CadOriginRadioControl => CadOriginRadio;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void FeatureCheckChanged(object sender, RoutedEventArgs e) => Forward(nameof(FeatureCheckChanged), sender, e);
        private void OriginChanged(object sender, RoutedEventArgs e) => Forward(nameof(OriginChanged), sender, e);
    }
}
