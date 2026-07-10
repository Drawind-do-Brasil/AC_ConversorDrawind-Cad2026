using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Linq;

namespace ConversorDrawind.Commands
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
                    messenger.WriteMessage(Localization.StartRemovingUnusedLayers);
                    try
                    {
                        ConvertBlocks.DeleteLayerNew(RuntimeConfigurationState.LayerRemove.ToList());
                        ConvertBlocks.DeleteLayer(RuntimeConfigurationState.LayerRemove.ToList());
                        messenger.WriteMessage(Localization.MessageCompleted + "\n");
                    }
                    catch (System.Exception e)
                    {
                        ConversionLog.Write(LogContext.RemoverCamadasDesnecessarias, e.Message);
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
