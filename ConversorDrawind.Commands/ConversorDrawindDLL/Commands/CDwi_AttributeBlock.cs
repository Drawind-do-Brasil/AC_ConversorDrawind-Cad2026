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
[CommandMethod("CDwi_AttributeBlock")]
        public static void CDwi_AttributeBlock()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (Configuration.Config.General.ConverterType == 0)
                        ConvertBlocks.SetText(RuntimeConfigurationState.TeklaBlocks);
                    else
                        ConvertBlocks.SetText2(RuntimeConfigurationState.InventorBlocks, RuntimeConfigurationState.OriginalBlocks);

                    messenger.WriteMessage("Editando o novo bloco ... Completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog(LogContext.EditarNovoBloco, e.Message);
                    messenger.WriteMessage("Editando o novo bloco ... Erro. \n" +
                                    "Descrição: Erro ao editar o novo bloco...\n");
                    if (Configuration.Config.General.ShowMessages)
                    {
                        string nomeBlocos = "";
                        foreach (var item in RuntimeConfigurationState.TeklaBlocks)
                        {
                            nomeBlocos = nomeBlocos + item.blockName + ", ";
                        }
                        nomeBlocos = nomeBlocos.Trim();
                        nomeBlocos = nomeBlocos.Trim(',');
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível atributar o formato. \nOs blocos ou atributos dentro dos blocos não correspondem ao especificado.\nNomes dos blocos especificados: " + nomeBlocos + ".",
                                             "Erro",
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