using ConversorDrawind.UI.Wpf.Configuration;
using System;

namespace ConversorDrawind
{
    public class ConfiguracaoAvancada : IDisposable
    {
        public UiDialogResult ShowDialog()
        {
            AdvancedConfigurationWindow window = new AdvancedConfigurationWindow();
            bool? result = window.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



