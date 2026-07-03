using ConversorDrawind.UI.Wpf.Common;

namespace ConversorDrawind
{
    public sealed class ChangeFormat
    {
        public static string Show(string txtMessage, string txtTitle)
        {
            ChangeFormatDialog dialog = new ChangeFormatDialog(txtMessage, txtTitle);
            dialog.ShowDialog();
            return dialog.ButtonId;
        }
    }
}



