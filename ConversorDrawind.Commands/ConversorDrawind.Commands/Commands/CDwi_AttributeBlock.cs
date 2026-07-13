using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using FORMS = System.Windows.Forms;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_AttributeBlock")]
        public static void CDwi_AttributeBlock()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            Configuration configuration = Configuration.Config;
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (configuration.General.ConverterType == 0)
                        ConvertBlocks.SetText(RuntimeConfigurationState.TeklaBlocks);
                    else
                        ConvertBlocks.SetText2(RuntimeConfigurationState.InventorBlocks, RuntimeConfigurationState.OriginalBlocks);

                    messenger.WriteMessage(Localization.StartEditingNewBlock + Localization.MessageCompleted + "\n");
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.EditarNovoBloco, e.Message);
                    messenger.WriteMessage(Localization.StartEditingNewBlock + Localization.MessageFailedPrefix + " \n" +
                                    Localization.ErrorEditingNewBlock + "\n");
                    if (configuration.General.ShowMessages)
                    {
                        string nomeBlocos = "";
                        foreach (var item in RuntimeConfigurationState.TeklaBlocks)
                        {
                            nomeBlocos = nomeBlocos + item.blockName + ", ";
                        }
                        nomeBlocos = nomeBlocos.Trim();
                        nomeBlocos = nomeBlocos.Trim(',');
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            Localization.WarningCouldNotAttributeFormat(nomeBlocos),
                                             Localization.TitleError,
                                             FORMS.MessageBoxButtons.OK,
                                             FORMS.MessageBoxIcon.Warning,
                                             FORMS.MessageBoxDefaultButton.Button1);
                    }

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }
    }
}
