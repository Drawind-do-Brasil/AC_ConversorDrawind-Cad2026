using Autodesk.AutoCAD.Runtime;

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {
        [CommandMethod("CDwi_Get2Point")]
        public static void CDwi_Get2Point()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), ConversionLog.Write).CaptureTwoPoints();
        }
    }
}
