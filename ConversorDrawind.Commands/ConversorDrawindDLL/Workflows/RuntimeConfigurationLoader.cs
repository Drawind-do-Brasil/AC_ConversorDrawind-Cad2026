using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ConversorDrawindDLL
{
    internal static class RuntimeConfigurationLoader
    {
        internal static void Load(string file, Configuration configuration)
        {
            var arranjos = new global::ConversorDrawind.Arranjos();
            var blocks = new List<global::ConversorDrawind.Block>();
            var blocksInv = new List<global::ConversorDrawind.Block>();
            var blocksOrig = new List<global::ConversorDrawind.Block>();

#pragma warning disable CS0618
            global::ConversorDrawind.ConverterConfiguration structuredConfiguration =
                new global::ConversorDrawind.ConverterConfiguration(
                    global::ConversorDrawind.ConverterConfigurationReader.Load(file));
#pragma warning restore CS0618

            global::ConversorDrawind.ConfigurationCompatibilityMapper.ApplyToLegacyState(
                structuredConfiguration,
                configuration,
                arranjos,
                blocks,
                blocksInv,
                blocksOrig);

            ApplyRuntimeState(configuration, arranjos, blocks, blocksInv, blocksOrig);
        }

        private static void ApplyRuntimeState(
            Configuration configuration,
            global::ConversorDrawind.Arranjos source,
            List<global::ConversorDrawind.Block> blocks,
            List<global::ConversorDrawind.Block> blocksInv,
            List<global::ConversorDrawind.Block> blocksOrig)
        {
            Arranjos.Arrj.AllNewLayerComposition.Clear();
            Arranjos.Arrj.Conversor.Clear();
            Arranjos.Arrj.LayerRemove.Clear();
            Arranjos.Arrj.ListLISPCommand.Clear();
            Arranjos.Arrj.AllExplodeLayers.Clear();
            Arranjos.Arrj.AllTextSyles.Clear();
            Arranjos.ListBlocks.Clear();
            Arranjos.ListBlocksInv.Clear();
            Arranjos.ListBlocksOrig.Clear();

            Arranjos.Arrj.AllNewLayerComposition.AddRange(source.allNewLayerComposition);
            Arranjos.Arrj.AllTextSyles.AddRange(source.allTextSyles);
            Arranjos.Arrj.LayerRemove.AddRange(source.layerRemove.Select(CloneFilter));
            Arranjos.Arrj.Conversor.AddRange(source.conversor);
            Arranjos.Arrj.ListLISPCommand.AddRange(source.listLISPCommand);
            Arranjos.Arrj.AllExplodeLayers.AddRange(source.allExplodeLayers);
            Arranjos.ListBlocks.AddRange(blocks.Select(CloneBlock));
            Arranjos.ListBlocksInv.AddRange(blocksInv.Select(CloneBlock));
            Arranjos.ListBlocksOrig.AddRange(blocksOrig.Select(CloneBlock));

            configuration.Text.DefaultSize = ResolveTextSize(source.allTextSyles, configuration.Text.DefaultStyleName);
            
            InstanciaConversor.ConversorInstancias.Clear();
            InstanciaConversor.ConversorInstancias.AddRange(Arranjos.Arrj.Conversor.Select(item => new InstanciaConversor(item)));
        }

        private static Block CloneBlock(global::ConversorDrawind.Block source)
        {
            Block block = new Block();
            block.blockName = source.blockName;
            block.blockNameRelacao = source.blockNameRelacao;
            block.cor = Color.FromArgb(source.cor.ToArgb());
            block.listTags.AddRange(source.listTags.Select(CloneTag));
            return block;
        }

        private static TagBlock CloneTag(global::ConversorDrawind.TagBlock source)
        {
            TagBlock tag = new TagBlock();
            tag.widthfactor = source.widthfactor;
            tag.tag = source.tag;
            tag.modify = source.modify;
            tag.p1 = new PointEspecial(source.p1);
            tag.p2 = new PointEspecial(source.p2);
            tag.filtro = CloneFilter(source.filtro);
            tag.indiceRelacao = source.indiceRelacao;
            tag.isSociate = source.isSociate;
            return tag;
        }

        private static Filter CloneFilter(global::ConversorDrawind.Filter source)
        {
            Filter filter = new Filter();
            filter.layerBase = source.layerBase;
            filter.tipoObjeto = source.tipoObjeto;
            filter.cor = source.cor;
            filter.tipoLinha = source.tipoLinha;
            filter.conteudoTexto = source.conteudoTexto;
            filter.alturaTexto = source.alturaTexto;
            filter.orientacao = source.orientacao;
            filter.SetConjunto(filter.GetConjunto());
            return filter;
        }

        private static double ResolveTextSize(IEnumerable<string> textStyles, string styleName)
        {
            string textStyle = textStyles.FirstOrDefault(style =>
                string.Equals(style.Split(':').First(), styleName, System.StringComparison.OrdinalIgnoreCase))
                ?? Arranjos.defaultTextStyle;

            string[] textStyleSplit = textStyle.Split(':');
            return textStyleSplit.Length > 4 ? textStyleSplit[4].ToDouble() : 2.5;
        }
    }
}
