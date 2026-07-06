using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ConversorDrawindDLL
{
    internal static class ConfigurationXmlReader
    {
        internal static ConfigurationXmlData Read(string file)
        {
            XElement importXML = XElement.Load(file);
            ConfigurationXmlData data = new ConfigurationXmlData();

            data.EXTCONFComments = GetElement(importXML, ConfigurationXmlConstants.Comments)
                .Attribute(ConfigurationXmlConstants.Text)
                .Value;

            XElement basicConfig = GetElement(importXML, ConfigurationXmlConstants.BasicConfig);
            data.ConvTekla0ConvInv1 = GetIntAttribute(basicConfig, ConfigurationXmlConstants.TeklaOrCad);
            data.EXTCONFIsConvertDimension = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.ConvertDimensions);
            data.EXTCONFIsConvertLayer = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.ConvertLayers);
            data.EXTCONFIsExchangeFormat = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.ExchangeFormat);
            data.EXTCONFIsPutOnTheScaleDrawing = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.Scale);
            data.EXTCONFIsExecuteLISP = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.LispOrDll);
            data.EXTCONFIsPurge = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.Purge);
            data.PROGRAMMessage = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.Message);
            data.EXTCONFIsDeleteTeklaStructures = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.DeleteTeklaStructures);
            data.ExplodeBlocks = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.ExplodBlocks);
            data.LayerTeklaString = GetStringAttribute(basicConfig, ConfigurationXmlConstants.LayerTeklaString);
            data.LayerBlockAttribute = GetStringAttribute(basicConfig, ConfigurationXmlConstants.LayerBlockAttribute);
            data.EXTCONFInventorExplode = GetBoolAttribute(basicConfig, ConfigurationXmlConstants.CadExplode);

            XElement dimensionConfig = GetElement(importXML, ConfigurationXmlConstants.DimensionConfig);
            data.EXTDIMGERALHabilit = GetBoolAttribute(dimensionConfig, ConfigurationXmlConstants.DimGeralHabilit);
            data.EXTDIMlayer = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimLayer);
            data.EXTDIMColorLine = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimLineColor);
            data.EXTDIMColorText = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimTextColor);
            data.EXTDIMStyleName = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimStyle);
            data.EXTDIMSeta = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimArrowType);
            data.EXTDIMScale = GetDoubleAttribute(dimensionConfig, ConfigurationXmlConstants.DimScale);
            data.EXTDIMPrecision = GetIntAttribute(dimensionConfig, ConfigurationXmlConstants.DimPrecision);
            data.EXTDIMAngularPrecision = GetIntAttribute(dimensionConfig, ConfigurationXmlConstants.DimAngularPrecision);
            data.EXTDIMUnit = GetIntAttribute(dimensionConfig, ConfigurationXmlConstants.DimUnit);
            data.EXTDIMAngularUnit = GetIntAttribute(dimensionConfig, ConfigurationXmlConstants.DimAngularUnit);
            data.EXTDIMSizeSeta = GetDoubleAttribute(dimensionConfig, ConfigurationXmlConstants.DimArrowSize);
            data.EXTDIMOffsetLineFromRefPoint = GetDoubleAttribute(dimensionConfig, ConfigurationXmlConstants.DimOffset);
            data.EXTDIMOutsideAlign = GetBoolAttribute(dimensionConfig, ConfigurationXmlConstants.DimOutsideAlign);
            data.EXTDIMTad = GetIntAttribute(dimensionConfig, ConfigurationXmlConstants.DimTad);
            data.EXTDIMDimensionPosition = GetBoolAttribute(dimensionConfig, ConfigurationXmlConstants.DimPosition);
            data.EXTDIMTextForced = GetBoolAttribute(dimensionConfig, ConfigurationXmlConstants.DimTextForced);
            data.EXTDIMLineForced = GetBoolAttribute(dimensionConfig, ConfigurationXmlConstants.DimLineForced);
            data.EXTDIMDIMEX = GetDoubleAttribute(dimensionConfig, ConfigurationXmlConstants.DimDimex);
            data.EXTDIMBaseLayer = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimBaseLayer);
            data.EXTDIMCorrigeSeta = GetBoolAttribute(dimensionConfig, ConfigurationXmlConstants.DimArrowFix);
            data.EXTDIMCorrigeSetaTipoSeta = GetStringAttribute(dimensionConfig, ConfigurationXmlConstants.DimArrowFixType);
            data.EXTDIMCorrigeSetaFactor = GetDoubleAttribute(dimensionConfig, ConfigurationXmlConstants.DimArrowFactor);

            XElement textConfig = GetElement(importXML, ConfigurationXmlConstants.TextConfig);
            data.EXTTEXTStyleName = GetStringAttribute(textConfig, ConfigurationXmlConstants.TextStyleName);

            XElement scaleConfig = GetElement(importXML, ConfigurationXmlConstants.ScaleConfig);
            data.EXTSCALEManual = GetBoolAttribute(scaleConfig, ConfigurationXmlConstants.ScaleMode);
            data.EXTSCALEMp1 = new PointEspecial2(
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleManualP1X),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleManualP1Y),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleManualP1Z));
            data.EXTSCALEMp2 = new PointEspecial2(
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleManualP2X),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleManualP2Y),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleManualP2Z));
            data.EXTSCALEAp1 = new PointEspecial2(
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleAutoP1X),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleAutoP1Y),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleAutoP1Z));
            data.EXTSCALEAp2 = new PointEspecial2(
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleAutoP2X),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleAutoP2Y),
                GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleAutoP2Z));
            data.EXTSCALELayer = GetStringAttribute(scaleConfig, ConfigurationXmlConstants.ScaleLayer);
            data.EXTSCALETextSize = GetDoubleAttribute(scaleConfig, ConfigurationXmlConstants.ScaleTextSize);

            data.EXTLINELtscale = GetDoubleAttribute(GetElement(importXML, ConfigurationXmlConstants.BasicLayers), ConfigurationXmlConstants.Ltscale);

            data.AllNewLayerComposition.AddRange(GetElement(importXML, ConfigurationXmlConstants.NewLayers)
                .Elements(ConfigurationXmlConstants.NewLayer)
                .Select(item => item.Value));

            data.AllTextSyles.AddRange(ReadTextStyles(importXML));

            foreach (var item in GetElement(importXML, ConfigurationXmlConstants.RemoveLayers)
                         .Elements(ConfigurationXmlConstants.RemoveLayer))
            {
                string line = item.Value;
                string[] treatment = line.Split('$');
                string[] st = treatment.Last().Split(';');
                Filter filter = new Filter();
                filter.layerBase = st[0];
                filter.SetConjunto(st[1]);
                data.LayerRemove.Add(filter);
            }

            data.Conversor.AddRange(GetElement(importXML, ConfigurationXmlConstants.Converters)
                .Elements(ConfigurationXmlConstants.Converter)
                .Select(item => item.Value));

            data.ListLISPCommand.AddRange(GetElement(importXML, ConfigurationXmlConstants.DllOrListCommands)
                .Elements(ConfigurationXmlConstants.Command)
                .Select(item => item.Value));

            XElement blockConfig = GetElement(importXML, ConfigurationXmlConstants.BlockConfig);
            data.PROGRAMblockFormatoCaminho = GetStringAttribute(blockConfig, ConfigurationXmlConstants.DirectoryTeklaConversion);
            data.EXTCONFCaminhoBlocoInv = GetStringAttribute(blockConfig, ConfigurationXmlConstants.DirectoryCadConversion);

            string[] allexplodelayers = GetStringAttribute(blockConfig, ConfigurationXmlConstants.LayerExplode).Split(';');
            data.AllExplodeLayers.AddRange(allexplodelayers);

            foreach (var item in blockConfig.Elements(ConfigurationXmlConstants.BlockAtt))
            {
                BlockClass blockClass = new BlockClass();
                blockClass.blockName = GetStringAttribute(item, ConfigurationXmlConstants.Name);
                foreach (var tag in item.Elements(ConfigurationXmlConstants.Tag))
                {
                    TagBlockClass tagTemp = new TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    blockClass.listTags.Add(tagTemp);
                }
                data.ListBlocks.Add(blockClass);
            }

            foreach (var item in blockConfig.Elements(ConfigurationXmlConstants.BlockAttCad))
            {
                BlockClass blockClass = new BlockClass();
                string[] linesplit = GetStringAttribute(item, ConfigurationXmlConstants.Name).Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements(ConfigurationXmlConstants.Tag))
                {
                    TagBlockClass tagTemp = new TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Length - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Length - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                data.ListBlocksInv.Add(blockClass);
            }

            foreach (var item in blockConfig.Elements(ConfigurationXmlConstants.BlockAttOrig))
            {
                BlockClass blockClass = new BlockClass();
                string[] linesplit = GetStringAttribute(item, ConfigurationXmlConstants.Name).Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements(ConfigurationXmlConstants.Tag))
                {
                    TagBlockClass tagTemp = new TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Length - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Length - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                data.ListBlocksOrig.Add(blockClass);
            }

            data.DMBlock = GetOptionalBoolAttribute(basicConfig, ConfigurationXmlConstants.DmBlock);

            data.TextSizeFromStyle = ResolveTextSize(data.AllTextSyles, data.EXTTEXTStyleName);

            return data;
        }

        internal static List<InstanciaConversor> CreateConverterInstances(IEnumerable<string> converters)
        {
            return converters.Select(p => new InstanciaConversor(p)).ToList();
        }

        internal static string ReadLoadConfigDll(string file)
        {
            if (!File.Exists(file))
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");
            }

            XElement importXML = XElement.Load(file);
            return GetElement(importXML, "configurations")
                .Attribute("BlocoDM")
                .Value;
        }

        private static IReadOnlyList<string> ReadTextStyles(XElement importXML)
        {
            try
            {
                XElement newTextStyles = GetElement(importXML, ConfigurationXmlConstants.NewTextStyles);
                List<string> textStyles = new List<string>();
                foreach (var item in newTextStyles.Elements(ConfigurationXmlConstants.TextStyle))
                {
                    textStyles.Add(item.Value);
                }
                if (textStyles.Count == 0)
                    textStyles.Add(Arranjos.defaultTextStyle);
                return textStyles;
            }
            catch (Exception)
            {
                return new[] { Arranjos.defaultTextStyle };
            }
        }

        private static double ResolveTextSize(IEnumerable<string> textStyles, string styleName)
        {
            return TextStyleResolver.ResolveTextSize(textStyles, styleName);
        }

        private static XElement GetElement(XElement parent, string name)
        {
            return parent.Element(name);
        }

        private static string GetStringAttribute(XElement element, string attributeName)
        {
            return element.Attribute(attributeName).Value;
        }

        private static int GetIntAttribute(XElement element, string attributeName)
        {
            return Convert.ToInt32(GetStringAttribute(element, attributeName));
        }

        private static bool GetBoolAttribute(XElement element, string attributeName)
        {
            return Convert.ToBoolean(GetStringAttribute(element, attributeName));
        }

        private static bool? GetOptionalBoolAttribute(XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null)
                return null;

            return Convert.ToBoolean(attribute.Value);
        }

        private static double GetDoubleAttribute(XElement element, string attributeName)
        {
            return Convert.ToDouble(GetStringAttribute(element, attributeName).ReplaceComma());
        }
    }

    internal static class ConfigurationXmlConstants
    {
        internal const string Comments = "COMMENTS";
        internal const string Text = "TEXT";
        internal const string BasicConfig = "BASIC_CONFIG";
        internal const string TeklaOrCad = "TEKLAORCAD";
        internal const string ConvertDimensions = "CONVERT_DIMENSIONS";
        internal const string ConvertLayers = "CONVERT_LAYERS";
        internal const string ExchangeFormat = "EXCHANGE_FORMAT";
        internal const string Scale = "SCALE";
        internal const string LispOrDll = "LISPORDLL";
        internal const string Purge = "PURGE";
        internal const string Message = "MESSAGE";
        internal const string DeleteTeklaStructures = "DELETE_TEKLA_STRUCTURES";
        internal const string ExplodBlocks = "EXPLOD_BLOCKS";
        internal const string LayerTeklaString = "LAYER_TEKLA_STRING";
        internal const string LayerBlockAttribute = "LAYER_BLOCK_ATTRIBUTE";
        internal const string CadExplode = "CAD_EXPLODE";
        internal const string DimLayer = "DIM_LAYER";
        internal const string DimLineColor = "DIM_LINE_COLOR";
        internal const string DimTextColor = "DIM_TEXT_COLOR";
        internal const string DimStyle = "DIM_STYLE";
        internal const string DimArrowType = "DIM_ARROW_TYPE";
        internal const string DimScale = "DIM_SCALE";
        internal const string DimPrecision = "DIM_PRECISION";
        internal const string DimAngularPrecision = "DIM_ANGULAR_PRECISION";
        internal const string DimUnit = "DIM_UNIT";
        internal const string DimAngularUnit = "DIM_ANGULAR_UNIT";
        internal const string DimArrowSize = "DIM_ARROW_SIZE";
        internal const string DimOffset = "DIM_OFFSET";
        internal const string DimOutsideAlign = "DIM_OUTSIDE_ALING";
        internal const string DimTad = "DIM_TAD";
        internal const string DimPosition = "DIM_POSITION";
        internal const string DimTextForced = "DIM_TEXT_FORCED";
        internal const string DimLineForced = "DIM_LINE_FORCED";
        internal const string DimDimex = "DIM_DIMEX";
        internal const string DimBaseLayer = "DIM_BASE_LAYER";
        internal const string DimArrowFix = "DIM_ARROW_FIX";
        internal const string DimArrowFixType = "DIM_ARROW_FIX_TYPE";
        internal const string DimArrowFactor = "DIM_ARROW_FACTOR";
        internal const string DimensionConfig = "DIMENSION_CONFIG";
        internal const string DimGeralHabilit = "DIM_GERAL_HABILIT";
        internal const string TextConfig = "TEXT_CONFIG";
        internal const string TextStyleName = "TEXT_STYPE";
        internal const string ScaleConfig = "SCALE_CONFIG";
        internal const string ScaleMode = "SCALE_MODE";
        internal const string ScaleManualP1X = "SCALE_MANUAL_P1_X";
        internal const string ScaleManualP1Y = "SCALE_MANUAL_P1_Y";
        internal const string ScaleManualP1Z = "SCALE_MANUAL_P1_Z";
        internal const string ScaleManualP2X = "SCALE_MANUAL_P2_X";
        internal const string ScaleManualP2Y = "SCALE_MANUAL_P2_Y";
        internal const string ScaleManualP2Z = "SCALE_MANUAL_P2_Z";
        internal const string ScaleAutoP1X = "SCALE_AUTO_P1_X";
        internal const string ScaleAutoP1Y = "SCALE_AUTO_P1_Y";
        internal const string ScaleAutoP1Z = "SCALE_AUTO_P1_Z";
        internal const string ScaleAutoP2X = "SCALE_AUTO_P2_X";
        internal const string ScaleAutoP2Y = "SCALE_AUTO_P2_Y";
        internal const string ScaleAutoP2Z = "SCALE_AUTO_P2_Z";
        internal const string ScaleLayer = "SCALE_LAYER";
        internal const string ScaleTextSize = "SCALE_TEXT_SIZE";
        internal const string BasicLayers = "BASIC_LAYERS";
        internal const string Ltscale = "LTSCALE";
        internal const string NewLayers = "NEW_LAYERS";
        internal const string NewLayer = "NEW_LAYER";
        internal const string NewTextStyles = "NEW_TEXTSTYLES";
        internal const string TextStyle = "TEXT_STYLE";
        internal const string RemoveLayers = "REMOVE_LAYERS";
        internal const string RemoveLayer = "REMOVE_LAYER";
        internal const string Converters = "CONVERTERS";
        internal const string Converter = "CONVERTER";
        internal const string DllOrListCommands = "DLL_OR_LIST_COMMANDS";
        internal const string Command = "COMMAND";
        internal const string BlockConfig = "BLOCK_CONFIG";
        internal const string DirectoryTeklaConversion = "DIRECTORY_TEKLA_CONVERSION";
        internal const string DirectoryCadConversion = "DIRECTORY_CAD_CONVERSION";
        internal const string LayerExplode = "LAYER_EXPLODE";
        internal const string BlockAtt = "BLOCK_ATT";
        internal const string BlockAttCad = "BLOCK_ATT_CAD";
        internal const string BlockAttOrig = "BLOCK_ATT_ORIG";
        internal const string Tag = "TAG";
        internal const string Name = "NOME";
        internal const string DmBlock = "DMBLOCK";
    }

    internal sealed class ConfigurationXmlData
    {
        internal string EXTCONFComments;
        internal int ConvTekla0ConvInv1;
        internal bool EXTCONFIsConvertDimension;
        internal bool EXTCONFIsConvertLayer;
        internal bool EXTCONFIsExchangeFormat;
        internal bool EXTCONFIsPutOnTheScaleDrawing;
        internal bool EXTCONFIsExecuteLISP;
        internal bool EXTCONFIsPurge;
        internal bool PROGRAMMessage;
        internal bool EXTCONFIsDeleteTeklaStructures;
        internal bool ExplodeBlocks;
        internal string LayerTeklaString;
        internal string LayerBlockAttribute;
        internal bool EXTCONFInventorExplode;
        internal bool EXTDIMGERALHabilit;
        internal string EXTDIMlayer;
        internal string EXTDIMColorLine;
        internal string EXTDIMColorText;
        internal string EXTDIMStyleName;
        internal string EXTDIMSeta;
        internal double EXTDIMScale;
        internal int EXTDIMPrecision;
        internal int EXTDIMAngularPrecision;
        internal int EXTDIMUnit;
        internal int EXTDIMAngularUnit;
        internal double EXTDIMSizeSeta;
        internal double EXTDIMOffsetLineFromRefPoint;
        internal bool EXTDIMOutsideAlign;
        internal int EXTDIMTad;
        internal bool EXTDIMDimensionPosition;
        internal bool EXTDIMTextForced;
        internal bool EXTDIMLineForced;
        internal double EXTDIMDIMEX;
        internal string EXTDIMBaseLayer;
        internal bool EXTDIMCorrigeSeta;
        internal string EXTDIMCorrigeSetaTipoSeta;
        internal double EXTDIMCorrigeSetaFactor;
        internal string EXTTEXTStyleName;
        internal bool EXTSCALEManual;
        internal PointEspecial2 EXTSCALEMp1;
        internal PointEspecial2 EXTSCALEMp2;
        internal PointEspecial2 EXTSCALEAp1;
        internal PointEspecial2 EXTSCALEAp2;
        internal string EXTSCALELayer;
        internal double EXTSCALETextSize;
        internal double EXTLINELtscale;
        internal string PROGRAMblockFormatoCaminho;
        internal string EXTCONFCaminhoBlocoInv;
        internal bool? DMBlock;
        internal List<string> AllNewLayerComposition = new List<string>();
        internal List<string> AllTextSyles = new List<string>();
        internal List<Filter> LayerRemove = new List<Filter>();
        internal List<string> Conversor = new List<string>();
        internal List<string> ListLISPCommand = new List<string>();
        internal List<string> AllExplodeLayers = new List<string>();
        internal List<BlockClass> ListBlocks = new List<BlockClass>();
        internal List<BlockClass> ListBlocksInv = new List<BlockClass>();
        internal List<BlockClass> ListBlocksOrig = new List<BlockClass>();
        internal double TextSizeFromStyle;
    }
}
