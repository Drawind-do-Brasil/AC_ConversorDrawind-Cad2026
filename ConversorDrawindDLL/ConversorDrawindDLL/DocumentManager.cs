
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConversorDrawindDLL
{
    class DocumentManager
    {
        static Regex regex = new Regex("%DM%[0-9 ]*[,.]{0,1}[0-9 ]*%DM%");
        static string nomeblock = "QUADRO_DRAWIND";
        static string layer = "DrawindDM";

        static Document document;
        static Database database;
        static Editor editor;
        static Transaction transaction;

        public static void AddBlockDM()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            document = documentContext.Document;
            database = documentContext.Database;
            editor = documentContext.Editor;
            using (transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    ObjectId[] objectIdList = FilterBlock(nomeblock);

                    if (objectIdList.Count() == 0)
                        ImportBlocksDM(nomeblock);


                    Dictionary<string, string> attributeValue = new Dictionary<string, string>();

                    string texto = null;

                    ObjectId[] objs = FilterObjsID();

                    foreach (ObjectId obj in objs)
                    {
                        texto = SearchWeight(obj);
                        if (texto != null)
                            break;
                    }

                    if (texto == null)
                        texto = "";

                    attributeValue.Add("PESO", texto);
                    //System.Windows.Forms.MessageBox.Show(texto);

                    if (objectIdList.Count() == 0)
                    {
                        ObjectId idbl = InsertBlock(nomeblock, new Point3d(1, 1, 0), attributeValue);
                        Entity entity = (Entity)idbl.GetObject(OpenMode.ForWrite);
                        CreateAndAssignALayer(layer, "Continuos", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 0));
                        entity.Layer = layer;
                        FREEZE(layer);
                    }
                    else
                    {
                        foreach (ObjectId item in objectIdList)
                        {
                            ChangeText((BlockReference)item.GetObject(OpenMode.ForWrite), 1, texto);
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }


        public static void CreateAndAssignALayer(string layer, string line, Autodesk.AutoCAD.Colors.Color color)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database acCurDb = documentContext.Database;

            try
            {
                LayerTable acLyrTbl = transaction.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite) as LayerTable;


                if (acLyrTbl.Has(layer) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = color;
                    acLyrTblRec.Name = layer;
                    acLyrTblRec.LinetypeObjectId = ConvertLayer.LoadLinetype(line);
                    acLyrTbl.Add(acLyrTblRec);
                    transaction.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }
                else
                {
                    LayerTableRecord acLyrTblRec = transaction.GetObject(acLyrTbl[layer], OpenMode.ForWrite) as LayerTableRecord;
                    acLyrTblRec.Color = color;
                    acLyrTblRec.Name = layer;
                    acLyrTblRec.LinetypeObjectId = ConvertLayer.LoadLinetype(line);

                }

            }
            catch (Exception)
            {

            }
        }
        public static void FREEZE(string nome)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;


            try
            {
                LayerTable layerTable = (LayerTable)database.LayerTableId.GetObject( OpenMode.ForWrite);
                LayerTableRecord layerTableRecord;
                if (layerTable.Has(nome))
                {
                    layerTableRecord = transaction.GetObject(layerTable[nome], OpenMode.ForWrite) as LayerTableRecord;

                    layerTableRecord.IsFrozen = true;
                }
            }
            catch (Exception)
            {


            }
        
        }

        public static void ChangeText(BlockReference bref, int index, string textonovo)
        {
            var block = (BlockTableRecord)bref.BlockTableRecord.GetObject(OpenMode.ForWrite);
            var benum = block.GetEnumerator();

            ObjectId item = ObjectId.Null;

            item = bref.AttributeCollection[index];

            Entity ent = transaction.GetObject(item, OpenMode.ForWrite, false) as Entity;

            if (ent.GetType() == typeof(AttributeReference))
            {
                AttributeReference attRef = ent as AttributeReference;
                attRef.TextString = textonovo;

            }
        }

        public static string SearchWeight(ObjectId id)
        {
            string texto = null;

            if (string.Equals(id.ObjectClass.DxfName, "TEXT", StringComparison.OrdinalIgnoreCase))
            {
                DBText dBText = (DBText)id.GetObject(OpenMode.ForRead);
                if (dBText == null)
                    return null;

                Match match = regex.Match(dBText.TextString);

                if (match.Success)
                {

                    string tmpTexto = match.Value;
                    texto = tmpTexto.Replace("%DM%", "");
                    dBText.UpgradeOpen();
                    dBText.TextString = dBText.TextString.Replace(tmpTexto, texto);
                    dBText.DowngradeOpen();
                    return texto.Trim();
                }
            }
            
            else if (string.Equals(id.ObjectClass.DxfName, "INSERT", StringComparison.OrdinalIgnoreCase))
            {
                BlockReference bref = (BlockReference)id.GetObject(OpenMode.ForRead);
                BlockTableRecord block = (BlockTableRecord)bref.BlockTableRecord.GetObject(OpenMode.ForRead);
                BlockTableRecordEnumerator benum = block.GetEnumerator();

                while (benum.MoveNext() && texto == null)
                {
                    texto = SearchWeight(benum.Current);
                }

            }
            return texto;
        }

        public static ObjectId[] FilterBlock(string name)
        {
            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertBlockByNameAnd(name));
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);
            if (promptSelectionResult.Status == PromptStatus.OK)
                return promptSelectionResult.Value.GetObjectIds();

            return new ObjectId[] { };
        }

        public static ObjectId[] FilterObjsID()
        {
            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.TextOrInsert());
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);

            if (promptSelectionResult.Status == PromptStatus.OK)
                return promptSelectionResult.Value.GetObjectIds();
            return new ObjectId[] { };
        }



        public static void ImportBlocksDM(string blockName)
        {
            string blockQualifiedFileName = Configuration.LoadConfigDLL();
            Database tmpDb = new Database(false, true);
            tmpDb.ReadDwgFile(blockQualifiedFileName, System.IO.FileShare.Read, true, "");

            database.Insert(blockName, tmpDb, true);

            Database sourceDb = new Database(false, true);
        }

        public static ObjectId InsertBlock(string blockName, Point3d insPoint, Dictionary<string, string> attributeValues)
        {
            ObjectId retBlockID = ObjectId.Null;
            ObjectId blkID = ObjectId.Null;

            try
            {
                BlockTable bt = (BlockTable)(transaction.GetObject(database.BlockTableId, OpenMode.ForWrite));
                BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                if (bt.Has(blockName))
                {
                    blkID = bt[blockName];
                }
                else
                {
                    return retBlockID;
                }

                BlockReference br = new BlockReference(insPoint, blkID);

                List<AttributeReference> lstAR = new List<AttributeReference>();

                BlockTableRecord empBtr = (BlockTableRecord)transaction.GetObject(blkID, OpenMode.ForWrite);

                foreach (ObjectId id in empBtr)
                {
                    Entity ent = (Entity)transaction.GetObject(id, OpenMode.ForWrite, false);

                    if (ent is AttributeDefinition) 
                    {

                        AttributeDefinition attDef = ((AttributeDefinition)(ent));

                        if (!attDef.Constant)
                        {
                            AttributeReference attRef = new AttributeReference();
                            attRef.SetDatabaseDefaults(database);
                            attRef.SetAttributeFromBlock(attDef, br.BlockTransform);

                            if (attributeValues.ContainsKey(attRef.Tag))
                            {
                                attRef.TextString = attributeValues[attRef.Tag];
                            }
                            else
                            {
                                attRef.TextString = attDef.TextString;
                            }

                            attRef.AdjustAlignment(HostApplicationServices.WorkingDatabase);

                            lstAR.Add(attRef);
                        }
                    }
                    else
                    {
                        ent.Visible = true;
                    }
                }
                retBlockID = btr.AppendEntity(br);
                transaction.AddNewlyCreatedDBObject(br, true);

                foreach (AttributeReference attRef in lstAR)
                {
                    br.AttributeCollection.AppendAttribute(attRef);
                    transaction.AddNewlyCreatedDBObject(attRef, true);
                }
            }
            catch (Exception)
            {
                retBlockID = ObjectId.Null;
            }

            return retBlockID;
        }
    }
}
