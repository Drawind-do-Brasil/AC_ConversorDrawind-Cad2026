using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConversorDrawindDLL
{
    class ConvertLayer
    {



        public static ObjectId[] Filter(string LayerName, string Start, string ColorName, string LinetypeName)
        {
            try
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                Editor editor = acDoc.Editor;
                List<TypedValue> typedValuelist = new List<TypedValue>();

                typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<and"));
                if (LayerName != "ALL")
                    typedValuelist.Add(new TypedValue((int)DxfCode.LayerName, LayerName));

                if (Start != "ALL")
                    typedValuelist.Add(new TypedValue((int)DxfCode.Start, Start));

                if (LinetypeName != "ALL")
                    typedValuelist.Add(new TypedValue((int)DxfCode.LinetypeName, LinetypeName));

                typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "and>"));
                SelectionFilter selectionFilter = new SelectionFilter(typedValuelist.ToArray());
                PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
                if (promptSelectionResult.Status.ToString() == "OK")
                    return promptSelectionResult.Value.GetObjectIds();
                return null;
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 51", e.Message);
                return null;
            }
        }

        public static ObjectId[] FilterLayers(params string[] layers)
        {
            try
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                Editor editor = acDoc.Editor;
                List<TypedValue> typedValuelist = new List<TypedValue>();

                typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<and"));
                typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<or"));
                foreach (string item in layers)
                {
                    typedValuelist.Add(new TypedValue((int)DxfCode.LayerName, item));
                }
                typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "or>"));
                typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "and>"));
                SelectionFilter selectionFilter = new SelectionFilter(typedValuelist.ToArray());
                PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
                if (promptSelectionResult.Status.ToString() == "OK")
                    return promptSelectionResult.Value.GetObjectIds();
                return null;
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 52", e.Message);
                return null;
            }
        }


        public static void ConvertLayersNewRecursive(ObjectId id, Transaction trans)
        {
            Entity obj = (Entity)trans.GetObject(id, OpenMode.ForRead);
            if (obj == null)
                return;
            if (Configuration.Config.ConvTekla0ConvInv1 == 1 && obj.Id.ObjectClass.DxfName.ToUpper() == "MTEXT")
            {
                ExplodeObjects(new ObjectId[] { id });
            }

            if (obj.Id.ObjectClass.DxfName.ToUpper() == "INSERT")
            {
                BlockReference bref = (BlockReference)obj;

                BlockTableRecord block = (BlockTableRecord)trans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                BlockTableRecordEnumerator benum = block.GetEnumerator();

                while (benum.MoveNext())
                {
                    ConvertLayersNewRecursive(benum.Current, trans);
                }
             
            }
            InstanciaConversor.ConvertInstance(obj);
         
        }

        public static void ConvertLayersNew()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            Database acCurDb = document.Database;
            PromptSelectionResult promptSelectionResult = editor.SelectAll();
            ObjectId[] objectIdList = null;
            using (Transaction transaction = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (promptSelectionResult.Status == PromptStatus.OK)
                        objectIdList = promptSelectionResult.Value.GetObjectIds();
                    if (objectIdList != null)
                    {
                        foreach (var id in objectIdList)
                        {
                            ConvertLayersNewRecursive(id, transaction);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }



        public static void CreateAndAssignALayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    LayerTable acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    for (int i = 0; i < Arranjos.Arrj.AllNewLayerComposition.Count; i++)
                    {
                        string[] temp = Arranjos.Arrj.AllNewLayerComposition[i].Split(':');

                        if (acLyrTbl.Has(temp[0]) == false)
                        {
                            LayerTableRecord acLyrTblRec = new LayerTableRecord();
                            acLyrTblRec.Color = GetColorForName(temp[1]);
                            acLyrTblRec.Name = temp[0];
                            acLyrTblRec.LinetypeObjectId = LoadLinetype(temp[2]);
                            acLyrTbl.Add(acLyrTblRec);
                            acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                        }
                        else
                        {
                            LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[temp[0]], OpenMode.ForWrite) as LayerTableRecord;
                            acLyrTblRec.Color = GetColorForName(temp[1]);
                            acLyrTblRec.Name = temp[0];
                            acLyrTblRec.LinetypeObjectId = LoadLinetype(temp[2]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 55", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static ObjectId CreateAndAssignALayer(string nome)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            ObjectId idLayer = ObjectId.Null;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {

                    LayerTable acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    for (int i = 0; i < Arranjos.Arrj.AllNewLayerComposition.Count; i++)
                    {
                        string[] temp = Arranjos.Arrj.AllNewLayerComposition[i].Split(':');

                        if (temp[0] == nome)
                        {
                            if (acLyrTbl.Has(temp[0]) == false)
                            {
                                LayerTableRecord acLyrTblRec = new LayerTableRecord();
                                acLyrTblRec.Color = GetColorForName(temp[1]);
                                acLyrTblRec.Name = temp[0];
                                acLyrTblRec.LinetypeObjectId = LoadLinetype(temp[2]);
                                acLyrTbl.Add(acLyrTblRec);
                                acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                                idLayer = acLyrTblRec.Id;
                            }
                            else
                            {
                                LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[temp[0]], OpenMode.ForWrite) as LayerTableRecord;
                                acLyrTblRec.Color = GetColorForName(temp[1]);
                                acLyrTblRec.Name = temp[0];
                                acLyrTblRec.LinetypeObjectId = LoadLinetype(temp[2]);
                                idLayer = acLyrTblRec.Id;
                            }
                        }
                    }

                    if (idLayer == ObjectId.Null)
                    {
                        LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl["0"], OpenMode.ForRead) as LayerTableRecord;
                        idLayer = acLyrTblRec.Id;
                    }
                }
                catch (Exception e)
                {

                    Conversor.EscreverLog("Erro 56", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }

            return idLayer;
        }

        public static Color GetColorForName(string color)
        {
            short colorInt = 0;
            if (short.TryParse(color, out colorInt))
                return Color.FromColorIndex(ColorMethod.ByAci, colorInt);

            if (color.ToUpper() == "RED")
                return Color.FromColorIndex(ColorMethod.ByAci, 1);
            if (color.ToUpper() == "YELLOW")
                return Color.FromColorIndex(ColorMethod.ByAci, 2);
            if (color.ToUpper() == "GREEN")
                return Color.FromColorIndex(ColorMethod.ByAci, 3);
            if (color.ToUpper() == "CYAN")
                return Color.FromColorIndex(ColorMethod.ByAci, 4);
            if (color.ToUpper() == "BLUE")
                return Color.FromColorIndex(ColorMethod.ByAci, 5);
            if (color.ToUpper() == "MAGENTA")
                return Color.FromColorIndex(ColorMethod.ByAci, 6);
            if (color.ToUpper() == "WHITE")
                return Color.FromColorIndex(ColorMethod.ByAci, 7);
            if (color.ToUpper() == "BYLAYER")
                return Color.FromColorIndex(ColorMethod.ByAci, 256);
            if (color.ToUpper() == "BYBLOCK")
                return Color.FromColorIndex(ColorMethod.ByAci, 0);
            if (color.ToUpper() == "ALL")
                return null;
            string[] stringRGB = color.Split(',');
            byte red = Convert.ToByte(stringRGB[0]);
            byte green = Convert.ToByte(stringRGB[1]);
            byte blue = Convert.ToByte(stringRGB[2]);
            return Color.FromRgb(red, green, blue);
        }

        public static ObjectId LoadLinetype(string sLineTypName)
        {
            if (sLineTypName.ToUpper() == "ALL")
                return ObjectId.Null;
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            ObjectId id = ObjectId.Null;
           
                try
                {
                    LinetypeTable linetypeTable;
                    linetypeTable = database.LinetypeTableId.GetObject( OpenMode.ForRead) as LinetypeTable;
                    if (linetypeTable.Has(sLineTypName) == false)
                    {
                        database.LoadLineTypeFile(sLineTypName, CadPaths.FindFileNetload("acad.lin"));
                    }

                    if (linetypeTable.Has(sLineTypName))
                    {
                        id = linetypeTable[sLineTypName];
                    }
                }


                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 57", e.Message);
                }
                finally
                {
          
                }
    
            return id;
        }

        public static void ExplodeRadialDimenstionLarge()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    foreach (ObjectId objId in acBlkTbl)
                    {
                        BlockTableRecord acBlkTblRec = acTrans.GetObject(objId, OpenMode.ForRead) as BlockTableRecord;
                        ObjectIdCollection objIdColl = acBlkTblRec.GetBlockReferenceIds(true, true);
                        foreach (ObjectId item in objIdColl)
                        {
                            BlockReference acBref = acTrans.GetObject(item, OpenMode.ForWrite) as BlockReference;
                            if (acBref.GetType() == typeof(RadialDimensionLarge))
                            {
                                acBref.ExplodeToOwnerSpace();
                                acBref.Erase();
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 58.2", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void ExplodeObjects()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite) as BlockTable;

                    foreach (ObjectId objId in acBlkTbl)
                    {
                        BlockTableRecord acBlkTblRec = acTrans.GetObject(objId, OpenMode.ForWrite) as BlockTableRecord;
                        ObjectIdCollection objIdColl = acBlkTblRec.GetBlockReferenceIds(true, true);
                        foreach (ObjectId item in objIdColl)
                        {
                            BlockReference acBref = acTrans.GetObject(item, OpenMode.ForWrite) as BlockReference;

                            acBref.ExplodeToOwnerSpace();
                            acBref.Erase();
                        }

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 58.1", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void ExplodeObjects(ObjectId[] mtexts)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {

                    DBObjectCollection objs = new DBObjectCollection();
                    foreach (ObjectId item in mtexts)
                    {
                        Entity ent = (Entity)acTrans.GetObject(item, OpenMode.ForWrite);
                        MText mt = ent as MText;

                        if (mt != null)
                        {

                            MTextFragmentCallback cb =

                              new MTextFragmentCallback((frag, obj) => { return MTextFragmentCallbackStatus.Continue; });

                            mt.ExplodeFragments(cb);

                        }
                        try
                        {
                            if (ent.GetType() == typeof(BlockReference))
                            {
                                ent.Explode(objs);
                                ent.Erase();
                            }
                        }
                        catch (Exception e)
                        {
                            Conversor.EscreverLog("Erro 59", e.Message);
                        }


                    }
                    BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (DBObject obj in objs)
                    {

                        Entity ent = (Entity)obj;

                        btr.AppendEntity(ent);
                        acTrans.AddNewlyCreatedDBObject(ent, true);

                    }

                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 60", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void ExplodeObjectsInv()
        {
            ObjectId[] myMtexts = FilterLayers(Arranjos.Arrj.AllExplodeLayers.ToArray());
            ExplodeObjectsInv1(myMtexts);
            myMtexts = FilterLayers(Arranjos.Arrj.AllExplodeLayers.ToArray());
            ExplodeObjectsInv1(myMtexts);
            myMtexts = Filter("ALL", "DIMENSION", "ALL", "ALL");
            ExplodeObjectsInv1(myMtexts);
            myMtexts = Filter("ALL", "ALL", "ALL", "ALL");
            ExplodeObjectsInv2(myMtexts);
            UPDATE_DIMENSTION();
        }

        public static void ExplodeObjectsInv1(ObjectId[] mtexts)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {

                    DBObjectCollection objs = new DBObjectCollection();
                    foreach (ObjectId item in mtexts)
                    {
                        Entity ent = (Entity)acTrans.GetObject(item, OpenMode.ForWrite);


                        try
                        {
                            if (ent.GetType() == typeof(BlockReference) ||
                                ent.GetType() == typeof(MText) ||
                                ent.GetType() == typeof(Leader) ||
                                ent.GetType() == typeof(DiametricDimension) ||
                                ent.GetType() == typeof(RadialDimension) ||
                                ent.GetType() == typeof(RadialDimensionLarge))
                            {
                                ent.Explode(objs);
                                ent.Erase();
                            }
                        }
                        catch (Exception e)
                        {
                            Conversor.EscreverLog("Erro 61", e.Message);
                        }


                    }
                    BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (DBObject obj in objs)
                    {

                        Entity ent = (Entity)obj;

                        btr.AppendEntity(ent);
                        acTrans.AddNewlyCreatedDBObject(ent, true);

                    }

                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 62", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void ExplodeObjectsInv2(ObjectId[] mtexts)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {

                    DBObjectCollection objs = new DBObjectCollection();
                    foreach (ObjectId item in mtexts)
                    {
                        Entity ent = (Entity)acTrans.GetObject(item, OpenMode.ForWrite);

                        string t = ent.GetType().ToString();
                        try
                        {
                            if (ent.GetType().ToString() == "Autodesk.AutoCAD.DatabaseServices.ImpDimension")
                            {
                                ent.Explode(objs);
                                ent.Erase();
                            }
                        }
                        catch (Exception e)
                        {
                            Conversor.EscreverLog("Erro 85", e.Message);
                        }


                    }
                    BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (DBObject obj in objs)
                    {

                        Entity ent = (Entity)obj;

                        btr.AppendEntity(ent);
                        acTrans.AddNewlyCreatedDBObject(ent, true);

                    }

                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 86", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void UPDATE_DIMENSTION()
        {
            Regex r = new Regex(@"\d+(\,\d*)?");

            ObjectId[] ids = ConvertLayer.Filter("ALL", "DIMENSION", "ALL", "ALL");
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {

                    foreach (ObjectId item in ids)
                    {

                        Entity myEntity = acTrans.GetObject(item, OpenMode.ForWrite) as Entity;
                        Dimension d = myEntity as Dimension;
                        DBObjectCollection objs = new DBObjectCollection();
                        d.Explode(objs);
                        List<string> listT = new List<string>();
                        d.TextPosition = d.TextPosition;
                        foreach (DBObject item2 in objs)
                        {
                            if (item2.GetType() == typeof(MText))
                            {
                                MText mt = item2 as MText;
                                Match m = r.Match(mt.Text);
                                if (m.Success)
                                    listT.Add(m.Value.ReplaceComma());
                            }
                        }
                        List<string> listT2 = listT.OrderBy(p => Math.Abs(Convert.ToDouble(p) - d.Measurement)).ToList();
                        if (d != null)
                        {
                            double measure = Math.Round(d.Measurement, d.Dimdec);
                            if (d.GetType() == typeof(Point3AngularDimension) ||
                                d.GetType() == typeof(LineAngularDimension2))
                                measure = Math.Round(d.Measurement, d.Dimadec);
                            int precisao = listT2.First().Split(',').Last().Length;
                            if (!listT2.First().Contains(','))
                                precisao = 0;
                            d.Dimadec = precisao;
                            d.Dimdec = precisao;
                        }


                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 65", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void PurgeDimensionSyles()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    DimStyleTable acLyrTbl = acTrans.GetObject(acCurDb.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    foreach (ObjectId acObjId in acLyrTbl)
                    {
                        acObjIdColl.Add(acObjId);
                    }
                    acCurDb.Purge(acObjIdColl);
                    editor.WriteMessage("Purging Dimension Styles: ");
                    foreach (ObjectId acObjId in acObjIdColl)
                    {
                        SymbolTableRecord acSymTblRec = acTrans.GetObject(acObjId, OpenMode.ForWrite) as SymbolTableRecord;

                        try
                        {
                            acSymTblRec.Erase(true);
                            editor.WriteMessage(acSymTblRec.Name.ToUpper() + "  ");
                        }

                        catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                        {
                            Conversor.EscreverLog("Erro 66", Ex.Message);
                            Application.ShowAlertDialog("Erro:\n" + Ex.Message);
                        }
                    }
                    editor.WriteMessage("\n");
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 67", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void PurgeTextSyles()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    TextStyleTable acLyrTbl = acTrans.GetObject(acCurDb.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    foreach (ObjectId acObjId in acLyrTbl)
                    {
                        acObjIdColl.Add(acObjId);
                    }
                    acCurDb.Purge(acObjIdColl);
                    editor.WriteMessage("Purging Text Styles: ");
                    foreach (ObjectId acObjId in acObjIdColl)
                    {
                        SymbolTableRecord acSymTblRec = acTrans.GetObject(acObjId, OpenMode.ForWrite) as SymbolTableRecord;

                        try
                        {
                            acSymTblRec.Erase(true);
                            editor.WriteMessage(acSymTblRec.Name.ToUpper() + "  ");
                        }

                        catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                        {
                            Conversor.EscreverLog("Erro 66", Ex.Message);
                            Application.ShowAlertDialog("Erro:\n" + Ex.Message);
                        }
                    }
                    editor.WriteMessage("\n");
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 67", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void PurgeUnreferencedLineTypes()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    LinetypeTable acLyrTbl = acTrans.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    foreach (ObjectId acObjId in acLyrTbl)
                    {
                        acObjIdColl.Add(acObjId);
                    }
                    acCurDb.Purge(acObjIdColl);
                    editor.WriteMessage("Purging Line Types: ");
                    foreach (ObjectId acObjId in acObjIdColl)
                    {
                        SymbolTableRecord acSymTblRec = acTrans.GetObject(acObjId, OpenMode.ForWrite) as SymbolTableRecord;

                        try
                        {
                            acSymTblRec.Erase(true);
                            editor.WriteMessage(acSymTblRec.Name.ToUpper() + "  ");
                        }

                        catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                        {
                            Conversor.EscreverLog("Erro 66", Ex.Message);
                            Application.ShowAlertDialog("Erro:\n" + Ex.Message);
                        }
                    }
                    editor.WriteMessage("\n");
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 67", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void PurgeUnreferencedLayers()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {

                    LayerTable acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    foreach (ObjectId acObjId in acLyrTbl)
                    {
                        acObjIdColl.Add(acObjId);
                    }
                    acCurDb.Purge(acObjIdColl);
                    editor.WriteMessage("Purging Layers: ");
                    foreach (ObjectId acObjId in acObjIdColl)
                    {
                        SymbolTableRecord acSymTblRec = acTrans.GetObject(acObjId, OpenMode.ForWrite) as SymbolTableRecord;
                        try
                        {
                            acSymTblRec.Erase(true);
                            editor.WriteMessage(acSymTblRec.Name.ToUpper() + "  ");
                        }

                        catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                        {
                            Conversor.EscreverLog("Erro 68", Ex.Message);
                            Application.ShowAlertDialog("Erro:\n" + Ex.Message);
                        }
                    }
                    editor.WriteMessage("\n");
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 69", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static void PurgeUnreferencedBlocks()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    BlockTable acLyrTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    foreach (ObjectId acObjId in acLyrTbl)
                    {
                        acObjIdColl.Add(acObjId);
                    }
                    acCurDb.Purge(acObjIdColl);
                    editor.WriteMessage("Purging Blocks: ");
                    foreach (ObjectId acObjId in acObjIdColl)
                    {
                        SymbolTableRecord acSymTblRec = acTrans.GetObject(acObjId, OpenMode.ForWrite) as SymbolTableRecord;
                        try
                        {
                            acSymTblRec.Erase(true);
                            editor.WriteMessage(acSymTblRec.Name.ToUpper() + "  ");
                        }

                        catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                        {
                            Conversor.EscreverLog("Erro 70", Ex.Message);
                            Application.ShowAlertDialog("Erro:\n" + Ex.Message);
                        }
                    }
                    editor.WriteMessage("\n");

                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 71", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static Autodesk.AutoCAD.GraphicsInterface.FontDescriptor UpdateTextFont(string font, bool italic, bool negrito)
        {
            return new Autodesk.AutoCAD.GraphicsInterface.FontDescriptor(font, negrito, italic, 0, 0);
        }

        public static ObjectId GetTextSyleByName(string name = null)
        {
            if (name == null || !Arranjos.Arrj.AllTextSyles.Contains(name))
                name = Arranjos.Arrj.AllTextSyles.First().Split(':').First();
    

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            TextStyleTable textStyleTable = (TextStyleTable)acCurDb.TextStyleTableId.GetObject( OpenMode.ForWrite);
            TextStyleTableRecord textStyleTableRecord = null;
            ObjectId id = ObjectId.Null;
            if (textStyleTable.Has(name))
            {
                textStyleTableRecord = textStyleTable[name].GetObject(OpenMode.ForWrite) as TextStyleTableRecord;
            }
         
            id = textStyleTableRecord.ObjectId;

            return id;

        }
            public static void CreateTextSyles()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                TextStyleTable textStyleTable = (TextStyleTable)acTrans.GetObject(acCurDb.TextStyleTableId, OpenMode.ForWrite);

                try
                {
                    foreach (var item in Arranjos.Arrj.AllTextSyles)
                    {
                        string[] itemSplit = item.Split(':');

                        TextStyleTableRecord textStyleTableRecord = null;
                        if (textStyleTable.Has(itemSplit[0]) == false)
                        {

                            if (textStyleTable.IsWriteEnabled == false)
                                textStyleTable.UpgradeOpen();
                            textStyleTableRecord = new TextStyleTableRecord();
                            textStyleTableRecord.Name = itemSplit[0];
                            textStyleTable.Add(textStyleTableRecord);
                            acTrans.AddNewlyCreatedDBObject(textStyleTableRecord, true);
                        }
                        else
                        {
                            textStyleTableRecord = acTrans.GetObject(textStyleTable[itemSplit[0]],
                                OpenMode.ForWrite) as TextStyleTableRecord;
                        }

                        textStyleTableRecord.XScale = itemSplit[5].ToDouble();
         
                        textStyleTableRecord.Font = UpdateTextFont(itemSplit[1], itemSplit[2].ToBollean(), itemSplit[3].ToBollean());
                        var file = CadPaths.FindFileNetload(itemSplit[1] + ".shx");
                        if (File.Exists(file))
                            textStyleTableRecord.FileName = file;
                        textStyleTableRecord.ObliquingAngle = ConvertDimension. DegreeToRadian( itemSplit[6].ToDouble());
                        textStyleTableRecord.TextSize = 0;
             
                    }

                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 72", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }

            }
        }
 
        public static ObjectId CreateDimstyle()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                ObjectId id = ObjectId.Null;
                try
                {
                    DimStyleTable dimStyleTable = (DimStyleTable)acTrans.GetObject(acCurDb.DimStyleTableId, OpenMode.ForWrite);
                    DimStyleTableRecord dimStyleTableRecord = null;
                    if (dimStyleTable.Has(Configuration.Config.EXTDIMStyleName) == false)
                    {

                        if (dimStyleTable.IsWriteEnabled == false)
                            dimStyleTable.UpgradeOpen();
                        dimStyleTableRecord = new DimStyleTableRecord();
                        dimStyleTableRecord.Name = Configuration.Config.EXTDIMStyleName;
                        dimStyleTable.Add(dimStyleTableRecord);
                        acTrans.AddNewlyCreatedDBObject(dimStyleTableRecord, true);
                    }
                    else
                    {
                        dimStyleTableRecord = acTrans.GetObject(dimStyleTable[Configuration.Config.EXTDIMStyleName],
                            OpenMode.ForWrite) as DimStyleTableRecord;
                    }

                    dimStyleTableRecord.Dimtxsty = ConvertLayer.GetTextSyleByName(Configuration.Config.EXTTEXTStyleName);

                    dimStyleTableRecord.Dimtxt = Configuration.Config.EXTTEXTSize;
                    dimStyleTableRecord.Dimscale = Configuration.Config.EXTDIMScale; //Escala da dimensão
                    dimStyleTableRecord.Dimdec = Configuration.Config.EXTDIMPrecision; //Precisão da dimensão
                    dimStyleTableRecord.Dimadec = Configuration.Config.EXTDIMAngularPrecision; //Precisão da dimensão algular
                    dimStyleTableRecord.Dimlunit = Configuration.Config.EXTDIMUnit; //Unidade da dimensão
                    dimStyleTableRecord.Dimaunit = Configuration.Config.EXTDIMAngularUnit; //Unidade da dimensão algular
                    dimStyleTableRecord.Dimtad = Configuration.Config.EXTDIMTad; //Posição do texto na dimensão verticalmente
                    dimStyleTableRecord.Dimtih = Configuration.Config.EXTDIMDimensionPosition; //Posição do texto em relação a dimensão
                    dimStyleTableRecord.Dimtix = Configuration.Config.EXTDIMTextForced; //Forçar o texto a permanecer alinhado com a dimensão, caso o espaço seja curto.
                    dimStyleTableRecord.Dimtofl = Configuration.Config.EXTDIMLineForced;
                    dimStyleTableRecord.Dimblk = ConvertLayer.GetArrowObjectId( ConvertLayer.GetArrowBlockNameString(Configuration.Config.EXTDIMSeta));
                    dimStyleTableRecord.Dimasz = Configuration.Config.EXTDIMSizeSeta;
                    dimStyleTableRecord.Dimgap = Configuration.Config.INTDIMTextOffset;
                    dimStyleTableRecord.Dimclrt = GetColorForName(Configuration.Config.EXTDIMColorText);
                    dimStyleTableRecord.Dimclre = GetColorForName(Configuration.Config.EXTDIMColorLine);
                    dimStyleTableRecord.Dimclrd = GetColorForName(Configuration.Config.EXTDIMColorLine);
                    dimStyleTableRecord.Dimexo = Configuration.Config.EXTDIMOffsetLineFromRefPoint;
                    dimStyleTableRecord.Dimtmove = Configuration.Config.EXTDIMTextMove;
                    dimStyleTableRecord.Dimtoh = Configuration.Config.EXTDIMOutsideAlign;
                    dimStyleTableRecord.Dimexe = Configuration.Config.EXTDIMDIMEX;
                    id = dimStyleTableRecord.ObjectId;
                    if (dimStyleTableRecord.ObjectId != acCurDb.Dimstyle)
                    {
                        acCurDb.Dimstyle = dimStyleTableRecord.ObjectId;

                        acCurDb.SetDimstyleData(dimStyleTableRecord);

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 73", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
                return id;
            }
        }

        public static ObjectId CreateDimstyle2()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                ObjectId id = ObjectId.Null;
                try
                {
                    DimStyleTable dimStyleTable = (DimStyleTable)acTrans.GetObject(acCurDb.DimStyleTableId, OpenMode.ForRead);
                    DimStyleTableRecord dimStyleTableRecord = null;

                    foreach (var item in dimStyleTable)
                    {
                        dimStyleTableRecord = acTrans.GetObject(item,
                            OpenMode.ForWrite) as DimStyleTableRecord;
                        dimStyleTableRecord.Dimclrt = GetColorForName(Configuration.Config.EXTDIMColorText);
                        dimStyleTableRecord.Dimclre = GetColorForName(Configuration.Config.EXTDIMColorLine);
                        dimStyleTableRecord.Dimclrd = GetColorForName(Configuration.Config.EXTDIMColorLine);
                        dimStyleTableRecord.Dimtxsty = GetTextSyleByName(Configuration.Config.EXTTEXTStyleName);
                        dimStyleTableRecord.Dimatfit = 3;
                    }

                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 74", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
                return id;
            }
        }

        public static ObjectId GetArrowObjectId(string newArrName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                ObjectId arrObjId = ObjectId.Null;
                try
                {

                    string oldArrName = GetArrowBlockName(newArrName);

                    Application.SetSystemVariable("DIMBLK", oldArrName);



                    if (oldArrName != ".")
                    {
                        BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                        arrObjId = bt[oldArrName];

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 75", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
                return arrObjId;
            }
        }

        public static ObjectId GetArrowObjectId(string newArrName, string tipo)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                ObjectId arrObjId = ObjectId.Null;
                string oldArrName = GetArrowBlockName(newArrName);
                try
                {
                    Application.SetSystemVariable(tipo, oldArrName);


                    if (oldArrName != ".")
                    {
                        BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                        arrObjId = bt[oldArrName];

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 76", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
                return arrObjId;
            }
        }

        public static string GetArrowBlockName(string name)
        {
            if (name.ToUpper() == "Closed Filled".ToUpper())
                return ".";
            if (name.ToUpper() == "Dot".ToUpper())
                return "_DOT";
            if (name.ToUpper() == "Dot Small".ToUpper())
                return "_DOTSMALL";
            if (name.ToUpper() == "Dot Blank".ToUpper())
                return "_DOTBLANK";
            if (name.ToUpper() == "Origin Indicator".ToUpper())
                return "_ORIGIN";
            if (name.ToUpper() == "Origin Indicator 2".ToUpper())
                return "_ORIGIN2";
            if (name.ToUpper() == "Open".ToUpper())
                return "_OPEN";
            if (name.ToUpper() == "Right Angle".ToUpper())
                return "_OPEN90";
            if (name.ToUpper() == "Open 30".ToUpper())
                return "_OPEN30";
            if (name.ToUpper() == "Closed".ToUpper())
                return "_CLOSED";
            if (name.ToUpper() == "Dot Small Blank".ToUpper())
                return "_SMALL";
            if (name.ToUpper() == "None".ToUpper())
                return "_NONE";
            if (name.ToUpper() == "Oblique".ToUpper())
                return "_OBLIQUE";
            if (name.ToUpper() == "Box Filled".ToUpper())
                return "_BOXFILLED";
            if (name.ToUpper() == "Box".ToUpper())
                return "_BOXBLANK";
            if (name.ToUpper() == "Closed Blank".ToUpper())
                return "_CLOSEDBLANK";
            if (name.ToUpper() == "Datum Triangle Filled".ToUpper())
                return "_DATUMFILLED";
            if (name.ToUpper() == "Datum Triangle".ToUpper())
                return "_DATUMBLANK";
            if (name.ToUpper() == "Integral".ToUpper())
                return "_INTEGRAL";
            if (name.ToUpper() == "Architectural Tick".ToUpper())
                return "_ARCHTICK";
            else
                return ".";
        }

        public static string GetArrowBlockNameString(string name)
        {
            if (name.ToUpper() == "Closed Filled".ToUpper())
                return "";
            if (name.ToUpper() == "Dot".ToUpper())
                return "DOT";
            if (name.ToUpper() == "Dot Small".ToUpper())
                return "DOTSMALL";
            if (name.ToUpper() == "Dot Blank".ToUpper())
                return "DOTBLANK";
            if (name.ToUpper() == "Origin Indicator".ToUpper())
                return "ORIGIN";
            if (name.ToUpper() == "Origin Indicator 2".ToUpper())
                return "ORIGIN2";
            if (name.ToUpper() == "Open".ToUpper())
                return "OPEN";
            if (name.ToUpper() == "Right Angle".ToUpper())
                return "OPEN90";
            if (name.ToUpper() == "Open 30".ToUpper())
                return "OPEN30";
            if (name.ToUpper() == "Closed".ToUpper())
                return "CLOSED";
            if (name.ToUpper() == "Dot Small Blank".ToUpper())
                return "SMALL";
            if (name.ToUpper() == "None".ToUpper())
                return "NONE";
            if (name.ToUpper() == "Oblique".ToUpper())
                return "OBLIQUE";
            if (name.ToUpper() == "Box Filled".ToUpper())
                return "BOXFILLED";
            if (name.ToUpper() == "Box".ToUpper())
                return "BOXBLANK";
            if (name.ToUpper() == "Closed Blank".ToUpper())
                return "CLOSEDBLANK";
            if (name.ToUpper() == "Datum Triangle Filled".ToUpper())
                return "DATUMFILLED";
            if (name.ToUpper() == "Datum Triangle".ToUpper())
                return "DATUMBLANK";
            if (name.ToUpper() == "Integral".ToUpper())
                return "INTEGRAL";
            if (name.ToUpper() == "Architectural Tick".ToUpper())
                return "ARCHTICK";
            else
                return "";
        }

        public static void Zoom()
        {
            Point3d pontoMax = Conversor.GetNewMax();
            Point3d pontoMin = Conversor.GetNewMin();
            Zoom(pontoMin, pontoMax);
        }

        public static void Zoom(Point3d pMin, Point3d pMax)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    using (ViewTableRecord viewTableRecord = editor.GetCurrentView())
                    {
                        viewTableRecord.Target = new Point3d((pMin.X + pMax.X) / 2, (pMin.Y + pMax.Y) / 2, (pMin.Z + pMax.Z) / 2);
                        viewTableRecord.CenterPoint = Point2d.Origin;

                        Point3d pMinAux = new Point3d(pMin.X, pMin.Y, 0);
                        Point3d pAuxDeltaYHeith = new Point3d(pMin.X, pMax.Y, 0);
                        Point3d pAuxDeltaXWidth = new Point3d(pMax.X, pMin.Y, 0);

                        viewTableRecord.Height = pMinAux.DistanceTo(pAuxDeltaYHeith);
                        viewTableRecord.Width = pMinAux.DistanceTo(pAuxDeltaXWidth);
                        editor.SetCurrentView(viewTableRecord);
                        editor.Regen();

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 78", e.Message);

                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private static void Scale(ObjectId id, Point3d basept, double scale)
        {
            Matrix3d transform = Matrix3d.Scaling(scale, basept);

            Database db = id.Database;

            using (Transaction tr = db.TransactionManager.MyStartTransaction())
            {
                try
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);

                    if (ent != null)
                    {
                        ent.TransformBy(transform);
                    }

                    if (id.ObjectClass.DxfName.ToUpper() == "DIMENSION")
                    {
                        Dimension d = ent as Dimension;
                        d.Dimscale = scale;
                    }


                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Conversor.EscreverLog("Erro 79", ex.Message);
                    Application.ShowAlertDialog(ex.Message);
                }
                finally
                {
                    tr.MyCommit();
                }
            }
        }

        public static SelectionFilter FilterText(string LayerName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = acDoc.Editor;

            List<TypedValue> typedValuelist = new List<TypedValue>();
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<and"));
            typedValuelist.Add(new TypedValue((int)DxfCode.LayerName, LayerName));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<or"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "MTEXT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return new SelectionFilter(typedValuelist.ToArray());
        }


        public static SelectionFilter FilterText2(params string[] LayerName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = acDoc.Editor;

            List<TypedValue> typedValuelist = new List<TypedValue>();
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<and"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<or"));
            foreach (var item in LayerName)
            {
                typedValuelist.Add(new TypedValue((int)DxfCode.LayerName, item));
            }
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<or"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "MTEXT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return new SelectionFilter(typedValuelist.ToArray());
        }
        public static SelectionFilter FilterTextTeste(string LayerName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = acDoc.Editor;

            List<TypedValue> typedValuelist = new List<TypedValue>();
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<and"));
            typedValuelist.Add(new TypedValue((int)DxfCode.LayerName, LayerName));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "<or"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Start, "INSERT"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "or>"));
            typedValuelist.Add(new TypedValue((int)DxfCode.Operator, "and>"));
            return new SelectionFilter(typedValuelist.ToArray());
        }


        public static void DeletingTekla(string layer)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            Database acCurDb = document.Database;

            TypedValue[] typedValue = new TypedValue[2];
            typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            typedValue.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

            SelectionFilter selectionFilter = new SelectionFilter(typedValue);
            PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
            ObjectId[] objectIdList = null;
            if (promptSelectionResult.Status.ToString() == "OK")
                objectIdList = promptSelectionResult.Value.GetObjectIds();
            if (objectIdList != null)
            {
                using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                {
                    try
                    {
                        foreach (var item in objectIdList)
                        {
                            BlockReference bref = (BlockReference)acTrans.GetObject(item, OpenMode.ForRead);

                            var block = (BlockTableRecord)acTrans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                            var benum = block.GetEnumerator();

                            while (benum.MoveNext())
                            {
                                Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                if (obj.GetType() == typeof(DBText))
                                {
                                    DBText text = obj as DBText;

                                    if (text.TextString == "Tekla structures")
                                    {
                                        text.UpgradeOpen();
                                        text.Erase(true);
                                        text.DowngradeOpen();
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 90", e.Message);
                    }
                    finally
                    {
                        acTrans.MyCommit();
                    }
                }
            }
        }
        public static void DeletingTekla()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
           

                    PromptSelectionResult promptSelectionResult = editor.SelectAll(FilterText2("Drawing sheet" , "ALL", "DrawingSheet", "Drawing_Sheet"));
                    ObjectId[] objectIdList = null;
                    if (promptSelectionResult.Status.ToString() == "OK")
                        objectIdList = promptSelectionResult.Value.GetObjectIds();
                    if (objectIdList != null)
                    {
                        foreach (ObjectId item in objectIdList)
                        {

                            Entity dBObject = acTrans.GetObject(item, OpenMode.ForRead) as Entity;
                            if (dBObject.GetType() == typeof(DBText))
                            {
                                DBText text = dBObject as DBText;
                                if (text.TextString.ToUpper() == "Tekla structures".ToUpper())
                                {
                                    text.UpgradeOpen();
                                    text.Erase(true);
                                    text.DowngradeOpen();
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Conversor.EscreverLog("Erro 80", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        public static Point3d GetPointDiference(Point3d pontoIni, Point3d pontoRef, double scale)
        {
            Point3d p1 = new Point3d((pontoRef.X * scale) + pontoIni.X, (pontoRef.Y * scale) + pontoIni.Y, (pontoRef.Z * scale) + pontoIni.Z);
            return p1;
        }
       /*
        public static double ScaleDrawingCaptureText(string layertext)
        {
            double escala1p1 = 1;
            double escala = -1;

            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            Database acCurDb = document.Database;

            TypedValue[] typedValue = new TypedValue[2];
            typedValue.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

            SelectionFilter selectionFilter = new SelectionFilter(typedValue);
            PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
            ObjectId[] objectIdList = null;
            if (promptSelectionResult.Status.ToString() == "OK")
                objectIdList = promptSelectionResult.Value.GetObjectIds();
            if (objectIdList != null)
            {
                using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                {
                    try
                    {
                        foreach (var item in objectIdList)
                        {
                           Entity entity = (Entity)acTrans.GetObject(item, OpenMode.ForRead);
                            if(entity.GetType() == typeof(BlockReference))
                            {
                                 BlockReference bref = (BlockReference)acTrans.GetObject(item, OpenMode.ForRead);

                            var block = (BlockTableRecord)acTrans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                            var benum = block.GetEnumerator();

                            while (benum.MoveNext())
                            {
                                Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                if (obj.GetType() == typeof(DBText))
                                {
                                    DBText text = obj as DBText;
                                    if (Configuration.Config.EXTSCALELayer.ToUpper() == text.Layer.ToUpper() && ConvertBlocks.CheckPoint(text.Position,
                   ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp1.X, Configuration.Config.EXTSCALEAp1.Y, Configuration.Config.EXTSCALEAp1.Z)),
                   ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp2.X, Configuration.Config.EXTSCALEAp2.Y, Configuration.Config.EXTSCALEAp2.Z))))
                                    {
                                        int arredondamento = Configuration.Config.EXTSCALETextSizeString.Split(',').Last().Length;
                                        if (text.Height > Configuration.Config.EXTSCALETextSize - 0.2 &&
                                            text.Height < Configuration.Config.EXTSCALETextSize + 0.2)
                                        {
                                            string[] temp = text.TextString.Split(':');
                                            double escalaConvertida = 0;
                                            if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                                                escala = escalaConvertida;
                                            if (escala1p1 == escala)
                                                escala = -1;
                                        }
                                    }
                                }
                            }
                        }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 90", e.Message);
                    }
                    finally
                    {
                        acTrans.MyCommit();
                    }
                }
            }
            return escala;
        }*/
        /*
                public static double ScaleDrawingCapture(string layer)
                {
                    double escala1p1 = 1;
                    double escala = -1;

                    Document document = Application.DocumentManager.MdiActiveDocument;
                    Editor editor = document.Editor;
                    Database acCurDb = document.Database;

                    TypedValue[] typedValue = new TypedValue[2];
                    typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                    typedValue.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

                    SelectionFilter selectionFilter = new SelectionFilter(typedValue);
                    PromptSelectionResult promptSelectionResult = editor.SelectAll(selectionFilter);
                    ObjectId[] objectIdList = null;
                    if (promptSelectionResult.Status.ToString() == "OK")
                        objectIdList = promptSelectionResult.Value.GetObjectIds();
                    if (objectIdList != null)
                    {
                        using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                        {
                            try
                            {
                                foreach (var item in objectIdList)
                                {
                                    BlockReference bref = (BlockReference)acTrans.GetObject(item, OpenMode.ForRead);

                                    var block = (BlockTableRecord)acTrans.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                                    var benum = block.GetEnumerator();

                                    while (benum.MoveNext())
                                    {
                                        Entity obj = (Entity)acTrans.GetObject(benum.Current, OpenMode.ForRead);
                                        if (obj.GetType() == typeof(DBText))
                                        {
                                            DBText text = obj as DBText;
                                            if (Configuration.Config.EXTSCALELayer.ToUpper() == text.Layer.ToUpper() && ConvertBlocks.CheckPoint(text.Position,
                           ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp1.X, Configuration.Config.EXTSCALEAp1.Y, Configuration.Config.EXTSCALEAp1.Z)),
                           ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.EXTSCALEAp2.X, Configuration.Config.EXTSCALEAp2.Y, Configuration.Config.EXTSCALEAp2.Z))))
                                            {
                                                int arredondamento = Configuration.Config.EXTSCALETextSizeString.Split(',').Last().Length;
                                                if (text.Height > Configuration.Config.EXTSCALETextSize - 0.2 &&
                                                    text.Height < Configuration.Config.EXTSCALETextSize + 0.2)
                                                {
                                                    string[] temp = text.TextString.Split(':');
                                                    double escalaConvertida = 0;
                                                    if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                                                        escala = escalaConvertida;
                                                    if (escala1p1 == escala)
                                                        escala = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                Conversor.EscreverLog("Erro 90", e.Message);
                            }
                            finally
                            {
                                acTrans.MyCommit();
                            }
                        }
                    }
                    return escala;
                }
                public static double ScaleDrawingCapture()
                {
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    Editor editor = doc.Editor;
                    Database acCurDb = doc.Database;

                    object ptMin = Conversor.GetNewMin();

                    Point3d pIni = ConvertBlocks.GetStartPoint();
                    double escala1p1 = 1;
                    double escala = -1;

                    Point3d pq1 = GetPointDiference(pIni, new Point3d(Configuration.Config.EXTSCALEAp1.X, Configuration.Config.EXTSCALEAp1.Y, Configuration.Config.EXTSCALEAp1.Z), escala1p1);
                    Point3d pq2 = GetPointDiference(pIni, new Point3d(Configuration.Config.EXTSCALEAp2.X, Configuration.Config.EXTSCALEAp2.Y, Configuration.Config.EXTSCALEAp2.Z), escala1p1);
                    ConvertLayer.Zoom(pq1, pq2);
                    editor.Regen();
                    editor.UpdateScreen();

                    using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
                    {
                        try
                        {
                            if (!Configuration.Config.EXTSCALEManual)
                            {
                                List<ObjectId> myIDsScale = new List<ObjectId>();


                                PromptSelectionResult psr = editor.SelectWindow(pq1,
                                                                                 pq2,
                                                                                 FilterTextTeste(Configuration.Config.EXTSCALELayer));
                                if (psr.Status == PromptStatus.OK)
                                    myIDsScale.AddRange(psr.Value.GetObjectIds());
                                if (myIDsScale.Count > 0)
                                {

                                    DBText dBObject = acTrans.GetObject(myIDsScale.First(), OpenMode.ForRead) as DBText;
                                    int arredondamento = Configuration.Config.EXTSCALETextSizeString.Split(',').Last().Length;

                                    if (dBObject.Height > Configuration.Config.EXTSCALETextSize - 0.2 &&
                                                  dBObject.Height < Configuration.Config.EXTSCALETextSize + 0.2)

                                    {
                                        string[] temp = dBObject.TextString.Split(':');
                                        double escalaConvertida = 0;
                                        if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                                            escala = escalaConvertida;
                                        if (escala1p1 == escala)
                                            escala = -1;
                                    }

                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Conversor.EscreverLog("Erro 81", e.Message);
                        }

                        finally
                        {
                            acTrans.MyCommit();
                        }
                    }
                    return escala;
                }
        */
        public static double GetScaleDrawing(double scale)
        {
            Point3d pIni = ConvertBlocks.GetStartPoint();
            double scaleDesenho = 1;

            if (Configuration.Config.EXTSCALEManual || scale <= 0)
            {
                Zoom(GetPointDiference(pIni, new Point3d(Configuration.Config.EXTSCALEMp1.X, Configuration.Config.EXTSCALEMp1.Y, Configuration.Config.EXTSCALEMp1.Z), scaleDesenho),
                     GetPointDiference(pIni, new Point3d(Configuration.Config.EXTSCALEMp2.X, Configuration.Config.EXTSCALEMp2.Y, Configuration.Config.EXTSCALEMp2.Z), scaleDesenho));
                ScaleForm scaleF = new ScaleForm();
                scaleF.TopMost = true;
                scaleF.ShowDialog();
                scale = scaleF.scale;
                scaleF.Dispose();
            }
            return scale;
        }

        public static double ScaleDrawing(double scale)
        {
            Point3d ptMax =   Conversor.GetNewMax();
            Point3d ptMin =   Conversor.GetNewMin();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = doc.Editor;
            Database acCurDb = doc.Database;
            Zoom(ptMin, ptMax);
            List<ObjectId> myIDs = new List<ObjectId>();
            PromptSelectionResult psr = editor.SelectAll();
            if (psr.Status == PromptStatus.OK)
                myIDs.AddRange(psr.Value.GetObjectIds());

            foreach (ObjectId item in myIDs)
            {
                Scale(item, (Point3d)ptMin, scale);
            }
            return scale;
        }

        public static double ScaleDrawingInv(double scale, List<BlockClass> blockClasso)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = doc.Editor;
            Database acCurDb = doc.Database;

            List<ObjectId> myIDs = new List<ObjectId>();
            object ptMax = Conversor.GetNewMax();
            object ptMin = Conversor.GetNewMin();
            Point3d pIni = ConvertBlocks.GetStartPoint();


            Zoom((Point3d)ptMin, (Point3d)ptMax);

            try
            {
                foreach (BlockClass item in blockClasso)
                {
                    myIDs.AddRange(ConvertBlocks.FilterBlock(item.blockName));
                }
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 83", e.Message);
            }

            foreach (ObjectId item in myIDs)
            {
                Scale(item, (Point3d)ptMin, scale);

            }

            return scale;
        }

        public static bool WhatIsTheOrientation(Point3d p1, Point3d p2, string orientacao)
        {
            if (orientacao == "HORIZONTAL" && Math.Round(p1.Y, 3) == Math.Round(p2.Y, 3))
                return true;
            if (orientacao == "VERTICAL" && Math.Round(p1.X, 3) == Math.Round(p2.X, 3))
                return true;
            return false;
        }
    }
}
