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
            if (!Directory.Exists(LOG_Diretorio))
                Directory.CreateDirectory(LOG_Diretorio);
            StreamWriter sw = File.AppendText(LOG_FileName);
            sw.WriteLine(log + " : " + erro);
            sw.Close();
        }
        public static void MoveElements(Point3d startPoint, Point3d endPoint)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Selecionar todos os elementos no espaço de modelo
                PromptSelectionResult selectionResult = ed.SelectAll();
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
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

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
                PromptSelectionResult selectionResult = ed.SelectAll();
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

            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            editor.WriteMessage("\nConversor Drawind 2011 @ 2016 - Versão 2016 - Drawind do Brasil Corporação Limitada. Todos os direitos reservados.\n");
            editor.WriteMessage("Desenvolvido por Nayara Ferreira de Jesus.\n");
            editor.WriteMessage("Compatível com Autocad 2023.\n");

            try
            {
                editor.WriteMessage("Extraindo os blocos ");
                InitialConversionLayer();
                editor.WriteMessage("... completado.\n");
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 14", e.Message);
                if (Configuration.Config.PROGRAMMessage)
                    FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                        "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.",
                                      "Erro",
                                      FORMS.MessageBoxButtons.OK,
                                      FORMS.MessageBoxIcon.Warning,
                                      FORMS.MessageBoxDefaultButton.Button1);
                editor.WriteMessage("... Erro. \n" +
                                    "Descrição: Erro ao extrair os layers dos blocos...\n");
            }

            GETREALMAXMIN();
            object ptMax = GetNewMax();
            object ptMin = GetNewMin();
            ConvertLayer.Zoom((Point3d)ptMax, (Point3d)ptMin);

          
            if (LOG_Diretorio == "")
            {
                LOG_Diretorio = Path.GetDirectoryName(document.Name);
                LOG_FileName = Path.Combine(LOG_Diretorio, "Conversor.log");
                StreamWriter sw = File.AppendText(LOG_FileName);
                sw.WriteLine("Log de erros internos da conversão: " + Environment.UserDomainName + " " + Environment.UserName + " " + DateTime.Now);
                sw.Close();
            }
            {
                StreamWriter sw = File.AppendText(LOG_FileName);
                sw.WriteLine("Drawing: " + document.Name);
                sw.Close();
            }




            string arqtemp = Path.GetTempPath();
            if (!Directory.Exists(arqtemp))
                Directory.CreateDirectory(arqtemp);
            arqtemp = Path.Combine(arqtemp, "ConversorDrawind.Temp");

            if (File.Exists(arqtemp))
            {

                try
                {
                    StreamReader sr = new StreamReader(arqtemp);

                    string file = sr.ReadLine();
                    sr.Close();
                    conversor = Path.GetFileNameWithoutExtension(file);
                    Configuration.Config.LoadXML(file);
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 12", e.Message);
                }
            }

            GETSCALE();
            //idLayer = ConvertLayer.CreateAndAssignALayer(Configuration.Config.EXTDIMlayer);

           
       
      

     
            Application.SetSystemVariable("DWGCHECK", 1);

            try
            {
                if (Configuration.Config.EXTCONFIsConvertLayer)
                {
                    editor.WriteMessage("Criando novos layers ");
                    if (Configuration.Config.ConvTekla0ConvInv1 == 0 && Configuration.Config.EXTLINELtscale != 0)
                        Application.SetSystemVariable("LTSCALE", Configuration.Config.EXTLINELtscale);
                    ConvertLayer.CreateAndAssignALayer();
                    editor.WriteMessage("... completado.\n");
                }
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 13", e.Message);
                if (Configuration.Config.PROGRAMMessage)
                    FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                                      "Não foi possível criar os novos layers.\nVerifique se a conversão ocorreu normalmente.",
                                      "Erro",
                                      FORMS.MessageBoxButtons.OK,
                                      FORMS.MessageBoxIcon.Warning,
                                      FORMS.MessageBoxDefaultButton.Button1);
                editor.WriteMessage("... Erro. \n" +
                                    "Descrição: Erro ao criar os novos layers...\n");
            }
            try
            {
                if (Configuration.Config.EXTCONFIsConvertLayer || Configuration.Config.EXTCONFIsConvertDimension || Configuration.Config.ConvTekla0ConvInv1 == 1)
                {
                    editor.WriteMessage("Criando novos estilos de textos ");
                    ConvertLayer.CreateTextSyles();
                    editor.WriteMessage("... completado.\n");
                }
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 13", e.Message);
                if (Configuration.Config.PROGRAMMessage)
                    FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                                      "Não foi possível criar os novos estilos de textos.\nVerifique se a conversão ocorreu normalmente.",
                                      "Erro",
                                      FORMS.MessageBoxButtons.OK,
                                      FORMS.MessageBoxIcon.Warning,
                                      FORMS.MessageBoxDefaultButton.Button1);
                editor.WriteMessage("... Erro. \n" +
                                    "Descrição: Erro ao criar os novos  estilos de textos...\n");
            }
           
            try
            {
                editor.WriteMessage("Movendo para origem ");
       
                    
                MoveToOrigin();
                NewMax.X = NewMax.X - NewMin.X;
                NewMax.Y = NewMax.Y - NewMin.Y;
                NewMax.Z = NewMax.Z - NewMin.Z;

                NewMin = new myPoint(0, 0, 0);
                ptMax = GetNewMax();
                ptMin = GetNewMin();
                ConvertLayer.Zoom((Point3d)ptMax, (Point3d)ptMin);


                editor.WriteMessage("... completado.\n");

            }
            catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 14", e.Message);
                    if (Configuration.Config.PROGRAMMessage)
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.",
                                          "Erro",
                                          FORMS.MessageBoxButtons.OK,
                                          FORMS.MessageBoxIcon.Warning,
                                          FORMS.MessageBoxDefaultButton.Button1);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao extrair os layers dos blocos...\n");
                }
            

            if (Configuration.Config.EXTCONFIsConvertDimension && Configuration.Config.ConvTekla0ConvInv1 == 0)
            {
                try
                {
                    editor.WriteMessage("Convertendo as dimensões ");
                    Application.SetSystemVariable("DIMSCALE", Configuration.Config.EXTDIMScale);
                    new ConvertDimension().ConvertD();
                    editor.WriteMessage("... completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 15", e.Message);
                    if (Configuration.Config.PROGRAMMessage)
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível converter as dimensões.\nVerifique se a conversão ocorreu normalmente.",
                                          "Erro",
                                          FORMS.MessageBoxButtons.OK,
                                          FORMS.MessageBoxIcon.Warning,
                                          FORMS.MessageBoxDefaultButton.Button1);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao convertar as dimensões...\n");
                }

            }

            if (Configuration.Config.ConvTekla0ConvInv1 == 1)
            {
                {
                    try
                    {
                        editor.WriteMessage("Explodindo os blocos ");
                        ConvertLayer.ExplodeObjectsInv();
                        editor.WriteMessage("... completado.\n");
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 16", e.Message);
                        if (Configuration.Config.PROGRAMMessage)
                            FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                                "Não foi possível explodir os blocos.\nVerifique se a conversão ocorreu normalmente.",
                                              "Erro",
                                              FORMS.MessageBoxButtons.OK,
                                              FORMS.MessageBoxIcon.Warning,
                                              FORMS.MessageBoxDefaultButton.Button1);
                        editor.WriteMessage("... Erro. \n" +
                                            "Descrição: Erro ao explodir os blocos...\n");
                    }
                }
                try
                {
                    editor.WriteMessage("Convertendo as dimensões ");
                    new ConvertDimension().ConvertDInv();
                    editor.WriteMessage("... completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 17", e.Message);
                    if (Configuration.Config.PROGRAMMessage)
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível converter as dimensões.\nVerifique se a conversão ocorreu normalmente.",
                                          "Erro",
                                          FORMS.MessageBoxButtons.OK,
                                          FORMS.MessageBoxIcon.Warning,
                                          FORMS.MessageBoxDefaultButton.Button1);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao convertar as dimensões...\n");
                }
            }


            if ((Configuration.Config.EXTCONFIsDeleteTeklaStructures ||
                 Configuration.Config.EXTCONFIsConvertLayer ||
                 Configuration.Config.EXTCONFIsPutOnTheScaleDrawing) &&
                 Configuration.Config.ExplodeBlocks)
            {
                if (Configuration.Config.ConvTekla0ConvInv1 == 0)
                {
                    try
                    {
                        editor.WriteMessage("Explodindo os blocos ");
                        ConvertLayer.ExplodeObjects();
                        editor.WriteMessage("... completado.\n");
                    }
                    catch (System.Exception e)
                    {
                        Conversor.EscreverLog("Erro 18", e.Message);
                        if (Configuration.Config.PROGRAMMessage)
                            FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                                "Não foi possível explodir os blocos.\nVerifique se a conversão ocorreu normalmente.",
                                              "Erro",
                                              FORMS.MessageBoxButtons.OK,
                                              FORMS.MessageBoxIcon.Warning,
                                              FORMS.MessageBoxDefaultButton.Button1);
                        editor.WriteMessage("... Erro. \n" +
                                            "Descrição: Erro ao explodir os blocos...\n");
                    }
                }

            }

            if (Configuration.Config.DMBlock)
            {
                try
                {
                    editor.WriteMessage("Adicionando bloco DM ");

                    DocumentManager.AddBlockDM();

                    editor.WriteMessage("... completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 508", e.Message);
                    if (Configuration.Config.PROGRAMMessage)
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível adicionar o bloco DM.\nVerifique se a conversão ocorreu normalmente.",
                                          "Erro",
                                          FORMS.MessageBoxButtons.OK,
                                          FORMS.MessageBoxIcon.Warning,
                                          FORMS.MessageBoxDefaultButton.Button1);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao adicionar bloco DM...\n");
                }

            }
            if (Configuration.Config.EXTCONFIsDeleteTeklaStructures && Configuration.Config.ConvTekla0ConvInv1 == 0)
            {
                try
                {
                    editor.WriteMessage("Excluindo a palavra \"Tekla structures\" ");
                    ConvertLayer.DeletingTekla(Configuration.Config.LayerTeklaString);
                    ConvertLayer.DeletingTekla();
                    editor.WriteMessage("... completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 19", e.Message);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao excluir a palavra \"Tekla structures\"...\n");
                }
            }

            if (Configuration.Config.EXTCONFIsConvertLayer)
            {
                try
                {
                    editor.WriteMessage("Convertendo os layers ");
                    ConvertLayer.ConvertLayersNew();

                    editor.WriteMessage("... completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 20", e.Message);
                    if (Configuration.Config.PROGRAMMessage)
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível converter os layers.\nVerifique se a conversão ocorreu normalmente.",
                                          "Erro",
                                          FORMS.MessageBoxButtons.OK,
                                          FORMS.MessageBoxIcon.Warning,
                                          FORMS.MessageBoxDefaultButton.Button1);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao converter os layers...\n");
                }
            }



        }


        public static void UPDATE_DIMENSTION(double dscale)
        {

            ObjectId[] ids = ConvertLayer.Filter("ALL", "DIMENSION", "ALL", "ALL");
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;

            Database database = document.Database;
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

            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            try
            {

                editor.WriteMessage("Colocando o desenho na escala real... ");


                escalaFinal = ConvertLayer.GetScaleDrawing(escalaCapiturada);

                ConvertLayer.ScaleDrawing(escalaFinal);
                if (Configuration.Config.ConvTekla0ConvInv1 == 1)
                    UPDATE_DIMENSTION(escalaFinal);
                Application.SetSystemVariable("LTSCALE", Configuration.Config.EXTLINELtscale * escalaFinal);
                Application.SetSystemVariable("DIMSCALE", Configuration.Config.EXTDIMScale * escalaFinal);
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
                editor.WriteMessage("... Completado.\n");

            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 23", e.Message);
                if (Configuration.Config.PROGRAMMessage)
                    FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                        "Não foi possível colocar o desenho na escala real!",
                                         "Erro",
                                         FORMS.MessageBoxButtons.OK,
                                         FORMS.MessageBoxIcon.Warning,
                                         FORMS.MessageBoxDefaultButton.Button1);
                editor.WriteMessage("... Erro. \n" +
                                    "Descrição: Erro ao tentar colocar o desenho na escala real...\n");
            }


        }

        [CommandMethod("DRAWINDCAD_ScaleBlock")]
        public static void DRAWINDCAD_ConvertToScaleInv()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;



            try
            {
                string newdate = editor.GetString("Digite o nome do bloco: ").StringResult.Replace("*******", " ");
                editor.WriteMessage("Colocando o formato na escala real... ");
                double scale = escalaFinal = Convert.ToDouble(Application.GetSystemVariable("LTSCALE"));
                ConvertLayer.ScaleDrawingInv(scale, new List<BlockClass>() { new BlockClass(newdate) });
                object ptMax2 = GetNewMax();
                object ptMin2 = GetNewMin();
                ConvertLayer.Zoom((Point3d)ptMin2, (Point3d)ptMax2);
                editor.WriteMessage("... Completado.\n");
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 24", e.Message);
                if (Configuration.Config.PROGRAMMessage)
                    FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                        "Não foi possível colocar o formato na escala real!",
                                         "Erro",
                                         FORMS.MessageBoxButtons.OK,
                                         FORMS.MessageBoxIcon.Warning,
                                         FORMS.MessageBoxDefaultButton.Button1);
                editor.WriteMessage("... Erro. \n" +
                                    "Descrição: Erro ao tentar colocar o formato na escala real...\n");
            }
        }


        [CommandMethod("DRAWINDCAD_Finalize")]
        public static void DRAWINDCAD_Message()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;

            if (Configuration.Config.EXTDIMCorrigeSeta)
            {
                try
                {
                    editor.WriteMessage("Consertando setas das dimensões... ");
                    DRAWINDCAD_ConsertarSetaSeta();
                    editor.WriteMessage("... Completado.\n");

                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 26", e.Message);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao tentar consertar as setas das dimensões...\n");
                }
            }

            if (Configuration.Config.EXTCONFIsPurge)
            {
                try
                {
                    editor.WriteMessage("Purgando desenho... ");
                    ConvertLayer.PurgeUnreferencedBlocks();
                    ConvertLayer.PurgeUnreferencedLineTypes();
                    ConvertLayer.PurgeUnreferencedLayers();
                    ConvertLayer.PurgeDimensionSyles();
                    ConvertLayer.PurgeTextSyles();
                    editor.WriteMessage("... Completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 28", e.Message);
                    if (Configuration.Config.PROGRAMMessage)
                        FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                            "Não foi possível remover layers, blocos e tipo de linhas desnessessario .nVerifique se a conversão ocorreu normalmente.",
                                             "Erro",
                                             FORMS.MessageBoxButtons.OK,
                                             FORMS.MessageBoxIcon.Warning,
                                             FORMS.MessageBoxDefaultButton.Button1);
                    editor.WriteMessage("... Erro. \n" +
                                        "Descrição: Erro ao tentar purgar o desenho...\n");
                }
            }

            TimeSpan ts = DateTime.Now.Subtract(timeini);
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
            editor.WriteMessage("\nConversão: " + conversor + "\tUsuário: " + Environment.UserName + "\tTempo: " + ts.Hours + "h:" + ts.Minutes + "mm:" + ts.Seconds + "s:" + ts.Milliseconds + "ms\n");
            editor.WriteMessage("Conversor Drawind 2011 @ 2016 - Versão 2016 - Drawind do Brasil Corporação Limitada. Todos os direitos reservados.\n");
            editor.WriteMessage("Desenvolvido por Nayara Ferreira de Jesus.\n");
            editor.WriteMessage("Conversão finalizada.\n");
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
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

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
                PromptSelectionResult selectionResult = ed.SelectAll(filter);
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
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

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
                PromptSelectionResult selectionResult = ed.SelectAll(filter);
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
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

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
                PromptSelectionResult selectionResult = ed.SelectAll(filter);
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            Database acCurDb = document.Database;

            TypedValue[] typedValue = new TypedValue[1];
            typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);

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
            if (text.Layer.ToUpper() != Configuration.Config.EXTSCALELayer.ToUpper())
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptPointOptions promptPointOptions = new PromptPointOptions("Selecione um ponto: ");
                    promptPointOptions.AllowNone = false;

                    PromptPointResult promptPointResult = editor.GetPoint(promptPointOptions);

                    if (promptPointResult.Status.ToString() == "OK")
                    {
                        Point3d myPoint = promptPointResult.Value;

                        string arq = Path.GetTempPath();
                        if (!Directory.Exists(arq))
                            Directory.CreateDirectory(arq);
                        arq = Path.Combine(arq, "ConvertTo.PointInfo");

                        //string arq = "C:\\ConvertTo.PointInfo";
                        if (File.Exists(arq))
                            File.Delete(arq);

                        StreamWriter sw = new StreamWriter(arq);
                        sw.WriteLine(myPoint.X + ";" + myPoint.Y + ";" + myPoint.Z);
                        sw.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 31", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        [CommandMethod("DRAWINDCAD_Get2Point")]
        public static void DRAWINDCAD_Get2Point()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            Point3d PTa;
            Point3d PTb;
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {

                    PTa = editor.GetPoint("Selecione o primeiro ponto: ").Value;
                    PTb = editor.GetCorner("\nSelecione o segundo ponto: ", PTa).Value;

                    //string arq = "C:\\ConvertTo.Point2Info";
                    string arq = Path.GetTempPath();
                    if (!Directory.Exists(arq))
                        Directory.CreateDirectory(arq);
                    arq = Path.Combine(arq, "ConvertTo.Point2Info");

                    if (File.Exists(arq))
                        File.Delete(arq);

                    StreamWriter sw = new StreamWriter(arq);
                    sw.WriteLine(PTa.X + ";" + PTa.Y + ";" + PTa.Z);
                    sw.WriteLine(PTb.X + ";" + PTb.Y + ";" + PTb.Z);
                    sw.Close();

                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 32", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }

        }


        [CommandMethod("DRAWINDCAD_GetLayer")]
        public static void DRAWINDCAD_GetLayer()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;


            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptEntityOptions promptEntityOptions = new PromptEntityOptions("Selecione um objeto: ");
                    promptEntityOptions.AllowNone = false;
                    promptEntityOptions.SetRejectMessage("");
                    PromptEntityResult promptEntityResult = editor.GetEntity(promptEntityOptions);
                    Entity dBObject = acTrans.GetObject(promptEntityResult.ObjectId, OpenMode.ForRead) as Entity;
                    if (dBObject.Layer != "")
                    {
                        //string arq = "C:\\ConvertTo.LayerInfo";

                        string arq = Path.GetTempPath();
                        if (!Directory.Exists(arq))
                            Directory.CreateDirectory(arq);
                        arq = Path.Combine(arq, "ConvertTo.LayerInfo");

                        if (File.Exists(arq))
                            File.Delete(arq);

                        StreamWriter sw = new StreamWriter(arq);
                        sw.WriteLine(dBObject.Layer);
                        sw.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 33", e.Message);
                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        [CommandMethod("DRAWINDCAD_TextHeight")]
        public static void DRAWINDCAD_TextHeight()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;


            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    PromptEntityOptions promptEntityOptions = new PromptEntityOptions("Selecione um objeto: ");
                    promptEntityOptions.AllowNone = false;
                    promptEntityOptions.SetRejectMessage("");
                    PromptEntityResult promptEntityResult = editor.GetEntity(promptEntityOptions);
                    Entity dBObject = acTrans.GetObject(promptEntityResult.ObjectId, OpenMode.ForRead) as Entity;
                    if (dBObject.GetType() == typeof(DBText))
                    {
                        DBText myText = dBObject as DBText;

                        //string arq = "C:\\ConvertTo.TextHeightInfo";
                        string arq = Path.GetTempPath();
                        if (!Directory.Exists(arq))
                            Directory.CreateDirectory(arq);
                        arq = Path.Combine(arq, "ConvertTo.TextHeightInfo");


                        if (File.Exists(arq))
                            File.Delete(arq);

                        StreamWriter sw = new StreamWriter(arq);
                        sw.WriteLine(myText.Height);
                        sw.Close();

                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 34", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }


        [CommandMethod("DRAWINDCAD_GetDistHorizontal")]
        public static void DRAWINDCAD_GetDistHorizontal()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {

                    PromptPointOptions promptPointOptions = new PromptPointOptions("Selecione o 1° ponto: ");
                    promptPointOptions.AllowNone = false;

                    PromptPointResult promptPointResult = editor.GetPoint(promptPointOptions);

                    PromptPointOptions promptPointOptions2 = new PromptPointOptions("Selecione o 2° ponto: ");
                    promptPointOptions2.AllowNone = false;

                    PromptPointResult promptPointResult2 = editor.GetPoint(promptPointOptions2);

                    if (promptPointResult.Status.ToString() == "OK" && promptPointResult2.Status.ToString() == "OK")
                    {
                        Point3d myPoint = promptPointResult.Value;
                        Point3d myPoint2 = promptPointResult2.Value;

                        //string arq = "C:\\ConvertTo.DistInfo";

                        string arq = Path.GetTempPath();
                        if (!Directory.Exists(arq))
                            Directory.CreateDirectory(arq);
                        arq = Path.Combine(arq, "ConvertTo.DistInfo");

                        if (File.Exists(arq))
                            File.Delete(arq);

                        StreamWriter sw = new StreamWriter(arq);
                        double dist;

                        if (myPoint.X < myPoint2.X)
                            dist = myPoint2.X - myPoint.X;
                        else
                            dist = myPoint.X - myPoint2.X;

                        sw.WriteLine(dist);
                        sw.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 35", e.Message);

                }
                finally
                {
                    acTrans.MyCommit();
                }
            }
        }

        [CommandMethod("DRAWINDCAD_GetDistVertical")]
        public static void DRAWINDCAD_GetDistVertical()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {

                    PromptPointOptions promptPointOptions = new PromptPointOptions("Selecione o 1° ponto: ");
                    promptPointOptions.AllowNone = false;

                    PromptPointResult promptPointResult = editor.GetPoint(promptPointOptions);

                    PromptPointOptions promptPointOptions2 = new PromptPointOptions("Selecione o 2° ponto: ");
                    promptPointOptions2.AllowNone = false;

                    PromptPointResult promptPointResult2 = editor.GetPoint(promptPointOptions2);

                    if (promptPointResult.Status.ToString() == "OK" && promptPointResult2.Status.ToString() == "OK")
                    {
                        Point3d myPoint = promptPointResult.Value;
                        Point3d myPoint2 = promptPointResult2.Value;
                        //string arq = "C:\\ConvertTo.DistInfo";

                        string arq = Path.GetTempPath();
                        if (!Directory.Exists(arq))
                            Directory.CreateDirectory(arq);
                        arq = Path.Combine(arq, "ConvertTo.DistInfo");

                        if (File.Exists(arq))
                            File.Delete(arq);

                        StreamWriter sw = new StreamWriter(arq);
                        double dist;

                        if (myPoint.Y < myPoint2.Y)
                            dist = myPoint2.Y - myPoint.Y;
                        else
                            dist = myPoint.Y - myPoint2.Y;

                        sw.WriteLine(dist);
                        sw.Close();
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
        [CommandMethod("SaveDXF")]
        public static void SaveDXF()
        {
            Document document;
            Database database;
            document = Application.DocumentManager.MdiActiveDocument;
            database = document.Database;
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;

            //editor.WriteMessage("TESTE" + document.Name);
            try
            {
                editor.WriteMessage("Capturando textos do formato ");
                if (Configuration.Config.ConvTekla0ConvInv1 == 0)
                {
                    ConvertBlocks.SetStartPointOverride(ConvertBlocks.GetFormatStartPoint(Configuration.Config.LayerBlockAttribute));
                    ConvertBlocks.GeTTextNew(Configuration.Config.LayerBlockAttribute);
                    ConvertBlocks.GeTText();
                }
                else
                    ConvertBlocks.GeTTextInv(Arranjos.ListBlocksInv);
                editor.WriteMessage("... Completado.\n");
            }
            catch (System.Exception e)
            {
                Conversor.EscreverLog("Erro 36", e.Message);
                editor.WriteMessage("... Erro. \n" +
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
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;
                Editor editor = document.Editor;
                using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
                {
                    editor.WriteMessage("Removendo layers desnecessários... ");
                    try
                    {
                        ConvertBlocks.DeleteLayerNew(Arranjos.Arrj.LayerRemove);
                        ConvertBlocks.DeleteLayer(Arranjos.Arrj.LayerRemove);
                        editor.WriteMessage("... Completado.\n");
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                editor.WriteMessage("Removendo blocos antigo.... ");
                try
                {
                    ConvertBlocks.DeleteBlocks(Arranjos.ListBlocksOrig);
                    editor.WriteMessage("... Completado.\n");
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
            using (Transaction acTrans = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    if (Configuration.Config.ConvTekla0ConvInv1 == 0)
                        ConvertBlocks.SetText(Arranjos.ListBlocks);
                    else
                        ConvertBlocks.SetText2(Arranjos.ListBlocksInv, Arranjos.ListBlocksOrig);

                    editor.WriteMessage("Editando o novo bloco ... Completado.\n");
                }
                catch (System.Exception e)
                {
                    Conversor.EscreverLog("Erro 39", e.Message);
                    editor.WriteMessage("Editando o novo bloco ... Erro. \n" +
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
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
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;
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
                TypedValue[] typedValue = new TypedValue[1];
                typedValue.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                SelectionFilter selectionFilter = new SelectionFilter(typedValue);
                ObjectId[] objectIdList = editor.SelectAll(selectionFilter).Value.GetObjectIds();
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
