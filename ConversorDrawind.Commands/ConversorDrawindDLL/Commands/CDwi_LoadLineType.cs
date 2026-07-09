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
        [CommandMethod("CDwi_LoadLineType")]
        public void CDwi_LoadLineType()
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
                    file += "TempImporLineType.Temp";
                    if (File.Exists(file))
                        File.Delete(file);

                    StreamWriter streamWriter = new StreamWriter(file, true);

                    LinetypeTable linetypeTable = transaction.GetObject(database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                    foreach (ObjectId item in linetypeTable)
                    {
                        streamWriter.WriteLine(((LinetypeTableRecord)transaction.GetObject(item, OpenMode.ForRead)).Name);
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