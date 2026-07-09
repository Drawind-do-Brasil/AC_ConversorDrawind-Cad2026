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
                "Criando novos layers ",
                () =>
                {
                    if (Configuration.Config.General.ConverterType == 0 && Configuration.Config.Lines.LineTypeScale != 0)
                        scaleWorkflow.ApplyLineTypeScale(Configuration.Config.Lines.LineTypeScale);
                    ConvertLayer.CreateAndAssignALayer();
                },
                "Erro 13",
                "Não foi possível criar os novos layers.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao criar os novos layers...\n");
        }

        internal void CreateTextStylesIfNeeded()
        {
            if (!Configuration.Config.General.ConvertLayers &&
                !Configuration.Config.General.ConvertDimensions &&
                Configuration.Config.General.ConverterType != 1)
                return;

            stepRunner.Run(
                "Criando novos estilos de textos ",
                ConvertLayer.CreateTextSyles,
                "Erro 13",
                "Não foi possível criar os novos estilos de textos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao criar os novos  estilos de textos...\n");
        }

        internal void ConvertDimensionsIfEnabled()
        {
            if (!Configuration.Config.General.ConvertDimensions || Configuration.Config.General.ConverterType != 0)
                return;

            stepRunner.Run(
                "Convertendo as dimensões ",
                () =>
                {
                    scaleWorkflow.ApplyDimensionScale(Configuration.Config.Dimensions.Scale);
                    new ConvertDimension().ConvertD();
                },
                "Erro 15",
                "Não foi possível converter as dimensões.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao convertar as dimensões...\n");
        }

        internal void RunTeklaInverseConversionIfNeeded()
        {
            if (Configuration.Config.General.ConverterType != 1)
                return;

            stepRunner.Run(
                "Explodindo os blocos ",
                ConvertLayer.ExplodeObjectsInv,
                "Erro 16",
                "Não foi possível explodir os blocos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao explodir os blocos...\n");

            stepRunner.Run(
                "Convertendo as dimensões ",
                () => new ConvertDimension().ConvertDInv(),
                "Erro 17",
                "Não foi possível converter as dimensões.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao convertar as dimensões...\n");
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
                "Explodindo os blocos ",
                ConvertLayer.ExplodeObjects,
                "Erro 18",
                "Não foi possível explodir os blocos.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao explodir os blocos...\n");
        }

        internal void AddDmBlockIfEnabled()
        {
            if (!Configuration.Config.Blocks.DimensionBlockEnabled)
                return;

            stepRunner.Run(
                "Adicionando bloco DM ",
                DocumentManager.AddBlockDM,
                "Erro 508",
                "Não foi possível adicionar o bloco DM.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao adicionar bloco DM...\n");
        }

        internal void DeleteTeklaStructuresIfEnabled()
        {
            if (!Configuration.Config.General.DeleteTeklaStructures || Configuration.Config.General.ConverterType != 0)
                return;

            stepRunner.Run(
                "Excluindo a palavra \"Tekla structures\" ",
                () =>
                {
                    ConvertLayer.DeletingTekla(Configuration.Config.Layers.TeklaDrawingSheetLayer);
                    ConvertLayer.DeletingTekla();
                },
                "Erro 19",
                string.Empty,
                "Descrição: Erro ao excluir a palavra \"Tekla structures\"...\n");
        }

        internal void ConvertLayersIfEnabled()
        {
            if (!Configuration.Config.General.ConvertLayers)
                return;

            stepRunner.Run(
                "Convertendo os layers ",
                ConvertLayer.ConvertLayersNew,
                "Erro 20",
                "Não foi possível converter os layers.\nVerifique se a conversão ocorreu normalmente.",
                "Descrição: Erro ao converter os layers...\n");
        }
    }
}
