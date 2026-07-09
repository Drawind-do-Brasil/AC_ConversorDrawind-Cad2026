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
            Arranjos arranjos,
            List<Block> teklaBlocks,
            List<Block> cadBlocks,
            List<Block> originalBlocks)
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
            result.Text.Styles = arranjos.allTextSyles.Select(LegacyConfigurationParsers.ParseTextStyleDefinition).ToList();

            result.Scale.Manual = configuration.EXTSCALEManual;
            result.Scale.Point1 = ToPoint(configuration.EXTSCALEp1);
            result.Scale.Point2 = ToPoint(configuration.EXTSCALEp2);
            result.Scale.Layer = configuration.EXTSCALELayer;
            result.Scale.TextSize = configuration.EXTSCALETextSize;

            result.Layers.TeklaDrawingSheetLayer = configuration.LayerTeklaString;
            result.Layers.BlockAttributeLayer = configuration.LayerBlockAttribute;
            result.Layers.BaseLayers = arranjos.allBaseLayer.ToList();
            result.Layers.NewLayers = arranjos.allNewLayerComposition.Select(LegacyConfigurationParsers.ParseLayerDefinition).ToList();
            result.Layers.RemoveRules = arranjos.layerRemove.Select(ToRemoveRule).ToList();
            result.Layers.ConversionRules = arranjos.conversor.Select(LegacyConfigurationParsers.ParseLayerConversionRule).ToList();
            result.Layers.ExplodeLayers = arranjos.allExplodeLayers.Where(item => !string.IsNullOrEmpty(item)).ToList();

            result.Lines.LineTypeScale = configuration.EXTLINELtscale;
            result.Lines.BaseLineTypes = arranjos.allLineType1.ToList();

            result.Commands.LispCommands = arranjos.listLISPCommand.ToList();
            result.Commands.DllCommands = arranjos.listDLLCommand.ToList();

            result.Blocks.TeklaBlockPath = configuration.PROGRAMblockFormatoCaminho;
            result.Blocks.CadBlockPath = configuration.EXTCONFCaminhoBlocoInv;
            result.Blocks.DimensionBlockEnabled = configuration.DMBlock;
            result.Blocks.TeklaBlocks = teklaBlocks.Select(ToBlockDefinition).ToList();
            result.Blocks.CadBlocks = cadBlocks.Select(ToBlockDefinition).ToList();
            result.Blocks.OriginalBlocks = originalBlocks.Select(ToBlockDefinition).ToList();

            result.Runtime.DbLineTypePath = configuration.PROGRAMDbLin;
            result.Runtime.TempDirectory = configuration.GetPROGRAMDirectoryTemp();

            return result;
        }

        public static void ApplyToLegacyState(
            ConverterConfiguration source,
            Configuration configuration,
            Arranjos arranjos,
            List<Block> teklaBlocks,
            List<Block> cadBlocks,
            List<Block> originalBlocks)
        {
            source = source ?? new ConverterConfiguration();

            arranjos.allBaseLayer.Clear();
            arranjos.allLineType1.Clear();
            arranjos.allNewLayerComposition.Clear();
            arranjos.allNewLayer.Clear();
            arranjos.conversor.Clear();
            arranjos.layerRemove.Clear();
            arranjos.listLISPCommand.Clear();
            arranjos.listDLLCommand.Clear();
            arranjos.allExplodeLayers.Clear();
            arranjos.allTextSyles.Clear();
            teklaBlocks.Clear();
            cadBlocks.Clear();
            originalBlocks.Clear();

            configuration.EXTCONFComments = source.Comments ?? string.Empty;
            configuration.EXTCONFOrigem = source.General.SourceMode;
            configuration.ConvTekla0ConvInv1 = source.General.ConverterType;
            configuration.EXTCONFIsConvertDimension = source.General.ConvertDimensions;
            configuration.EXTCONFIsConvertLayer = source.General.ConvertLayers;
            configuration.EXTCONFIsExchangeFormat = source.General.ExchangeFormat;
            configuration.EXTCONFIsExchangeLM = source.General.ExchangeLm;
            configuration.EXTCONFIsPutOnTheScaleDrawing = source.General.ApplyDrawingScale;
            configuration.EXTCONFIsExecuteLISP = source.General.ExecuteLisp;
            configuration.EXTCONFIsExecuteDLL = source.General.ExecuteDll;
            configuration.EXTCONFIsFirstRum = source.General.FirstRunMode;
            configuration.EXTCONFIsDeleteTeklaStructures = source.General.DeleteTeklaStructures;
            configuration.EXTCONFIsPurge = source.General.Purge;
            configuration.PROGRAMMessage = source.General.ShowMessages;
            configuration.ExplodeBlocks = source.General.ExplodeBlocks;
            configuration.EXTCONFInventorExplode = source.General.InventorExplode;

            Configuration.INTREFTamFormato = source.Dimensions.ReferenceFormatSize;
            configuration.INTFactorLengthChar = source.Dimensions.InternalLengthCharFactor;
            configuration.INTDIMTextOffset = source.Dimensions.InternalTextOffset;
            configuration.EXTDIMGERALHabilit = source.Dimensions.Enabled;
            configuration.EXTDIMlayer = source.Dimensions.Layer;
            configuration.EXTDIMBaseLayer = source.Dimensions.BaseLayer;
            configuration.EXTDIMColorLine = source.Dimensions.LineColor;
            configuration.EXTDIMColorText = source.Dimensions.TextColor;
            configuration.EXTDIMStyleName = source.Dimensions.StyleName;
            configuration.EXTDIMSeta = source.Dimensions.ArrowType;
            configuration.EXTDIMSeta1 = source.Dimensions.ArrowType1;
            configuration.EXTDIMSeta2 = source.Dimensions.ArrowType2;
            configuration.EXTDIMScale = source.Dimensions.Scale;
            configuration.EXTDIMPrecision = source.Dimensions.Precision;
            configuration.EXTDIMAngularPrecision = source.Dimensions.AngularPrecision;
            configuration.EXTDIMUnit = source.Dimensions.Unit;
            configuration.EXTDIMAngularUnit = source.Dimensions.AngularUnit;
            configuration.EXTDIMSizeSeta = source.Dimensions.ArrowSize;
            configuration.EXTDIMTad = source.Dimensions.TextVerticalPosition;
            configuration.EXTDIMDimensionPosition = source.Dimensions.TextRelativeToDimensionLine;
            configuration.EXTDIMTextForced = source.Dimensions.ForceTextInside;
            configuration.EXTDIMLineForced = source.Dimensions.ForceDimensionLine;
            configuration.EXTDIMOffsetLineFromRefPoint = source.Dimensions.OffsetLineFromReferencePoint;
            configuration.EXTDIMTextMove = source.Dimensions.TextMove;
            configuration.EXTDIMOutsideAlign = source.Dimensions.OutsideAlign;
            configuration.EXTDIMDIMEX = source.Dimensions.ExtensionLineOffset;
            configuration.EXTDIMCorrigeSeta = source.Dimensions.FixArrow;
            configuration.EXTDIMCorrigeSetaTipoSeta = source.Dimensions.FixArrowType;
            configuration.EXTDIMCorrigeSetaFactor = source.Dimensions.FixArrowFactor;

            configuration.EXTTEXTStyleName = source.Text.DefaultStyleName;
            configuration.EXTTEXTSize = source.Text.DefaultSize;
            arranjos.allTextSyles.AddRange(source.Text.Styles.Select(LegacyConfigurationParsers.FormatTextStyleDefinition));
            if (arranjos.allTextSyles.Count == 0)
                arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);

            configuration.EXTSCALEManual = source.Scale.Manual;
            configuration.EXTSCALEp1 = ToPointEspecial(source.Scale.Point1);
            configuration.EXTSCALEp2 = ToPointEspecial(source.Scale.Point2);
            configuration.EXTSCALELayer = source.Scale.Layer;
            configuration.EXTSCALETextSize = source.Scale.TextSize;

            configuration.LayerTeklaString = source.Layers.TeklaDrawingSheetLayer;
            configuration.LayerBlockAttribute = source.Layers.BlockAttributeLayer;
            arranjos.allBaseLayer.AddRange(source.Layers.BaseLayers);
            arranjos.allLineType1.AddRange(source.Lines.BaseLineTypes);
            arranjos.allNewLayerComposition.AddRange(source.Layers.NewLayers.Select(LegacyConfigurationParsers.FormatLayerDefinition));
            arranjos.allNewLayer.AddRange(source.Layers.NewLayers.Select(layer => layer.Name));
            arranjos.layerRemove.AddRange(source.Layers.RemoveRules.Select(rule => ToFilter(rule.Filter, arranjos)));
            arranjos.conversor.AddRange(source.Layers.ConversionRules.Select(LegacyConfigurationParsers.FormatLayerConversionRule));
            arranjos.allExplodeLayers.AddRange(source.Layers.ExplodeLayers);

            configuration.EXTLINELtscale = source.Lines.LineTypeScale;
            arranjos.listLISPCommand.AddRange(source.Commands.LispCommands);
            arranjos.listDLLCommand.AddRange(source.Commands.DllCommands);

            configuration.PROGRAMblockFormatoCaminho = source.Blocks.TeklaBlockPath;
            configuration.EXTCONFCaminhoBlocoInv = source.Blocks.CadBlockPath;
            configuration.DMBlock = source.Blocks.DimensionBlockEnabled;
            teklaBlocks.AddRange(source.Blocks.TeklaBlocks.Select(ToBlock));
            cadBlocks.AddRange(source.Blocks.CadBlocks.Select(ToBlock));
            originalBlocks.AddRange(source.Blocks.OriginalBlocks.Select(ToBlock));
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

        private static Filter ToFilter(EntityFilter source, Arranjos arranjos)
        {
            Filter result = new Filter(arranjos);
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
            result.filtro = ToFilter(source.Filter, new Arranjos());
            result.widthfactor = LegacyConfigurationParsers.FormatDouble(source.WidthFactor);
            result.indiceRelacao = source.RelatedIndex;
            result.isSociate = source.IsAssociated;
            return result;
        }

        private static Point3DConfiguration ToPoint(PointEspecial point)
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
