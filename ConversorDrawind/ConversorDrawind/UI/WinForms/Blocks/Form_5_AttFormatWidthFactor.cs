using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class Form_5_AttFormatWidthFactor : IDisposable
    {
        public string WicthFactor = "1";

        public Form_5_AttFormatWidthFactor(string wicthFactor)
        {
            WicthFactor = wicthFactor;
        }

        public DialogResult ShowDialog()
        {
            WidthFactorDialog dialog = new WidthFactorDialog(WicthFactor);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                WicthFactor = dialog.WidthFactor;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
