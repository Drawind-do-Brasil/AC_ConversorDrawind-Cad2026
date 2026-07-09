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
        [CommandMethod("CDwi_GetDistHorizontal")]
        public static void CDwi_GetDistHorizontal()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureHorizontalDistance();
        }
    }
}