using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Blocks
{
    public partial class InventorTagRelationDialog : Window
    {
        private readonly List<TagBlock> sourceTags;
        private readonly List<TagBlock> workingTags = new List<TagBlock>();
        private readonly ObservableCollection<TagOption> options = new ObservableCollection<TagOption>();
        private readonly TagBlock original;
        private bool ignoreSelectionChange;

        public InventorTagRelationDialog(List<TagBlock> tags, TagBlock original)
        {
            InitializeComponent();
            sourceTags = tags;
            this.original = original;
            LoadOptions(tags);
            SelectedIndex = -1;

            if (original.indiceRelacao != -1)
            {
                TagComboBox.Text = original.indiceRelacao + "  -  " + tags[original.indiceRelacao].tag;
            }
        }

        public int SelectedIndex { get; private set; }

        private void LoadOptions(List<TagBlock> tags)
        {
            options.Add(new TagOption(-1, " ", false));
            int index = 0;
            foreach (TagBlock item in tags)
            {
                options.Add(new TagOption(index, index + "  -  " + item.tag, item.isSociate));
                workingTags.Add(item.DeepCopy());
                index++;
            }

            TagComboBox.ItemsSource = options;
        }

        private void TagSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreSelectionChange)
            {
                return;
            }

            if (original.indiceRelacao != -1)
            {
                workingTags[original.indiceRelacao].isSociate = false;
            }

            if (TagComboBox.SelectedItem is not TagOption selected || selected.Index == -1)
            {
                TagComboBox.Text = options[0].Label;
                SelectedIndex = -1;
                return;
            }

            if (workingTags[selected.Index].isSociate)
            {
                SelectedIndex = -1;
                ignoreSelectionChange = true;
                TagComboBox.SelectedIndex = 0;
                TagComboBox.Text = options[0].Label;
                ignoreSelectionChange = false;
                return;
            }

            workingTags[selected.Index].isSociate = true;
            SelectedIndex = selected.Index;
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            original.indiceRelacao = SelectedIndex;
            sourceTags.Clear();
            foreach (TagBlock item in workingTags)
            {
                sourceTags.Add(item.DeepCopy());
            }

            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            SelectedIndex = original.indiceRelacao;
            DialogResult = false;
        }

        public class TagOption
        {
            public TagOption(int index, string label, bool isAssociated)
            {
                Index = index;
                Label = label;
                IsAssociated = isAssociated;
            }

            public int Index { get; }
            public string Label { get; }
            public bool IsAssociated { get; }
        }
    }
}
