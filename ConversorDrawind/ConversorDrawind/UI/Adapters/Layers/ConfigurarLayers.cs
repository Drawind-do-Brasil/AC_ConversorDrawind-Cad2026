using System;
using ConversorDrawind.UI.Wpf.Layers;

namespace ConversorDrawind
{
    public class ConfigurarLayers : IDisposable
    {
        public Arranjos arranjos = new Arranjos();

        public ConfigurarLayers(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public UiDialogResult ShowDialog()
        {
            NewLayersConfigurationWindow window = new NewLayersConfigurationWindow(arranjos);
            bool? result = window.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void CheckLines()
        {
            NewLayersConfigurationWindow.CheckLines(arranjos);
        }

        public void OpenAcadLoadLayerExterno()
        {
            NewLayersConfigurationWindow window = new NewLayersConfigurationWindow(arranjos);
            window.OpenAcadLoadLayerExterno();
        }

        public void Dispose()
        {
        }
    }
}



