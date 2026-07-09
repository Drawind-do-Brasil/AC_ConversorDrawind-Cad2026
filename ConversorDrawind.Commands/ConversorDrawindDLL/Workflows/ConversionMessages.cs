using FORMS = System.Windows.Forms;

namespace ConversorDrawindDLL
{
    internal static class ConversionMessages
    {
        internal static void ShowWarningIfEnabled(string message)
        {
            if (!Configuration.Config.General.ShowMessages)
                return;

            FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                message,
                "Erro",
                FORMS.MessageBoxButtons.OK,
                FORMS.MessageBoxIcon.Warning,
                FORMS.MessageBoxDefaultButton.Button1);
        }
    }
}
