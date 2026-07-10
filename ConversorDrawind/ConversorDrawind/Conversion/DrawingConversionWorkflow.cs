using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind
{
    internal sealed class DrawingConversionWorkflow
    {
        private readonly AutoCadSession session;
        private readonly Param1 parameters;
        private readonly Action<string> sendCommand;
        private readonly Action<string> loadFile;
        private readonly Func<string> getPasteClipInsertionPoint;
        private readonly Func<List<Block>> getBlocks;
        private readonly Action<ACAD.AcadDocument> closeFailedDrawing;

        internal DrawingConversionWorkflow(
            AutoCadSession session,
            Param1 parameters,
            Action<string> sendCommand,
            Action<string> loadFile,
            Func<string> getPasteClipInsertionPoint,
            Func<List<Block>> getBlocks,
            Action<ACAD.AcadDocument> closeFailedDrawing)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.sendCommand = sendCommand ?? throw new ArgumentNullException(nameof(sendCommand));
            this.loadFile = loadFile ?? throw new ArgumentNullException(nameof(loadFile));
            this.getPasteClipInsertionPoint = getPasteClipInsertionPoint ?? throw new ArgumentNullException(nameof(getPasteClipInsertionPoint));
            this.getBlocks = getBlocks ?? throw new ArgumentNullException(nameof(getBlocks));
            this.closeFailedDrawing = closeFailedDrawing ?? throw new ArgumentNullException(nameof(closeFailedDrawing));
        }

        internal bool Execute(string file, bool isLastDrawing)
        {
            if (!File.Exists(file))
            {
                ShowMissingFileMessage(file);
                return false;
            }

            TryCreateBackup(file);

            if (!TryOpenDrawing(file, out ACAD.AcadDocument drawingDocument))
                return false;

            try
            {
                ExecuteConversionSteps(isLastDrawing);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                closeFailedDrawing(drawingDocument);
                return false;
            }

            return TrySaveAndClose(drawingDocument);
        }

        private void TryCreateBackup(string file)
        {
            try
            {
                File.Copy(file, DrawingProcessPaths.GetBackupPath(file), true);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private bool TryOpenDrawing(string file, out ACAD.AcadDocument drawingDocument)
        {
            drawingDocument = null;

            try
            {
                ACAD.AcadDocument openedDocument =
                    ComRetry.Invoke(() => session.Application.Documents.Open(file, false), 120, 100);
                drawingDocument = openedDocument;
                session.CurrentDocument = drawingDocument;
                session.OpenedDocuments.Add(drawingDocument);
                ComRetry.Invoke(() => session.Application.ActiveDocument = openedDocument);
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                closeFailedDrawing(drawingDocument);
                return false;
            }
        }

        private void ExecuteConversionSteps(bool isLastDrawing)
        {
            sendCommand("ZOOM E\n");
            sendCommand("CDwi_Convert\n");

            if (parameters.configuration.General.ExchangeFormat)
                CopyExchangeFormatBlocks();

            if (parameters.configuration.General.ConvertLayers)
                sendCommand("CDwi_DeleteLayers\n");

            ApplyDrawingScaleIfRequired();
            ExecuteLispCommands(isLastDrawing);

            sendCommand("CDwi_Finalize\n");
            ComRetry.Invoke(() => session.CurrentDocument.Application.ZoomExtents());
        }

        private void CopyExchangeFormatBlocks()
        {
            sendCommand("CDwi_GetAttributeText\n");

            session.CurrentDocument = session.AttributeDocument;
            ComRetry.Invoke(() => session.Application.ActiveDocument = session.CurrentDocument);
            sendCommand("COPYBASE 0,0,0 all \n");

            session.CurrentDocument = session.OpenedDocuments.Last();
            ComRetry.Invoke(() => session.Application.ActiveDocument = session.CurrentDocument);
            sendCommand("ZOOM E\n");
            sendCommand("REGEN\n");

            List<Block> blocksBeforePaste = parameters.configuration.General.SourceMode == 1
                ? getBlocks()
                : null;

            string pasteClipPoint = getPasteClipInsertionPoint();
            sendCommand("PASTECLIP " + pasteClipPoint + "\n");

            if (blocksBeforePaste != null)
                ScaleNewlyPastedBlock(blocksBeforePaste);

            sendCommand("CDwi_AttributeBlock\n");

            if (parameters.configuration.General.SourceMode == 1)
                sendCommand("CDwi_DeleteBlocks\n");
        }

        private void ScaleNewlyPastedBlock(List<Block> blocksBeforePaste)
        {
            List<Block> blocksAfterPaste = getBlocks();

            foreach (Block existingBlock in blocksBeforePaste)
                blocksAfterPaste.RemoveAll(block => block.blockName == existingBlock.blockName);

            if (blocksAfterPaste.Count == 0)
                return;

            string command = blocksAfterPaste[0].blockName.Replace(" ", "*******");
            sendCommand("CDwi_ScaleBlock\n" + command + "\n");
        }

        private void ApplyDrawingScaleIfRequired()
        {
            if (!parameters.configuration.General.ApplyDrawingScale)
                return;

            ApplicationRuntime.ControladorT2 = false;
            try
            {
                sendCommand("CDwi_Scale\n");
            }
            finally
            {
                ApplicationRuntime.ControladorT2 = true;
            }
        }

        private void ExecuteLispCommands(bool isLastDrawing)
        {
            if (!parameters.configuration.General.ExecuteLisp)
                return;

            IReadOnlyList<LispCommandDefinition> lispCommands =
                LispCommandDefinition.ParseAll(parameters.configuration.Commands.LispCommands);

            ComRetry.Invoke(() => session.CurrentDocument.SetVariable("FILEDIA", 0));
            try
            {
                ExecuteLispCommands(lispCommands.Where(command => !command.ExecuteAfterConversion));

                if (isLastDrawing)
                    ExecuteLispCommands(lispCommands.Where(command => command.ExecuteAfterConversion));
            }
            finally
            {
                ComRetry.Invoke(() => session.CurrentDocument.SetVariable("FILEDIA", 1));
            }
        }

        private void ExecuteLispCommands(IEnumerable<LispCommandDefinition> commands)
        {
            foreach (LispCommandDefinition command in commands)
            {
                loadFile(command.SourceFile);
                sendCommand(command.Command + "\n");
            }
        }

        private bool TrySaveAndClose(ACAD.AcadDocument drawingDocument)
        {
            if (!TrySaveCurrentDrawing())
            {
                closeFailedDrawing(drawingDocument);
                return false;
            }

            try
            {
                CloseDrawingWhenRequired();
                return true;
            }
            catch (Exception exception)
            {
                ShowSaveError(exception);
                closeFailedDrawing(drawingDocument);
                return false;
            }
        }

        private bool TrySaveCurrentDrawing()
        {
            try
            {
                if (ApplicationRuntime.ExtensaoGeral == "DWG")
                    ComRetry.Invoke(() => session.CurrentDocument.Save());
                else
                    sendCommand("SaveDXF\n");

                return true;
            }
            catch
            {
                ApplicationRuntime.ControladorT2 = false;
                try
                {
                    System.Windows.MessageBox.Show(
                        Localization.MessageCouldNotSaveFile,
                        Localization.TitleWarningNoExclamation,
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning);
                }
                finally
                {
                    ApplicationRuntime.ControladorT2 = true;
                }

                return false;
            }
        }

        private void CloseDrawingWhenRequired()
        {
            if (parameters.closedesenhos)
                return;

            int documentCount = ComRetry.Invoke(() => session.Application.Documents.Count);
            ComRetry.Invoke(() => session.CurrentDocument.Close());

            Stopwatch waitClose = Stopwatch.StartNew();
            while (waitClose.ElapsedMilliseconds < 30000)
            {
                if (ComRetry.Invoke(() => session.Application.Documents.Count) < documentCount)
                    break;

                Thread.Sleep(50);
            }

            session.OpenedDocuments.RemoveAt(session.OpenedDocuments.Count - 1);
        }

        private static void ShowMissingFileMessage(string file)
        {
            System.Windows.MessageBox.Show(
                Localization.FormatDrawingDoesNotExist(file),
                Localization.TitleAttentionPlain,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Exclamation);
        }

        private static void ShowSaveError(Exception exception)
        {
            System.Windows.MessageBox.Show(
                exception.Message,
                Localization.TitleWarningNoExclamation,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }
    }
}
