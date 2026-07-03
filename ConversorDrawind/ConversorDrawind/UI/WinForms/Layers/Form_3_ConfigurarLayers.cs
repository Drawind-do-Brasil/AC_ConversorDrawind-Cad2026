using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Layers;

namespace ConversorDrawind
{
    public class Form_3_ConfigurarLayers : IDisposable
    {
        public Arranjos arranjos = new Arranjos();

        public Form_3_ConfigurarLayers(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public DialogResult ShowDialog()
        {
            NewLayersConfigurationWindow window = new NewLayersConfigurationWindow(arranjos);
            bool? result = window.ShowDialog();
            return result == true ? DialogResult.OK : DialogResult.Cancel;
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
