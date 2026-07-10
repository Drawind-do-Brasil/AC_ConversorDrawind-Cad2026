using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_GetDistHorizontal")]
        public static void CDwi_GetDistHorizontal()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), ConversionLog.Write).CaptureHorizontalDistance();
        }
    }
}
