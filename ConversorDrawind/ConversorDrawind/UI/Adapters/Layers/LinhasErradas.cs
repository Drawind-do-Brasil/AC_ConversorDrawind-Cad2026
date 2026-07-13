using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Collections.Generic;
namespace ConversorDrawind
{
    public sealed class LinhasErradas : IDisposable
    {
        private readonly List<CorrecaoLinhas> linhasErradas;
        private readonly Configuration configuration;

        public LinhasErradas(List<CorrecaoLinhas> linhasErradas, Configuration configuration)
        {
            this.linhasErradas = linhasErradas;
            this.configuration = configuration ?? new Configuration();
        }

        public bool CheckLines()
        {
            return new WrongLineTypesDialog(linhasErradas, configuration).CheckLines();
        }

        public UiDialogResult ShowDialog()
        {
            WrongLineTypesDialog dialog = new WrongLineTypesDialog(linhasErradas, configuration);
            bool? result = dialog.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



