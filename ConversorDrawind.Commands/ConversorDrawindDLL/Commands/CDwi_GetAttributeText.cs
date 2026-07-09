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
[CommandMethod("CDwi_GetAttributeText")]
        public static void CDwi_GetAttributeText()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);

            //editor.WriteMessage("TESTE" + document.Name);
            try
            {
                messenger.WriteMessage("Capturando textos do formato ");
                if (Configuration.Config.General.ConverterType == 0)
                {
                    ConvertBlocks.SetStartPointOverride(ConvertBlocks.GetFormatStartPoint(Configuration.Config.Layers.BlockAttributeLayer));
                    ConvertBlocks.GeTTextNew(Configuration.Config.Layers.BlockAttributeLayer);
                    ConvertBlocks.GeTText();
                }
                else
                    ConvertBlocks.GeTTextInv(RuntimeConfigurationState.InventorBlocks);
                messenger.WriteMessage("... Completado.\n");
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog(LogContext.CapturarTextosDoFormato, e);
                messenger.WriteMessage("... Erro. \n" +
                            "Descrição: Erro ao capturar os textos no formato...\n");
            }
            finally
            {
                ConvertBlocks.ClearStartPointOverride();
            }

        }
    }
}