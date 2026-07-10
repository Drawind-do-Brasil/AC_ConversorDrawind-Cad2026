using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_GetPoint")]
        public static void CDwi_GetPoint()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), ConversionLog.Write).CapturePoint();
        }
    }
}
