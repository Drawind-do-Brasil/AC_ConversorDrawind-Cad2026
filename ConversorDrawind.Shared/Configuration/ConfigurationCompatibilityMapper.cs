using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConversorDrawind
{
#pragma warning disable CS0618
    public static class ConfigurationCompatibilityMapper
    {
        public static ConverterConfiguration FromLegacyState(
            Configuration configuration,
            LegacyConfigurationState state)
        {
            ConverterConfiguration result = new ConverterConfiguration();

            result.Comments = configuration.EXTCONFComments;
            result.General.SourceMode = configuration.EXTCONFOrigem;
            result.General.ConverterType = configuration.ConvTekla0ConvInv1;
            result.General.ConvertDimensions = configuration.EXTCONFIsConvertDimension;
            result.General.ConvertLayers = configuration.EXTCONFIsConvertLayer;
            result.General.ExchangeFormat = configuration.EXTCONFIsExchangeFormat;
            result.General.ExchangeLm = configuration.EXTCONFIsExchangeLM;
            result.General.ApplyDrawingScale = configuration.EXTCONFIsPutOnTheScaleDrawing;
            result.General.ExecuteLisp = configuration.EXTCONFIsExecuteLISP;
            result.General.ExecuteDll = configuration.EXTCONFIsExecuteDLL;
            result.General.FirstRunMode = configuration.EXTCONFIsFirstRum;
            result.General.DeleteTeklaStructures = configuration.EXTCONFIsDeleteTeklaStructures;
            result.General.Purge = configuration.EXTCONFIsPurge;
            result.General.ShowMessages = configuration.PROGRAMMessage;
            result.General.ExplodeBlocks = configuration.ExplodeBlocks;
            result.General.InventorExplode = configuration.EXTCONFInventorExplode;

            result.Dimensions.ReferenceFormatSize = Configuration.INTREFTamFormato;
            result.Dimensions.InternalLengthCharFactor = configuration.INTFactorLengthChar;
            result.Dimensions.InternalTextOffset = configuration.INTDIMTextOffset;
            result.Dimensions.Enabled = configuration.EXTDIMGERALHabilit;
            result.Dimensions.Layer = configuration.EXTDIMlayer;
            result.Dimensions.BaseLayer = configuration.EXTDIMBaseLayer;
            result.Dimensions.LineColor = configuration.EXTDIMColorLine;
            result.Dimensions.TextColor = configuration.EXTDIMColorText;
            result.Dimensions.StyleName = configuration.EXTDIMStyleName;
            result.Dimensions.ArrowType = configuration.EXTDIMSeta;
            result.Dimensions.ArrowType1 = configuration.EXTDIMSeta1;
            result.Dimensions.ArrowType2 = configuration.EXTDIMSeta2;
            result.Dimensions.Scale = configuration.EXTDIMScale;
            result.Dimensions.Precision = configuration.EXTDIMPrecision;
            result.Dimensions.AngularPrecision = configuration.EXTDIMAngularPrecision;
            result.Dimensions.Unit = configuration.EXTDIMUnit;
            result.Dimensions.AngularUnit = configuration.EXTDIMAngularUnit;
            result.Dimensions.ArrowSize = configuration.EXTDIMSizeSeta;
            result.Dimensions.TextVerticalPosition = configuration.EXTDIMTad;
            result.Dimensions.TextRelativeToDimensionLine = configuration.EXTDIMDimensionPosition;
            result.Dimensions.ForceTextInside = configuration.EXTDIMTextForced;
            result.Dimensions.ForceDimensionLine = configuration.EXTDIMLineForced;
            result.Dimensions.OffsetLineFromReferencePoint = configuration.EXTDIMOffsetLineFromRefPoint;
            result.Dimensions.TextMove = configuration.EXTDIMTextMove;
            result.Dimensions.OutsideAlign = configuration.EXTDIMOutsideAlign;
            result.Dimensions.ExtensionLineOffset = configuration.EXTDIMDIMEX;
            result.Dimensions.FixArrow = configuration.EXTDIMCorrigeSeta;
            result.Dimensions.FixArrowType = configuration.EXTDIMCorrigeSetaTipoSeta;
            result.Dimensions.FixArrowFactor = configuration.EXTDIMCorrigeSetaFactor;

            result.Text.DefaultStyleName = configuration.EXTTEXTStyleName;
            result.Text.DefaultSize = configuration.EXTTEXTSize;
            result.Text.Styles = state.TextStyles.Select(LegacyConfigurationParsers.ParseTextStyleDefinition).ToList();

            result.Scale.Manual = configuration.EXTSCALEManual;
            result.Scale.Point1 = ToPoint(configuration.EXTSCALEp1);
            result.Scale.Point2 = ToPoint(configuration.EXTSCALEp2);
            result.Scale.Layer = configuration.EXTSCALELayer;
            result.Scale.TextSize = configuration.EXTSCALETextSize;

            result.Layers.TeklaDrawingSheetLayer = configuration.LayerTeklaString;
            result.Layers.BlockAttributeLayer = configuration.LayerBlockAttribute;
            result.Layers.BaseLayers = state.BaseLayers.ToList();
            result.Layers.NewLayers = state.NewLayerDefinitions.Select(LegacyConfigurationParsers.ParseLayerDefinition).ToList();
            result.Layers.RemoveRules = state.RemoveRules.Select(ToRemoveRule).ToList();
            result.Layers.ConversionRules = state.ConversionRules.Select(LegacyConfigurationParsers.ParseLayerConversionRule).ToList();
            result.Layers.ExplodeLayers = state.ExplodeLayers.Where(item => !string.IsNullOrEmpty(item)).ToList();

            result.Lines.LineTypeScale = configuration.EXTLINELtscale;
            result.Lines.BaseLineTypes = configuration.Lines.BaseLineTypes.ToList();
            result.Catalogs.Colors = configuration.Catalogs.Colors.ToList();
            result.Catalogs.ObjectTypes = configuration.Catalogs.ObjectTypes.ToList();
            result.Catalogs.FilterLineTypes = configuration.Catalogs.FilterLineTypes.ToList();
            result.Catalogs.LayerLineTypes = configuration.Catalogs.LayerLineTypes.ToList();
            result.Catalogs.RemovedLineTypes = configuration.Catalogs.RemovedLineTypes.ToList();

            result.Commands.LispCommands = state.LispCommands.ToList();
            result.Commands.DllCommands = state.DllCommands.ToList();

            result.Blocks.TeklaBlockPath = configuration.PROGRAMblockFormatoCaminho;
            result.Blocks.CadBlockPath = configuration.EXTCONFCaminhoBlocoInv;
            result.Blocks.DimensionBlockEnabled = configuration.DMBlock;
            result.Blocks.TeklaBlocks = configuration.Blocks.TeklaBlocks.Select(CopyBlockDefinition).ToList();
            result.Blocks.CadBlocks = configuration.Blocks.CadBlocks.Select(CopyBlockDefinition).ToList();
            result.Blocks.OriginalBlocks = configuration.Blocks.OriginalBlocks.Select(CopyBlockDefinition).ToList();

            result.Runtime.DbLineTypePath = configuration.PROGRAMDbLin;
            result.Runtime.TempDirectory = configuration.GetPROGRAMDirectoryTemp();
            result.EnsureDefaults();

            return result;
        }


        public static void ApplyBlocksToLegacyLists(
            Configuration source,
            List<Block> teklaBlocks,
            List<Block> cadBlocks,
            List<Block> originalBlocks)
        {
            source = source ?? new Configuration();
            source.EnsureDefaults();

            teklaBlocks.Clear();
            cadBlocks.Clear();
            originalBlocks.Clear();

            teklaBlocks.AddRange(source.Blocks.TeklaBlocks.Select(ToBlock));
            cadBlocks.AddRange(source.Blocks.CadBlocks.Select(ToBlock));
            originalBlocks.AddRange(source.Blocks.OriginalBlocks.Select(ToBlock));
        }

        public static void ApplyBlocksFromLegacyLists(
            Configuration target,
            List<Block> teklaBlocks,
            List<Block> cadBlocks,
            List<Block> originalBlocks)
        {
            target = target ?? new Configuration();
            target.Blocks.TeklaBlocks = (teklaBlocks ?? new List<Block>()).Select(ToBlockDefinition).ToList();
            target.Blocks.CadBlocks = (cadBlocks ?? new List<Block>()).Select(ToBlockDefinition).ToList();
            target.Blocks.OriginalBlocks = (originalBlocks ?? new List<Block>()).Select(ToBlockDefinition).ToList();
        }

        private static LayerRemoveRule ToRemoveRule(Filter filter)
        {
            return new LayerRemoveRule { Filter = ToEntityFilter(filter) };
        }

        private static EntityFilter ToEntityFilter(Filter filter)
        {
            return new EntityFilter
            {
                BaseLayer = filter.layerBase,
                ObjectType = filter.tipoObjeto,
                Color = filter.cor,
                LineType = filter.tipoLinha,
                TextContent = filter.conteudoTexto,
                TextHeight = filter.alturaTexto,
                Orientation = filter.orientacao
            };
        }

        private static Filter ToFilter(EntityFilter source)
        {
            Filter result = new Filter(new Configuration().Catalogs);
            result.layerBase = source.BaseLayer;
            result.tipoObjeto = source.ObjectType;
            result.cor = source.Color;
            result.tipoLinha = source.LineType;
            result.conteudoTexto = source.TextContent;
            result.alturaTexto = source.TextHeight;
            result.orientacao = string.IsNullOrEmpty(source.Orientation) ? "ALL" : source.Orientation;
            return result;
        }

        private static BlockDefinition ToBlockDefinition(Block block)
        {
            return new BlockDefinition
            {
                Name = block.blockName,
                RelatedName = block.blockNameRelacao,
                ColorArgb = block.cor.ToArgb(),
                Tags = block.listTags.Select(ToTagDefinition).ToList()
            };
        }

        private static BlockDefinition CopyBlockDefinition(BlockDefinition source)
        {
            return new BlockDefinition
            {
                Name = source.Name,
                RelatedName = source.RelatedName,
                ColorArgb = source.ColorArgb,
                Tags = source.Tags.Select(CopyTagDefinition).ToList()
            };
        }

        private static BlockTagDefinition CopyTagDefinition(BlockTagDefinition source)
        {
            return new BlockTagDefinition
            {
                Name = source.Name,
                Modify = source.Modify,
                Point1 = CopyPoint(source.Point1),
                Point2 = CopyPoint(source.Point2),
                Filter = CopyFilter(source.Filter),
                WidthFactor = source.WidthFactor,
                RelatedIndex = source.RelatedIndex,
                IsAssociated = source.IsAssociated
            };
        }

        private static EntityFilter CopyFilter(EntityFilter source)
        {
            source = source ?? new EntityFilter();
            return new EntityFilter
            {
                BaseLayer = source.BaseLayer,
                ObjectType = source.ObjectType,
                Color = source.Color,
                LineType = source.LineType,
                TextContent = source.TextContent,
                TextHeight = source.TextHeight,
                Orientation = source.Orientation
            };
        }

        private static Block ToBlock(BlockDefinition source)
        {
            Block result = new Block
            {
                blockName = source.Name,
                blockNameRelacao = source.RelatedName,
                cor = Color.FromArgb(source.ColorArgb)
            };
            result.listTags.AddRange(source.Tags.Select(ToTagBlock));
            return result;
        }

        private static BlockTagDefinition ToTagDefinition(TagBlock tag)
        {
            return new BlockTagDefinition
            {
                Name = tag.tag,
                Modify = tag.modify,
                Point1 = ToPoint(tag.p1),
                Point2 = ToPoint(tag.p2),
                Filter = ToEntityFilter(tag.filtro),
                WidthFactor = LegacyConfigurationParsers.ParseDouble(tag.widthfactor, 1),
                RelatedIndex = tag.indiceRelacao,
                IsAssociated = tag.isSociate
            };
        }

        private static TagBlock ToTagBlock(BlockTagDefinition source)
        {
            TagBlock result = new TagBlock();
            result.tag = source.Name;
            result.modify = source.Modify;
            result.p1 = ToPointEspecial(source.Point1);
            result.p2 = ToPointEspecial(source.Point2);
            result.filtro = ToFilter(source.Filter);
            result.widthfactor = LegacyConfigurationParsers.FormatDouble(source.WidthFactor);
            result.indiceRelacao = source.RelatedIndex;
            result.isSociate = source.IsAssociated;
            return result;
        }

        private static Point3DConfiguration ToPoint(Point point)
        {
            return new Point3DConfiguration { X = point.X, Y = point.Y, Z = point.Z };
        }

        private static PointEspecial ToPointEspecial(Point3DConfiguration point)
        {
            return new PointEspecial(point.X, point.Y, point.Z);
        }

        private static Point3DConfiguration CopyPoint(Point3DConfiguration point)
        {
            point = point ?? new Point3DConfiguration();
            return new Point3DConfiguration { X = point.X, Y = point.Y, Z = point.Z };
        }
    }
#pragma warning restore CS0618
}

