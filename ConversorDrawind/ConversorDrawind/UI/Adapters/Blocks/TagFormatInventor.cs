using ConversorDrawind.UI.Wpf.Blocks;
using System;
using System.Collections.Generic;

namespace ConversorDrawind
{
    public class TagFormatInventor : IDisposable
    {
        public int indice = -1;

        private readonly List<TagBlock> tags;
        private readonly TagBlock original;

        public TagFormatInventor(List<TagBlock> tags, TagBlock original)
        {
            this.tags = tags;
            this.original = original;
        }

        public UiDialogResult ShowDialog()
        {
            InventorTagRelationDialog dialog = new InventorTagRelationDialog(tags, original);
            bool? result = dialog.ShowDialog();
            indice = dialog.SelectedIndex;
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



