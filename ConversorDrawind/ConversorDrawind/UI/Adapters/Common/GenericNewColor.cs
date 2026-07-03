using ConversorDrawind.UI.Wpf.Common;
using System;
namespace ConversorDrawind
{
    public sealed class GenericNewColor : IDisposable
    {
        public string colorClass = string.Empty;

        public GenericNewColor(string corAtual)
        {
            colorClass = corAtual;
        }

        public UiDialogResult ShowDialog()
        {
            ColorPickerDialog dialog = new ColorPickerDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                colorClass = dialog.ColorValue;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



