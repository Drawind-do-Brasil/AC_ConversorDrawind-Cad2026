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
                    "Consertando setas das dimensões... ",
                    CDwi_ConsertarSetaSeta,
                    LogContext.FinalizarConversao,
                    string.Empty,
                    "Descrição: Erro ao tentar consertar as setas das dimensões...\n",
                    "... Completado.\n");
            }

            if (Configuration.Config.General.Purge)
            {
                stepRunner.Run(
                    "Purgando desenho... ",
                    () =>
                    {
                        ConvertLayer.PurgeUnreferencedBlocks();
                        ConvertLayer.PurgeUnreferencedLineTypes();
                        ConvertLayer.PurgeUnreferencedLayers();
                        ConvertLayer.PurgeDimensionSyles();
                        ConvertLayer.PurgeTextSyles();
                    },
                    LogContext.SalvarDesenho,
                    "Não foi possível remover layers, blocos e tipo de linhas desnessessario .nVerifique se a conversão ocorreu normalmente.",
                    "Descrição: Erro ao tentar purgar o desenho...\n",
                    "... Completado.\n");
            }

            TimeSpan ts = DateTime.Now.Subtract(timeini);
            editor.Regen();
            messenger.WriteMessage("\nConversão: " + conversor + "\tUsuário: " + Environment.UserName + "\tTempo: " + ts.Hours + "h:" + ts.Minutes + "mm:" + ts.Seconds + "s:" + ts.Milliseconds + "ms\n");
            messenger.WriteMessage("Conversor Drawind 2011 @ 2016 - Versão 2016 - Drawind do Brasil Corporação Limitada. Todos os direitos reservados.\n");
            messenger.WriteMessage("Desenvolvido por Nayara Ferreira de Jesus.\n");
            messenger.WriteMessage("Conversão finalizada.\n");
        }
    }
}