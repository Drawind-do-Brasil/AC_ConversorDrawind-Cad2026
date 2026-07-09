using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FORMS = System.Windows.Forms;

namespace ConversorDrawindDLL
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
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);

            if (Configuration.Config.Dimensions.FixArrow)
            {
                stepRunner.Run(
                    Localization.StartFixingDimensionArrows,
                    CDwi_ConsertarSetaSeta,
                    LogContext.FinalizarConversao,
                    string.Empty,
                    Localization.ErrorFixingDimensionArrows + "\n",
                    Localization.MessageCompleted + "\n");
            }

            if (Configuration.Config.General.Purge)
            {
                stepRunner.Run(
                    Localization.StartPurgingDrawing,
                    () =>
                    {
                        ConvertLayer.PurgeUnreferencedBlocks();
                        ConvertLayer.PurgeUnreferencedLineTypes();
                        ConvertLayer.PurgeUnreferencedLayers();
                        ConvertLayer.PurgeDimensionSyles();
                        ConvertLayer.PurgeTextSyles();
                    },
                    LogContext.SalvarDesenho,
                    Localization.WarningCouldNotPurgeDrawing,
                    Localization.ErrorPurgingDrawing + "\n",
                    Localization.MessageCompleted + "\n");
            }

            TimeSpan ts = DateTime.Now.Subtract(timeini);
            editor.Regen();
            messenger.WriteMessage("\n" + Localization.FormatConversionSummary(conversor, Environment.UserName, ts) + "\n");
            messenger.WriteMessage(Localization.AppCopyright + "\n");
            messenger.WriteMessage(Localization.AppDevelopedBy + "\n");
            messenger.WriteMessage(Localization.MessageConversionFinished + "\n");
        }
    }
}