using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace ConversorDrawindDLL
{
    public sealed class ConversionContext
    {
        public ConversionSettings Settings { get; }
        public LayerConversionPlan Layers { get; }
        public BlockConversionPlan Blocks { get; }
        public DimensionConversionSettings Dimensions { get; }
        public ScaleConversionSettings Scale { get; }
        public BlockScaleReference BlockScale { get; }

        private ConversionContext(
            ConversionSettings settings,
            LayerConversionPlan layers,
            BlockConversionPlan blocks,
            DimensionConversionSettings dimensions,
            ScaleConversionSettings scale,
            BlockScaleReference blockScale)
        {
            Settings = settings;
            Layers = layers;
            Blocks = blocks;
            Dimensions = dimensions;
            Scale = scale;
            BlockScale = blockScale;
        }

        public static ConversionContext FromCurrent()
        {
            return From(Configuration.Config, Arranjos.Arrj, ConvertBlocks.GetCurrentScaleReference());
        }

        public static ConversionContext From(Configuration configuration, Arranjos arranjos)
        {
            return From(configuration, arranjos, ConvertBlocks.GetCurrentScaleReference());
        }

        public static ConversionContext From(Configuration configuration, Arranjos arranjos, BlockScaleReference blockScale)
        {
            return new ConversionContext(
                ConversionSettings.From(configuration),
                LayerConversionPlan.From(arranjos),
                BlockConversionPlan.From(configuration),
                DimensionConversionSettings.From(configuration),
                ScaleConversionSettings.From(configuration),
                blockScale);
        }

        internal static ReadOnlyCollection<string> CopyStrings(IEnumerable<string> values)
        {
            return values.ToList().AsReadOnly();
        }

        internal static ReadOnlyCollection<Filter> CopyFilters(IEnumerable<Filter> values)
        {
            return values.Select(CloneFilter).ToList().AsReadOnly();
        }

        internal static ReadOnlyCollection<Block> CopyBlocks(IEnumerable<Block> values)
        {
            return values.Select(CloneBlock).ToList().AsReadOnly();
        }

        private static Block CloneBlock(Block source)
        {
            Block clone = new Block();
            clone.blockName = source.blockName;
            clone.blockNameRelacao = source.blockNameRelacao;
            clone.cor = Color.FromArgb(source.cor.ToArgb());
            clone.listTags.AddRange(source.listTags.Select(CloneTag));
            return clone;
        }

        private static TagBlock CloneTag(TagBlock source)
        {
            TagBlock clone = new TagBlock();
            clone.verifiqued = source.verifiqued;
            clone.widthfactor = source.widthfactor;
            clone.tag = source.tag;
            clone.modify = source.modify;
            clone.p1 = new PointEspecial(source.p1);
            clone.p2 = new PointEspecial(source.p2);
            clone.filtro = CloneFilter(source.filtro);
            clone.text = source.text;
            clone.indiceRelacao = source.indiceRelacao;
            clone.isSociate = source.isSociate;
            return clone;
        }

        private static Filter CloneFilter(Filter source)
        {
            Filter clone = new Filter();
            clone.layerBase = source.layerBase;
            clone.tipoObjeto = source.tipoObjeto;
            clone.cor = source.cor;
            clone.tipoLinha = source.tipoLinha;
            clone.conteudoTexto = source.conteudoTexto;
            clone.alturaTexto = source.alturaTexto;
            clone.orientacao = source.orientacao;
            clone.alturaTextoRound = source.alturaTextoRound;
            return clone;
        }
    }

    public sealed class ConversionSettings
    {
        public string Comments { get; }
        public int ConversionMode { get; }
        public bool ConvertDimension { get; }
        public bool ConvertLayer { get; }
        public bool ExchangeFormat { get; }
        public bool PutOnTheScaleDrawing { get; }
        public bool ExecuteLisp { get; }
        public bool Purge { get; }
        public bool ShowMessages { get; }
        public bool DeleteTeklaStructures { get; }
        public bool InventorExplode { get; }
        public double LineTypeScale { get; }

        private ConversionSettings(Configuration configuration)
        {
            Comments = configuration.EXTCONFComments;
            ConversionMode = configuration.ConvTekla0ConvInv1;
            ConvertDimension = configuration.EXTCONFIsConvertDimension;
            ConvertLayer = configuration.EXTCONFIsConvertLayer;
            ExchangeFormat = configuration.EXTCONFIsExchangeFormat;
            PutOnTheScaleDrawing = configuration.EXTCONFIsPutOnTheScaleDrawing;
            ExecuteLisp = configuration.EXTCONFIsExecuteLISP;
            Purge = configuration.EXTCONFIsPurge;
            ShowMessages = configuration.PROGRAMMessage;
            DeleteTeklaStructures = configuration.EXTCONFIsDeleteTeklaStructures;
            InventorExplode = configuration.EXTCONFInventorExplode;
            LineTypeScale = configuration.EXTLINELtscale;
        }

        internal static ConversionSettings From(Configuration configuration)
        {
            return new ConversionSettings(configuration);
        }
    }

    public sealed class LayerConversionPlan
    {
        public IReadOnlyList<string> NewLayerCompositions { get; }
        public IReadOnlyList<string> ConverterLines { get; }
        public IReadOnlyList<Filter> LayersToRemove { get; }
        public IReadOnlyList<string> LispCommands { get; }
        public IReadOnlyList<string> ExplodeLayers { get; }
        public IReadOnlyList<string> TextStyles { get; }

        private LayerConversionPlan(Arranjos arranjos)
        {
            NewLayerCompositions = ConversionContext.CopyStrings(arranjos.AllNewLayerComposition);
            ConverterLines = ConversionContext.CopyStrings(arranjos.Conversor);
            LayersToRemove = ConversionContext.CopyFilters(arranjos.LayerRemove);
            LispCommands = ConversionContext.CopyStrings(arranjos.ListLISPCommand);
            ExplodeLayers = ConversionContext.CopyStrings(arranjos.AllExplodeLayers);
            TextStyles = ConversionContext.CopyStrings(arranjos.AllTextSyles);
        }

        internal static LayerConversionPlan From(Arranjos arranjos)
        {
            return new LayerConversionPlan(arranjos);
        }
    }

    public sealed class BlockConversionPlan
    {
        public string TeklaFormatBlockPath { get; }
        public string InventorFormatBlockPath { get; }
        public bool ExchangeDmBlock { get; }
        public bool ExplodeBlocks { get; }
        public string TeklaSheetLayer { get; }
        public string BlockAttributeLayer { get; }
        public IReadOnlyList<Block> TeklaBlocks { get; }
        public IReadOnlyList<Block> InventorBlocks { get; }
        public IReadOnlyList<Block> OriginalBlocks { get; }

        private BlockConversionPlan(Configuration configuration)
        {
            TeklaFormatBlockPath = configuration.PROGRAMblockFormatoCaminho;
            InventorFormatBlockPath = configuration.EXTCONFCaminhoBlocoInv;
            ExchangeDmBlock = configuration.DMBlock;
            ExplodeBlocks = configuration.ExplodeBlocks;
            TeklaSheetLayer = configuration.LayerTeklaString;
            BlockAttributeLayer = configuration.LayerBlockAttribute;
            TeklaBlocks = ConversionContext.CopyBlocks(Arranjos.ListBlocks);
            InventorBlocks = ConversionContext.CopyBlocks(Arranjos.ListBlocksInv);
            OriginalBlocks = ConversionContext.CopyBlocks(Arranjos.ListBlocksOrig);
        }

        internal static BlockConversionPlan From(Configuration configuration)
        {
            return new BlockConversionPlan(configuration);
        }
    }

    public sealed class DimensionConversionSettings
    {
        public string Layer { get; }
        public string BaseLayer { get; }
        public string StyleName { get; }
        public string TextStyleName { get; }
        public double TextSize { get; }
        public double Scale { get; }
        public int Precision { get; }
        public int AngularPrecision { get; }
        public int Unit { get; }
        public int AngularUnit { get; }
        public string ArrowType { get; }
        public double ArrowSize { get; }
        public string TextColor { get; }
        public string LineColor { get; }
        public double OffsetLineFromRefPoint { get; }
        public int TextMove { get; }
        public bool OutsideAlign { get; }
        public int Tad { get; }
        public bool DimensionPosition { get; }
        public bool TextForced { get; }
        public bool LineForced { get; }
        public double Dimex { get; }
        public bool FixArrow { get; }
        public string FixArrowType { get; }
        public double FixArrowFactor { get; }

        private DimensionConversionSettings(Configuration configuration)
        {
            Layer = configuration.EXTDIMlayer;
            BaseLayer = configuration.EXTDIMBaseLayer;
            StyleName = configuration.EXTDIMStyleName;
            TextStyleName = configuration.EXTTEXTStyleName;
            TextSize = configuration.EXTTEXTSize;
            Scale = configuration.EXTDIMScale;
            Precision = configuration.EXTDIMPrecision;
            AngularPrecision = configuration.EXTDIMAngularPrecision;
            Unit = configuration.EXTDIMUnit;
            AngularUnit = configuration.EXTDIMAngularUnit;
            ArrowType = configuration.EXTDIMSeta;
            ArrowSize = configuration.EXTDIMSizeSeta;
            TextColor = configuration.EXTDIMColorText;
            LineColor = configuration.EXTDIMColorLine;
            OffsetLineFromRefPoint = configuration.EXTDIMOffsetLineFromRefPoint;
            TextMove = configuration.EXTDIMTextMove;
            OutsideAlign = configuration.EXTDIMOutsideAlign;
            Tad = configuration.EXTDIMTad;
            DimensionPosition = configuration.EXTDIMDimensionPosition;
            TextForced = configuration.EXTDIMTextForced;
            LineForced = configuration.EXTDIMLineForced;
            Dimex = configuration.EXTDIMDIMEX;
            FixArrow = configuration.EXTDIMCorrigeSeta;
            FixArrowType = configuration.EXTDIMCorrigeSetaTipoSeta;
            FixArrowFactor = configuration.EXTDIMCorrigeSetaFactor;
        }

        internal static DimensionConversionSettings From(Configuration configuration)
        {
            return new DimensionConversionSettings(configuration);
        }
    }

    public sealed class ScaleConversionSettings
    {
        public bool Manual { get; }
        public PointEspecial2 Point1 { get; }
        public PointEspecial2 Point2 { get; }
        public string Layer { get; }
        public double TextSize { get; }
        public string TextSizeString { get; }

        private ScaleConversionSettings(Configuration configuration)
        {
            Manual = configuration.EXTSCALEManual;
            Point1 = new PointEspecial2(configuration.EXTSCALEp1);
            Point2 = new PointEspecial2(configuration.EXTSCALEp2);
            Layer = configuration.EXTSCALELayer;
            TextSize = configuration.EXTSCALETextSize;
            TextSizeString = configuration.EXTSCALETextSizeString;
        }

        internal static ScaleConversionSettings From(Configuration configuration)
        {
            return new ScaleConversionSettings(configuration);
        }
    }

    public sealed class BlockScaleReference
    {
        public double Scale { get; }
        public double ReferenceFormatSize { get; }
        public Point3d StartPoint { get; }

        public BlockScaleReference(double scale, double referenceFormatSize, Point3d startPoint)
        {
            Scale = scale;
            ReferenceFormatSize = referenceFormatSize;
            StartPoint = startPoint;
        }
    }
}
