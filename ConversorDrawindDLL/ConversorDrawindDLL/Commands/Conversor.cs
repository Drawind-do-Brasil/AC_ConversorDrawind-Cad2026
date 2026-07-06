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
    public class Conversor
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

        public static void EscreverLog(string log, string erro)
        {
            ConversionLogger.Write(LOG_Diretorio, LOG_FileName, log, erro);
        }
        public static void MoveElements(Point3d startPoint, Point3d endPoint)
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
        

        [CommandMethod("DRAWINDCAD_Convert")]
        public static void DRAWINDCAD_ConvertToDimension()
        {    
            NewMin = new myPoint(double.MaxValue, double.MaxValue, double.MaxValue);
        NewMax = new myPoint(double.MinValue, double.MinValue, double.MinValue);

        timeini = DateTime.Now;
            //Main Instances
            escalaCapiturada = -1;
            escalaFinal = 1;

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            ConversionCommandRunner commandRunner = new ConversionCommandRunner(
                documentContext,
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);
            ConversionCommandContext commandContext = commandRunner.CreateContext();
            Document document = commandContext.DocumentContext.Document;
            ISystemVariableService systemVariables = commandContext.SystemVariables;
            ScaleWorkflow scaleWorkflow = commandContext.ScaleWorkflow;
            ConversionStepRunner stepRunner = commandContext.StepRunner;
            ConversionWorkflow workflow = new ConversionWorkflow(stepRunner, scaleWorkflow);
            ConversionExtentsWorkflow extentsWorkflow = new ConversionExtentsWorkflow(
                GETREALMAXMIN,
                GetNewMin,
                GetNewMax,
                MoveToOrigin,
                point => NewMin = point,
                (x, y, z) =>
                {
                    NewMax.X = x;
                    NewMax.Y = y;
                    NewMax.Z = z;
                });
            commandRunner.WriteStartupBanner(commandContext.Messenger);

            stepRunner.Run(
                "Extraindo os blocos ",
                InitialConversionLayer,
                "Erro 14",
                "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao extrair os layers dos blocos...\n");

            extentsWorkflow.RefreshAndZoom();

          
            commandRunner.InitializeLogger(document, ref LOG_Diretorio, ref LOG_FileName);




            commandRunner.LoadTempConfiguration(Configuration.Config, ref conversor);

            GETSCALE();
            //idLayer = ConvertLayer.CreateAndAssignALayer(Configuration.Config.EXTDIMlayer);

           
       
      

     
            systemVariables.Set("DWGCHECK", 1);

            workflow.CreateLayersIfEnabled();

            workflow.CreateTextStylesIfNeeded();
           
            stepRunner.Run(
                "Movendo para origem ",
                extentsWorkflow.MoveToOriginAndRefreshZoom,
                "Erro 14",
                "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao extrair os layers dos blocos...\n");
            

            workflow.ConvertDimensionsIfEnabled();

            workflow.RunTeklaInverseConversionIfNeeded();

            workflow.ExplodeBlocksIfConfigured();

            workflow.AddDmBlockIfEnabled();

            workflow.DeleteTeklaStructuresIfEnabled();

            workflow.ConvertLayersIfEnabled();



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
                            Conversor.EscreverLog("Erro 21", e.Message);
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

        [CommandMethod("DRAWINDCAD_Save")]
        public static void DRAWINDCAD_Save()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Editor editor = documentContext.Editor;
            Database database = documentContext.Database;
            {

                PromptKeywordOptions pko = new PromptKeywordOptions("Tipo: ");
                pko.AllowNone = true;
                pko.Keywords.Add("DWG");
                pko.Keywords.Add("DXF");
                pko.Keywords.Default = "DWG";
                PromptResult tipoOperacao = editor.GetKeywords(pko);
                if (tipoOperacao.Status == PromptStatus.Cancel)
                {
                    return;
                }
                if (tipoOperacao.StringResult == "DWG")
                    database.Save();
                else
                    SaveDXF();

            }
            {
                PromptKeywordOptions pko = new PromptKeywordOptions("Fechar: ");
                pko.AllowNone = true;
                pko.Keywords.Add("Sim");
                pko.Keywords.Add("Não");
                pko.Keywords.Default = "Sim";
                PromptResult tipoOperacao = editor.GetKeywords(pko);
                if (tipoOperacao.Status == PromptStatus.Cancel)
                {
                    return;
                }
                if (tipoOperacao.StringResult == "Sim")
                {
                    document.CloseAndDiscard();
                    document.Dispose();
                }

            }

        }
        [CommandMethod("DRAWINDCAD_Scale")]
        public static void DRAWINDCAD_ConvertToScale()
        {

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            ScaleWorkflow scaleWorkflow = new ScaleWorkflow(systemVariables);
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                new AcadEditorMessenger(editor),
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);

            stepRunner.Run(
                "Colocando o desenho na escala real... ",
                () =>
                {
                    escalaFinal = ConvertLayer.GetScaleDrawing(escalaCapiturada);

                    ConvertLayer.ScaleDrawing(escalaFinal);
                    if (Configuration.Config.ConvTekla0ConvInv1 == 1)
                        UPDATE_DIMENSTION(escalaFinal);
                    scaleWorkflow.ApplyDrawingScale(Configuration.Config.EXTLINELtscale, Configuration.Config.EXTDIMScale, escalaFinal);
                    Point3d ptMax = GetNewMax();
                    Point3d ptMin = GetNewMin();
                    database.Limmax = new Point2d(ptMax.X * escalaFinal, ptMax.Y * escalaFinal);
                    database.Limmin = new Point2d(ptMin.X, ptMin.Y);

                    using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
                    {
                        try
                        {
                            ViewportTableRecord acVportTblRec = acTrans.GetObject(document.Editor.ActiveViewportId, OpenMode.ForWrite) as ViewportTableRecord;
                            acVportTblRec.GridEnabled = true;
                            acVportTblRec.GridIncrements = new Point2d(escalaFinal * 10, escalaFinal * 10);
                            document.Editor.UpdateTiledViewportsFromDatabase();
                            if (Configuration.Config.ConvTekla0ConvInv1 == 0)
                            {
                                DimStyleTable dimStyleTable = (DimStyleTable)acTrans.GetObject(database.DimStyleTableId, OpenMode.ForRead);
                                DimStyleTableRecord dimStyleTableRecord = null;
                                if (dimStyleTable.Has(Configuration.Config.EXTDIMStyleName) == true)
                                {
                                    dimStyleTableRecord = acTrans.GetObject(dimStyleTable[Configuration.Config.EXTDIMStyleName],
                                                          OpenMode.ForWrite) as DimStyleTableRecord;
                                    dimStyleTableRecord.Dimscale = Configuration.Config.EXTDIMScale * escalaFinal;
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Conversor.EscreverLog("Erro 22", e.Message);
                        }
                        finally
                        {
                            acTrans.MyCommit();
                        }
                    }

                    ConvertLayer.Zoom(ptMin, new Point3d(ptMax.X * escalaFinal, ptMax.Y * escalaFinal, ptMax.Z * escalaFinal));
                },
                "Erro 23",
                "Não foi possível colocar o desenho na escala real!",
                "Descrição: Erro ao tentar colocar o desenho na escala real...\n",
                "... Completado.\n");


        }

        [CommandMethod("DRAWINDCAD_ScaleBlock")]
        public static void DRAWINDCAD_ConvertToScaleInv()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            ScaleWorkflow scaleWorkflow = new ScaleWorkflow(systemVariables);
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                new AcadEditorMessenger(editor),
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);



            stepRunner.Run(
                "Colocando o formato na escala real... ",
                () =>
                {
                    string newdate = editor.GetString("Digite o nome do bloco: ").StringResult.Replace("*******", " ");
                    double scale = escalaFinal = scaleWorkflow.ReadLineTypeScale();
                    ConvertLayer.ScaleDrawingInv(scale, new List<BlockClass>() { new BlockClass(newdate) });
                    object ptMax2 = GetNewMax();
                    object ptMin2 = GetNewMin();
                    ConvertLayer.Zoom((Point3d)ptMin2, (Point3d)ptMax2);
                },
                "Erro 24",
                "Não foi possível colocar o formato na escala real!",
                "Descrição: Erro ao tentar colocar o formato na escala real...\n",
                "... Completado.\n");
        }


        [CommandMethod("DRAWINDCAD_Finalize")]
        public static void DRAWINDCAD_Message()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                messenger,
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);

            if (Configuration.Config.EXTDIMCorrigeSeta)
            {
                stepRunner.Run(
                    "Consertando setas das dimensões... ",
                    DRAWINDCAD_ConsertarSetaSeta,
                    "Erro 26",
                    string.Empty,
                    "Descrição: Erro ao tentar consertar as setas das dimensões...\n",
                    "... Completado.\n");
            }

            if (Configuration.Config.EXTCONFIsPurge)
            {
                stepRunner.Run(
                    "Purgando desenho... ",
                    () =>
                    {
                        ConvertLayer.PurgeUnreferencedBlocks();
                        ConvertLayer.PurgeUnreferencedLineTypes();
                        ConvertLayer.PurgeUnreferencedLayers();
                        ConvertLayer.PurgeDimensionSyles();
                        ConvertLayer.PurgeTextSyles();
                    },
                    "Erro 28",
                    "Não foi possível remover layers, blocos e tipo de linhas desnessessario .nVerifique se a conversão ocorreu normalmente.",
                    "Descrição: Erro ao tentar purgar o desenho...\n",
                    "... Completado.\n");
            }

            TimeSpan ts = DateTime.Now.Subtract(timeini);
            editor.Regen();
            messenger.WriteMessage("\nConversão: " + conversor + "\tUsuário: " + Environment.UserName + "\tTempo: " + ts.Hours + "h:" + ts.Minutes + "mm:" + ts.Seconds + "s:" + ts.Milliseconds + "ms\n");
            messenger.WriteMessage("Conversor Drawind 2011 @ 2016 - Versão 2016 - Drawind do Brasil Corporação Limitada. Todos os direitos reservados.\n");
            messenger.WriteMessage("Desenvolvido por Nayara Ferreira de Jesus.\n");
            messenger.WriteMessage("Conversão finalizada.\n");
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
                                        if(ent2.GetType() == typeof(Line))
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
                        Conversor.EscreverLog("Erro 30", e.Message);
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
             [CommandMethod("tteste")]
        public static void FindTextsInBlock()
        {
            GETREALMAXMIN();
            GETSCALE();
        }

        private static bool ChecarEscala(DBText text, Point3d positionInSpace)
        {
            if (!string.Equals(text.Layer, Configuration.Config.EXTSCALELayer, StringComparison.OrdinalIgnoreCase))
                return false;
            if (ConvertBlocks.CheckPoint(positionInSpace,
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
                    {
                        escalaCapiturada = escalaConvertida;
                        return true;
                    }

                }
            }
            return false;
        }

        [CommandMethod("DRAWINDCAD_GetPoint")]
        public static void DRAWINDCAD_GetPoint()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CapturePoint();
        }

        [CommandMethod("DRAWINDCAD_Get2Point")]
        public static void DRAWINDCAD_Get2Point()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureTwoPoints();
        }


        [CommandMethod("DRAWINDCAD_GetLayer")]
        public static void DRAWINDCAD_GetLayer()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureLayer();
        }

        [CommandMethod("DRAWINDCAD_TextHeight")]
        public static void DRAWINDCAD_TextHeight()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureTextHeight();
        }


        [CommandMethod("DRAWINDCAD_GetDistHorizontal")]
        public static void DRAWINDCAD_GetDistHorizontal()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureHorizontalDistance();
        }

        [CommandMethod("DRAWINDCAD_GetDistVertical")]
        public static void DRAWINDCAD_GetDistVertical()
        {
            new DrawingInfoCommandService(new AcadDocumentContext(), Conversor.EscreverLog).CaptureVerticalDistance();
        }
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
        [CommandMethod("DRAWINDCAD_GetAttributeText")]
        public static void DRAWINDCAD_GetAttributeText()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);

            //editor.WriteMessage("TESTE" + document.Name);
            try
            {
                messenger.WriteMessage("Capturando textos do formato ");
                if (Configuration.Config.ConvTekla0ConvInv1 == 0)
                {
                    ConvertBlocks.SetStartPointOverride(ConvertBlocks.GetFormatStartPoint(Configuration.Config.LayerBlockAttribute));
                    ConvertBlocks.GeTTextNew(Configuration.Config.LayerBlockAttribute);
                    ConvertBlocks.GeTText();
                }
                else
                    ConvertBlocks.GeTTextInv(Arranjos.ListBlocksInv);
                messenger.WriteMessage("... Completado.\n");
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 36", e.Message);
                messenger.WriteMessage("... Erro. \n" +
                            "Descrição: Erro ao capturar os textos no formato...\n");
            }
            finally
            {
                ConvertBlocks.ClearStartPointOverride();
            }

        }

        [CommandMethod("DRAWINDCAD_DeleteLayers")]
        public static void DRAWINDCAD_DeleteLayers()
        {
            if (Arranjos.Arrj.LayerRemove.Count > 0)
            {
                IAcadDocumentContext documentContext = new AcadDocumentContext();
                Database database = documentContext.Database;
                Editor editor = documentContext.Editor;
                IEditorMessenger messenger = new AcadEditorMessenger(editor);
                using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
                {
                    messenger.WriteMessage("Removendo layers desnecessários... ");
                    try
                    {
                        ConvertBlocks.DeleteLayerNew(Arranjos.Arrj.LayerRemove);
                        ConvertBlocks.DeleteLayer(Arranjos.Arrj.LayerRemove);
                        messenger.WriteMessage("... Completado.\n");
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 37", e.Message);
                    }

                    finally
                    {
                        acTrans.MyCommit();
                    }
                }
            }
        }

        [CommandMethod("DRAWINDCAD_DeleteBlocks")]
        public static void DRAWINDCAD_DeleteBlocks()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                messenger.WriteMessage("Removendo blocos antigo.... ");
                try
                {
                    ConvertBlocks.DeleteBlocks(Arranjos.ListBlocksOrig);
                    messenger.WriteMessage("... Completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 38", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        [CommandMethod("DRAWINDCAD_AttributeBlock")]
        public static void DRAWINDCAD_AttributeBlock()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            IEditorMessenger messenger = new AcadEditorMessenger(editor);
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (Configuration.Config.ConvTekla0ConvInv1 == 0)
                        ConvertBlocks.SetText(Arranjos.ListBlocks);
                    else
                        ConvertBlocks.SetText2(Arranjos.ListBlocksInv, Arranjos.ListBlocksOrig);

                    messenger.WriteMessage("Editando o novo bloco ... Completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 39", e.Message);
                    messenger.WriteMessage("Editando o novo bloco ... Erro. \n" +
                                    "Descrição: Erro ao editar o novo bloco...\n");
                    if (Configuration.Config.PROGRAMMessage)
                    {
                        string nomeBlocos = "";
                        foreach (var item in Arranjos.ListBlocks)
                        {
                            nomeBlocos = nomeBlocos + item.blockName + ", ";
                        }
                        nomeBlocos = nomeBlocos.Trim();
                        nomeBlocos = nomeBlocos.Trim(',');
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível atributar o formato. \nOs blocos ou atributos dentro dos blocos não correspondem ao especificado.\nNomes dos blocos especificados: " + nomeBlocos + ".",
                                             "Erro",
                                             FORMS.MessageBoxButtons.OK,
                                             FORMS.MessageBoxIcon.Warning,
                                             FORMS.MessageBoxDefaultButton.Button1);
                    }

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [CommandMethod("DRAWINDCAD_LoadLayer")]
        public void DRAWINDCAD_LoadLayer()
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

        [CommandMethod("DRAWINDCAD_LoadLineType")]
        public void DRAWINDCAD_LoadLineType()
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


        [CommandMethod("DRAWINDCAD_NewLayer")]
        public void DRAWINDCAD_NewLayer()
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
                    file += "TempImporNewLayer.Temp";
                    if (File.Exists(file))
                        File.Delete(file);

                    StreamWriter streamWriter = new StreamWriter(file, true);

                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                    foreach (ObjectId item in layerTable)
                    {
                        LayerTableRecord ltr = (LayerTableRecord)transaction.GetObject(item, OpenMode.ForRead);
                        LinetypeTableRecord linetypeTableRecord = transaction.GetObject(ltr.LinetypeObjectId, OpenMode.ForRead) as LinetypeTableRecord;

                        string mynewlayer = ltr.Name + ":" +
                                            ltr.Color + ":" +
                                            linetypeTableRecord.Name;

                        streamWriter.WriteLine(mynewlayer);
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


        [CommandMethod("DRAWINDCAD_GetBlocks")]
        public void DRAWINDCAD_GetBlocks()
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
                        ObjectId[] objectIdList = Filter1(editor);
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
        public static void DRAWINDCAD_ConsertarSetaSeta()
        {
            try
            {
                FixArrow.ConsetaSetaSeta(Configuration.Config.EXTDIMCorrigeSetaTipoSeta, escalaFinal, Configuration.Config.EXTDIMCorrigeSetaFactor);
            }

            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 40", e.Message);
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
