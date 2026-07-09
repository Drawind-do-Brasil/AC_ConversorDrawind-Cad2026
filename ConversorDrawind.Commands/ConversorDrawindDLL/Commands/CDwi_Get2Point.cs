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
        [CommandMethod("CDwi_Get2Point")]
        public static void CDwi_Get2Point()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureTwoPoints();
        }
    }
}