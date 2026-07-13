using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_GetLayer")]
        public static void CDwi_GetLayer()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), ConversionLog.Write).CaptureLayer();
        }
    }
}
