using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
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
                messenger.WriteMessage(Localization.StartRemovingOldBlocks);
                try
                {
                    ConvertBlocks.DeleteBlocks(RuntimeConfigurationState.OriginalBlocks);
                    messenger.WriteMessage(Localization.MessageCompleted + "\n");
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.RemoverBlocos, e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }
    }
}
