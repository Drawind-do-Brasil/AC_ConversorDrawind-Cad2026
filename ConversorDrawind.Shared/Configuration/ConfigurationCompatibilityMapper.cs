using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConversorDrawind
{
    public static class ConfigurationCompatibilityMapper
    {
        public static Configuration FromLegacyState(
            Configuration configuration,
            Arranjos arranjos,
            List<Block> teklaBlocks,
            List<Block> cadBlocks,
            List<Block> originalBlocks)
        {
            Configuration result = new Configuration();

            result.Comments = configuration.Comments;
            result.General.SourceMode = configuration.General.SourceMode;
            result.General.ConverterType = configuration.General.ConverterType;
            result.General.ConvertDimensions = configuration.General.ConvertDimensions;
            result.General.ConvertLayers = configuration.General.ConvertLayers;
            result.General.ExchangeFormat = configuration.General.ExchangeFormat;
            result.General.ExchangeLm = configuration.General.ExchangeLm;
            result.General.ApplyDrawingScale = configuration.General.ApplyDrawingScale;
            result.General.ExecuteLisp = configuration.General.ExecuteLisp;
            result.General.ExecuteDll = configuration.General.ExecuteDll;
            result.General.FirstRunMode = configuration.General.FirstRunMode;
            result.General.DeleteTeklaStructures = configuration.General.DeleteTeklaStructures;
            result.General.Purge = configuration.General.Purge;
            result.General.ShowMessages = configuration.General.ShowMessages;
            result.General.ExplodeBlocks = configuration.General.ExplodeBlocks;
            result.General.InventorExplode = configuration.General.InventorExplode;

            result.Dimensions.ReferenceFormatSize = Configuration.INTREFTamFormato;
            result.Dimensions.InternalLengthCharFactor = configuration.Dimensions.InternalLengthCharFactor;
            result.Dimensions.InternalTextOffset = configuration.Dimensions.InternalTextOffset;
            result.Dimensions.Enabled = configuration.Dimensions.Enabled;
            result.Dimensions.Layer = configuration.Dimensions.Layer;
            result.Dimensions.BaseLayer = configuration.Dimensions.BaseLayer;
            result.Dimensions.LineColor = configuration.Dimensions.LineColor;
            result.Dimensions.TextColor = configuration.Dimensions.TextColor;
            result.Dimensions.StyleName = configuration.Dimensions.StyleName;
            result.Dimensions.ArrowType = configuration.Dimensions.ArrowType;
            result.Dimensions.ArrowType1 = configuration.Dimensions.ArrowType1;
            result.Dimensions.ArrowType2 = configuration.Dimensions.ArrowType2;
            result.Dimensions.Scale = configuration.Dimensions.Scale;
            result.Dimensions.Precision = configuration.Dimensions.Precision;
            result.Dimensions.AngularPrecision = configuration.Dimensions.AngularPrecision;
            result.Dimensions.Unit = configuration.Dimensions.Unit;
            result.Dimensions.AngularUnit = configuration.Dimensions.AngularUnit;
            result.Dimensions.ArrowSize = configuration.Dimensions.ArrowSize;
            result.Dimensions.TextVerticalPosition = configuration.Dimensions.TextVerticalPosition;
            result.Dimensions.TextRelativeToDimensionLine = configuration.Dimensions.TextRelativeToDimensionLine;
            result.Dimensions.ForceTextInside = configuration.Dimensions.ForceTextInside;
            result.Dimensions.ForceDimensionLine = configuration.Dimensions.ForceDimensionLine;
            result.Dimensions.OffsetLineFromReferencePoint = configuration.Dimensions.OffsetLineFromReferencePoint;
            result.Dimensions.TextMove = configuration.Dimensions.TextMove;
            result.Dimensions.OutsideAlign = configuration.Dimensions.OutsideAlign;
            result.Dimensions.ExtensionLineOffset = configuration.Dimensions.ExtensionLineOffset;
            result.Dimensions.FixArrow = configuration.Dimensions.FixArrow;
            result.Dimensions.FixArrowType = configuration.Dimensions.FixArrowType;
            result.Dimensions.FixArrowFactor = configuration.Dimensions.FixArrowFactor;

            result.Text.DefaultStyleName = configuration.Text.DefaultStyleName;
            result.Text.DefaultSize = configuration.Text.DefaultSize;
            result.Text.Styles = arranjos.allTextSyles.Select(LegacyConfigurationParsers.ParseTextStyleDefinition).ToList();

            result.Scale.Manual = configuration.Scale.Manual;
            result.Scale.Point1 = CopyPoint(configuration.Scale.Point1);
            result.Scale.Point2 = CopyPoint(configuration.Scale.Point2);
            result.Scale.Layer = configuration.Scale.Layer;
            result.Scale.TextSize = configuration.Scale.TextSize;

            result.Layers.TeklaDrawingSheetLayer = configuration.Layers.TeklaDrawingSheetLayer;
            result.Layers.BlockAttributeLayer = configuration.Layers.BlockAttributeLayer;
            result.Layers.BaseLayers = arranjos.allBaseLayer.ToList();
            result.Layers.NewLayers = arranjos.allNewLayerComposition.Select(LegacyConfigurationParsers.ParseLayerDefinition).ToList();
            result.Layers.RemoveRules = arranjos.layerRemove.Select(ToRemoveRule).ToList();
            result.Layers.ConversionRules = arranjos.conversor.Select(LegacyConfigurationParsers.ParseLayerConversionRule).ToList();
            result.Layers.ExplodeLayers = arranjos.allExplodeLayers.Where(item => !string.IsNullOrEmpty(item)).ToList();

            result.Lines.LineTypeScale = configuration.Lines.LineTypeScale;
            result.Lines.BaseLineTypes = arranjos.allLineType1.ToList();

            result.Commands.LispCommands = arranjos.listLISPCommand.ToList();
            result.Commands.DllCommands = arranjos.listDLLCommand.ToList();

            result.Blocks.TeklaBlockPath = configuration.Blocks.TeklaBlockPath;
            result.Blocks.CadBlockPath = configuration.Blocks.CadBlockPath;
            result.Blocks.DimensionBlockEnabled = configuration.Blocks.DimensionBlockEnabled;
            result.Blocks.TeklaBlocks = teklaBlocks.Select(ToBlockDefinition).ToList();
            result.Blocks.CadBlocks = cadBlocks.Select(ToBlockDefinition).ToList();
            result.Blocks.OriginalBlocks = originalBlocks.Select(ToBlockDefinition).ToList();

            result.Runtime.DbLineTypePath = configuration.Runtime.DbLineTypePath;
            result.Runtime.TempDirectory = configuration.GetTempDirectory();

            return result;
        }

        public static void ApplyToLegacyState(
            Configuration source,
            Configuration configuration,
            Arranjos arranjos,
            List<Block> teklaBlocks,
            List<Block> cadBlocks,
            List<Block> originalBlocks)
        {
            source = source ?? new Configuration();

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

            configuration.Comments = source.Comments ?? string.Empty;
            configuration.General.SourceMode = source.General.SourceMode;
            configuration.General.ConverterType = source.General.ConverterType;
            configuration.General.ConvertDimensions = source.General.ConvertDimensions;
            configuration.General.ConvertLayers = source.General.ConvertLayers;
            configuration.General.ExchangeFormat = source.General.ExchangeFormat;
            configuration.General.ExchangeLm = source.General.ExchangeLm;
            configuration.General.ApplyDrawingScale = source.General.ApplyDrawingScale;
            configuration.General.ExecuteLisp = source.General.ExecuteLisp;
            configuration.General.ExecuteDll = source.General.ExecuteDll;
            configuration.General.FirstRunMode = source.General.FirstRunMode;
            configuration.General.DeleteTeklaStructures = source.General.DeleteTeklaStructures;
            configuration.General.Purge = source.General.Purge;
            configuration.General.ShowMessages = source.General.ShowMessages;
            configuration.General.ExplodeBlocks = source.General.ExplodeBlocks;
            configuration.General.InventorExplode = source.General.InventorExplode;

            Configuration.INTREFTamFormato = source.Dimensions.ReferenceFormatSize;
            configuration.Dimensions.InternalLengthCharFactor = source.Dimensions.InternalLengthCharFactor;
            configuration.Dimensions.InternalTextOffset = source.Dimensions.InternalTextOffset;
            configuration.Dimensions.Enabled = source.Dimensions.Enabled;
            configuration.Dimensions.Layer = source.Dimensions.Layer;
            configuration.Dimensions.BaseLayer = source.Dimensions.BaseLayer;
            configuration.Dimensions.LineColor = source.Dimensions.LineColor;
            configuration.Dimensions.TextColor = source.Dimensions.TextColor;
            configuration.Dimensions.StyleName = source.Dimensions.StyleName;
            configuration.Dimensions.ArrowType = source.Dimensions.ArrowType;
            configuration.Dimensions.ArrowType1 = source.Dimensions.ArrowType1;
            configuration.Dimensions.ArrowType2 = source.Dimensions.ArrowType2;
            configuration.Dimensions.Scale = source.Dimensions.Scale;
            configuration.Dimensions.Precision = source.Dimensions.Precision;
            configuration.Dimensions.AngularPrecision = source.Dimensions.AngularPrecision;
            configuration.Dimensions.Unit = source.Dimensions.Unit;
            configuration.Dimensions.AngularUnit = source.Dimensions.AngularUnit;
            configuration.Dimensions.ArrowSize = source.Dimensions.ArrowSize;
            configuration.Dimensions.TextVerticalPosition = source.Dimensions.TextVerticalPosition;
            configuration.Dimensions.TextRelativeToDimensionLine = source.Dimensions.TextRelativeToDimensionLine;
            configuration.Dimensions.ForceTextInside = source.Dimensions.ForceTextInside;
            configuration.Dimensions.ForceDimensionLine = source.Dimensions.ForceDimensionLine;
            configuration.Dimensions.OffsetLineFromReferencePoint = source.Dimensions.OffsetLineFromReferencePoint;
            configuration.Dimensions.TextMove = source.Dimensions.TextMove;
            configuration.Dimensions.OutsideAlign = source.Dimensions.OutsideAlign;
            configuration.Dimensions.ExtensionLineOffset = source.Dimensions.ExtensionLineOffset;
            configuration.Dimensions.FixArrow = source.Dimensions.FixArrow;
            configuration.Dimensions.FixArrowType = source.Dimensions.FixArrowType;
            configuration.Dimensions.FixArrowFactor = source.Dimensions.FixArrowFactor;

            configuration.Text.DefaultStyleName = source.Text.DefaultStyleName;
            configuration.Text.DefaultSize = source.Text.DefaultSize;
            arranjos.allTextSyles.AddRange(source.Text.Styles.Select(LegacyConfigurationParsers.FormatTextStyleDefinition));
            if (arranjos.allTextSyles.Count == 0)
                arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);

            configuration.Scale.Manual = source.Scale.Manual;
            configuration.Scale.Point1 = CopyPoint(source.Scale.Point1);
            configuration.Scale.Point2 = CopyPoint(source.Scale.Point2);
            configuration.Scale.Layer = source.Scale.Layer;
            configuration.Scale.TextSize = source.Scale.TextSize;

            configuration.Layers.TeklaDrawingSheetLayer = source.Layers.TeklaDrawingSheetLayer;
            configuration.Layers.BlockAttributeLayer = source.Layers.BlockAttributeLayer;
            arranjos.allBaseLayer.AddRange(source.Layers.BaseLayers);
            arranjos.allLineType1.AddRange(source.Lines.BaseLineTypes);
            arranjos.allNewLayerComposition.AddRange(source.Layers.NewLayers.Select(LegacyConfigurationParsers.FormatLayerDefinition));
            arranjos.allNewLayer.AddRange(source.Layers.NewLayers.Select(layer => layer.Name));
            arranjos.layerRemove.AddRange(source.Layers.RemoveRules.Select(rule => ToFilter(rule.Filter, arranjos)));
            arranjos.conversor.AddRange(source.Layers.ConversionRules.Select(LegacyConfigurationParsers.FormatLayerConversionRule));
            arranjos.allExplodeLayers.AddRange(source.Layers.ExplodeLayers);

            configuration.Lines.LineTypeScale = source.Lines.LineTypeScale;
            arranjos.listLISPCommand.AddRange(source.Commands.LispCommands);
            arranjos.listDLLCommand.AddRange(source.Commands.DllCommands);

            configuration.Blocks.TeklaBlockPath = source.Blocks.TeklaBlockPath;
            configuration.Blocks.CadBlockPath = source.Blocks.CadBlockPath;
            configuration.Blocks.DimensionBlockEnabled = source.Blocks.DimensionBlockEnabled;
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
}
