using System;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class Form_6_AttFormatInventor : IDisposable
    {
        private readonly Block inventor;
        private readonly Block original;
        private Block inventorCopy;
        private Block originalCopy;

        public Form_6_AttFormatInventor(Block inventor, Block original)
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

        public DialogResult ShowDialog()
        {
            InventorAttributeFormatWindow window = new InventorAttributeFormatWindow(inventor, original);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                inventorCopy = window.Inventor;
                originalCopy = window.Original;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
