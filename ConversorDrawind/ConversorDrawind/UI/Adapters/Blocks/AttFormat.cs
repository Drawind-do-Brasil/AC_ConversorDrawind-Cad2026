using System;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class AttFormat : IDisposable
    {
        private readonly Block block;
        private readonly Arranjos arranjos;
        public GetInfo myDrawingBlock = null;

        public AttFormat(Block blockTemp, Arranjos arranjosTemp, GetInfo drawingBlock)
        {
            block = blockTemp;
            arranjos = arranjosTemp;
            myDrawingBlock = drawingBlock;
        }

        public UiDialogResult ShowDialog()
        {
            BlockAttributeFormatWindow window = new BlockAttributeFormatWindow(block, arranjos, myDrawingBlock);
            bool? result = window.ShowDialog();
            myDrawingBlock = window.DrawingBlock;
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



