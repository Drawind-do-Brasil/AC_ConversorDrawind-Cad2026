using ConversorDrawind.Commands.Services.Styles;

namespace ConversorDrawind.Commands
{
    internal sealed class ConversionWorkflow
    {
        private readonly ConversionStepRunner stepRunner;
        private readonly ScaleWorkflow scaleWorkflow;
        private readonly Configuration configuration;

        internal ConversionWorkflow(
            ConversionStepRunner stepRunner,
            ScaleWorkflow scaleWorkflow,
            Configuration configuration)
        {
            this.stepRunner = stepRunner;
            this.scaleWorkflow = scaleWorkflow;
            this.configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        internal void CreateLayersIfEnabled()
        {
            if (!configuration.General.ConvertLayers)
                return;

            stepRunner.Run(
                Localization.StartCreatingLayers,
                () =>
                {
                    if (configuration.General.ConverterType == 0 && configuration.Lines.LineTypeScale != 0)
                        scaleWorkflow.ApplyLineTypeScale(configuration.Lines.LineTypeScale);
                    LayerSetupOperations.CreateAndAssignALayer();
                },
                LogContext.PrepararConversao,
                Localization.WarningCouldNotCreateLayers,
                Localization.ErrorCreatingLayers + "\n");
        }

        internal void CreateTextStylesIfNeeded()
        {
            if (!configuration.General.ConvertLayers &&
                !configuration.General.ConvertDimensions &&
                configuration.General.ConverterType != 1)
                return;

            stepRunner.Run(
                Localization.StartCreatingTextStyles,
                StyleOperations.CreateTextSyles,
                LogContext.PrepararConversao,
                Localization.WarningCouldNotCreateTextStyles,
                Localization.ErrorCreatingTextStyles + "\n");
        }

        internal void ConvertDimensionsIfEnabled()
        {
            if (!configuration.General.ConvertDimensions || configuration.General.ConverterType != 0)
                return;

            stepRunner.Run(
                Localization.StartConvertingDimensions,
                () =>
                {
                    scaleWorkflow.ApplyDimensionScale(configuration.Dimensions.Scale);
                    new ConvertDimension(configuration).ConvertByTekla();
                },
                LogContext.CarregarConfiguracaoTemporaria,
                Localization.WarningCouldNotConvertDimensions,
                Localization.ErrorConvertingDimensions + "\n");
        }

        internal void RunTeklaInverseConversionIfNeeded()
        {
            if (configuration.General.ConverterType != 1)
                return;

            stepRunner.Run(
                Localization.StartExplodingBlocks,
                EntityExplosionWorkflow.ExplodeObjectsInv,
                LogContext.CarregarLayer,
                Localization.WarningCouldNotExplodeBlocks,
                Localization.ErrorExplodingBlocks + "\n");

            stepRunner.Run(
                Localization.StartConvertingDimensions,
                () => new ConvertDimension(configuration).ConvertByInventor(),
                LogContext.CarregarLinetype,
                Localization.WarningCouldNotConvertDimensions,
                Localization.ErrorConvertingDimensions + "\n");
        }

        internal void ExplodeBlocksIfConfigured()
        {
            if ((!configuration.General.DeleteTeklaStructures &&
                 !configuration.General.ConvertLayers &&
                 !configuration.General.ApplyDrawingScale) ||
                !configuration.General.ExplodeBlocks ||
                configuration.General.ConverterType != 0)
                return;

            stepRunner.Run(
                Localization.StartExplodingBlocks,
                EntityExplosionWorkflow.ExplodeObjects,
                LogContext.CriarBloco,
                Localization.WarningCouldNotExplodeBlocks,
                Localization.ErrorExplodingBlocks + "\n");
        }

        internal void AddDmBlockIfEnabled()
        {
            if (!configuration.Blocks.DimensionBlockEnabled)
                return;

            stepRunner.Run(
                Localization.StartAddingDmBlock,
                () =>
                {
                    IAcadDocumentContext documentContext = new AcadDocumentContext();
                    new DmBlockService(documentContext, new AcadEntitySelector(documentContext.Editor)).AddOrUpdate();
                },
                LogContext.PrepararConversao,
                Localization.WarningCouldNotAddDmBlock,
                Localization.ErrorAddingDmBlock + "\n");
        }

        internal void DeleteTeklaStructuresIfEnabled()
        {
            if (!configuration.General.DeleteTeklaStructures || configuration.General.ConverterType != 0)
                return;

            stepRunner.Run(
                Localization.StartDeletingTeklaStructuresWord,
                () =>
                {
                    DrawingTransformOperations.DeletingTekla(configuration.Layers.TeklaDrawingSheetLayer);
                    DrawingTransformOperations.DeletingTekla();
                },
                LogContext.FinalizarConversao,
                string.Empty,
                Localization.ErrorDeletingTeklaStructuresWord + "\n");
        }

        internal void ConvertLayersIfEnabled()
        {
            if (!configuration.General.ConvertLayers)
                return;

            stepRunner.Run(
                Localization.StartConvertingLayers,
                LayerConversionWorkflow.ConvertLayers,
                LogContext.PublicarArquivo,
                Localization.WarningCouldNotConvertLayers,
                Localization.ErrorConvertingLayers + "\n");
        }
    }
}
