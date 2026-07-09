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
        private static myPoint NewMin = new myPoint(double.MaxValue, double.MaxValue, double.MaxValue);
        private static myPoint NewMax = new myPoint(double.MinValue, double.MinValue, double.MinValue);
        public static string LOG_Diretorio = "";
        public static string LOG_FileName = "";
        private static double escalaCapiturada = -1;
        private static double escalaFinal = 1;
        //public static ObjectId idLayer = ObjectId.Null;
        static DateTime timeini = new DateTime();
        static string conversor = "";
        /// <summary>
        /// Constructor
        /// </summary>
        public Conversor()
        {

        }

        public static void EscreverLog(string context, string detail)
        {
            ConversionLogger.Write(LOG_Diretorio, LOG_FileName, context, detail);
        }

        public static void EscreverLog(string context, System.Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            EscreverLog(context, exception.Message);
        }        public static void MoveElements(Point3d startPoint, Point3d endPoint)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database db = documentContext.Database;
            Editor ed = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(ed);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Selecionar todos os elementos no espaço de modelo
                PromptSelectionResult selectionResult = entitySelector.SelectAll();
                if (selectionResult.Status == PromptStatus.OK)
                {
                    SelectionSet selectionSet = selectionResult.Value;
                    ObjectId[] selectedIds = selectionSet.GetObjectIds();

                    // Mover cada objeto selecionado para o novo ponto
                    foreach (ObjectId id in selectedIds)
                    {
                        Entity entity = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (entity != null)
                        {
                            // Calcular o vetor de deslocamento entre os pontos de origem e destino
                            Vector3d displacement = endPoint.GetVectorTo(startPoint);

                            // Aplicar a transformação para mover o objeto
                            entity.TransformBy(Matrix3d.Displacement(displacement));
                        }
                    }
                }

                trans.Commit();
            }

            // Atualizar a viewport para refletir as alterações
            ed.Regen();
        }
        public static void MoveToOrigin()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database db = documentContext.Database;
            Editor ed = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(ed);

            // Obter as coordenadas mínimas e máximas do desenho
            Point3d extMin = new Point3d(NewMin.X, NewMin.Y, NewMin.Z);
            Point3d extMax = new Point3d(NewMax.X, NewMax.Y, NewMax.Z);


            // Calcular o vetor de deslocamento necessário para mover do EXTMIN para (0,0,0)
            Vector3d displacement = new Vector3d(-extMin.X, -extMin.Y, -extMin.Z);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Abrir o espaço de modelo para seleção
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                // Selecionar todos os elementos no espaço de modelo
                PromptSelectionResult selectionResult = entitySelector.SelectAll();
                if (selectionResult.Status == PromptStatus.OK)
                {
                    SelectionSet selectionSet = selectionResult.Value;
                    ObjectId[] selectedIds = selectionSet.GetObjectIds();

                    // Mover cada objeto selecionado para o ponto (0,0,0)
                    foreach (ObjectId id in selectedIds)
                    {
                        Entity entity = (Entity)trans.GetObject(id, OpenMode.ForWrite);
                        entity.TransformBy(Matrix3d.Displacement(displacement));
                    }
                }

                // Commit das alterações e encerrar a transação
                trans.Commit();
            }

            // Atualizar a viewport para refletir as alterações
            ed.Regen();
        }
        public static void UPDATE_DIMENSTION(double dscale)
        {

            ObjectId[] ids = ConvertLayer.Filter("ALL", "DIMENSION", "ALL", "ALL");
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database acCurDb = documentContext.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.MyStartTransaction())
            {
                try
                {
                    foreach (ObjectId item in ids)
                    {

                        try
                        {
                            Entity myEntity = acTrans.GetObject(item, OpenMode.ForWrite) as Entity;
                            Dimension d = myEntity as Dimension;
                            if (d != null)
                            {
                                d.Dimscale = dscale;
                                d.Dimlfac = d.Dimlfac / dscale;

                            }
                        }
                        catch (System.Exception e)
                        {
                            Conversor.EscreverLog(LogContext.AtualizarConfiguracaoDaDimensao, e.Message);
                        }


                    }
                }
                catch (System.Exception)
                {

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }
  public static void GETREALMAXMIN()
        {
            if (!GETREALMAXMINTEKLA())
                GETREALMAXMINGENERAL();

            if (NewMin.X == double.MaxValue ||
                NewMin.Y == double.MaxValue ||
                NewMin.Z == double.MaxValue ||
                NewMax.X == double.MinValue ||
                NewMax.Y == double.MinValue ||
                NewMax.Z == double.MinValue)
                GETREALMAXMINGENERAL();
        }
        public static bool GETREALMAXMINTEKLA()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database db = documentContext.Database;
            Editor ed = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(ed);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                // Criar um filtro para todos os tipos de entidades
                List<TypedValue> filterList = new List<TypedValue>();
                filterList.Add(new TypedValue((int)DxfCode.Operator, "<and"));
                filterList.Add(new TypedValue((int)DxfCode.Operator, "<or"));
                filterList.Add(new TypedValue((int)DxfCode.LayerName, "ALL"));
                filterList.Add(new TypedValue((int)DxfCode.LayerName, "DrawingSheet"));
                filterList.Add(new TypedValue((int)DxfCode.LayerName, "Drawing Sheet"));
                filterList.Add(new TypedValue((int)DxfCode.LayerName, "Drawing_Sheet"));


                filterList.Add(new TypedValue((int)DxfCode.Operator, "or>"));

                filterList.Add(new TypedValue((int)DxfCode.Start, "INSERT"));

                filterList.Add(new TypedValue((int)DxfCode.Operator, "and>"));


                SelectionFilter filter = new SelectionFilter(filterList.ToArray());

                // Selecionar todos os elementos no espaço de modelo com o filtro
                PromptSelectionResult selectionResult = entitySelector.SelectAll(filter);
                if (selectionResult.Status == PromptStatus.OK)
                {
                    SelectionSet selectionSet = selectionResult.Value;
                    ObjectId[] selectedIds = selectionSet.GetObjectIds();

                    // Percorrer cada objeto selecionado
                    foreach (ObjectId id in selectedIds)
                    {
                        Entity entity = trans.GetObject(id, OpenMode.ForRead) as Entity;
                        if (entity != null)
                        {
                            if (entity.GetType() == typeof(BlockReference))
                            {
                                BlockReference blockReference = entity as BlockReference;

                                if (blockReference != null)
                                {
                                    Matrix3d blockTransform = blockReference.BlockTransform;

                                    BlockTableRecord blockDefinition = trans.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                                    foreach (ObjectId objectId in blockDefinition)
                                    {

                                        Entity ent2 = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
                                        if (ent2.GetType() == typeof(Line))
                                            SetPtsByEntity(ent2, blockReference);


                                    }
                                }
                            }
                            else
                            {
                                SetPtsByEntity(entity);
                            }

                        }
                    }
                    trans.Commit();
                    return true;
                }
                else
                {
                    trans.Commit();
                    return false;

                }
            }
        }

        public static void GETREALMAXMINGENERAL()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database db = documentContext.Database;
            Editor ed = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(ed);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                // Criar um filtro para todos os tipos de entidades
                TypedValue[] filterList = new TypedValue[]
                {
            new TypedValue((int)DxfCode.Start, "INSERT, CIRCLE, LINE, TEXT, ARC, HATCH, DIMENSION, MTEXT, LWPOLYLINE, SPLINE, ATTDEF, SOLID, POINT, MTEXT") // Adicione outros tipos de entidades, se necessário
                };

                SelectionFilter filter = new SelectionFilter(filterList);

                // Selecionar todos os elementos no espaço de modelo com o filtro
                PromptSelectionResult selectionResult = entitySelector.SelectAll(filter);
                if (selectionResult.Status == PromptStatus.OK)
                {
                    SelectionSet selectionSet = selectionResult.Value;
                    ObjectId[] selectedIds = selectionSet.GetObjectIds();

                    // Percorrer cada objeto selecionado
                    foreach (ObjectId id in selectedIds)
                    {
                        Entity entity = trans.GetObject(id, OpenMode.ForRead) as Entity;
                        if (entity != null)
                        {

                            SetPtsByEntity(entity);


                        }
                    }
                }

                trans.Commit();
            }
        }

        public static void SetPtsByEntity(Entity entity, BlockReference block)
        {
            try
            {
                Matrix3d blockTransform = block.BlockTransform;
                Extents3d extents3D = (Extents3d)entity.Bounds;

                Point3d minp = extents3D.MinPoint.TransformBy(blockTransform);
                Point3d maxp = extents3D.MaxPoint.TransformBy(blockTransform);

                if (minp.X < NewMin.X)
                    NewMin.X = minp.X;
                if (minp.Y < NewMin.Y)
                    NewMin.Y = minp.Y;
                if (minp.Z < NewMin.Z)
                    NewMin.Z = minp.Z;

                if (maxp.X > NewMax.X)
                    NewMax.X = maxp.X;
                if (maxp.Y > NewMax.Y)
                    NewMax.Y = maxp.Y;
                if (maxp.Z > NewMax.Z)
                    NewMax.Z = maxp.Z;

            }
            catch (System.Exception)
            {


            }
        }
        public static void SetPtsByEntity(Entity entity)
        {
            try
            {
                Extents3d extents3D = (Extents3d)entity.Bounds;
                if (extents3D.MinPoint.X < NewMin.X)
                    NewMin.X = extents3D.MinPoint.X;
                if (extents3D.MinPoint.Y < NewMin.Y)
                    NewMin.Y = extents3D.MinPoint.Y;
                if (extents3D.MinPoint.Z < NewMin.Z)
                    NewMin.Z = extents3D.MinPoint.Z;

                if (extents3D.MaxPoint.X > NewMax.X)
                    NewMax.X = extents3D.MaxPoint.X;
                if (extents3D.MaxPoint.Y > NewMax.Y)
                    NewMax.Y = extents3D.MaxPoint.Y;
                if (extents3D.MaxPoint.Z > NewMax.Z)
                    NewMax.Z = extents3D.MaxPoint.Z;

            }
            catch (System.Exception)
            {


            }
        }
        public static void GETSCALE()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database db = documentContext.Database;
            Editor ed = documentContext.Editor;
            IEntitySelector entitySelector = new AcadEntitySelector(ed);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                // Criar um filtro para todos os tipos de entidades
                TypedValue[] filterList = new TypedValue[]
                {
            new TypedValue((int)DxfCode.Start, "INSERT, TEXT, MTEXT") // Adicione outros tipos de entidades, se necessário
                };

                SelectionFilter filter = new SelectionFilter(filterList);

                // Selecionar todos os elementos no espaço de modelo com o filtro
                PromptSelectionResult selectionResult = entitySelector.SelectAll(filter);
                if (selectionResult.Status == PromptStatus.OK)
                {
                    SelectionSet selectionSet = selectionResult.Value;
                    ObjectId[] selectedIds = selectionSet.GetObjectIds();

                    // Percorrer cada objeto selecionado
                    foreach (ObjectId id in selectedIds)
                    {
                        Entity entity = trans.GetObject(id, OpenMode.ForRead) as Entity;
                        if (entity != null)
                        {
                            try
                            {

                                if (entity.GetType() == typeof(DBText))
                                {
                                    DBText text = entity as DBText;
                                    Point3d textPositionInModelSpace = text.Position;
                                    if (ChecarEscala(text, textPositionInModelSpace))
                                    {
                                        trans.Commit();
                                        return;
                                    }
                                }

                                else if (entity.GetType() == typeof(BlockReference))
                                {
                                    BlockReference blockReference = entity as BlockReference;

                                    if (blockReference != null)
                                    {
                                        Matrix3d blockTransform = blockReference.BlockTransform;

                                        BlockTableRecord blockDefinition = trans.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                                        foreach (ObjectId objectId in blockDefinition)
                                        {
                                            if (objectId.ObjectClass.DxfName.Equals("TEXT", StringComparison.OrdinalIgnoreCase))
                                            {
                                                DBText text = trans.GetObject(objectId, OpenMode.ForRead) as DBText;
                                                Point3d textPositionInModelSpace = text.Position.TransformBy(blockTransform);
                                                if (ChecarEscala(text, textPositionInModelSpace))
                                                {
                                                    trans.Commit();
                                                    return;
                                                }
                                            }
                                        }
                                    }


                                    // Transformar a posição do texto para o espaço do modelo


                                }
                            }
                            catch (System.Exception)
                            {


                            }

                        }
                    }
                }

                trans.Commit();
            }
        }
        /// <summary>
        /// Initial Conversion Layer
        /// </summary>
        private static void InitialConversionLayer()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Editor editor = documentContext.Editor;
            Database acCurDb = documentContext.Database;
            IEntitySelector entitySelector = new AcadEntitySelector(editor);

            SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnly());
            PromptSelectionResult promptSelectionResult = entitySelector.SelectAll(selectionFilter);
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
                            changeLayers((BlockReference)acTrans.GetObject(item, OpenMode.ForWrite));
                        }
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog(LogContext.ConverterCamadasIniciais, e.Message);
                    }
                    finally
                    {
                        acTrans.MyCommit();
                    }
                }
            }

        }


        public static Point3d GetNewMin()
        {
            return new Point3d(NewMin.X, NewMin.Y, NewMin.Z);
        }
        public static Point3d GetNewMax()
        {
            return new Point3d(NewMax.X, NewMax.Y, NewMax.Z);
        }

        private static void changeLayers(BlockReference bref)
        {
            if (bref.Layer != "0")
                return;
            var block = (BlockTableRecord)bref.BlockTableRecord.GetObject(OpenMode.ForRead);

            var benum = block.GetEnumerator();
            if (benum.MoveNext() == false) return;
            var obj = (Entity)benum.Current.GetObject(OpenMode.ForRead);
            bref.Layer = obj.Layer;

        }
        private static bool ChecarEscala(DBText text, Point3d positionInSpace)
        {
            if (!string.Equals(text.Layer, Configuration.Config.Scale.Layer, StringComparison.OrdinalIgnoreCase))
                return false;
            if (ConvertBlocks.CheckPoint(positionInSpace,
                      ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.Scale.Point1.X, Configuration.Config.Scale.Point1.Y, Configuration.Config.Scale.Point1.Z)),
                      ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.Scale.Point2.X, Configuration.Config.Scale.Point2.Y, Configuration.Config.Scale.Point2.Z))))

            {
                int arredondamento = Configuration.Config.Scale.TextSize.ToString().Split(',').Last().Length;
                if (text.Height > Configuration.Config.Scale.TextSize - 0.2 &&
                    text.Height < Configuration.Config.Scale.TextSize + 0.2)
                {
                    string[] temp = text.TextString.Split(':');
                    double escalaConvertida = 0;
                    if (Double.TryParse(temp.Last().ReplaceComma(), out escalaConvertida))
                    {
                        escalaCapiturada = escalaConvertida;
                        return true;
                    }

                }
            }
            return false;
        }
        private ObjectId[] Filter1(Editor editor)
        {
            try
            {
                IEntitySelector entitySelector = new AcadEntitySelector(editor);
                SelectionFilter selectionFilter = new SelectionFilter(LayerFilterFactory.InsertOnly());
                ObjectId[] objectIdList = entitySelector.SelectAll(selectionFilter).Value.GetObjectIds();
                return objectIdList;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public static void CDwi_ConsertarSetaSeta()
        {
            try
            {
                FixArrow.ConsetaSetaSeta(Configuration.Config.Dimensions.FixArrowType, escalaFinal, Configuration.Config.Dimensions.FixArrowFactor);
            }

            catch (System.Exception e)
            {
                Conversor.EscreverLog(LogContext.FixarSetaDaCota, e.Message);
            }
        }
    }

    public class myPoint
    {
        double x, y, z;

        public myPoint(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }
    }

}
