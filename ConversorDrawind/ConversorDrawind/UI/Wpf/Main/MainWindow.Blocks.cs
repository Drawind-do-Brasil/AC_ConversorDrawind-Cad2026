using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void RefreshBlockViews()
        {
            EditorView.RefreshBlockViews(configuration);
        }

        private void BrowseAttributedFormat()
        {
            string fileName = BrowseDrawingFile(configuration.Blocks.TeklaBlockPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            configuration.Blocks.TeklaBlockPath = fileName;
            viewModel.RefreshConfiguration();
            LoadTeklaBlocks(fileName);
        }

        private void LoadTeklaBlocks()
        {
            LoadTeklaBlocks(configuration.Blocks.TeklaBlockPath);
        }

        private void LoadTeklaBlocks(string filePath)
        {
            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                configuration.Blocks.TeklaBlocks.Clear();
                teklaDrawingBlockPath = string.Empty;
                RefreshBlockViews();
                UpdateRelationControls();
                return;
            }

            DisposeTeklaDrawingBlock();
            teklaDrawingBlock = drawingBlock;
            teklaDrawingBlockPath = filePath;
            configuration.Blocks.TeklaBlocks = DeduplicateBlocks(drawingBlock.GetListBlocks())
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void LoadCadBlocks()
        {
            string fileName = BrowseDrawingFile(configuration.Blocks.CadBlockPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            configuration.Blocks.CadBlockPath = fileName;
            viewModel.RefreshConfiguration();
            LoadCadBlocks(fileName);
        }

        private void LoadCadBlocks(string filePath)
        {
            List<Block> blocks = LoadBlockListFromPath(filePath);
            if (blocks == null)
            {
                return;
            }

            configuration.Blocks.CadBlocks = blocks
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
            EditorView.ResetBlockRelationsState(configuration);
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private void LoadOriginalBlocks()
        {
            string fileName = BrowseDrawingFile(configuration.Blocks.CadBlockPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            configuration.Blocks.CadBlockPath = fileName;
            viewModel.RefreshConfiguration();
            LoadOriginalBlocks(fileName);
        }

        private void LoadOriginalBlocks(string filePath)
        {
            List<Block> blocks = LoadBlockListFromPath(filePath);
            if (blocks == null)
            {
                return;
            }

            configuration.Blocks.OriginalBlocks = blocks
                .Select(ConfigurationCompatibilityMapper.ToBlockDefinition)
                .ToList();
            EditorView.ResetBlockRelationsState(configuration);
            RefreshBlockViews();
            UpdateRelationControls();
        }

        private string BrowseDrawingFile(string currentPath)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = Localization.FilterCadDrawings,
                Multiselect = false
            };

            if (File.Exists(currentPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(currentPath);
                dialog.FileName = Path.GetFileName(currentPath);
            }

            return dialog.ShowDialog(this) == true ? dialog.FileName : string.Empty;
        }

        private GetInfo OpenDrawingBlock(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            Thread statusThread = new Thread(new ThreadStart(ApplicationRuntime.ThreadMethodAbrindoCad));
            statusThread.SetApartmentState(ApartmentState.STA);
            statusThread.Start();
            GetInfo drawingBlock = new GetInfo(filePath);
            ApplicationRuntime.StopStatusThread(statusThread);
            return drawingBlock.Status() == "ERROR" ? null : drawingBlock;
        }

        private List<Block> LoadBlockListFromPath(string filePath)
        {
            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                return null;
            }

            List<Block> blocks = DeduplicateBlocks(drawingBlock.GetListBlocks());
            drawingBlock.Dispose();
            return blocks;
        }

        private List<Block> DeduplicateBlocks(List<Block> blocks)
        {
            return blocks
                .GroupBy(block => block.blockName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();
        }

        private void EditTeklaBlock()
        {
            int index = EditorView.GetSelectedTeklaBlockIndex(configuration);
            if (index < 0)
            {
                return;
            }

            EnsureTeklaDrawingBlock();
            if (teklaDrawingBlock == null)
            {
                return;
            }

            Block editableBlock = ConfigurationCompatibilityMapper.ToBlockModel(configuration.Blocks.TeklaBlocks[index]);
            using (AttFormat dialog = new AttFormat(editableBlock, configuration, teklaDrawingBlock))
            {
                if (dialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                teklaDrawingBlock = dialog.myDrawingBlock;
                configuration.Blocks.TeklaBlocks[index] = ConfigurationCompatibilityMapper.ToBlockDefinition(editableBlock);
                RefreshBlockViews();
            }
        }

        private void RemoveTeklaBlock()
        {
            int index = EditorView.GetSelectedTeklaBlockIndex(configuration);
            if (index < 0 || index >= configuration.Blocks.TeklaBlocks.Count)
            {
                return;
            }

            configuration.Blocks.TeklaBlocks.RemoveAt(index);
            RefreshBlockViews();
        }

        private void RelateSelectedBlocks()
        {
            EditorView.RelateSelectedBlocks(configuration);
        }

        private void EditBlockRelationParameters()
        {
            EditorView.EditBlockRelationParameters(configuration);
        }

        private void RemoveSelectedRelation()
        {
            EditorView.RemoveSelectedRelation(configuration);
        }

        private void UpdateRelationControls()
        {
            EditorView.UpdateRelationControls(configuration);
        }

        private void EnsureTeklaDrawingBlock()
        {
            string filePath = configuration.Blocks.TeklaBlockPath;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (teklaDrawingBlock != null)
            {
                teklaDrawingBlock.UpdateStatus();
                if (teklaDrawingBlock.Status() != "ERROR" &&
                    string.Equals(teklaDrawingBlockPath, filePath, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            GetInfo drawingBlock = OpenDrawingBlock(filePath);
            if (drawingBlock == null)
            {
                return;
            }

            DisposeTeklaDrawingBlock();
            teklaDrawingBlock = drawingBlock;
            teklaDrawingBlockPath = filePath;
        }

        private void DisposeTeklaDrawingBlock()
        {
            if (teklaDrawingBlock == null)
            {
                return;
            }

            teklaDrawingBlock.Dispose();
            teklaDrawingBlock = null;
            teklaDrawingBlockPath = string.Empty;
        }

    }
}
