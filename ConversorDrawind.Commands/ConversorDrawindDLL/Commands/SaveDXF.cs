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
[CommandMethod("SaveDXF")]
        public static void SaveDXF()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Database database = documentContext.Database;
            string path = Path.Combine(Path.GetDirectoryName(document.Name), Path.GetFileNameWithoutExtension(document.Name) + ".dxf");

            int precision = 16;
            // document.DowngradeDocOpen(false);
            if (File.Exists(path))
                File.Delete(path);
            database.DxfOut(path, precision, DwgVersion.AC1021);

            //database.SaveAs(path, DwgVersion.AC1009);

        }
    }
}