using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ConversorDrawind.UI.Wpf.Blocks;

namespace ConversorDrawind
{
    public class Form_6_TagFormatInventor : IDisposable
    {
        public int indice = -1;

        private readonly List<TagBlock> tags;
        private readonly TagBlock original;

        public Form_6_TagFormatInventor(List<TagBlock> tags, TagBlock original)
        {
            this.tags = tags;
            this.original = original;
        }

        public DialogResult ShowDialog()
        {
            InventorTagRelationDialog dialog = new InventorTagRelationDialog(tags, original);
            bool? result = dialog.ShowDialog();
            indice = dialog.SelectedIndex;
            return result == true ? DialogResult.OK : DialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}
