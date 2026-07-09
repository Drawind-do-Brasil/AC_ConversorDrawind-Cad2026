namespace ConversorDrawindDLL
{
    internal sealed class ConversionWorkflow
    {
        private readonly ConversionStepRunner stepRunner;
        private readonly ScaleWorkflow scaleWorkflow;

        internal ConversionWorkflow(ConversionStepRunner stepRunner, ScaleWorkflow scaleWorkflow)
        {
            this.stepRunner = stepRunner;
            this.scaleWorkflow = scaleWorkflow;
        }

        internal void CreateLayersIfEnabled()
        {
            if (!Configuration.Config.General.ConvertLayers)
                return;

            stepRunner.Run(
                Localization.StartCreatingLayers,
                () =>
                {
                    if (Configuration.Config.General.ConverterType == 0 && Configuration.Config.Lines.LineTypeScale != 0)
                        scaleWorkflow.ApplyLineTypeScale(Configuration.Config.Lines.LineTypeScale);
                    ConvertLayer.CreateAndAssignALayer();
                },
                LogContext.PrepararConversao,
                Localization.WarningCouldNotCreateLayers,
                Localization.ErrorCreatingLayers + "\n");
        }

        internal void CreateTextStylesIfNeeded()
        {
            if (!Configuration.Config.General.ConvertLayers &&
                !Configuration.Config.General.ConvertDimensions &&
                Configuration.Config.General.ConverterType != 1)
                return;

            stepRunner.Run(
                Localization.StartCreatingTextStyles,
                ConvertLayer.CreateTextSyles,
                LogContext.PrepararConversao,
                Localization.WarningCouldNotCreateTextStyles,
                Localization.ErrorCreatingTextStyles + "\n");
        }

        internal void ConvertDimensionsIfEnabled()
        {
            if (!Configuration.Config.General.ConvertDimensions || Configuration.Config.General.ConverterType != 0)
                return;

            stepRunner.Run(
                Localization.StartConvertingDimensions,
                () =>
                {
                    scaleWorkflow.ApplyDimensionScale(Configuration.Config.Dimensions.Scale);
                    new ConvertDimension().ConvertD();
                },
                LogContext.CarregarConfiguracaoTemporaria,
                Localization.WarningCouldNotConvertDimensions,
                Localization.ErrorConvertingDimensions + "\n");
        }

        internal void RunTeklaInverseConversionIfNeeded()
        {
            if (Configuration.Config.General.ConverterType != 1)
                return;

            stepRunner.Run(
                Localization.StartExplodingBlocks,
                ConvertLayer.ExplodeObjectsInv,
                LogContext.CarregarLayer,
                Localization.WarningCouldNotExplodeBlocks,
                Localization.ErrorExplodingBlocks + "\n");

            stepRunner.Run(
                Localization.StartConvertingDimensions,
                () => new ConvertDimension().ConvertDInv(),
                LogContext.CarregarLinetype,
                Localization.WarningCouldNotConvertDimensions,
                Localization.ErrorConvertingDimensions + "\n");
        }

        internal void ExplodeBlocksIfConfigured()
        {
            if ((!Configuration.Config.General.DeleteTeklaStructures &&
                 !Configuration.Config.General.ConvertLayers &&
                 !Configuration.Config.General.ApplyDrawingScale) ||
                !Configuration.Config.General.ExplodeBlocks ||
                Configuration.Config.General.ConverterType != 0)
                return;

            stepRunner.Run(
                Localization.StartExplodingBlocks,
                ConvertLayer.ExplodeObjects,
                LogContext.CriarBloco,
                Localization.WarningCouldNotExplodeBlocks,
                Localization.ErrorExplodingBlocks + "\n");
        }

        internal void AddDmBlockIfEnabled()
        {
            if (!Configuration.Config.Blocks.DimensionBlockEnabled)
                return;

            stepRunner.Run(
                Localization.StartAddingDmBlock,
                DocumentManager.AddBlockDM,
                LogContext.PrepararConversao,
                Localization.WarningCouldNotAddDmBlock,
                Localization.ErrorAddingDmBlock + "\n");
        }

        internal void DeleteTeklaStructuresIfEnabled()
        {
            if (!Configuration.Config.General.DeleteTeklaStructures || Configuration.Config.General.ConverterType != 0)
                return;

            stepRunner.Run(
                Localization.StartDeletingTeklaStructuresWord,
                () =>
                {
                    ConvertLayer.DeletingTekla(Configuration.Config.Layers.TeklaDrawingSheetLayer);
                    ConvertLayer.DeletingTekla();
                },
                LogContext.FinalizarConversao,
                string.Empty,
                Localization.ErrorDeletingTeklaStructuresWord + "\n");
        }

        internal void ConvertLayersIfEnabled()
        {
            if (!Configuration.Config.General.ConvertLayers)
                return;

            stepRunner.Run(
                Localization.StartConvertingLayers,
                ConvertLayer.ConvertLayersNew,
                LogContext.PublicarArquivo,
                Localization.WarningCouldNotConvertLayers,
                Localization.ErrorConvertingLayers + "\n");
        }
    }
}