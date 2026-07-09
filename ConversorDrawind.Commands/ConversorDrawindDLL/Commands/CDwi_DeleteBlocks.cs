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
[CommandMethod("CDwi_DeleteBlocks")]
        public static void CDwi_DeleteBlocks()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                messenger.WriteMessage("Removendo blocos antigo.... ");
                try
                {
                    ConvertBlocks.DeleteBlocks(RuntimeConfigurationState.OriginalBlocks);
                    messenger.WriteMessage("... Completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog(LogContext.RemoverBlocos, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }
    }
}