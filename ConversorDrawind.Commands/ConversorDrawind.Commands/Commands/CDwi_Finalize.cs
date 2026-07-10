using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_Finalize")]
        public static void CDwi_Message()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                messenger,
                ConversionLog.Write,
                ConversionMessages.ShowWarningIfEnabled);
            Configuration configuration = Configuration.Config;

            if (configuration.Dimensions.FixArrow)
            {
                stepRunner.Run(
                    Localization.StartFixingDimensionArrows,
                    () => FixArrow.ConsetaSetaSeta(
                        configuration.Dimensions.FixArrowType,
                        ConversionSession.AppliedScale,
                        configuration.Dimensions.FixArrowFactor),
                    LogContext.FinalizarConversao,
                    string.Empty,
                    Localization.ErrorFixingDimensionArrows + "\n",
                    Localization.MessageCompleted + "\n");
            }

            if (configuration.General.Purge)
            {
                stepRunner.Run(
                    Localization.StartPurgingDrawing,
                    () =>
                    {
                        SymbolTableCleanup.PurgeUnreferencedBlocks();
                        SymbolTableCleanup.PurgeUnreferencedLineTypes();
                        SymbolTableCleanup.PurgeUnreferencedLayers();
                        SymbolTableCleanup.PurgeDimensionSyles();
                        SymbolTableCleanup.PurgeTextSyles();
                    },
                    LogContext.SalvarDesenho,
                    Localization.WarningCouldNotPurgeDrawing,
                    Localization.ErrorPurgingDrawing + "\n",
                    Localization.MessageCompleted + "\n");
            }

            TimeSpan ts = DateTime.Now.Subtract(ConversionSession.StartedAt);
            editor.Regen();
            messenger.WriteMessage("\n" + Localization.FormatConversionSummary(ConversionSession.ConverterName, Environment.UserName, ts) + "\n");
            messenger.WriteMessage(Localization.AppCopyright + "\n");
            messenger.WriteMessage(Localization.AppDevelopedBy + "\n");
            messenger.WriteMessage(Localization.MessageConversionFinished + "\n");
        }
    }
}
