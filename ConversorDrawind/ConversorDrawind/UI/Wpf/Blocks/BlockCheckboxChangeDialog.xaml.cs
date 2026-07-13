using System.Windows;

namespace ConversorDrawind.UI.Wpf.Blocks
{
    public partial class BlockCheckboxChangeDialog : Window
    {
        public BlockCheckboxChangeDialog(bool modificar)
        {
            InitializeComponent();
            ModifyComboBox.SelectedIndex = modificar ? 0 : 1;
        }

        public bool Modificar { get; private set; } = true;

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Modificar = ModifyComboBox.SelectedIndex == 0;
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}



