using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
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
            Configuration configuration = Configuration.Config;

            try
            {
                messenger.WriteMessage(Localization.StartCapturingFormatTexts);
                if (configuration.General.ConverterType == 0)
                {
                    ConvertBlocks.SetStartPointOverride(ConvertBlocks.GetFormatStartPoint(configuration.Layers.BlockAttributeLayer));
                    ConvertBlocks.GeTTextNew(configuration.Layers.BlockAttributeLayer);
                    ConvertBlocks.GeTText();
                }
                else
                    ConvertBlocks.GeTTextInv(RuntimeConfigurationState.InventorBlocks);
                messenger.WriteMessage(Localization.MessageCompleted + "\n");
            }
            catch (System.Exception e)
            {
                ConversionLog.Write(LogContext.CapturarTextosDoFormato, e);
                messenger.WriteMessage(Localization.MessageFailedPrefix + " \n" +
                            Localization.ErrorCapturingFormatTexts + "\n");
            }
            finally
            {
                ConvertBlocks.ClearStartPointOverride();
            }

        }
    }
}
