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

namespace ConversorDrawind.Commands
{
    public partial class Conversor
    {

        [CommandMethod("CDwi_LoadLayer")]
        public void CDwi_LoadLayer()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    string file = Path.GetTempPath() + "ConversorDrawindTemp\\";
                    if (!Directory.Exists(file))
                        Directory.CreateDirectory(file);
                    file += "TempImporLayer.Temp";
                    if (File.Exists(file))
                        File.Delete(file);

                    StreamWriter streamWriter = new StreamWriter(file, true);

                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                    foreach (ObjectId item in layerTable)
                    {
                        streamWriter.WriteLine(((LayerTableRecord)transaction.GetObject(item, OpenMode.ForRead)).Name);
                    }
                    streamWriter.Close();
                }
                catch (System.Exception)
                {

                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}