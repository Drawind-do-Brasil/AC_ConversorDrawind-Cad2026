namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void RefreshLispCommandRows()
        {
            EditorView.ShowLispCommands(configuration.Commands.LispCommands);
        }

        private void AddLispCommand()
        {
            EditorView.AddLispCommand(configuration, this);
        }

        private void ModifyLispCommand()
        {
            EditorView.ModifyLispCommand(configuration, this);
        }

        private void DeleteLispCommand()
        {
            EditorView.DeleteLispCommand(configuration);
        }

        private void MoveLispCommand(int direction)
        {
            EditorView.MoveLispCommand(configuration, direction);
        }

    }
}
