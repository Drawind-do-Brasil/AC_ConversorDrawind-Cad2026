using ConversorDrawind.UI.Wpf.Common;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_8_GenericNewColor : IDisposable
    {
        public string colorClass = string.Empty;

        public Form_8_GenericNewColor(string corAtual)
        {
            colorClass = corAtual;
        }

        public DialogResult ShowDialog()
        {
            ColorPickerDialog dialog = new ColorPickerDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                colorClass = dialog.ColorValue;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
