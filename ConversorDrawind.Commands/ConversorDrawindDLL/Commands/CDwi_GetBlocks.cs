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
        [CommandMethod("CDwi_GetBlocks")]
        public void CDwi_GetBlocks()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {

                try
                {
                    string file = Path.GetTempPath() + "ConversorDrawindTemp\\";

                    file += "TempImporBlocks.Temp";
                    if (File.Exists(file))
                        File.Delete(file);

                    StreamWriter streamWriter = new StreamWriter(file, true);

                    try
                    {
                        ObjectId[] objectIdList = new BlockSelectionService(new AcadEntitySelector(editor)).SelectBlockReferences();
                        if (objectIdList != null)
                        {
                            foreach (ObjectId id1 in objectIdList)
                            {
                                BlockReference blockReference = (BlockReference)transaction.GetObject(id1, OpenMode.ForRead);
                                streamWriter.WriteLine("***" + blockReference.Name);
                                Entity Entity = null;
                                foreach (ObjectId id2 in blockReference.AttributeCollection)
                                {
                                    Entity = transaction.GetObject(id2, OpenMode.ForRead) as Entity;
                                    if (Entity.GetType() == typeof(AttributeReference))
                                    {
                                        AttributeReference attributeReference = Entity as AttributeReference;
                                        streamWriter.WriteLine("---" + attributeReference.Tag + ";" + Convert.ToString(attributeReference.WidthFactor).ReplaceComma());
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception)
                    {

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
