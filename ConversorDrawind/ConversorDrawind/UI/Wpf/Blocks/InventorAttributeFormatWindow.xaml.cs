using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Blocks
{
    public partial class InventorAttributeFormatWindow : Window
    {
        public InventorAttributeFormatWindow(Block inventor, Block original)
        {
            InitializeComponent();
            Inventor = inventor;
            Original = original;
            Rows = new ObservableCollection<AssociationRow>();
            LoadRows();
            DataContext = this;
        }

        public Block Original { get; private set; }
        public Block Inventor { get; private set; }
        public ObservableCollection<AssociationRow> Rows { get; }

        private void LoadRows()
        {
            foreach (TagBlock item in Original.listTags)
            {
                string relation = string.Empty;
                if (item.indiceRelacao != -1)
                {
                    relation = item.indiceRelacao + "  -  " + Inventor.listTags[item.indiceRelacao].tag;
                    item.isSociate = true;
                }

                Rows.Add(new AssociationRow(item.tag, relation, item.widthfactor));
            }
        }

        private void AssociationGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AssociationRow row = AssociationGrid.CurrentItem as AssociationRow;
            if (row == null || AssociationGrid.CurrentColumn == null)
            {
                return;
            }

            int rowIndex = Rows.IndexOf(row);
            int columnIndex = AssociationGrid.Columns.IndexOf(AssociationGrid.CurrentColumn);

            if (columnIndex == 1)
            {
                TagFormatInventor myBlock = new TagFormatInventor(Inventor.listTags, Original.listTags[rowIndex]);
                myBlock.ShowDialog();
                row.InventorTag = myBlock.indice != -1
                    ? myBlock.indice + "  -  " + Inventor.listTags[myBlock.indice].tag
                    : string.Empty;
                myBlock.Dispose();
            }
            else if (columnIndex == 2)
            {
                EditWidthFactor(rowIndex, new[] { row });
            }
        }

        private void AssociationGridMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociationRow row = AssociationGrid.CurrentItem as AssociationRow;
            if (row == null || AssociationGrid.CurrentColumn == null)
            {
                return;
            }

            int columnIndex = AssociationGrid.Columns.IndexOf(AssociationGrid.CurrentColumn);
            if (columnIndex != 2)
            {
                return;
            }

            List<AssociationRow> selectedRows = AssociationGrid.SelectedCells
                .Where(cell => AssociationGrid.Columns.IndexOf(cell.Column) == 2)
                .Select(cell => cell.Item as AssociationRow)
                .Where(item => item != null)
                .Distinct()
                .ToList();

            if (selectedRows.Count == 0)
            {
                selectedRows.Add(row);
            }

            EditWidthFactor(Rows.IndexOf(selectedRows[0]), selectedRows);
        }

        private void EditWidthFactor(int rowIndex, IEnumerable<AssociationRow> rows)
        {
            string current = Rows[rowIndex].WidthFactor ?? string.Empty;
            AttFormatWidthFactor widthFactor = new AttFormatWidthFactor(current);
            widthFactor.ShowDialog();

            foreach (AssociationRow row in rows)
            {
                row.WidthFactor = widthFactor.WicthFactor;
                Original.listTags[Rows.IndexOf(row)].widthfactor = widthFactor.WicthFactor;
            }

            widthFactor.Dispose();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void RecalculateTags()
        {
            Inventor.ResetTagReference();
            Original.ResetTagReference();
            int ids = 0;
            foreach (AssociationRow item in Rows)
            {
                int index = -1;
                try
                {
                    index = Convert.ToInt32((item.InventorTag ?? string.Empty).Split('-').First().Trim());
                }
                catch (Exception)
                {
                }

                Original.listTags[ids].indiceRelacao = index;
                if (index != -1)
                {
                    Inventor.listTags[index].isSociate = true;
                }

                ids++;
            }
        }

        private void AssociateByTagsClick(object sender, RoutedEventArgs e)
        {
            Inventor.ResetTagReference();
            Original.ResetTagReference();
            foreach (AssociationRow row in Rows)
            {
                row.InventorTag = string.Empty;
            }

            for (int i = 0; i < Original.listTags.Count; i++)
            {
                for (int j = 0; j < Inventor.listTags.Count; j++)
                {
                    if (!Inventor.listTags[j].isSociate &&
                        Inventor.listTags[j].tag.ToUpper() == Original.listTags[i].tag.ToUpper())
                    {
                        Original.listTags[i].indiceRelacao = j;
                        Inventor.listTags[j].isSociate = true;
                        Rows[i].InventorTag = j + "  -  " + Inventor.listTags[j].tag;
                    }
                }
            }
        }

        private void AssociateByOrderClick(object sender, RoutedEventArgs e)
        {
            Inventor.ResetTagReference();
            Original.ResetTagReference();
            foreach (AssociationRow row in Rows)
            {
                row.InventorTag = string.Empty;
            }

            for (int i = 0; i < Inventor.listTags.Count && i < Original.listTags.Count; i++)
            {
                Original.listTags[i].indiceRelacao = i;
                Inventor.listTags[i].isSociate = true;
                Rows[i].InventorTag = i + "  -  " + Inventor.listTags[i].tag;
            }
        }

        private void MoveTagsDownClick(object sender, RoutedEventArgs e)
        {
            List<int> rowIndexes = GetSelectedInventorRows().OrderByDescending(index => index).ToList();
            foreach (int rowIndex in rowIndexes)
            {
                if (rowIndex + 1 > Rows.Count - 1)
                {
                    continue;
                }

                Rows[rowIndex + 1].InventorTag = Rows[rowIndex].InventorTag;
                Rows[rowIndex].InventorTag = string.Empty;
            }

            RecalculateTags();
        }

        private void MoveTagsUpClick(object sender, RoutedEventArgs e)
        {
            List<int> rowIndexes = GetSelectedInventorRows().OrderBy(index => index).ToList();
            foreach (int rowIndex in rowIndexes)
            {
                if (rowIndex - 1 == -1)
                {
                    continue;
                }

                Rows[rowIndex - 1].InventorTag = Rows[rowIndex].InventorTag;
                Rows[rowIndex].InventorTag = string.Empty;
            }

            RecalculateTags();
        }

        private IEnumerable<int> GetSelectedInventorRows()
        {
            List<int> selectedRows = AssociationGrid.SelectedCells
                .Where(cell => AssociationGrid.Columns.IndexOf(cell.Column) == 1)
                .Select(cell => Rows.IndexOf((AssociationRow)cell.Item))
                .Where(index => index >= 0)
                .Distinct()
                .ToList();

            if (selectedRows.Count > 0)
            {
                return selectedRows;
            }

            if (AssociationGrid.CurrentItem is AssociationRow row &&
                AssociationGrid.CurrentColumn != null &&
                AssociationGrid.Columns.IndexOf(AssociationGrid.CurrentColumn) == 1)
            {
                return new[] { Rows.IndexOf(row) };
            }

            return Array.Empty<int>();
        }

        public class AssociationRow : INotifyPropertyChanged
        {
            private string inventorTag;
            private string widthFactor;

            public AssociationRow(string originalTag, string inventorTag, string widthFactor)
            {
                OriginalTag = originalTag;
                this.inventorTag = inventorTag;
                this.widthFactor = widthFactor;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string OriginalTag { get; }

            public string InventorTag
            {
                get { return inventorTag; }
                set
                {
                    inventorTag = value;
                    OnPropertyChanged();
                }
            }

            public string WidthFactor
            {
                get { return widthFactor; }
                set
                {
                    widthFactor = value;
                    OnPropertyChanged();
                }
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}



