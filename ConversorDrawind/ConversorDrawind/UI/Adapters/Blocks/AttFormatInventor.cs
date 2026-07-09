using ConversorDrawind.UI.Wpf.Blocks;
using System;

namespace ConversorDrawind
{
    public class AttFormatInventor : IDisposable
    {
        private readonly Block inventor;
        private readonly Block original;
        private Block inventorCopy;
        private Block originalCopy;

        public AttFormatInventor(Block inventor, Block original)
        {
            this.inventor = inventor;
            this.original = original;
            inventorCopy = inventor.DeepCopy();
            originalCopy = original.DeepCopy();
        }

        public Block Original
        {
            get { return originalCopy; }
        }

        public Block Inventor
        {
            get { return inventorCopy; }
        }

        public UiDialogResult ShowDialog()
        {
            InventorAttributeFormatWindow window = new InventorAttributeFormatWindow(inventor, original);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                inventorCopy = window.Inventor;
                originalCopy = window.Original;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



