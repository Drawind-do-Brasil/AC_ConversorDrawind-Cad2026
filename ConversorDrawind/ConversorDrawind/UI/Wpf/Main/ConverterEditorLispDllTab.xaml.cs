using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorLispDllTab : UserControl
    {
        public ConverterEditorLispDllTab()
        {
            InitializeComponent();
        }

        public ListBox CommandsListBox => LispListBox;
        public TextBox CommandNameTextBox => LispCommandTextBox;
        public TextBox CommandPathTextBox => LispPathTextBox;
        public CheckBox RunOnlyAtEndCheckBoxControl => RunOnlyAtEndCheckBox;

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardSelection(string action, object sender, SelectionChangedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void LispListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(LispListBoxSelectionChanged), sender, e);
        private void BrowseLispPathClick(object sender, RoutedEventArgs e) => Forward(nameof(BrowseLispPathClick), sender, e);
        private void AddLispClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLispClick), sender, e);
        private void ModifyLispClick(object sender, RoutedEventArgs e) => Forward(nameof(ModifyLispClick), sender, e);
        private void DeleteLispClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteLispClick), sender, e);
        private void MoveLispUpClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLispUpClick), sender, e);
        private void MoveLispDownClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLispDownClick), sender, e);
    }
}
