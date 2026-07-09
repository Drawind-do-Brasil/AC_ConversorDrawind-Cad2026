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
        [CommandMethod("CDwi_Convert")]
        public static void CDwi_ConvertToDimension()
        {
            ConversionSession.Reset();

            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            DrawingExtentsService extentsService = new DrawingExtentsService(
                documentContext,
                entitySelector,
                ConversionSession.DrawingMinPoint,
                ConversionSession.DrawingMaxPoint);
            EntityMoveService moveService = new EntityMoveService(documentContext, entitySelector);
            ConversionCommandRunner commandRunner = new ConversionCommandRunner(
                documentContext,
                ConversionLog.Write,
                ConversionMessages.ShowWarningIfEnabled);
            ConversionCommandContext commandContext = commandRunner.CreateContext();
            Document document = commandContext.DocumentContext.Document;
            ISystemVariableService systemVariables = commandContext.SystemVariables;
            ScaleWorkflow scaleWorkflow = commandContext.ScaleWorkflow;
            ConversionStepRunner stepRunner = commandContext.StepRunner;
            ConversionWorkflow workflow = new ConversionWorkflow(stepRunner, scaleWorkflow);
            ConversionExtentsWorkflow extentsWorkflow = new ConversionExtentsWorkflow(
                extentsService.Refresh,
                () => ConversionSession.MinPoint3d,
                () => ConversionSession.MaxPoint3d,
                () => moveService.MoveAllToOrigin(ConversionSession.MinPoint3d),
                ConversionSession.SetMinPoint,
                ConversionSession.SetMaxPoint);
            commandRunner.WriteStartupBanner(commandContext.Messenger);

            stepRunner.Run(
                Localization.StartExtractingBlocks,
                () => new InitialBlockLayerService(documentContext, entitySelector, ConversionLog.Write).ConvertBlockLayers(),
                LogContext.ConverterDesenho,
                Localization.WarningCouldNotExtractBlockLayers,
                Localization.ErrorExtractingBlockLayers + "\n");

            extentsWorkflow.RefreshAndZoom();


            string logDirectory = ConversionSession.LogDirectory;
            string logFileName = ConversionSession.LogFileName;
            commandRunner.InitializeLogger(document, ref logDirectory, ref logFileName);
            ConversionSession.SetLogFile(logDirectory, logFileName);

            string converterName = ConversionSession.ConverterName;
            commandRunner.LoadTempConfiguration(Configuration.Config, ref converterName);
            ConversionSession.ConverterName = converterName;

            double? capturedScale = new DrawingScaleDetectionService(documentContext, entitySelector).CaptureScale();
            if (capturedScale.HasValue)
                ConversionSession.CapturedScale = capturedScale.Value;

            systemVariables.Set("DWGCHECK", 1);

            workflow.CreateLayersIfEnabled();

            workflow.CreateTextStylesIfNeeded();

            stepRunner.Run(
                Localization.StartMoveToOrigin,
                extentsWorkflow.MoveToOriginAndRefreshZoom,
                LogContext.ConverterDesenho,
                Localization.WarningCouldNotExtractBlockLayers,
                Localization.ErrorExtractingBlockLayers + "\n");


            workflow.ConvertDimensionsIfEnabled();

            workflow.RunTeklaInverseConversionIfNeeded();

            workflow.ExplodeBlocksIfConfigured();

            workflow.AddDmBlockIfEnabled();

            workflow.DeleteTeklaStructuresIfEnabled();

            workflow.ConvertLayersIfEnabled();

        }
    }
}

