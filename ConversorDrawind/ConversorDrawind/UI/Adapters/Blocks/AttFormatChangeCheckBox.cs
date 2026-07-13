using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class AttFormatChangeCheckBox
    {
        public bool modificar = true;

        public AttFormatChangeCheckBox()
        {
        }

        public UiDialogResult ShowDialog()
        {
            BlockCheckboxChangeDialog dialog = new BlockCheckboxChangeDialog(modificar);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                modificar = dialog.Modificar;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }
    }
}



