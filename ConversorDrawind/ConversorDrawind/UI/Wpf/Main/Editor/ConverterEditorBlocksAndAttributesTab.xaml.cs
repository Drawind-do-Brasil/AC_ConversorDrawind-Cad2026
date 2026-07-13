using ConversorDrawind.UI.Wpf.Main.Rows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorBlocksAndAttributesTab : UserControl
    {
        public ConverterEditorBlocksAndAttributesTab()
        {
            InitializeComponent();
        }

        public TextBox AttributedFormatPathTextBoxControl => AttributedFormatPathTextBox;
        public TextBox CadBlocksPathTextBoxControl => CadBlocksPathTextBox;
        public TextBox OriginalBlocksPathTextBoxControl => OriginalBlocksPathTextBox;
        public ListBox TeklaBlocksListBoxControl => BlocksListBox;
        public ListBox CadBlocksListBoxControl => InventorBlocksListBox;
        public ListBox OriginalBlocksListBoxControl => OriginalBlocksListBox;
        public ListBox BlockRelationsListBoxControl => BlockRelationsListBox;
        public Button RelateButtonControl => FindName("RelateButton") as Button;
        public Button RemoveRelationButtonControl => FindName("RemoveRelationButton") as Button;
        public Button EditRelationButtonControl => FindName("EditRelationButton") as Button;

        public void ShowTeklaBlocks(IEnumerable<BlockDefinition> blocks)
        {
            BlocksListBox.ItemsSource = (blocks ?? Enumerable.Empty<BlockDefinition>())
                .Select(block => block.Name)
                .ToList();
        }

        public void ShowCadBlocks(IEnumerable<BlockDefinition> blocks)
        {
            InventorBlocksListBox.ItemsSource = (blocks ?? Enumerable.Empty<BlockDefinition>())
                .Select(block => block.Name)
                .ToList();
        }

        public void ShowOriginalBlocks(IEnumerable<BlockDefinition> blocks)
        {
            OriginalBlocksListBox.ItemsSource = (blocks ?? Enumerable.Empty<BlockDefinition>())
                .Select(block => block.Name)
                .ToList();
        }

        public void ShowBlockRelations(IEnumerable<BlockDefinition> cadBlocks, IEnumerable<BlockDefinition> originalBlocks)
        {
            List<BlockDefinition> cadList = (cadBlocks ?? Enumerable.Empty<BlockDefinition>()).ToList();
            BlockRelationsListBox.ItemsSource = (originalBlocks ?? Enumerable.Empty<BlockDefinition>())
                .Where(original => !string.IsNullOrWhiteSpace(original.RelatedName))
                .Select(original => new
                {
                    Original = original,
                    Cad = cadList.FirstOrDefault(block => string.Equals(block.Name, original.RelatedName, StringComparison.OrdinalIgnoreCase))
                })
                .Where(item => item.Cad != null)
                .Select(item => new BlockRelationRow(item.Cad, item.Original))
                .ToList();
        }

        public void RefreshBlockViews(global::ConversorDrawind.Configuration configuration)
        {
            ShowTeklaBlocks(configuration.Blocks.TeklaBlocks);
            ShowCadBlocks(configuration.Blocks.CadBlocks);
            ShowOriginalBlocks(configuration.Blocks.OriginalBlocks);
            ShowBlockRelations(configuration.Blocks.CadBlocks, configuration.Blocks.OriginalBlocks);
        }

        public void ResetBlockRelationsState(global::ConversorDrawind.Configuration configuration)
        {
            foreach (BlockDefinition block in configuration.Blocks.CadBlocks)
            {
                block.RelatedName = string.Empty;
                ResetTagReference(block);
                block.ColorArgb = Color.Black.ToArgb();
            }

            foreach (BlockDefinition block in configuration.Blocks.OriginalBlocks)
            {
                block.RelatedName = string.Empty;
                ResetTagReference(block);
                block.ColorArgb = Color.Black.ToArgb();
            }

            ShowBlockRelations(configuration.Blocks.CadBlocks, configuration.Blocks.OriginalBlocks);
        }

        public void UpdateRelationControls(global::ConversorDrawind.Configuration configuration)
        {
            if (RelateButtonControl != null)
            {
                RelateButtonControl.IsEnabled = CanRelateSelectedBlocks(configuration);
            }

            bool hasRelationSelection = BlockRelationsListBox.SelectedIndex >= 0;
            if (RemoveRelationButtonControl != null)
            {
                RemoveRelationButtonControl.IsEnabled = hasRelationSelection;
            }

            if (EditRelationButtonControl != null)
            {
                EditRelationButtonControl.IsEnabled = hasRelationSelection;
            }
        }

        public void RelateSelectedBlocks(global::ConversorDrawind.Configuration configuration)
        {
            int cadIndex = GetSelectedBlockIndex(InventorBlocksListBox, configuration.Blocks.CadBlocks);
            int originalIndex = GetSelectedBlockIndex(OriginalBlocksListBox, configuration.Blocks.OriginalBlocks);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return;
            }

            BlockDefinition cadBlock = configuration.Blocks.CadBlocks[cadIndex];
            BlockDefinition originalBlock = configuration.Blocks.OriginalBlocks[originalIndex];
            if (!string.IsNullOrWhiteSpace(originalBlock.RelatedName) || IsCadBlockAlreadyRelated(configuration, cadBlock.Name))
            {
                return;
            }

            originalBlock.RelatedName = cadBlock.Name;
            originalBlock.ColorArgb = Color.LightGray.ToArgb();
            cadBlock.ColorArgb = Color.LightGray.ToArgb();

            RefreshBlockViews(configuration);
            SelectRelatedBlocks(configuration, cadBlock.Name, originalBlock.Name);
            UpdateRelationControls(configuration);
        }

        public void EditBlockRelationParameters(global::ConversorDrawind.Configuration configuration)
        {
            if (!TryGetSelectedRelation(configuration, out BlockDefinition cadBlock, out BlockDefinition originalBlock, out int cadIndex, out int originalIndex))
            {
                return;
            }

            using (AttFormatInventor dialog = new AttFormatInventor(
                ConfigurationCompatibilityMapper.ToBlockModel(cadBlock),
                ConfigurationCompatibilityMapper.ToBlockModel(originalBlock)))
            {
                if (dialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                configuration.Blocks.CadBlocks[cadIndex] = ConfigurationCompatibilityMapper.ToBlockDefinition(dialog.Inventor);
                configuration.Blocks.OriginalBlocks[originalIndex] = ConfigurationCompatibilityMapper.ToBlockDefinition(dialog.Original);
                RefreshBlockViews(configuration);
                SelectRelatedBlocks(configuration, dialog.Inventor.blockName, dialog.Original.blockName);
            }
        }

        public void RemoveSelectedRelation(global::ConversorDrawind.Configuration configuration)
        {
            if (!TryGetSelectedRelation(configuration, out BlockDefinition cadBlock, out BlockDefinition originalBlock, out int cadIndex, out int originalIndex))
            {
                return;
            }

            originalBlock.RelatedName = string.Empty;
            ResetTagReference(originalBlock);
            originalBlock.ColorArgb = Color.Black.ToArgb();
            ResetTagReference(cadBlock);
            cadBlock.ColorArgb = Color.Black.ToArgb();

            RefreshBlockViews(configuration);
            SelectRelatedBlocks(configuration, cadBlock.Name, originalBlock.Name);
            UpdateRelationControls(configuration);
        }

        public int GetSelectedTeklaBlockIndex(global::ConversorDrawind.Configuration configuration)
        {
            return GetSelectedBlockIndex(BlocksListBox, configuration.Blocks.TeklaBlocks);
        }

        private static void ResetTagReference(BlockDefinition block)
        {
            foreach (BlockTagDefinition tag in block.Tags ?? new List<BlockTagDefinition>())
            {
                tag.RelatedIndex = -1;
                tag.IsAssociated = false;
            }
        }

        private bool CanRelateSelectedBlocks(global::ConversorDrawind.Configuration configuration)
        {
            int cadIndex = GetSelectedBlockIndex(InventorBlocksListBox, configuration.Blocks.CadBlocks);
            int originalIndex = GetSelectedBlockIndex(OriginalBlocksListBox, configuration.Blocks.OriginalBlocks);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return false;
            }

            BlockDefinition cadBlock = configuration.Blocks.CadBlocks[cadIndex];
            BlockDefinition originalBlock = configuration.Blocks.OriginalBlocks[originalIndex];
            return string.IsNullOrWhiteSpace(originalBlock.RelatedName) && !IsCadBlockAlreadyRelated(configuration, cadBlock.Name);
        }

        private static bool IsCadBlockAlreadyRelated(global::ConversorDrawind.Configuration configuration, string cadBlockName)
        {
            return configuration.Blocks.OriginalBlocks.Any(block => string.Equals(block.RelatedName, cadBlockName, StringComparison.OrdinalIgnoreCase));
        }

        private static int GetSelectedBlockIndex(ListBox listBox, List<BlockDefinition> blocks)
        {
            if (!(listBox.SelectedItem is string selectedName))
            {
                return -1;
            }

            return blocks.FindIndex(block => string.Equals(block.Name, selectedName, StringComparison.OrdinalIgnoreCase));
        }

        private bool TryGetSelectedRelation(
            global::ConversorDrawind.Configuration configuration,
            out BlockDefinition cadBlock,
            out BlockDefinition originalBlock,
            out int cadIndex,
            out int originalIndex)
        {
            cadBlock = null;
            originalBlock = null;
            cadIndex = -1;
            originalIndex = -1;

            if (!(BlockRelationsListBox.SelectedItem is BlockRelationRow relation))
            {
                return false;
            }

            cadIndex = configuration.Blocks.CadBlocks.IndexOf(relation.CadBlock);
            originalIndex = configuration.Blocks.OriginalBlocks.IndexOf(relation.OriginalBlock);
            if (cadIndex < 0 || originalIndex < 0)
            {
                return false;
            }

            cadBlock = configuration.Blocks.CadBlocks[cadIndex];
            originalBlock = configuration.Blocks.OriginalBlocks[originalIndex];
            return true;
        }

        private void SelectRelatedBlocks(global::ConversorDrawind.Configuration configuration, string cadBlockName, string originalBlockName)
        {
            int cadIndex = configuration.Blocks.CadBlocks.FindIndex(block => string.Equals(block.Name, cadBlockName, StringComparison.OrdinalIgnoreCase));
            int originalIndex = configuration.Blocks.OriginalBlocks.FindIndex(block => string.Equals(block.Name, originalBlockName, StringComparison.OrdinalIgnoreCase));
            if (cadIndex >= 0)
            {
                InventorBlocksListBox.SelectedIndex = cadIndex;
            }

            if (originalIndex >= 0)
            {
                OriginalBlocksListBox.SelectedIndex = originalIndex;
            }
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardMouse(string action, object sender, MouseButtonEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);
        private void ForwardSelection(string action, object sender, SelectionChangedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void BrowseAttributedFormatClick(object sender, RoutedEventArgs e) => Forward(nameof(BrowseAttributedFormatClick), sender, e);
        private void EditBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockClick), sender, e);
        private void EditBlockDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(EditBlockDoubleClick), sender, e);
        private void RemoveBlockClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockClick), sender, e);
        private void LoadInventorBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadInventorBlocksClick), sender, e);
        private void LoadOriginalBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(LoadOriginalBlocksClick), sender, e);
        private void CadBlocksSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(CadBlocksSelectionChanged), sender, e);
        private void OriginalBlocksSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(OriginalBlocksSelectionChanged), sender, e);
        private void BlockRelationsSelectionChanged(object sender, SelectionChangedEventArgs e) => ForwardSelection(nameof(BlockRelationsSelectionChanged), sender, e);
        private void BlockRelationsDoubleClick(object sender, MouseButtonEventArgs e) => ForwardMouse(nameof(BlockRelationsDoubleClick), sender, e);
        private void RelateBlocksClick(object sender, RoutedEventArgs e) => Forward(nameof(RelateBlocksClick), sender, e);
        private void EditBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(EditBlockRelationClick), sender, e);
        private void RemoveBlockRelationClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveBlockRelationClick), sender, e);
    }
}
