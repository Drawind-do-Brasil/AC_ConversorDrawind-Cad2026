using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConversorDrawind.Commands
{
    internal static class RuntimeConfigurationState
    {
        private static readonly List<Block> teklaBlocks = new List<Block>();
        private static readonly List<Block> inventorBlocks = new List<Block>();
        private static readonly List<Block> originalBlocks = new List<Block>();
        private static readonly IReadOnlyList<Block> readOnlyTeklaBlocks = teklaBlocks.AsReadOnly();
        private static readonly IReadOnlyList<Block> readOnlyInventorBlocks = inventorBlocks.AsReadOnly();
        private static readonly IReadOnlyList<Block> readOnlyOriginalBlocks = originalBlocks.AsReadOnly();

        internal static IReadOnlyList<string> NewLayerCompositions =>
            Configuration.Config.Layers.NewLayers
                .Select(global::ConversorDrawind.LegacyConfigurationParsers.FormatLayerDefinition)
                .ToList();

        internal static IReadOnlyList<string> ConverterLines =>
            Configuration.Config.Layers.ConversionRules
                .Select(global::ConversorDrawind.LegacyConfigurationParsers.FormatLayerConversionRule)
                .ToList();

        internal static IReadOnlyList<Filter> LayerRemove =>
            Configuration.Config.Layers.RemoveRules
                .Select(rule => ToFilter(rule.Filter))
                .ToList();

        internal static IReadOnlyList<string> LispCommands =>
            Configuration.Config.Commands.LispCommands.ToList();

        internal static IReadOnlyList<string> ExplodeLayers =>
            Configuration.Config.Layers.ExplodeLayers.ToList();

        internal static IReadOnlyList<string> TextStyles
        {
            get
            {
                List<string> styles = Configuration.Config.Text.Styles
                    .Select(global::ConversorDrawind.LegacyConfigurationParsers.FormatTextStyleDefinition)
                    .Where(style => !string.IsNullOrWhiteSpace(style))
                    .ToList();

                if (styles.Count == 0)
                {
                    styles.Add(global::ConversorDrawind.LegacyConfigurationParsers.FormatTextStyleDefinition(
                        new global::ConversorDrawind.TextStyleDefinition
                        {
                            Name = "TEXTO",
                            Font = "RomanS",
                            Size = 2.5,
                            WidthFactor = 1
                        }));
                }

                return styles;
            }
        }

        internal static IReadOnlyList<Block> TeklaBlocks => readOnlyTeklaBlocks;

        internal static IReadOnlyList<Block> InventorBlocks => readOnlyInventorBlocks;

        internal static IReadOnlyList<Block> OriginalBlocks => readOnlyOriginalBlocks;

        internal static void ResetWorkingStateFromConfiguration()
        {
            teklaBlocks.Clear();
            inventorBlocks.Clear();
            originalBlocks.Clear();

            teklaBlocks.AddRange(Configuration.Config.Blocks.TeklaBlocks.Select(ToBlock));
            inventorBlocks.AddRange(Configuration.Config.Blocks.CadBlocks.Select(ToBlock));
            originalBlocks.AddRange(Configuration.Config.Blocks.OriginalBlocks.Select(ToBlock));
        }

        internal static void RebuildConverterInstances()
        {
            InstanciaConversor.ConversorInstancias.Clear();
            InstanciaConversor.ConversorInstancias.AddRange(ConverterLines.Select(item => new InstanciaConversor(item)));
        }

        private static Filter ToFilter(global::ConversorDrawind.EntityFilter source)
        {
            source = source ?? new global::ConversorDrawind.EntityFilter();
            Filter filter = new Filter
            {
                layerBase = source.BaseLayer,
                tipoObjeto = source.ObjectType,
                cor = source.Color,
                tipoLinha = source.LineType,
                conteudoTexto = source.TextContent,
                alturaTexto = source.TextHeight,
                orientacao = string.IsNullOrWhiteSpace(source.Orientation) ? "ALL" : source.Orientation
            };
            filter.SetConjunto(filter.GetConjunto());
            return filter;
        }

        private static Block ToBlock(global::ConversorDrawind.BlockDefinition source)
        {
            source = source ?? new global::ConversorDrawind.BlockDefinition();
            Block block = new Block
            {
                blockName = source.Name,
                blockNameRelacao = source.RelatedName,
                cor = Color.FromArgb(source.ColorArgb)
            };
            block.listTags.AddRange(source.Tags.Select(ToTagBlock));
            return block;
        }

        private static TagBlock ToTagBlock(global::ConversorDrawind.BlockTagDefinition source)
        {
            source = source ?? new global::ConversorDrawind.BlockTagDefinition();
            return new TagBlock
            {
                tag = source.Name,
                modify = source.Modify,
                p1 = new ConversorDrawind.Point(source.Point1.X, source.Point1.Y, source.Point1.Z),
                p2 = new ConversorDrawind.Point(source.Point2.X, source.Point2.Y, source.Point2.Z),
                filtro = ToFilter(source.Filter),
                widthfactor = global::ConversorDrawind.LegacyConfigurationParsers.FormatDouble(source.WidthFactor),
                indiceRelacao = source.RelatedIndex,
                isSociate = source.IsAssociated
            };
        }
    }
}
