using System;
using System.IO;
using System.Xml.Linq;

namespace ConversorDrawind
{
    internal static class ConfigurationXmlContract
    {
        public const string Root = "CONVERSOR";
        public const string ConfigurationRoot = "configuration";
        public const string DllConfigurations = "configurations";

        public const string Comments = "COMMENTS";
        public const string BasicConfig = "BASIC_CONFIG";
        public const string DimensionConfig = "DIMENSION_CONFIG";
        public const string TextConfig = "TEXT_CONFIG";
        public const string ScaleConfig = "SCALE_CONFIG";
        public const string BasicLayers = "BASIC_LAYERS";
        public const string BasicLines = "BASIC_LINES";
        public const string NewTextStyles = "NEW_TEXTSTYLES";
        public const string NewLayers = "NEW_LAYERS";
        public const string RemoveLayers = "REMOVE_LAYERS";
        public const string Converters = "CONVERTERS";
        public const string DllOrListCommands = "DLL_OR_LIST_COMMANDS";
        public const string BlockConfig = "BLOCK_CONFIG";

        public const string BaseLayer = "BASE_LAYER";
        public const string BaseLine = "BASE_LINE";
        public const string TextStyle = "TEXT_STYLE";
        public const string NewLayer = "NEW_LAYER";
        public const string RemoveLayer = "REMOVE_LAYER";
        public const string Converter = "CONVERTER";
        public const string Command = "COMMAND";
        public const string BlockAtt = "BLOCK_ATT";
        public const string BlockAttCad = "BLOCK_ATT_CAD";
        public const string BlockAttOrig = "BLOCK_ATT_ORIG";
        public const string Tag = "TAG";

        public const string Text = "TEXT";
        public const string TeklaOrCad = "TEKLAORCAD";
        public const string ConvertDimensions = "CONVERT_DIMENSIONS";
        public const string ConvertLayers = "CONVERT_LAYERS";
        public const string ExchangeFormat = "EXCHANGE_FORMAT";
        public const string Scale = "SCALE";
        public const string LispOrDll = "LISPORDLL";
        public const string Purge = "PURGE";
        public const string Message = "MESSAGE";
        public const string DeleteTeklaStructures = "DELETE_TEKLA_STRUCTURES";
        public const string ExplodeBlocks = "EXPLOD_BLOCKS";
        public const string LayerTeklaString = "LAYER_TEKLA_STRING";
        public const string LayerBlockAttribute = "LAYER_BLOCK_ATTRIBUTE";
        public const string CadExplode = "CAD_EXPLODE";
        public const string DmBlock = "DMBLOCK";

        public const string DimGeralHabilit = "DIM_GERAL_HABILIT";
        public const string DimLayer = "DIM_LAYER";
        public const string DimLineColor = "DIM_LINE_COLOR";
        public const string DimTextColor = "DIM_TEXT_COLOR";
        public const string DimStyle = "DIM_STYLE";
        public const string DimArrowType = "DIM_ARROW_TYPE";
        public const string DimScale = "DIM_SCALE";
        public const string DimPrecision = "DIM_PRECISION";
        public const string DimAngularPrecision = "DIM_ANGULAR_PRECISION";
        public const string DimUnit = "DIM_UNIT";
        public const string DimAngularUnit = "DIM_ANGULAR_UNIT";
        public const string DimArrowSize = "DIM_ARROW_SIZE";
        public const string DimOffset = "DIM_OFFSET";
        public const string DimOutsideAling = "DIM_OUTSIDE_ALING";
        public const string DimTad = "DIM_TAD";
        public const string DimPosition = "DIM_POSITION";
        public const string DimTextForced = "DIM_TEXT_FORCED";
        public const string DimLineForced = "DIM_LINE_FORCED";
        public const string DimDimex = "DIM_DIMEX";
        public const string DimBaseLayer = "DIM_BASE_LAYER";
        public const string DimArrowFix = "DIM_ARROW_FIX";
        public const string DimArrowFixType = "DIM_ARROW_FIX_TYPE";
        public const string DimArrowFactor = "DIM_ARROW_FACTOR";

        public const string TextStyleName = "TEXT_STYPE";
        public const string TextFonte = "TEXT_FONTE";
        public const string TextTamanho = "TEXT_TAMANHO";
        public const string TextLargura = "TEXT_LARGURA";
        public const string TextItalico = "TEXT_ITALICO";
        public const string TextNegrito = "TEXT_NEGRITO";
        public const string TextObliqueAngle = "TEXT_OBLIQUE_ANGLE";

        public const string ScaleMode = "SCALE_MODE";
        public const string ScaleP1X = "SCALE_P1_X";
        public const string ScaleP1Y = "SCALE_P1_Y";
        public const string ScaleP1Z = "SCALE_P1_Z";
        public const string ScaleP2X = "SCALE_P2_X";
        public const string ScaleP2Y = "SCALE_P2_Y";
        public const string ScaleP2Z = "SCALE_P2_Z";
        public const string ScaleLayer = "SCALE_LAYER";
        public const string ScaleTextSize = "SCALE_TEXT_SIZE";

        public const string Ltscale = "LTSCALE";
        public const string DirectoryTeklaConversion = "DIRECTORY_TEKLA_CONVERSION";
        public const string DirectoryCadConversion = "DIRECTORY_CAD_CONVERSION";
        public const string LayerExplode = "LAYER_EXPLODE";
        public const string Nome = "NOME";
        public const string BlocoDm = "BlocoDM";

        public static string EnsureFolder(StatusConversorItem statusConversorItem)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, statusConversorItem.Pasta);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        public static string TxmlPath(string file, StatusConversorItem statusConversorItem)
        {
            return Path.Combine(EnsureFolder(statusConversorItem), file + ".txml");
        }
        public static XElement Element(XElement xml, string elementName)
        {
            return xml.Element(elementName);
        }

        public static string AttributeValue(XElement xml, string elementName, string attributeName)
        {
            return Element(xml, elementName).Attribute(attributeName).Value;
        }

        public static string AttributeValue(XElement element, string attributeName)
        {
            return element.Attribute(attributeName).Value;
        }

        public static bool BoolAttribute(XElement xml, string elementName, string attributeName)
        {
            return Convert.ToBoolean(AttributeValue(xml, elementName, attributeName));
        }

        public static int IntAttribute(XElement xml, string elementName, string attributeName)
        {
            return Convert.ToInt32(AttributeValue(xml, elementName, attributeName));
        }

        public static double DoubleAttribute(XElement xml, string elementName, string attributeName)
        {
            return NumericTextParser.ToDouble(AttributeValue(xml, elementName, attributeName));
        }
    }
}



