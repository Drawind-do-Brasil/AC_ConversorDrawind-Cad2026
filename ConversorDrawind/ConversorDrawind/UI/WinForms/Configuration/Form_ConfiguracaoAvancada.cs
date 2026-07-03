using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Configuration;

namespace ConversorDrawind
{
    public class Form_ConfiguracaoAvancada : IDisposable
    {
        public DialogResult ShowDialog()
        {
            AdvancedConfigurationWindow window = new AdvancedConfigurationWindow();
            bool? result = window.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
