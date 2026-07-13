using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_TextHeight")]
        public static void CDwi_TextHeight()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), ConversionLog.Write).CaptureTextHeight();
        }
    }
}
