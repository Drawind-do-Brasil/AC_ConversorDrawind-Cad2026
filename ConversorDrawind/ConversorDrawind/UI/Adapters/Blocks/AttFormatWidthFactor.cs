using System;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class AttFormatWidthFactor : IDisposable
    {
        public string WicthFactor = "1";

        public AttFormatWidthFactor(string wicthFactor)
        {
            WicthFactor = wicthFactor;
        }

        public UiDialogResult ShowDialog()
        {
            WidthFactorDialog dialog = new WidthFactorDialog(WicthFactor);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                WicthFactor = dialog.WidthFactor;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



