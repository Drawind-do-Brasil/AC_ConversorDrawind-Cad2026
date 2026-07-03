using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public sealed class Form_3_ConfigurarLayersNome : IDisposable
    {
        public string nome;
        private readonly Arranjos arranjos;

        public Form_3_ConfigurarLayersNome(string nome, Arranjos arranjos)
        {
            this.nome = nome;
            this.arranjos = arranjos;
        }

        public DialogResult ShowDialog()
        {
            LayerNameDialog dialog = new LayerNameDialog(nome, arranjos.allNewLayer);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                nome = dialog.LayerName;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
