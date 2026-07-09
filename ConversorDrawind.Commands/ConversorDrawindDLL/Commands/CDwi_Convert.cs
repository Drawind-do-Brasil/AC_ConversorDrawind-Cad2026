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
[CommandMethod("CDwi_Convert")]
        public static void CDwi_ConvertToDimension()
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
                LogContext.ConverterDesenho,
                "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao extrair os layers dos blocos...\n");

            extentsWorkflow.RefreshAndZoom();


            commandRunner.InitializeLogger(document, ref LOG_Diretorio, ref LOG_FileName);




            commandRunner.LoadTempConfiguration(Configuration.Config, ref conversor);

            GETSCALE();
            //idLayer = ConvertLayer.CreateAndAssignALayer(Configuration.Config.Dimensions.Layer);






            systemVariables.Set("DWGCHECK", 1);

            workflow.CreateLayersIfEnabled();

            workflow.CreateTextStylesIfNeeded();

            stepRunner.Run(
                "Movendo para origem ",
                extentsWorkflow.MoveToOriginAndRefreshZoom,
                LogContext.ConverterDesenho,
                "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao extrair os layers dos blocos...\n");


            workflow.ConvertDimensionsIfEnabled();

            workflow.RunTeklaInverseConversionIfNeeded();

            workflow.ExplodeBlocksIfConfigured();

            workflow.AddDmBlockIfEnabled();

            workflow.DeleteTeklaStructuresIfEnabled();

            workflow.ConvertLayersIfEnabled();



        }
    }
}