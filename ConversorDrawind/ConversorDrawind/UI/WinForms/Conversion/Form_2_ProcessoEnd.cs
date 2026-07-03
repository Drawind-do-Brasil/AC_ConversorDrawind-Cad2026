using ConversorDrawind.UI.Wpf.Conversion;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_2_ProcessoEnd : IDisposable
    {
        public DialogResult ShowDialog()
        {
            ConversionFinishedDialog dialog = new ConversionFinishedDialog(Form_2_Processo.tempo);
            bool? result = dialog.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
