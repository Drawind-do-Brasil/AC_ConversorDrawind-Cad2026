using ConversorDrawind.UI.Wpf.TextStyles;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_3_ConfigurarTextStyle : IDisposable
    {
        public Arranjos arranjos = new Arranjos();

        public Form_3_ConfigurarTextStyle(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public DialogResult ShowDialog()
        {
            TextStyleConfigurationWindow window = new TextStyleConfigurationWindow(arranjos);
            bool? result = window.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
