using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ConversorDrawind
{
    [XmlRoot("CONVERSOR")]
    public sealed class ConfigurationXmlDocument
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ConfigurationXmlDocument));

        [XmlElement("COMMENTS", Order = 0)]
        public CommentsXml Comments { get; set; } = new CommentsXml();

        [XmlElement("BASIC_CONFIG", Order = 1)]
        public BasicConfigXml BasicConfig { get; set; } = new BasicConfigXml();

        [XmlElement("DIMENSION_CONFIG", Order = 2)]
        public DimensionConfigXml DimensionConfig { get; set; } = new DimensionConfigXml();

        [XmlElement("TEXT_CONFIG", Order = 3)]
        public TextConfigXml TextConfig { get; set; } = new TextConfigXml();

        [XmlElement("SCALE_CONFIG", Order = 4)]
        public ScaleConfigXml ScaleConfig { get; set; } = new ScaleConfigXml();

        [XmlElement("BASIC_LAYERS", Order = 5)]
        public BasicLayersXml BasicLayers { get; set; } = new BasicLayersXml();

        [XmlElement("BASIC_LINES", Order = 6)]
        public BasicLinesXml BasicLines { get; set; } = new BasicLinesXml();

        [XmlElement("NEW_TEXTSTYLES", Order = 7)]
        public TextStylesXml NewTextStyles { get; set; } = new TextStylesXml();

        [XmlElement("NEW_LAYERS", Order = 8)]
        public NewLayersXml NewLayers { get; set; } = new NewLayersXml();

        [XmlElement("REMOVE_LAYERS", Order = 9)]
        public RemoveLayersXml RemoveLayers { get; set; } = new RemoveLayersXml();

        [XmlElement("CONVERTERS", Order = 10)]
        public ConvertersXml Converters { get; set; } = new ConvertersXml();

        [XmlElement("DLL_OR_LIST_COMMANDS", Order = 11)]
        public CommandsXml Commands { get; set; } = new CommandsXml();

        [XmlElement("BLOCK_CONFIG", Order = 12)]
        public BlockConfigXml BlockConfig { get; set; } = new BlockConfigXml();

        public static ConfigurationXmlDocument Load(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return (ConfigurationXmlDocument)Serializer.Deserialize(stream);
            }
        }

        public void Save(string file)
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true
            };

            using (var writer = XmlWriter.Create(file, settings))
            {
                Serializer.Serialize(writer, this, namespaces);
            }
        }

        public static ConfigurationXmlDocument From(Configuration configuration, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso)
        {
            return new ConfigurationXmlDocument
            {
                Comments = new CommentsXml { Text = configuration.EXTCONFComments },
                BasicConfig = new BasicConfigXml
                {
                    TeklaOrCad = configuration.EXTCONFOrigem,
                    ConvertDimensions = configuration.EXTCONFIsConvertDimension,
                    ConvertLayers = configuration.EXTCONFIsConvertLayer,
                    ExchangeFormat = configuration.EXTCONFIsExchangeFormat,
                    Scale = configuration.EXTCONFIsPutOnTheScaleDrawing,
                    LispOrDll = configuration.EXTCONFIsExecuteLISP,
                    Purge = configuration.EXTCONFIsPurge,
                    Message = configuration.PROGRAMMessage,
                    DeleteTeklaStructures = configuration.EXTCONFIsDeleteTeklaStructures,
                    ExplodeBlocks = configuration.ExplodeBlocks,
                    LayerTeklaString = configuration.LayerTeklaString,
                    LayerBlockAttribute = configuration.LayerBlockAttribute,
                    CadExplode = configuration.EXTCONFInventorExplode,
                    DmBlock = configuration.DMBlock,
                    DmBlockSpecified = true
                },
                DimensionConfig = new DimensionConfigXml
                {
                    DimGeralHabilit = configuration.EXTDIMGERALHabilit,
                    DimLayer = configuration.EXTDIMlayer,
                    DimLineColor = configuration.EXTDIMColorLine,
                    DimTextColor = configuration.EXTDIMColorText,
                    DimStyle = configuration.EXTDIMStyleName,
                    DimArrowType = configuration.EXTDIMSeta,
                    DimScale = configuration.EXTDIMScale,
                    DimPrecision = configuration.EXTDIMPrecision,
                    DimAngularPrecision = configuration.EXTDIMAngularPrecision,
                    DimUnit = configuration.EXTDIMUnit,
                    DimAngularUnit = configuration.EXTDIMAngularUnit,
                    DimArrowSize = configuration.EXTDIMSizeSeta,
                    DimOffset = configuration.EXTDIMOffsetLineFromRefPoint,
                    DimOutsideAling = configuration.EXTDIMOutsideAlign,
                    DimTad = configuration.EXTDIMTad,
                    DimPosition = configuration.EXTDIMDimensionPosition,
                    DimTextForced = configuration.EXTDIMTextForced,
                    DimLineForced = configuration.EXTDIMLineForced,
                    DimDimex = configuration.EXTDIMDIMEX,
                    DimBaseLayer = configuration.EXTDIMBaseLayer,
                    DimArrowFix = configuration.EXTDIMCorrigeSeta,
                    DimArrowFixType = configuration.EXTDIMCorrigeSetaTipoSeta,
                    DimArrowFactor = configuration.EXTDIMCorrigeSetaFactor
                },
                TextConfig = new TextConfigXml { TextStyleName = configuration.EXTTEXTStyleName },
                ScaleConfig = new ScaleConfigXml
                {
                    ScaleMode = configuration.EXTSCALEManual,
                    ScaleP1X = configuration.EXTSCALEp1.X,
                    ScaleP1Y = configuration.EXTSCALEp1.Y,
                    ScaleP1Z = configuration.EXTSCALEp1.Z,
                    ScaleP2X = configuration.EXTSCALEp2.X,
                    ScaleP2Y = configuration.EXTSCALEp2.Y,
                    ScaleP2Z = configuration.EXTSCALEp2.Z,
                    ScaleLayer = configuration.EXTSCALELayer,
                    ScaleTextSize = configuration.EXTSCALETextSize
                },
                BasicLayers = new BasicLayersXml
                {
                    LtScale = configuration.EXTLINELtscale,
                    BaseLayers = arranjos.allBaseLayer.ToList()
                },
                BasicLines = new BasicLinesXml { BaseLines = arranjos.allLineType1.ToList() },
                NewTextStyles = new TextStylesXml { TextStyles = arranjos.allTextSyles.ToList() },
                NewLayers = new NewLayersXml
                {
                    Layers = arranjos.allNewLayerComposition.Count == 0
                        ? new List<string> { "0:WHITE:CONTINUOUS" }
                        : arranjos.allNewLayerComposition.ToList()
                },
                RemoveLayers = new RemoveLayersXml
                {
                    Layers = arranjos.layerRemove.Select(item => item.layerBase + ";" + item.GetConjunto()).ToList()
                },
                Converters = new ConvertersXml { Items = arranjos.conversor.ToList() },
                Commands = new CommandsXml { Items = arranjos.listLISPCommand.ToList() },
                BlockConfig = new BlockConfigXml
                {
                    DirectoryTeklaConversion = configuration.PROGRAMblockFormatoCaminho,
                    DirectoryCadConversion = configuration.EXTCONFCaminhoBlocoInv,
                    LayerExplode = string.Join(";", arranjos.allExplodeLayers),
                    TeklaBlocks = blocks.Select(CreateTeklaBlock).ToList(),
                    CadBlocks = blocosi.Select(CreateRelatedBlock).ToList(),
                    OrigBlocks = blocoso.Select(CreateRelatedBlock).ToList()
                }
            };
        }

        public void ApplyTo(Configuration configuration, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso)
        {
            arranjos.allBaseLayer.Clear();
            arranjos.allLineType1.Clear();
            arranjos.allNewLayerComposition.Clear();
            arranjos.allNewLayer.Clear();
            arranjos.conversor.Clear();
            arranjos.layerRemove.Clear();
            arranjos.listLISPCommand.Clear();
            arranjos.allExplodeLayers.Clear();
            blocks.Clear();
            blocosi.Clear();
            blocoso.Clear();

            configuration.EXTCONFComments = Comments?.Text ?? string.Empty;

            var basic = BasicConfig ?? new BasicConfigXml();
            configuration.EXTCONFOrigem = basic.TeklaOrCad;
            configuration.EXTCONFIsConvertDimension = basic.ConvertDimensions;
            configuration.EXTCONFIsConvertLayer = basic.ConvertLayers;
            configuration.EXTCONFIsExchangeFormat = basic.ExchangeFormat;
            configuration.EXTCONFIsPutOnTheScaleDrawing = basic.Scale;
            configuration.EXTCONFIsExecuteLISP = basic.LispOrDll;
            configuration.EXTCONFIsPurge = basic.Purge;
            configuration.PROGRAMMessage = basic.Message;
            configuration.EXTCONFIsDeleteTeklaStructures = basic.DeleteTeklaStructures;
            configuration.ExplodeBlocks = basic.ExplodeBlocks;
            configuration.LayerTeklaString = basic.LayerTeklaString;
            configuration.LayerBlockAttribute = basic.LayerBlockAttribute;
            configuration.EXTCONFInventorExplode = basic.CadExplode;
            if (basic.DmBlockSpecified)
                configuration.DMBlock = basic.DmBlock;

            var dimension = DimensionConfig ?? new DimensionConfigXml();
            configuration.EXTDIMGERALHabilit = dimension.DimGeralHabilit;
            configuration.EXTDIMlayer = dimension.DimLayer;
            configuration.EXTDIMColorLine = dimension.DimLineColor;
            configuration.EXTDIMColorText = dimension.DimTextColor;
            configuration.EXTDIMStyleName = dimension.DimStyle;
            configuration.EXTDIMSeta = dimension.DimArrowType;
            configuration.EXTDIMScale = dimension.DimScale;
            configuration.EXTDIMPrecision = dimension.DimPrecision;
            configuration.EXTDIMAngularPrecision = dimension.DimAngularPrecision;
            configuration.EXTDIMUnit = dimension.DimUnit;
            configuration.EXTDIMAngularUnit = dimension.DimAngularUnit;
            configuration.EXTDIMSizeSeta = dimension.DimArrowSize;
            configuration.EXTDIMOffsetLineFromRefPoint = dimension.DimOffset;
            configuration.EXTDIMOutsideAlign = dimension.DimOutsideAling;
            configuration.EXTDIMTad = dimension.DimTad;
            configuration.EXTDIMDimensionPosition = dimension.DimPosition;
            configuration.EXTDIMTextForced = dimension.DimTextForced;
            configuration.EXTDIMLineForced = dimension.DimLineForced;
            configuration.EXTDIMDIMEX = dimension.DimDimex;
            configuration.EXTDIMBaseLayer = dimension.DimBaseLayer;
            configuration.EXTDIMCorrigeSeta = dimension.DimArrowFix;
            configuration.EXTDIMCorrigeSetaTipoSeta = dimension.DimArrowFixType;
            configuration.EXTDIMCorrigeSetaFactor = dimension.DimArrowFactor;

            var textConfig = TextConfig ?? new TextConfigXml();
            configuration.EXTTEXTStyleName = textConfig.TextStyleName;

            var scale = ScaleConfig ?? new ScaleConfigXml();
            configuration.EXTSCALEManual = scale.ScaleMode;
            configuration.EXTSCALEp1.X = scale.GetPoint1X();
            configuration.EXTSCALEp1.Y = scale.GetPoint1Y();
            configuration.EXTSCALEp1.Z = scale.GetPoint1Z();
            configuration.EXTSCALEp2.X = scale.GetPoint2X();
            configuration.EXTSCALEp2.Y = scale.GetPoint2Y();
            configuration.EXTSCALEp2.Z = scale.GetPoint2Z();
            configuration.EXTSCALELayer = scale.ScaleLayer;
            configuration.EXTSCALETextSize = scale.ScaleTextSize;

            configuration.EXTLINELtscale = BasicLayers?.LtScale ?? configuration.EXTLINELtscale;
            arranjos.allBaseLayer.AddRange(BasicLayers?.BaseLayers ?? new List<string>());
            arranjos.allLineType1.AddRange(BasicLines?.BaseLines ?? new List<string>());
            arranjos.allNewLayerComposition.AddRange(NewLayers?.Layers ?? new List<string>());

            ApplyTextStyles(arranjos, textConfig);

            foreach (string layer in arranjos.allNewLayerComposition)
                arranjos.allNewLayer.Add(layer.Split(':').First());

            foreach (string line in RemoveLayers?.Layers ?? new List<string>())
            {
                string[] st = line.Split('$').Last().Split(';');
                Filter filter = new Filter(arranjos);
                filter.layerBase = st[0];
                filter.SetConjunto(st[1]);
                arranjos.layerRemove.Add(filter);
            }

            arranjos.conversor.AddRange(Converters?.Items ?? new List<string>());
            arranjos.listLISPCommand.AddRange(Commands?.Items ?? new List<string>());

            var blockConfig = BlockConfig ?? new BlockConfigXml();
            configuration.PROGRAMblockFormatoCaminho = blockConfig.DirectoryTeklaConversion;
            configuration.EXTCONFCaminhoBlocoInv = blockConfig.DirectoryCadConversion;
            arranjos.allExplodeLayers.AddRange((blockConfig.LayerExplode ?? string.Empty).Split(';'));

            blocks.AddRange((blockConfig.TeklaBlocks ?? new List<BlockXml>()).Select(CreateTeklaBlock));
            blocosi.AddRange((blockConfig.CadBlocks ?? new List<BlockXml>()).Select(CreateRelatedBlock));
            blocoso.AddRange((blockConfig.OrigBlocks ?? new List<BlockXml>()).Select(CreateRelatedBlock));
        }

        private void ApplyTextStyles(Arranjos arranjos, TextConfigXml textConfig)
        {
            arranjos.allTextSyles.Clear();

            if (NewTextStyles?.TextStyles != null && NewTextStyles.TextStyles.Count > 0)
            {
                arranjos.allTextSyles.AddRange(NewTextStyles.TextStyles);
                return;
            }

            if (!string.IsNullOrWhiteSpace(textConfig.Font))
            {
                arranjos.allTextSyles.Add(textConfig.TextStyleName + ":" +
                    textConfig.Font + ":" +
                    textConfig.Italic + ":" +
                    textConfig.Bold + ":" +
                    textConfig.Size + ":" +
                    textConfig.WidthFactor + ":" +
                    textConfig.ObliqueAngle);
                return;
            }

            arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);
        }

        private static BlockXml CreateTeklaBlock(Block block)
        {
            return new BlockXml
            {
                Nome = block.blockName,
                Tags = block.listTags.Select(tag => tag.GetConjuntoString()).ToList()
            };
        }

        private static Block CreateTeklaBlock(BlockXml block)
        {
            Block result = new Block { blockName = block.Nome };
            foreach (string tag in block.Tags ?? new List<string>())
            {
                TagBlock tagBlock = new TagBlock();
                tagBlock.SetConjunto(tag);
                result.listTags.Add(tagBlock);
            }

            return result;
        }

        private static BlockXml CreateRelatedBlock(Block block)
        {
            return new BlockXml
            {
                Nome = block.blockName + ";" + block.blockNameRelacao + ";" + block.cor.ToArgb(),
                Tags = block.listTags.Select(tag => tag.GetConjuntoString() + "@" + tag.indiceRelacao + "@" + tag.isSociate).ToList()
            };
        }

        private static Block CreateRelatedBlock(BlockXml block)
        {
            string[] linesplit = block.Nome.Split(';');
            Block result = new Block
            {
                blockName = linesplit[0],
                blockNameRelacao = linesplit[1],
                cor = Color.FromArgb(System.Convert.ToInt32(linesplit[2]))
            };

            foreach (string tag in block.Tags ?? new List<string>())
            {
                TagBlock tagBlock = new TagBlock();
                tagBlock.SetConjunto(tag);
                string[] linetemp = tag.Split('@');
                tagBlock.indiceRelacao = System.Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                tagBlock.isSociate = System.Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                result.listTags.Add(tagBlock);
            }

            return result;
        }
    }

    public sealed class CommentsXml
    {
        [XmlAttribute("TEXT")]
        public string Text { get; set; } = string.Empty;
    }

    public sealed class BasicConfigXml
    {
        [XmlAttribute("TEKLAORCAD")]
        public int TeklaOrCad { get; set; }

        [XmlAttribute("CONVERT_DIMENSIONS")]
        public bool ConvertDimensions { get; set; }

        [XmlAttribute("CONVERT_LAYERS")]
        public bool ConvertLayers { get; set; }

        [XmlAttribute("EXCHANGE_FORMAT")]
        public bool ExchangeFormat { get; set; }

        [XmlAttribute("SCALE")]
        public bool Scale { get; set; }

        [XmlAttribute("LISPORDLL")]
        public bool LispOrDll { get; set; }

        [XmlAttribute("PURGE")]
        public bool Purge { get; set; }

        [XmlAttribute("MESSAGE")]
        public bool Message { get; set; }

        [XmlAttribute("DELETE_TEKLA_STRUCTURES")]
        public bool DeleteTeklaStructures { get; set; }

        [XmlAttribute("EXPLOD_BLOCKS")]
        public bool ExplodeBlocks { get; set; }

        [XmlAttribute("LAYER_TEKLA_STRING")]
        public string LayerTeklaString { get; set; } = string.Empty;

        [XmlAttribute("LAYER_BLOCK_ATTRIBUTE")]
        public string LayerBlockAttribute { get; set; } = string.Empty;

        [XmlAttribute("CAD_EXPLODE")]
        public bool CadExplode { get; set; }

        [XmlAttribute("DMBLOCK")]
        public bool DmBlock { get; set; }

        [XmlIgnore]
        public bool DmBlockSpecified { get; set; }
    }

    public sealed class DimensionConfigXml
    {
        [XmlAttribute("DIM_GERAL_HABILIT")]
        public bool DimGeralHabilit { get; set; }

        [XmlAttribute("DIM_LAYER")]
        public string DimLayer { get; set; } = string.Empty;

        [XmlAttribute("DIM_LINE_COLOR")]
        public string DimLineColor { get; set; } = string.Empty;

        [XmlAttribute("DIM_TEXT_COLOR")]
        public string DimTextColor { get; set; } = string.Empty;

        [XmlAttribute("DIM_STYLE")]
        public string DimStyle { get; set; } = string.Empty;

        [XmlAttribute("DIM_ARROW_TYPE")]
        public string DimArrowType { get; set; } = string.Empty;

        [XmlAttribute("DIM_SCALE")]
        public double DimScale { get; set; }

        [XmlAttribute("DIM_PRECISION")]
        public int DimPrecision { get; set; }

        [XmlAttribute("DIM_ANGULAR_PRECISION")]
        public int DimAngularPrecision { get; set; }

        [XmlAttribute("DIM_UNIT")]
        public int DimUnit { get; set; }

        [XmlAttribute("DIM_ANGULAR_UNIT")]
        public int DimAngularUnit { get; set; }

        [XmlAttribute("DIM_ARROW_SIZE")]
        public double DimArrowSize { get; set; }

        [XmlAttribute("DIM_OFFSET")]
        public double DimOffset { get; set; }

        [XmlAttribute("DIM_OUTSIDE_ALING")]
        public bool DimOutsideAling { get; set; }

        [XmlAttribute("DIM_TAD")]
        public int DimTad { get; set; }

        [XmlAttribute("DIM_POSITION")]
        public bool DimPosition { get; set; }

        [XmlAttribute("DIM_TEXT_FORCED")]
        public bool DimTextForced { get; set; }

        [XmlAttribute("DIM_LINE_FORCED")]
        public bool DimLineForced { get; set; }

        [XmlAttribute("DIM_DIMEX")]
        public double DimDimex { get; set; }

        [XmlAttribute("DIM_BASE_LAYER")]
        public string DimBaseLayer { get; set; } = string.Empty;

        [XmlAttribute("DIM_ARROW_FIX")]
        public bool DimArrowFix { get; set; }

        [XmlAttribute("DIM_ARROW_FIX_TYPE")]
        public string DimArrowFixType { get; set; } = string.Empty;

        [XmlAttribute("DIM_ARROW_FACTOR")]
        public double DimArrowFactor { get; set; }
    }

    public sealed class TextConfigXml
    {
        [XmlAttribute("TEXT_STYPE")]
        public string TextStyleName { get; set; } = string.Empty;

        [XmlAttribute("TEXT_FONTE")]
        public string Font { get; set; } = string.Empty;

        [XmlAttribute("TEXT_TAMANHO")]
        public string Size { get; set; } = string.Empty;

        [XmlAttribute("TEXT_LARGURA")]
        public string WidthFactor { get; set; } = string.Empty;

        [XmlAttribute("TEXT_ITALICO")]
        public string Italic { get; set; } = string.Empty;

        [XmlAttribute("TEXT_NEGRITO")]
        public string Bold { get; set; } = string.Empty;

        [XmlAttribute("TEXT_OBLIQUE_ANGLE")]
        public string ObliqueAngle { get; set; } = string.Empty;
    }

    public sealed class ScaleConfigXml
    {
        [XmlAttribute("SCALE_MODE")]
        public bool ScaleMode { get; set; }

        [XmlAttribute("SCALE_P1_X")]
        public double ScaleP1X { get; set; }

        [XmlIgnore]
        public bool ScaleP1XSpecified { get; set; }

        [XmlAttribute("SCALE_P1_Y")]
        public double ScaleP1Y { get; set; }

        [XmlIgnore]
        public bool ScaleP1YSpecified { get; set; }

        [XmlAttribute("SCALE_P1_Z")]
        public double ScaleP1Z { get; set; }

        [XmlIgnore]
        public bool ScaleP1ZSpecified { get; set; }

        [XmlAttribute("SCALE_P2_X")]
        public double ScaleP2X { get; set; }

        [XmlIgnore]
        public bool ScaleP2XSpecified { get; set; }

        [XmlAttribute("SCALE_P2_Y")]
        public double ScaleP2Y { get; set; }

        [XmlIgnore]
        public bool ScaleP2YSpecified { get; set; }

        [XmlAttribute("SCALE_P2_Z")]
        public double ScaleP2Z { get; set; }

        [XmlIgnore]
        public bool ScaleP2ZSpecified { get; set; }

        [XmlAttribute("SCALE_MANUAL_P1_X")]
        public double ScaleManualP1X { get; set; }

        [XmlAttribute("SCALE_MANUAL_P1_Y")]
        public double ScaleManualP1Y { get; set; }

        [XmlAttribute("SCALE_MANUAL_P1_Z")]
        public double ScaleManualP1Z { get; set; }

        [XmlAttribute("SCALE_MANUAL_P2_X")]
        public double ScaleManualP2X { get; set; }

        [XmlAttribute("SCALE_MANUAL_P2_Y")]
        public double ScaleManualP2Y { get; set; }

        [XmlAttribute("SCALE_MANUAL_P2_Z")]
        public double ScaleManualP2Z { get; set; }

        [XmlAttribute("SCALE_AUTO_P1_X")]
        public double ScaleAutoP1X { get; set; }

        [XmlAttribute("SCALE_AUTO_P1_Y")]
        public double ScaleAutoP1Y { get; set; }

        [XmlAttribute("SCALE_AUTO_P1_Z")]
        public double ScaleAutoP1Z { get; set; }

        [XmlAttribute("SCALE_AUTO_P2_X")]
        public double ScaleAutoP2X { get; set; }

        [XmlAttribute("SCALE_AUTO_P2_Y")]
        public double ScaleAutoP2Y { get; set; }

        [XmlAttribute("SCALE_AUTO_P2_Z")]
        public double ScaleAutoP2Z { get; set; }

        [XmlAttribute("SCALE_LAYER")]
        public string ScaleLayer { get; set; } = string.Empty;

        [XmlAttribute("SCALE_TEXT_SIZE")]
        public double ScaleTextSize { get; set; }

        public double GetPoint1X() => ScaleP1XSpecified ? ScaleP1X : GetLegacyPoint(ScaleManualP1X, ScaleAutoP1X);
        public double GetPoint1Y() => ScaleP1YSpecified ? ScaleP1Y : GetLegacyPoint(ScaleManualP1Y, ScaleAutoP1Y);
        public double GetPoint1Z() => ScaleP1ZSpecified ? ScaleP1Z : GetLegacyPoint(ScaleManualP1Z, ScaleAutoP1Z);
        public double GetPoint2X() => ScaleP2XSpecified ? ScaleP2X : GetLegacyPoint(ScaleManualP2X, ScaleAutoP2X);
        public double GetPoint2Y() => ScaleP2YSpecified ? ScaleP2Y : GetLegacyPoint(ScaleManualP2Y, ScaleAutoP2Y);
        public double GetPoint2Z() => ScaleP2ZSpecified ? ScaleP2Z : GetLegacyPoint(ScaleManualP2Z, ScaleAutoP2Z);

        private double GetLegacyPoint(double manualValue, double autoValue)
        {
            return ScaleMode ? manualValue : autoValue;
        }
    }

    public sealed class BasicLayersXml
    {
        [XmlAttribute("LTSCALE")]
        public double LtScale { get; set; }

        [XmlElement("BASE_LAYER")]
        public List<string> BaseLayers { get; set; } = new List<string>();
    }

    public sealed class BasicLinesXml
    {
        [XmlElement("BASE_LINE")]
        public List<string> BaseLines { get; set; } = new List<string>();
    }

    public sealed class TextStylesXml
    {
        [XmlElement("TEXT_STYLE")]
        public List<string> TextStyles { get; set; } = new List<string>();
    }

    public sealed class NewLayersXml
    {
        [XmlElement("NEW_LAYER")]
        public List<string> Layers { get; set; } = new List<string>();
    }

    public sealed class RemoveLayersXml
    {
        [XmlElement("REMOVE_LAYER")]
        public List<string> Layers { get; set; } = new List<string>();
    }

    public sealed class ConvertersXml
    {
        [XmlElement("CONVERTER")]
        public List<string> Items { get; set; } = new List<string>();
    }

    public sealed class CommandsXml
    {
        [XmlElement("COMMAND")]
        public List<string> Items { get; set; } = new List<string>();
    }

    public sealed class BlockConfigXml
    {
        [XmlAttribute("DIRECTORY_TEKLA_CONVERSION")]
        public string DirectoryTeklaConversion { get; set; } = string.Empty;

        [XmlAttribute("DIRECTORY_CAD_CONVERSION")]
        public string DirectoryCadConversion { get; set; } = string.Empty;

        [XmlAttribute("LAYER_EXPLODE")]
        public string LayerExplode { get; set; } = string.Empty;

        [XmlElement("BLOCK_ATT")]
        public List<BlockXml> TeklaBlocks { get; set; } = new List<BlockXml>();

        [XmlElement("BLOCK_ATT_CAD")]
        public List<BlockXml> CadBlocks { get; set; } = new List<BlockXml>();

        [XmlElement("BLOCK_ATT_ORIG")]
        public List<BlockXml> OrigBlocks { get; set; } = new List<BlockXml>();
    }

    public sealed class BlockXml
    {
        [XmlAttribute("NOME")]
        public string Nome { get; set; } = string.Empty;

        [XmlElement("TAG")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}



