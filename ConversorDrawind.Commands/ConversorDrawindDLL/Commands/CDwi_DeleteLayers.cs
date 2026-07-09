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
[CommandMethod("CDwi_DeleteLayers")]
        public static void CDwi_DeleteLayers()
        {
            if (RuntimeConfigurationState.LayerRemove.Count > 0)
            {
                IAcadDocumentContext documentContext = new AcadDocumentContext();
                Database database = documentContext.Database;
                Editor editor = documentContext.Editor;
                IEditorMessenger messenger = new AcadEditorMessenger(editor);
                using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
                {
                    messenger.WriteMessage("Removendo layers desnecessários... ");
                    try
                    {
                        ConvertBlocks.DeleteLayerNew(RuntimeConfigurationState.LayerRemove.ToList());
                        ConvertBlocks.DeleteLayer(RuntimeConfigurationState.LayerRemove.ToList());
                        messenger.WriteMessage("... Completado.\n");
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog(LogContext.RemoverCamadasDesnecessarias, e.Message);
                    }

                    finally
                    {
                        acTrans.MyCommit();
                    }
                }
            }
        }
    }
}