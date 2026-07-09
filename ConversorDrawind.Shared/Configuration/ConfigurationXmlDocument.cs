using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ConversorDrawind
{
#pragma warning disable CS0618
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
            XDocument document = XDocument.Load(file);
            NormalizeLegacyDecimalAttributes(document);
            NormalizeLegacyRootElementOrder(document);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting))))
            {
                return (ConfigurationXmlDocument)Serializer.Deserialize(stream);
            }
        }

        private static void NormalizeLegacyDecimalAttributes(XDocument document)
        {
            foreach (XAttribute attribute in document.Descendants().Attributes())
            {
                string value = attribute.Value;

                if (string.IsNullOrWhiteSpace(value) || !value.Contains(","))
                    continue;

                string normalized = value.Replace(',', '.');
                double parsed;

                if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                    attribute.Value = parsed.ToString("R", CultureInfo.InvariantCulture);
            }
        }

        private static void NormalizeLegacyRootElementOrder(XDocument document)
        {
            XElement root = document.Root;
            if (root == null)
                return;

            string[] orderedNames =
            {
                "COMMENTS",
                "BASIC_CONFIG",
                "DIMENSION_CONFIG",
                "TEXT_CONFIG",
                "SCALE_CONFIG",
                "BASIC_LAYERS",
                "BASIC_LINES",
                "NEW_TEXTSTYLES",
                "NEW_LAYERS",
                "REMOVE_LAYERS",
                "CONVERTERS",
                "DLL_OR_LIST_COMMANDS",
                "BLOCK_CONFIG"
            };

            List<XElement> elements = root.Elements().ToList();
            if (elements.Count == 0)
                return;

            root.RemoveNodes();

            foreach (string name in orderedNames)
                root.Add(elements.Where(element => element.Name.LocalName == name));

            root.Add(elements.Where(element => !orderedNames.Contains(element.Name.LocalName)));
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


        public void ApplyTo(Configuration configuration, LegacyConfigurationState state)
        {
            state.BaseLayers.Clear();
            state.NewLayerDefinitions.Clear();
            state.NewLayerNames.Clear();
            state.ConversionRules.Clear();
            state.RemoveRules.Clear();
            state.LispCommands.Clear();
            state.ExplodeLayers.Clear();
            configuration.Blocks.TeklaBlocks.Clear();
            configuration.Blocks.CadBlocks.Clear();
            configuration.Blocks.OriginalBlocks.Clear();

            configuration.Comments = Comments?.Text ?? string.Empty;

            var basic = BasicConfig ?? new BasicConfigXml();
            configuration.General.SourceMode = basic.TeklaOrCad;
            configuration.General.ConverterType = basic.TeklaOrCad;
            configuration.General.ConvertDimensions = basic.ConvertDimensions;
            configuration.General.ConvertLayers = basic.ConvertLayers;
            configuration.General.ExchangeFormat = basic.ExchangeFormat;
            configuration.General.ApplyDrawingScale = basic.Scale;
            configuration.General.ExecuteLisp = basic.LispOrDll;
            configuration.General.Purge = basic.Purge;
            configuration.General.ShowMessages = basic.Message;
            configuration.General.DeleteTeklaStructures = basic.DeleteTeklaStructures;
            configuration.General.ExplodeBlocks = basic.ExplodeBlocks;
            configuration.Layers.TeklaDrawingSheetLayer = basic.LayerTeklaString;
            configuration.Layers.BlockAttributeLayer = basic.LayerBlockAttribute;
            configuration.General.InventorExplode = basic.CadExplode;
            if (basic.DmBlockSpecified)
                configuration.Blocks.DimensionBlockEnabled = basic.DmBlock;

            var dimension = DimensionConfig ?? new DimensionConfigXml();
            configuration.Dimensions.Enabled = dimension.DimGeralHabilit;
            configuration.Dimensions.Layer = dimension.DimLayer;
            configuration.Dimensions.LineColor = dimension.DimLineColor;
            configuration.Dimensions.TextColor = dimension.DimTextColor;
            configuration.Dimensions.StyleName = dimension.DimStyle;
            configuration.Dimensions.ArrowType = dimension.DimArrowType;
            configuration.Dimensions.Scale = dimension.DimScale;
            configuration.Dimensions.Precision = dimension.DimPrecision;
            configuration.Dimensions.AngularPrecision = dimension.DimAngularPrecision;
            configuration.Dimensions.Unit = dimension.DimUnit;
            configuration.Dimensions.AngularUnit = dimension.DimAngularUnit;
            configuration.Dimensions.ArrowSize = dimension.DimArrowSize;
            configuration.Dimensions.OffsetLineFromReferencePoint = dimension.DimOffset;
            configuration.Dimensions.OutsideAlign = dimension.DimOutsideAling;
            configuration.Dimensions.TextVerticalPosition = dimension.DimTad;
            configuration.Dimensions.TextRelativeToDimensionLine = dimension.DimPosition;
            configuration.Dimensions.ForceTextInside = dimension.DimTextForced;
            configuration.Dimensions.ForceDimensionLine = dimension.DimLineForced;
            configuration.Dimensions.ExtensionLineOffset = dimension.DimDimex;
            configuration.Dimensions.BaseLayer = dimension.DimBaseLayer;
            configuration.Dimensions.FixArrow = dimension.DimArrowFix;
            configuration.Dimensions.FixArrowType = dimension.DimArrowFixType;
            configuration.Dimensions.FixArrowFactor = dimension.DimArrowFactor;

            var textConfig = TextConfig ?? new TextConfigXml();
            configuration.Text.DefaultStyleName = textConfig.TextStyleName;

            var scale = ScaleConfig ?? new ScaleConfigXml();
            configuration.Scale.Manual = scale.ScaleMode;
            configuration.Scale.Point1.X = scale.GetPoint1X();
            configuration.Scale.Point1.Y = scale.GetPoint1Y();
            configuration.Scale.Point1.Z = scale.GetPoint1Z();
            configuration.Scale.Point2.X = scale.GetPoint2X();
            configuration.Scale.Point2.Y = scale.GetPoint2Y();
            configuration.Scale.Point2.Z = scale.GetPoint2Z();
            configuration.Scale.Layer = scale.ScaleLayer;
            configuration.Scale.TextSize = scale.ScaleTextSize;

            configuration.Lines.LineTypeScale = BasicLayers?.LtScale ?? configuration.Lines.LineTypeScale;
            state.BaseLayers.AddRange(BasicLayers?.BaseLayers ?? new List<string>());
            configuration.Lines.BaseLineTypes = (BasicLines?.BaseLines ?? new List<string>()).ToList();
            configuration.Catalogs.FilterLineTypes = configuration.Lines.BaseLineTypes.ToList();
            state.NewLayerDefinitions.AddRange(NewLayers?.Layers ?? new List<string>());

            ApplyTextStyles(state, textConfig);

            foreach (string layer in state.NewLayerDefinitions)
                state.NewLayerNames.Add(layer.Split(':').First());

            foreach (string line in RemoveLayers?.Layers ?? new List<string>())
            {
                string[] st = line.Split('$').Last().Split(';');
                Filter filter = new Filter(configuration.Catalogs);
                filter.layerBase = st[0];
                filter.SetConjunto(st[1]);
                state.RemoveRules.Add(filter);
            }

            state.ConversionRules.AddRange(Converters?.Items ?? new List<string>());
            state.LispCommands.AddRange(Commands?.Items ?? new List<string>());

            var blockConfig = BlockConfig ?? new BlockConfigXml();
            configuration.Blocks.TeklaBlockPath = blockConfig.DirectoryTeklaConversion;
            configuration.Blocks.CadBlockPath = blockConfig.DirectoryCadConversion;
            state.ExplodeLayers.AddRange((blockConfig.LayerExplode ?? string.Empty).Split(';'));

            configuration.Blocks.TeklaBlocks.AddRange((blockConfig.TeklaBlocks ?? new List<BlockXml>()).Select(CreateTeklaBlockDefinition));
            configuration.Blocks.CadBlocks.AddRange((blockConfig.CadBlocks ?? new List<BlockXml>()).Select(CreateRelatedBlockDefinition));
            configuration.Blocks.OriginalBlocks.AddRange((blockConfig.OrigBlocks ?? new List<BlockXml>()).Select(CreateRelatedBlockDefinition));
        }

        private void ApplyTextStyles(LegacyConfigurationState state, TextConfigXml textConfig)
        {
            state.TextStyles.Clear();

            if (NewTextStyles?.TextStyles != null && NewTextStyles.TextStyles.Count > 0)
            {
                state.TextStyles.AddRange(NewTextStyles.TextStyles);
                return;
            }

            if (!string.IsNullOrWhiteSpace(textConfig.Font))
            {
                state.TextStyles.Add(textConfig.TextStyleName + ":" +
                    textConfig.Font + ":" +
                    textConfig.Italic + ":" +
                    textConfig.Bold + ":" +
                    textConfig.Size + ":" +
                    textConfig.WidthFactor + ":" +
                    textConfig.ObliqueAngle);
                return;
            }

            state.TextStyles.Add(Defaults.LegacyTextStyle());
        }

        private static BlockXml CreateTeklaBlock(Block block)
        {
            return new BlockXml
            {
                Nome = block.blockName,
                Tags = block.listTags.Select(tag => tag.GetConjuntoString()).ToList()
            };
        }

        private static BlockDefinition CreateTeklaBlockDefinition(BlockXml block)
        {
            BlockDefinition result = new BlockDefinition { Name = block.Nome };
            foreach (string tag in block.Tags ?? new List<string>())
            {
                TagBlock tagBlock = new TagBlock();
                tagBlock.SetConjunto(tag);
                result.Tags.Add(ToTagDefinition(tagBlock));
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

        private static BlockDefinition CreateRelatedBlockDefinition(BlockXml block)
        {
            string[] linesplit = block.Nome.Split(';');
            BlockDefinition result = new BlockDefinition
            {
                Name = linesplit[0],
                RelatedName = linesplit[1],
                ColorArgb = System.Convert.ToInt32(linesplit[2])
            };

            foreach (string tag in block.Tags ?? new List<string>())
            {
                TagBlock tagBlock = new TagBlock();
                tagBlock.SetConjunto(tag);
                string[] linetemp = tag.Split('@');
                tagBlock.indiceRelacao = System.Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                tagBlock.isSociate = System.Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                result.Tags.Add(ToTagDefinition(tagBlock));
            }

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

        private static Point3DConfiguration ToPoint(PointEspecial point)
        {
            point = point ?? new PointEspecial(0, 0, 0);
            return new Point3DConfiguration { X = point.X, Y = point.Y, Z = point.Z };
        }
    }
#pragma warning restore CS0618

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




