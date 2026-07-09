using ConversorDrawind.UI.Wpf.Blocks;
using System;

namespace ConversorDrawind
{
    public class AttFormat : IDisposable
    {
        private readonly Block block;
        private readonly Configuration configuration;
        public GetInfo myDrawingBlock = null;

        public AttFormat(Block blockTemp, Configuration configuration, GetInfo drawingBlock)
        {
            block = blockTemp;
            this.configuration = configuration ?? new Configuration();
            myDrawingBlock = drawingBlock;
        }

        public UiDialogResult ShowDialog()
        {
            BlockAttributeFormatWindow window = new BlockAttributeFormatWindow(block, configuration, myDrawingBlock);
            bool? result = window.ShowDialog();
            myDrawingBlock = window.DrawingBlock;
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



