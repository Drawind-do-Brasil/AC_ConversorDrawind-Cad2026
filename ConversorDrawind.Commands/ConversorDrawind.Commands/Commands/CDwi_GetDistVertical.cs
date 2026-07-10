using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_GetDistVertical")]
        public static void CDwi_GetDistVertical()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), ConversionLog.Write).CaptureVerticalDistance();
        }
    }
}
