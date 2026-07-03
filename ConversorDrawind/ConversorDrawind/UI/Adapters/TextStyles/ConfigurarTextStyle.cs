using ConversorDrawind.UI.Wpf.TextStyles;
using System;
namespace ConversorDrawind
{
    public sealed class ConfigurarTextStyle : IDisposable
    {
        public Arranjos arranjos = new Arranjos();

        public ConfigurarTextStyle(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public UiDialogResult ShowDialog()
        {
            TextStyleConfigurationWindow window = new TextStyleConfigurationWindow(arranjos);
            bool? result = window.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



