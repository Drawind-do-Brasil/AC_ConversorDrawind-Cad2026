using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class Form_5_AttFormatChangeCheckBox
    {
        public bool modificar = true;

        public Form_5_AttFormatChangeCheckBox()
        {
        }

        public DialogResult ShowDialog()
        {
            BlockCheckboxChangeDialog dialog = new BlockCheckboxChangeDialog(modificar);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                modificar = dialog.Modificar;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }
    }
}
