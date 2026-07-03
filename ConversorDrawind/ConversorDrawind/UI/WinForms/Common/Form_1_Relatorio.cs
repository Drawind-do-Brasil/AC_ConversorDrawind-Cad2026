using ConversorDrawind.UI.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_1_Relatorio : IDisposable
    {
        private readonly List<string> items;
        private readonly string message;

        public Form_1_Relatorio(List<string> x, string message)
        {
            items = x;
            this.message = message;
        }

        public DialogResult ShowDialog()
        {
            ReportDialog dialog = new ReportDialog(items, message);
            bool? result = dialog.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
