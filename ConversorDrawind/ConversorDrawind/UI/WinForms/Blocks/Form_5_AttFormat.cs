using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class Form_5_AttFormat : IDisposable
    {
        private readonly Block block;
        private readonly Arranjos arranjos;
        public GetInfo myDrawingBlock = null;

        public Form_5_AttFormat(Block blockTemp, Arranjos arranjosTemp, GetInfo drawingBlock)
        {
            block = blockTemp;
            arranjos = arranjosTemp;
            myDrawingBlock = drawingBlock;
        }

        public DialogResult ShowDialog()
        {
            BlockAttributeFormatWindow window = new BlockAttributeFormatWindow(block, arranjos, myDrawingBlock);
            bool? result = window.ShowDialog();
            myDrawingBlock = window.DrawingBlock;
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
