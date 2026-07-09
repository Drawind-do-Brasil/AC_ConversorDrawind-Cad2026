using ConversorDrawind.UI.Wpf.LispDll;
using ConversorDrawind.UI.Wpf.Main.Rows;
using System.Collections.Generic;
using System.Linq;
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

        public DataGrid CommandsListBox => LispCommandsGrid;

        public void ShowCommands(IEnumerable<string> commands)
        {
            LispCommandsGrid.ItemsSource = (commands ?? Enumerable.Empty<string>())
                .Select(LispCommandRow.FromCommandEntry)
                .ToList();
        }

        public void AddLispCommand(global::ConversorDrawind.Configuration configuration, Window owner)
        {
            LispDllCommandDialog dialog = new LispDllCommandDialog
            {
                Owner = owner
            };

            if (dialog.ShowDialog() == true)
            {
                configuration.Commands.LispCommands.Add(dialog.CommandEntry);
                ShowCommands(configuration.Commands.LispCommands);
                LispCommandsGrid.SelectedIndex = configuration.Commands.LispCommands.Count - 1;
            }
        }

        public void ModifyLispCommand(global::ConversorDrawind.Configuration configuration, Window owner)
        {
            int index = LispCommandsGrid.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            LispDllCommandDialog dialog = new LispDllCommandDialog(configuration.Commands.LispCommands[index])
            {
                Owner = owner
            };

            if (dialog.ShowDialog() == true)
            {
                configuration.Commands.LispCommands[index] = dialog.CommandEntry;
                ShowCommands(configuration.Commands.LispCommands);
                LispCommandsGrid.SelectedIndex = index;
            }
        }

        public void DeleteLispCommand(global::ConversorDrawind.Configuration configuration)
        {
            int index = LispCommandsGrid.SelectedIndex;
            if (index < 0 || index >= configuration.Commands.LispCommands.Count)
            {
                return;
            }

            configuration.Commands.LispCommands.RemoveAt(index);
            ShowCommands(configuration.Commands.LispCommands);
            if (configuration.Commands.LispCommands.Count > 0)
            {
                LispCommandsGrid.SelectedIndex = index < configuration.Commands.LispCommands.Count
                    ? index
                    : configuration.Commands.LispCommands.Count - 1;
            }
        }

        public void MoveLispCommand(global::ConversorDrawind.Configuration configuration, int direction)
        {
            int index = LispCommandsGrid.SelectedIndex;
            int newIndex = index + direction;
            if (index < 0 || newIndex < 0 || newIndex >= configuration.Commands.LispCommands.Count)
            {
                return;
            }

            string item = configuration.Commands.LispCommands[index];
            configuration.Commands.LispCommands[index] = configuration.Commands.LispCommands[newIndex];
            configuration.Commands.LispCommands[newIndex] = item;
            ShowCommands(configuration.Commands.LispCommands);
            LispCommandsGrid.SelectedIndex = newIndex;
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardSelection(string action, object sender, SelectionChangedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void LispListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(LispListBoxSelectionChanged), sender, e);
        private void LispListBoxDoubleClick(object sender, MouseButtonEventArgs e) => Forward(nameof(LispListBoxDoubleClick), sender, e);
        private void AddLispClick(object sender, RoutedEventArgs e) => Forward(nameof(AddLispClick), sender, e);
        private void ModifyLispClick(object sender, RoutedEventArgs e) => Forward(nameof(ModifyLispClick), sender, e);
        private void DeleteLispClick(object sender, RoutedEventArgs e) => Forward(nameof(DeleteLispClick), sender, e);
        private void MoveLispUpClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLispUpClick), sender, e);
        private void MoveLispDownClick(object sender, RoutedEventArgs e) => Forward(nameof(MoveLispDownClick), sender, e);
    }
}
