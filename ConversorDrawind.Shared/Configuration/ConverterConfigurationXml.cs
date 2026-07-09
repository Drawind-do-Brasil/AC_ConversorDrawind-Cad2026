using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ConversorDrawind
{
    public interface IConfigurationRepository
    {
        bool Exists(string converterName, StatusConversorItem statusConversorItem);
        Configuration Load(string converterName, StatusConversorItem statusConversorItem);
        void Save(string converterName, StatusConversorItem statusConversorItem, Configuration configuration);
    }

    public sealed class TxmlConfigurationRepository : IConfigurationRepository
    {
        public bool Exists(string converterName, StatusConversorItem statusConversorItem)
        {
            return File.Exists(ResolvePath(converterName, statusConversorItem));
        }

        public Configuration Load(string converterName, StatusConversorItem statusConversorItem)
        {
            return ConfigurationReader.Load(ResolvePath(converterName, statusConversorItem));
        }

        public void Save(string converterName, StatusConversorItem statusConversorItem, Configuration configuration)
        {
            StructuredConfigurationXmlWriter.Save(ResolvePath(converterName, statusConversorItem), configuration.ToConverterConfiguration());
        }

        private static string ResolvePath(string converterName, StatusConversorItem statusConversorItem)
        {
            if (Path.IsPathRooted(converterName))
                return converterName.EndsWith(".txml", StringComparison.OrdinalIgnoreCase) ? converterName : converterName + ".txml";

            return ConfigurationPaths.TxmlPath(converterName, statusConversorItem);
        }
    }

    public static class ConfigurationReader
    {
        public static Configuration Load(string file)
        {
            return new Configuration(ConverterConfigurationReader.Load(file));
        }
    }

    public static class ConverterConfigurationReader
    {
        public static Configuration Load(string file)
        {
            XDocument document = XDocument.Load(file);
            string version = (string)document.Root.Attribute("VERSION");
            if (version == "2")
                return StructuredConfigurationXmlReader.Read(document);

            return LegacyConfigurationXmlReader.Load(file);
        }
    }

    public static class LegacyConfigurationXmlReader
    {
        public static Configuration Load(string file)
        {
            Configuration configuration = new Configuration();
            Arranjos arranjos = new Arranjos();
            List<Block> teklaBlocks = new List<Block>();
            List<Block> cadBlocks = new List<Block>();
            List<Block> originalBlocks = new List<Block>();

            ConfigurationXmlDocument
                .Load(file)
                .ApplyTo(configuration, arranjos, teklaBlocks, cadBlocks, originalBlocks);

            return ConfigurationCompatibilityMapper.FromLegacyState(
                configuration,
                arranjos,
                teklaBlocks,
                cadBlocks,
                originalBlocks);
        }
    }

    public static class StructuredConfigurationXmlWriter
    {
        public static void Save(string file, Configuration configuration)
        {
            string directory = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            XDocument document = CreateDocument(configuration ?? new Configuration());
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true
            };

            using (XmlWriter writer = XmlWriter.Create(file, settings))
            {
                document.Save(writer);
            }
        }

        public static XDocument CreateDocument(Configuration configuration)
        {
            return new XDocument(
                new XElement("CONVERSOR",
                    new XAttribute("VERSION", "2"),
                    new XElement("COMMENTS", Attr("TEXT", configuration.Comments)),
                    General(configuration.General),
                    Dimensions(configuration.Dimensions),
                    Text(configuration.Text),
                    Scale(configuration.Scale),
                    Layers(configuration.Layers),
                    Lines(configuration.Lines),
                    Catalogs(configuration.Catalogs),
                    Commands(configuration.Commands),
                    Blocks(configuration.Blocks)));
        }

        private static XElement General(GeneralConfiguration value)
        {
            value = value ?? new GeneralConfiguration();
            return new XElement("GENERAL",
                Attr("SOURCE_MODE", value.SourceMode),
                Attr("CONVERTER_TYPE", value.ConverterType),
                Attr("CONVERT_DIMENSIONS", value.ConvertDimensions),
                Attr("CONVERT_LAYERS", value.ConvertLayers),
                Attr("EXCHANGE_FORMAT", value.ExchangeFormat),
                Attr("EXCHANGE_LM", value.ExchangeLm),
                Attr("APPLY_DRAWING_SCALE", value.ApplyDrawingScale),
                Attr("EXECUTE_LISP", value.ExecuteLisp),
                Attr("EXECUTE_DLL", value.ExecuteDll),
                Attr("FIRST_RUN_MODE", value.FirstRunMode),
                Attr("DELETE_TEKLA_STRUCTURES", value.DeleteTeklaStructures),
                Attr("PURGE", value.Purge),
                Attr("SHOW_MESSAGES", value.ShowMessages),
                Attr("EXPLODE_BLOCKS", value.ExplodeBlocks),
                Attr("INVENTOR_EXPLODE", value.InventorExplode));
        }

        private static XElement Dimensions(DimensionConfiguration value)
        {
            value = value ?? new DimensionConfiguration();
            return new XElement("DIMENSIONS",
                Attr("REFERENCE_FORMAT_SIZE", value.ReferenceFormatSize),
                Attr("INTERNAL_LENGTH_CHAR_FACTOR", value.InternalLengthCharFactor),
                Attr("INTERNAL_TEXT_OFFSET", value.InternalTextOffset),
                Attr("ENABLED", value.Enabled),
                Attr("LAYER", value.Layer),
                Attr("BASE_LAYER", value.BaseLayer),
                Attr("LINE_COLOR", value.LineColor),
                Attr("TEXT_COLOR", value.TextColor),
                Attr("STYLE_NAME", value.StyleName),
                Attr("ARROW_TYPE", value.ArrowType),
                Attr("ARROW_TYPE_1", value.ArrowType1),
                Attr("ARROW_TYPE_2", value.ArrowType2),
                Attr("SCALE", value.Scale),
                Attr("PRECISION", value.Precision),
                Attr("ANGULAR_PRECISION", value.AngularPrecision),
                Attr("UNIT", value.Unit),
                Attr("ANGULAR_UNIT", value.AngularUnit),
                Attr("ARROW_SIZE", value.ArrowSize),
                Attr("TEXT_VERTICAL_POSITION", value.TextVerticalPosition),
                Attr("TEXT_RELATIVE_TO_DIMENSION_LINE", value.TextRelativeToDimensionLine),
                Attr("FORCE_TEXT_INSIDE", value.ForceTextInside),
                Attr("FORCE_DIMENSION_LINE", value.ForceDimensionLine),
                Attr("OFFSET_LINE_FROM_REFERENCE_POINT", value.OffsetLineFromReferencePoint),
                Attr("TEXT_MOVE", value.TextMove),
                Attr("OUTSIDE_ALIGN", value.OutsideAlign),
                Attr("EXTENSION_LINE_OFFSET", value.ExtensionLineOffset),
                Attr("FIX_ARROW", value.FixArrow),
                Attr("FIX_ARROW_TYPE", value.FixArrowType),
                Attr("FIX_ARROW_FACTOR", value.FixArrowFactor));
        }

        private static XElement Text(TextConfiguration value)
        {
            value = value ?? new TextConfiguration();
            return new XElement("TEXT",
                Attr("DEFAULT_STYLE_NAME", value.DefaultStyleName),
                Attr("DEFAULT_SIZE", value.DefaultSize),
                (value.Styles ?? new List<TextStyleDefinition>()).Select(Style));
        }

        private static XElement Style(TextStyleDefinition value)
        {
            return new XElement("STYLE",
                Attr("NAME", value.Name),
                Attr("FONT", value.Font),
                Attr("ITALIC", value.Italic),
                Attr("BOLD", value.Bold),
                Attr("SIZE", value.Size),
                Attr("WIDTH_FACTOR", value.WidthFactor),
                Attr("OBLIQUE_ANGLE", value.ObliqueAngle));
        }

        private static XElement Scale(ScaleConfiguration value)
        {
            value = value ?? new ScaleConfiguration();
            return new XElement("SCALE",
                Attr("MANUAL", value.Manual),
                Point("POINT1", value.Point1),
                Point("POINT2", value.Point2),
                Attr("LAYER", value.Layer),
                Attr("TEXT_SIZE", value.TextSize));
        }

        private static XElement Layers(LayerConfiguration value)
        {
            value = value ?? new LayerConfiguration();
            return new XElement("LAYERS",
                Attr("TEKLA_DRAWING_SHEET_LAYER", value.TeklaDrawingSheetLayer),
                Attr("BLOCK_ATTRIBUTE_LAYER", value.BlockAttributeLayer),
                (value.BaseLayers ?? new List<string>()).Select(item => new XElement("BASE_LAYER", Attr("NAME", item))),
                (value.NewLayers ?? new List<LayerDefinition>()).Select(item => new XElement("NEW_LAYER",
                    Attr("NAME", item.Name),
                    Attr("COLOR", item.Color),
                    Attr("LINE_TYPE", item.LineType))),
                (value.RemoveRules ?? new List<LayerRemoveRule>()).Select(item => new XElement("REMOVE_RULE", Filter("FILTER", item.Filter))),
                (value.ConversionRules ?? new List<LayerConversionRule>()).Select(item => new XElement("CONVERSION_RULE",
                    Filter("SOURCE", item.Source),
                    new XElement("TARGET",
                        Attr("LAYER_NAME", item.Target.LayerName),
                        Attr("COLOR", item.Target.Color),
                        Attr("LINE_TYPE", item.Target.LineType),
                        Attr("TEXT_CONTENT", item.Target.TextContent),
                        Attr("TEXT_HEIGHT", item.Target.TextHeight),
                        Attr("TEXT_STYLE", item.Target.TextStyle)))),
                (value.ExplodeLayers ?? new List<string>()).Select(item => new XElement("EXPLODE_LAYER", Attr("NAME", item))));
        }

        private static XElement Lines(LineConfiguration value)
        {
            value = value ?? new LineConfiguration();
            return new XElement("LINES",
                Attr("LINE_TYPE_SCALE", value.LineTypeScale),
                (value.BaseLineTypes ?? new List<string>()).Select(item => new XElement("BASE_LINE_TYPE", Attr("NAME", item))));
        }

        private static XElement Catalogs(CatalogConfiguration value)
        {
            value = value ?? new CatalogConfiguration();
            return new XElement("CATALOGS",
                StringList("COLOR", value.Colors),
                StringList("OBJECT_TYPE", value.ObjectTypes),
                StringList("FILTER_LINE_TYPE", value.FilterLineTypes),
                StringList("LAYER_LINE_TYPE", value.LayerLineTypes),
                StringList("REMOVED_LINE_TYPE", value.RemovedLineTypes));
        }

        private static IEnumerable<XElement> StringList(string elementName, IEnumerable<string> values)
        {
            return (values ?? new List<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => new XElement(elementName, Attr("NAME", value)));
        }

        private static XElement Commands(CommandConfiguration value)
        {
            value = value ?? new CommandConfiguration();
            return new XElement("COMMANDS",
                (value.LispCommands ?? new List<string>()).Select(item => new XElement("LISP_COMMAND", Attr("TEXT", item))),
                (value.DllCommands ?? new List<string>()).Select(item => new XElement("DLL_COMMAND", Attr("TEXT", item))));
        }

        private static XElement Blocks(BlockConfiguration value)
        {
            value = value ?? new BlockConfiguration();
            return new XElement("BLOCKS",
                Attr("TEKLA_BLOCK_PATH", value.TeklaBlockPath),
                Attr("CAD_BLOCK_PATH", value.CadBlockPath),
                Attr("DIMENSION_BLOCK_ENABLED", value.DimensionBlockEnabled),
                (value.TeklaBlocks ?? new List<BlockDefinition>()).Select(item => Block("TEKLA_BLOCK", item)),
                (value.CadBlocks ?? new List<BlockDefinition>()).Select(item => Block("CAD_BLOCK", item)),
                (value.OriginalBlocks ?? new List<BlockDefinition>()).Select(item => Block("ORIGINAL_BLOCK", item)));
        }

        private static XElement Block(string name, BlockDefinition value)
        {
            return new XElement(name,
                Attr("NAME", value.Name),
                Attr("RELATED_NAME", value.RelatedName),
                Attr("COLOR_ARGB", value.ColorArgb),
                (value.Tags ?? new List<BlockTagDefinition>()).Select(Tag));
        }

        private static XElement Tag(BlockTagDefinition value)
        {
            return new XElement("TAG",
                Attr("NAME", value.Name),
                Attr("MODIFY", value.Modify),
                Attr("WIDTH_FACTOR", value.WidthFactor),
                Attr("RELATED_INDEX", value.RelatedIndex),
                Attr("IS_ASSOCIATED", value.IsAssociated),
                Point("POINT1", value.Point1),
                Point("POINT2", value.Point2),
                Filter("FILTER", value.Filter));
        }

        private static XElement Point(string name, Point3DConfiguration value)
        {
            value = value ?? new Point3DConfiguration();
            return new XElement(name, Attr("X", value.X), Attr("Y", value.Y), Attr("Z", value.Z));
        }

        private static XElement Filter(string name, EntityFilter value)
        {
            value = value ?? new EntityFilter();
            return new XElement(name,
                Attr("BASE_LAYER", value.BaseLayer),
                Attr("OBJECT_TYPE", value.ObjectType),
                Attr("COLOR", value.Color),
                Attr("LINE_TYPE", value.LineType),
                Attr("TEXT_CONTENT", value.TextContent),
                Attr("TEXT_HEIGHT", value.TextHeight),
                Attr("ORIENTATION", value.Orientation));
        }

        private static XAttribute Attr(string name, string value)
        {
            return new XAttribute(name, value ?? string.Empty);
        }

        private static XAttribute Attr(string name, int value)
        {
            return new XAttribute(name, value.ToString(CultureInfo.InvariantCulture));
        }

        private static XAttribute Attr(string name, double value)
        {
            return new XAttribute(name, value.ToString("R", CultureInfo.InvariantCulture));
        }

        private static XAttribute Attr(string name, bool value)
        {
            return new XAttribute(name, value ? "true" : "false");
        }
    }

    public static class StructuredConfigurationXmlReader
    {
        public static Configuration Read(XDocument document)
        {
            XElement root = document.Root;
            Configuration result = new Configuration();

            result.Comments = S(root.Element("COMMENTS"), "TEXT");
            ReadGeneral(root.Element("GENERAL"), result.General);
            ReadDimensions(root.Element("DIMENSIONS"), result.Dimensions);
            ReadText(root.Element("TEXT"), result.Text);
            ReadScale(root.Element("SCALE"), result.Scale);
            ReadLayers(root.Element("LAYERS"), result.Layers);
            ReadLines(root.Element("LINES"), result.Lines);
            ReadCatalogs(root.Element("CATALOGS"), result.Catalogs);
            ReadCommands(root.Element("COMMANDS"), result.Commands);
            ReadBlocks(root.Element("BLOCKS"), result.Blocks);
            result.EnsureDefaults();

            return result;
        }

        private static void ReadGeneral(XElement element, GeneralConfiguration value)
        {
            if (element == null)
                return;

            value.SourceMode = I(element, "SOURCE_MODE", value.SourceMode);
            value.ConverterType = I(element, "CONVERTER_TYPE", value.ConverterType);
            value.ConvertDimensions = B(element, "CONVERT_DIMENSIONS", value.ConvertDimensions);
            value.ConvertLayers = B(element, "CONVERT_LAYERS", value.ConvertLayers);
            value.ExchangeFormat = B(element, "EXCHANGE_FORMAT", value.ExchangeFormat);
            value.ExchangeLm = B(element, "EXCHANGE_LM", value.ExchangeLm);
            value.ApplyDrawingScale = B(element, "APPLY_DRAWING_SCALE", value.ApplyDrawingScale);
            value.ExecuteLisp = B(element, "EXECUTE_LISP", value.ExecuteLisp);
            value.ExecuteDll = B(element, "EXECUTE_DLL", value.ExecuteDll);
            value.FirstRunMode = S(element, "FIRST_RUN_MODE", value.FirstRunMode);
            value.DeleteTeklaStructures = B(element, "DELETE_TEKLA_STRUCTURES", value.DeleteTeklaStructures);
            value.Purge = B(element, "PURGE", value.Purge);
            value.ShowMessages = B(element, "SHOW_MESSAGES", value.ShowMessages);
            value.ExplodeBlocks = B(element, "EXPLODE_BLOCKS", value.ExplodeBlocks);
            value.InventorExplode = B(element, "INVENTOR_EXPLODE", value.InventorExplode);
        }

        private static void ReadDimensions(XElement element, DimensionConfiguration value)
        {
            if (element == null)
                return;

            value.ReferenceFormatSize = D(element, "REFERENCE_FORMAT_SIZE", value.ReferenceFormatSize);
            value.InternalLengthCharFactor = D(element, "INTERNAL_LENGTH_CHAR_FACTOR", value.InternalLengthCharFactor);
            value.InternalTextOffset = D(element, "INTERNAL_TEXT_OFFSET", value.InternalTextOffset);
            value.Enabled = B(element, "ENABLED", value.Enabled);
            value.Layer = S(element, "LAYER", value.Layer);
            value.BaseLayer = S(element, "BASE_LAYER", value.BaseLayer);
            value.LineColor = S(element, "LINE_COLOR", value.LineColor);
            value.TextColor = S(element, "TEXT_COLOR", value.TextColor);
            value.StyleName = S(element, "STYLE_NAME", value.StyleName);
            value.ArrowType = S(element, "ARROW_TYPE", value.ArrowType);
            value.ArrowType1 = S(element, "ARROW_TYPE_1", value.ArrowType1);
            value.ArrowType2 = S(element, "ARROW_TYPE_2", value.ArrowType2);
            value.Scale = D(element, "SCALE", value.Scale);
            value.Precision = I(element, "PRECISION", value.Precision);
            value.AngularPrecision = I(element, "ANGULAR_PRECISION", value.AngularPrecision);
            value.Unit = I(element, "UNIT", value.Unit);
            value.AngularUnit = I(element, "ANGULAR_UNIT", value.AngularUnit);
            value.ArrowSize = D(element, "ARROW_SIZE", value.ArrowSize);
            value.TextVerticalPosition = I(element, "TEXT_VERTICAL_POSITION", value.TextVerticalPosition);
            value.TextRelativeToDimensionLine = B(element, "TEXT_RELATIVE_TO_DIMENSION_LINE", value.TextRelativeToDimensionLine);
            value.ForceTextInside = B(element, "FORCE_TEXT_INSIDE", value.ForceTextInside);
            value.ForceDimensionLine = B(element, "FORCE_DIMENSION_LINE", value.ForceDimensionLine);
            value.OffsetLineFromReferencePoint = D(element, "OFFSET_LINE_FROM_REFERENCE_POINT", value.OffsetLineFromReferencePoint);
            value.TextMove = I(element, "TEXT_MOVE", value.TextMove);
            value.OutsideAlign = B(element, "OUTSIDE_ALIGN", value.OutsideAlign);
            value.ExtensionLineOffset = D(element, "EXTENSION_LINE_OFFSET", value.ExtensionLineOffset);
            value.FixArrow = B(element, "FIX_ARROW", value.FixArrow);
            value.FixArrowType = S(element, "FIX_ARROW_TYPE", value.FixArrowType);
            value.FixArrowFactor = D(element, "FIX_ARROW_FACTOR", value.FixArrowFactor);
        }

        private static void ReadText(XElement element, TextConfiguration value)
        {
            if (element == null)
                return;

            value.DefaultStyleName = S(element, "DEFAULT_STYLE_NAME", value.DefaultStyleName);
            value.DefaultSize = D(element, "DEFAULT_SIZE", value.DefaultSize);
            value.Styles = element.Elements("STYLE").Select(item => new TextStyleDefinition
            {
                Name = S(item, "NAME"),
                Font = S(item, "FONT"),
                Italic = B(item, "ITALIC"),
                Bold = B(item, "BOLD"),
                Size = D(item, "SIZE"),
                WidthFactor = D(item, "WIDTH_FACTOR", 1),
                ObliqueAngle = D(item, "OBLIQUE_ANGLE")
            }).ToList();
        }

        private static void ReadScale(XElement element, ScaleConfiguration value)
        {
            if (element == null)
                return;

            value.Manual = B(element, "MANUAL", value.Manual);
            value.Point1 = ReadPoint(element.Element("POINT1"));
            value.Point2 = ReadPoint(element.Element("POINT2"));
            value.Layer = S(element, "LAYER", value.Layer);
            value.TextSize = D(element, "TEXT_SIZE", value.TextSize);
        }

        private static void ReadLayers(XElement element, LayerConfiguration value)
        {
            if (element == null)
                return;

            value.TeklaDrawingSheetLayer = S(element, "TEKLA_DRAWING_SHEET_LAYER", value.TeklaDrawingSheetLayer);
            value.BlockAttributeLayer = S(element, "BLOCK_ATTRIBUTE_LAYER", value.BlockAttributeLayer);
            value.BaseLayers = element.Elements("BASE_LAYER").Select(item => S(item, "NAME")).ToList();
            value.NewLayers = element.Elements("NEW_LAYER").Select(item => new LayerDefinition
            {
                Name = S(item, "NAME"),
                Color = S(item, "COLOR"),
                LineType = S(item, "LINE_TYPE")
            }).ToList();
            value.RemoveRules = element.Elements("REMOVE_RULE")
                .Select(item => new LayerRemoveRule { Filter = ReadFilter(item.Element("FILTER")) })
                .ToList();
            value.ConversionRules = element.Elements("CONVERSION_RULE").Select(item => new LayerConversionRule
            {
                Source = ReadFilter(item.Element("SOURCE")),
                Target = ReadTarget(item.Element("TARGET"))
            }).ToList();
            value.ExplodeLayers = element.Elements("EXPLODE_LAYER").Select(item => S(item, "NAME")).ToList();
        }

        private static void ReadLines(XElement element, LineConfiguration value)
        {
            if (element == null)
                return;

            value.LineTypeScale = D(element, "LINE_TYPE_SCALE", value.LineTypeScale);
            value.BaseLineTypes = element.Elements("BASE_LINE_TYPE").Select(item => S(item, "NAME")).ToList();
        }

        private static void ReadCatalogs(XElement element, CatalogConfiguration value)
        {
            if (element == null)
                return;

            value.Colors = element.Elements("COLOR").Select(item => S(item, "NAME")).ToList();
            value.ObjectTypes = element.Elements("OBJECT_TYPE").Select(item => S(item, "NAME")).ToList();
            value.FilterLineTypes = element.Elements("FILTER_LINE_TYPE").Select(item => S(item, "NAME")).ToList();
            value.LayerLineTypes = element.Elements("LAYER_LINE_TYPE").Select(item => S(item, "NAME")).ToList();
            value.RemovedLineTypes = element.Elements("REMOVED_LINE_TYPE").Select(item => S(item, "NAME")).ToList();
        }

        private static void ReadCommands(XElement element, CommandConfiguration value)
        {
            if (element == null)
                return;

            value.LispCommands = element.Elements("LISP_COMMAND").Select(item => S(item, "TEXT")).ToList();
            value.DllCommands = element.Elements("DLL_COMMAND").Select(item => S(item, "TEXT")).ToList();
        }

        private static void ReadBlocks(XElement element, BlockConfiguration value)
        {
            if (element == null)
                return;

            value.TeklaBlockPath = S(element, "TEKLA_BLOCK_PATH", value.TeklaBlockPath);
            value.CadBlockPath = S(element, "CAD_BLOCK_PATH", value.CadBlockPath);
            value.DimensionBlockEnabled = B(element, "DIMENSION_BLOCK_ENABLED", value.DimensionBlockEnabled);
            value.TeklaBlocks = element.Elements("TEKLA_BLOCK").Select(ReadBlock).ToList();
            value.CadBlocks = element.Elements("CAD_BLOCK").Select(ReadBlock).ToList();
            value.OriginalBlocks = element.Elements("ORIGINAL_BLOCK").Select(ReadBlock).ToList();
        }

        private static BlockDefinition ReadBlock(XElement element)
        {
            return new BlockDefinition
            {
                Name = S(element, "NAME"),
                RelatedName = S(element, "RELATED_NAME"),
                ColorArgb = I(element, "COLOR_ARGB"),
                Tags = element.Elements("TAG").Select(ReadTag).ToList()
            };
        }

        private static BlockTagDefinition ReadTag(XElement element)
        {
            return new BlockTagDefinition
            {
                Name = S(element, "NAME"),
                Modify = B(element, "MODIFY"),
                WidthFactor = D(element, "WIDTH_FACTOR", 1),
                RelatedIndex = I(element, "RELATED_INDEX", -1),
                IsAssociated = B(element, "IS_ASSOCIATED"),
                Point1 = ReadPoint(element.Element("POINT1")),
                Point2 = ReadPoint(element.Element("POINT2")),
                Filter = ReadFilter(element.Element("FILTER"))
            };
        }

        private static LayerOutput ReadTarget(XElement element)
        {
            return new LayerOutput
            {
                LayerName = S(element, "LAYER_NAME"),
                Color = S(element, "COLOR"),
                LineType = S(element, "LINE_TYPE"),
                TextContent = S(element, "TEXT_CONTENT"),
                TextHeight = S(element, "TEXT_HEIGHT"),
                TextStyle = S(element, "TEXT_STYLE")
            };
        }

        private static EntityFilter ReadFilter(XElement element)
        {
            return new EntityFilter
            {
                BaseLayer = S(element, "BASE_LAYER", "ALL"),
                ObjectType = S(element, "OBJECT_TYPE", "ALL"),
                Color = S(element, "COLOR", "ALL"),
                LineType = S(element, "LINE_TYPE", "ALL"),
                TextContent = S(element, "TEXT_CONTENT"),
                TextHeight = S(element, "TEXT_HEIGHT"),
                Orientation = S(element, "ORIENTATION", "ALL")
            };
        }

        private static Point3DConfiguration ReadPoint(XElement element)
        {
            return new Point3DConfiguration
            {
                X = D(element, "X"),
                Y = D(element, "Y"),
                Z = D(element, "Z")
            };
        }

        private static string S(XElement element, string attribute, string defaultValue = "")
        {
            if (element == null)
                return defaultValue;

            XAttribute value = element.Attribute(attribute);
            return value == null ? defaultValue : value.Value;
        }

        private static int I(XElement element, string attribute, int defaultValue = 0)
        {
            string value = S(element, attribute, null);
            int result;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        private static double D(XElement element, string attribute, double defaultValue = 0)
        {
            string value = S(element, attribute, null);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return NumericTextParser.ToDouble(value);
        }

        private static bool B(XElement element, string attribute, bool defaultValue = false)
        {
            string value = S(element, attribute, null);
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }
    }
}
