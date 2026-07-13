using ConversorDrawind.UI.Wpf.Conversion;
using System;
namespace ConversorDrawind
{
    public sealed class ProcessoEnd : IDisposable
    {
        public UiDialogResult ShowDialog()
        {
            ConversionFinishedDialog dialog = new ConversionFinishedDialog(Processo.tempo);
            bool? result = dialog.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



