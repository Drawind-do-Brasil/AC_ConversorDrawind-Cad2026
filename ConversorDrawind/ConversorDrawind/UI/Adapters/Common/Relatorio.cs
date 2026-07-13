using ConversorDrawind.UI.Wpf.Common;
using System;
using System.Collections.Generic;
namespace ConversorDrawind
{
    public sealed class Relatorio : IDisposable
    {
        private readonly List<string> items;
        private readonly string message;

        public Relatorio(List<string> x, string message)
        {
            items = x;
            this.message = message;
        }

        public UiDialogResult ShowDialog()
        {
            ReportDialog dialog = new ReportDialog(items, message);
            bool? result = dialog.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



