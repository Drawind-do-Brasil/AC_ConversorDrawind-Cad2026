using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Xml.Linq;

namespace ConversorDrawind
{
    public class Configuration
    {
        /// <summary>
        /// INT CONVERT DIMENSION
        /// </summary>
       // public string INTFILTERCOTALayerName = "Dimension";
        public double INTFactorLengthChar = 1.5;
        public double INTDIMTextOffset = 0.6100;
        public bool DMBlock = true;
        /// <summary>
        /// EXT CONVERT DIMENSION
        /// </summary>
        public string EXTDIMStyleName = "COTAS";
        public double EXTDIMScale = 1; //Escala da dimensão
        public int EXTDIMPrecision = 0; //Precisão da dimensão 
        public int EXTDIMAngularPrecision = 1; //Precisão da dimensão algular
        public int EXTDIMUnit = 2;//Unidade da dimensão 
        public int EXTDIMAngularUnit = 2;//Unidade da dimensão algular
        public string EXTDIMSeta = "Oblique";
        public string EXTDIMSeta1 = "Oblique";
        public string EXTDIMSeta2 = "Oblique";
        public double EXTDIMSizeSeta = 1.25;
        public int EXTDIMTad = 1; //Posição do texto na dimensão verticalmente
        public bool EXTDIMDimensionPosition = false; //Posição do texto em relação a dimensão
        public bool EXTDIMTextForced = true; //Forçar o texto a permanecer alinhado com a dimensão, caso o espaço seja curto.
        public bool EXTDIMLineForced = true;
        public string EXTDIMColorText = "GREEN";
        public string EXTDIMColorLine = "RED";
        public double EXTDIMOffsetLineFromRefPoint = 0;
        public int EXTDIMTextMove = 2;
        public bool EXTDIMOutsideAlign = false;
        public string EXTDIMlayer = "0";
        public string EXTTEXTStyleName = "TEXTO";
        
        public bool EXTDIMGERALHabilit = true;
        public double EXTDIMDIMEX = 1.25;
        public string EXTDIMBaseLayer = "DIMENSION";
        public string LayerTeklaString = "DRAWING SHEET";
        public string LayerBlockAttribute = "OTHER OBJECT TYPE";
       // public double EXTTEXTObliqueAngle = 0;
        /// <summary>
        /// EXT SCALE
        /// </summary>
        public bool EXTSCALEManual = true;
        public PointEspecial EXTSCALEMp1 = new PointEspecial(0, 0, 0);
        public PointEspecial EXTSCALEMp2 = new PointEspecial(0, 0, 0);
        public PointEspecial EXTSCALEAp1 = new PointEspecial(0, 0, 0);
        public PointEspecial EXTSCALEAp2 = new PointEspecial(0, 0, 0);
        public string EXTSCALELayer = "0";
        public double EXTSCALETextSize = 2.5;

        /// <summary>
        /// EXT CONFIGURATION
        /// </summary> 
        public string EXTCONFComments = "";
        public bool EXTCONFIsConvertDimension = true;
        public bool EXTCONFIsConvertLayer = true;
        public bool EXTCONFIsExchangeFormat = false;
        public bool EXTCONFIsExchangeLM = false;
        public bool EXTCONFIsPutOnTheScaleDrawing = false;
        public bool EXTCONFIsExecuteLISP = false;
        public bool EXTCONFIsExecuteDLL = false;
        public bool EXTCONFIsDeleteTeklaStructures = true;
        public string EXTCONFIsFirstRum = "LISP";
        public bool EXTCONFIsPurge = true;
        /// <summary>
        /// LINE CONFIGURATION
        /// </summary> 
        public double EXTLINELtscale = 10;

        /// <summary>
        /// ROGRAM CONFIGURATION
        /// </summary> 
        static private string PROGRAMDirectoryTemp = Path.GetTempPath() + "ConversorDrawindTemp\\";



        public string PROGRAMDbLin;
        public bool PROGRAMMessage = true;

        public bool EXTDIMCorrigeSeta = false;
        public string EXTDIMCorrigeSetaTipoSeta = "Oblique";
        public double EXTDIMCorrigeSetaFactor = 7.23;

        public string PROGRAMblockFormatoCaminho = " ";

        public int EXTCONFOrigem = 0;
        public bool EXTCONFInventorExplode = false;
        public string EXTCONFCaminhoBlocoInv = "";


        public bool ExplodeBlocks = false;

        public Configuration()
        { 
            var t = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            PROGRAMDbLin = Path.Combine(t, @"Autodesk\AutoCAD 2026\R25.1\enu\support\") + @"acad.lin";
        }

        public string GetPROGRAMDirectoryTemp()
        {
            return PROGRAMDirectoryTemp;
        }

        public bool CheckFileTxmlExist(string file, StatusConversorItem statusConversorItem)
        {
            return File.Exists(ConfigurationPaths.TxmlPath(file, statusConversorItem));
        }

        public void Load(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            LoadXML(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }
        public void LoadXML(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso , StatusConversorItem statusConversorItem)
        {
            ConfigurationXmlReader.Load(this, file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }

        internal void LoadXMLCore(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            arranjos.allBaseLayer.Clear();
            arranjos.allLineType1.Clear();
            arranjos.allNewLayerComposition.Clear();
            arranjos.allNewLayer.Clear();
            arranjos.conversor.Clear();
            arranjos.layerRemove.Clear();
            arranjos.listLISPCommand.Clear();
            blocks.Clear();
            blocosi.Clear();
            blocoso.Clear();
            arranjos.allExplodeLayers.Clear();


            string arquivo = ConfigurationPaths.TxmlPath(file, statusConversorItem);

            XElement importXML = XElement.Load(arquivo);

            EXTCONFComments = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.Comments, ConfigurationXmlContract.Text);


            EXTCONFOrigem = ConfigurationXmlContract.IntAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.TeklaOrCad);
            EXTCONFIsConvertDimension = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.ConvertDimensions);
            EXTCONFIsConvertLayer = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.ConvertLayers);
            EXTCONFIsExchangeFormat = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.ExchangeFormat);
            EXTCONFIsPutOnTheScaleDrawing = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.Scale);
            EXTCONFIsExecuteLISP = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.LispOrDll);
            EXTCONFIsPurge = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.Purge);
            PROGRAMMessage = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.Message);
            EXTCONFIsDeleteTeklaStructures = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.DeleteTeklaStructures);
            ExplodeBlocks = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.ExplodeBlocks);
            LayerTeklaString = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.LayerTeklaString);
            LayerBlockAttribute = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.LayerBlockAttribute);
            EXTCONFInventorExplode = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.CadExplode);


            EXTDIMGERALHabilit = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimGeralHabilit);
            EXTDIMlayer = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimLayer);
            EXTDIMColorLine = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimLineColor);
            EXTDIMColorText = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimTextColor);
            EXTDIMStyleName = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimStyle);
            EXTDIMSeta = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimArrowType);
            EXTDIMScale = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimScale);
            EXTDIMPrecision = ConfigurationXmlContract.IntAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimPrecision);
            EXTDIMAngularPrecision = ConfigurationXmlContract.IntAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimAngularPrecision);
            EXTDIMUnit = ConfigurationXmlContract.IntAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimUnit);
            EXTDIMAngularUnit = ConfigurationXmlContract.IntAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimAngularUnit);
            EXTDIMSizeSeta = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimArrowSize);
            EXTDIMOffsetLineFromRefPoint = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimOffset);
            EXTDIMOutsideAlign = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimOutsideAling);
            EXTDIMTad = ConfigurationXmlContract.IntAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimTad);
            EXTDIMDimensionPosition = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimPosition);
            EXTDIMTextForced = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimTextForced);
            EXTDIMLineForced = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimLineForced);
            EXTDIMDIMEX = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimDimex);
            EXTDIMBaseLayer = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimBaseLayer);
            EXTDIMCorrigeSeta = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimArrowFix);
            EXTDIMCorrigeSetaTipoSeta = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimArrowFixType);
            EXTDIMCorrigeSetaFactor = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.DimensionConfig, ConfigurationXmlContract.DimArrowFactor);
            EXTTEXTStyleName = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextStyleName);
           
            //EXTTEXTObliqueAngle = Convert.ToDouble(importXML.Element("TEXT_CONFIG").Attribute("TEXT_OBLIQUE_ANGLE").Value.Replace('.', ','));


            EXTSCALEManual = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleMode);
            EXTSCALEMp1.X = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleManualP1X);
            EXTSCALEMp1.Y = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleManualP1Y);
            EXTSCALEMp1.Z = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleManualP1Z);
            EXTSCALEMp2.X = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleManualP2X);
            EXTSCALEMp2.Y = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleManualP2Y);
            EXTSCALEMp2.Z = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleManualP2Z);
            EXTSCALEAp1.X = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleAutoP1X);
            EXTSCALEAp1.Y = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleAutoP1Y);
            EXTSCALEAp1.Z = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleAutoP1Z);
            EXTSCALEAp2.X = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleAutoP2X);
            EXTSCALEAp2.Y = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleAutoP2Y);
            EXTSCALEAp2.Z = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleAutoP2Z);
            EXTSCALELayer = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleLayer);
            EXTSCALETextSize = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.ScaleConfig, ConfigurationXmlContract.ScaleTextSize);



            EXTLINELtscale = ConfigurationXmlContract.DoubleAttribute(importXML, ConfigurationXmlContract.BasicLayers, ConfigurationXmlContract.Ltscale);
            foreach (var item in importXML.Element(ConfigurationXmlContract.BasicLayers).Elements(ConfigurationXmlContract.BaseLayer))
            {
                arranjos.allBaseLayer.Add(item.Value);
            }
            foreach (var item in importXML.Element(ConfigurationXmlContract.BasicLines).Elements(ConfigurationXmlContract.BaseLine))
            {
                arranjos.allLineType1.Add(item.Value);
            }

            foreach (var item in importXML.Element(ConfigurationXmlContract.NewLayers).Elements(ConfigurationXmlContract.NewLayer))
            {
                arranjos.allNewLayerComposition.Add(item.Value);
            }

            try
            {
                arranjos.allTextSyles.Clear();
                foreach (var item in importXML.Element(ConfigurationXmlContract.NewTextStyles).Elements(ConfigurationXmlContract.TextStyle))
                {
                    arranjos.allTextSyles.Add(item.Value);
                }
            }
            catch (Exception)
            {
                try
                {
                    string font = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextFonte);
                    string size = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextTamanho);
                    string factor = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextLargura);
                    string italic = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextItalico);
                    string negrito = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextNegrito);
                    string angle = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.TextConfig, ConfigurationXmlContract.TextObliqueAngle);

                    arranjos.allTextSyles.Add(EXTTEXTStyleName + ":" +
                        font + ":" +
                        italic + ":" +
                        negrito + ":" +
                        size + ":" +
                        factor + ":" +
                        angle );
                }
                catch (Exception)
                {
                    arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);
                }
        
            }

            for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
            {
                arranjos.allNewLayer.Add(arranjos.allNewLayerComposition[i].Split(':').First());
            }

         

            string line;
            foreach (var item in importXML.Element(ConfigurationXmlContract.RemoveLayers).Elements(ConfigurationXmlContract.RemoveLayer))
            {
                Filter f = new Filter(arranjos);
                line = item.Value;
                string[] treatment = line.Split('$');
                string[] st = treatment.Last().Split(';');
                f.layerBase = st[0];
                f.SetConjunto(st[1]);
                arranjos.layerRemove.Add(f);
            }

            foreach (var item in importXML.Element(ConfigurationXmlContract.Converters).Elements(ConfigurationXmlContract.Converter))
            {
                arranjos.conversor.Add(item.Value);
            }
            foreach (var item in importXML.Element(ConfigurationXmlContract.DllOrListCommands).Elements(ConfigurationXmlContract.Command))
            {
                arranjos.listLISPCommand.Add(item.Value);
            }

            PROGRAMblockFormatoCaminho = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.BlockConfig, ConfigurationXmlContract.DirectoryTeklaConversion);
            EXTCONFCaminhoBlocoInv = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.BlockConfig, ConfigurationXmlContract.DirectoryCadConversion);


            string[] allexplodelayers = ConfigurationXmlContract.AttributeValue(importXML, ConfigurationXmlContract.BlockConfig, ConfigurationXmlContract.LayerExplode).Split(';');
            arranjos.allExplodeLayers.AddRange(allexplodelayers);

            foreach (var item in importXML.Element(ConfigurationXmlContract.BlockConfig).Elements(ConfigurationXmlContract.BlockAtt))
            {
                Block blockClass = new Block();
                blockClass.blockName = item.Attribute(ConfigurationXmlContract.Nome).Value;
                foreach (var tag in item.Elements(ConfigurationXmlContract.Tag))
                {
                    TagBlock tagTemp = new TagBlock();
                    tagTemp.SetConjunto(tag.Value);
                    blockClass.listTags.Add(tagTemp);
                }
                blocks.Add(blockClass);
            }
            foreach (var item in importXML.Element(ConfigurationXmlContract.BlockConfig).Elements(ConfigurationXmlContract.BlockAttCad))
            {
                Block blockClass = new Block();

                string[] linesplit = item.Attribute(ConfigurationXmlContract.Nome).Value.Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements(ConfigurationXmlContract.Tag))
                {
                    TagBlock tagTemp = new TagBlock();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                blocosi.Add(blockClass);
            }
            foreach (var item in importXML.Element(ConfigurationXmlContract.BlockConfig).Elements(ConfigurationXmlContract.BlockAttOrig))
            {
                Block blockClass = new Block();

                string[] linesplit = item.Attribute(ConfigurationXmlContract.Nome).Value.Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements(ConfigurationXmlContract.Tag))
                {
                    TagBlock tagTemp = new TagBlock();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                blocoso.Add(blockClass);
            }

            try
            {
                DMBlock = ConfigurationXmlContract.BoolAttribute(importXML, ConfigurationXmlContract.BasicConfig, ConfigurationXmlContract.DmBlock);
            }
            catch (Exception)
            {

            }
           
        }
        
        public void SaveXML(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            ConfigurationXmlWriter.Save(this, file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }

        internal void SaveXMLCore(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            string arquivo = ConfigurationPaths.TxmlPath(file, statusConversorItem);

            XElement xml = new XElement(ConfigurationXmlContract.Root);
            //COMENTARIO
            {
                XElement x = new XElement(ConfigurationXmlContract.Comments);
                x.Add(new XAttribute(ConfigurationXmlContract.Text, EXTCONFComments));
                xml.Add(x);
            }

            //CONFIGURACAO BASICAS
            {
                XElement x = new XElement(ConfigurationXmlContract.BasicConfig);
                x.Add(new XAttribute(ConfigurationXmlContract.TeklaOrCad, EXTCONFOrigem));
                x.Add(new XAttribute(ConfigurationXmlContract.ConvertDimensions, EXTCONFIsConvertDimension));
                x.Add(new XAttribute(ConfigurationXmlContract.ConvertLayers, EXTCONFIsConvertLayer));
                x.Add(new XAttribute(ConfigurationXmlContract.ExchangeFormat, EXTCONFIsExchangeFormat));
                x.Add(new XAttribute(ConfigurationXmlContract.Scale, EXTCONFIsPutOnTheScaleDrawing));
                x.Add(new XAttribute(ConfigurationXmlContract.LispOrDll, EXTCONFIsExecuteLISP));
                x.Add(new XAttribute(ConfigurationXmlContract.Purge, EXTCONFIsPurge));
                x.Add(new XAttribute(ConfigurationXmlContract.Message, PROGRAMMessage));
                x.Add(new XAttribute(ConfigurationXmlContract.DeleteTeklaStructures, EXTCONFIsDeleteTeklaStructures));
                x.Add(new XAttribute(ConfigurationXmlContract.ExplodeBlocks, ExplodeBlocks));
                x.Add(new XAttribute(ConfigurationXmlContract.LayerTeklaString, LayerTeklaString));
                x.Add(new XAttribute(ConfigurationXmlContract.LayerBlockAttribute, LayerBlockAttribute));
                x.Add(new XAttribute(ConfigurationXmlContract.CadExplode, EXTCONFInventorExplode));
                x.Add(new XAttribute(ConfigurationXmlContract.DmBlock, DMBlock));
                xml.Add(x);
            }
            {
                XElement x = new XElement(ConfigurationXmlContract.DimensionConfig);
                x.Add(new XAttribute(ConfigurationXmlContract.DimGeralHabilit, EXTDIMGERALHabilit));
                x.Add(new XAttribute(ConfigurationXmlContract.DimLayer, EXTDIMlayer));
                x.Add(new XAttribute(ConfigurationXmlContract.DimLineColor, EXTDIMColorLine));
                x.Add(new XAttribute(ConfigurationXmlContract.DimTextColor, EXTDIMColorText));
                x.Add(new XAttribute(ConfigurationXmlContract.DimStyle, EXTDIMStyleName));
                x.Add(new XAttribute(ConfigurationXmlContract.DimArrowType, EXTDIMSeta));
                x.Add(new XAttribute(ConfigurationXmlContract.DimScale, EXTDIMScale));
                x.Add(new XAttribute(ConfigurationXmlContract.DimPrecision, EXTDIMPrecision));
                x.Add(new XAttribute(ConfigurationXmlContract.DimAngularPrecision, EXTDIMAngularPrecision));
                x.Add(new XAttribute(ConfigurationXmlContract.DimUnit, EXTDIMUnit));
                x.Add(new XAttribute(ConfigurationXmlContract.DimAngularUnit, EXTDIMAngularUnit));
                x.Add(new XAttribute(ConfigurationXmlContract.DimArrowSize, EXTDIMSizeSeta));
                x.Add(new XAttribute(ConfigurationXmlContract.DimOffset, EXTDIMOffsetLineFromRefPoint));
                x.Add(new XAttribute(ConfigurationXmlContract.DimOutsideAling, EXTDIMOutsideAlign));
                x.Add(new XAttribute(ConfigurationXmlContract.DimTad, EXTDIMTad));
                x.Add(new XAttribute(ConfigurationXmlContract.DimPosition, EXTDIMDimensionPosition));
                x.Add(new XAttribute(ConfigurationXmlContract.DimTextForced, EXTDIMTextForced));
                x.Add(new XAttribute(ConfigurationXmlContract.DimLineForced, EXTDIMLineForced));
                x.Add(new XAttribute(ConfigurationXmlContract.DimDimex, EXTDIMDIMEX));
                x.Add(new XAttribute(ConfigurationXmlContract.DimBaseLayer, EXTDIMBaseLayer));
                x.Add(new XAttribute(ConfigurationXmlContract.DimArrowFix, EXTDIMCorrigeSeta));
                x.Add(new XAttribute(ConfigurationXmlContract.DimArrowFixType, EXTDIMCorrigeSetaTipoSeta));
                x.Add(new XAttribute(ConfigurationXmlContract.DimArrowFactor, EXTDIMCorrigeSetaFactor));
                xml.Add(x);
            }
            {
                XElement x = new XElement(ConfigurationXmlContract.TextConfig);
                x.Add(new XAttribute(ConfigurationXmlContract.TextStyleName, EXTTEXTStyleName));
                //x.Add(new XAttribute("TEXT_OBLIQUE_ANGLE", EXTTEXTObliqueAngle));
                xml.Add(x);

            }
            {
                XElement x = new XElement(ConfigurationXmlContract.ScaleConfig);
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleMode, EXTSCALEManual));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleManualP1X, EXTSCALEMp1.X));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleManualP1Y, EXTSCALEMp1.Y));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleManualP1Z, EXTSCALEMp1.Z));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleManualP2X, EXTSCALEMp2.X));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleManualP2Y, EXTSCALEMp2.Y));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleManualP2Z, EXTSCALEMp2.Z));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleAutoP1X, EXTSCALEAp1.X));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleAutoP1Y, EXTSCALEAp1.Y));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleAutoP1Z, EXTSCALEAp1.Z));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleAutoP2X, EXTSCALEAp2.X));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleAutoP2Y, EXTSCALEAp2.Y));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleAutoP2Z, EXTSCALEAp2.Z));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleLayer, EXTSCALELayer));
                x.Add(new XAttribute(ConfigurationXmlContract.ScaleTextSize, EXTSCALETextSize));
                xml.Add(x);

            }

            {
                XElement x = new XElement(ConfigurationXmlContract.BasicLayers);
                x.Add(new XAttribute(ConfigurationXmlContract.Ltscale, EXTLINELtscale));

                foreach (var item in arranjos.allBaseLayer)
                {
                    x.Add(new XElement(ConfigurationXmlContract.BaseLayer, item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.BasicLines);
                foreach (var item in arranjos.allLineType1)
                {
                    x.Add(new XElement(ConfigurationXmlContract.BaseLine, item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.NewTextStyles);
                foreach (var item in arranjos.allTextSyles)
                {
                    x.Add(new XElement(ConfigurationXmlContract.TextStyle, item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.NewLayers);
                if (arranjos.allNewLayerComposition.Count == 0)
                {
                    x.Add(new XElement(ConfigurationXmlContract.NewLayer, "0:WHITE:CONTINUOUS"));
                }

                foreach (var item in arranjos.allNewLayerComposition)
                {
                    x.Add(new XElement(ConfigurationXmlContract.NewLayer, item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.RemoveLayers);
                foreach (var item in arranjos.layerRemove)
                {
                    x.Add(new XElement(ConfigurationXmlContract.RemoveLayer, item.layerBase + ";" + item.GetConjunto()));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.Converters);
                foreach (var item in arranjos.conversor)
                {
                    x.Add(new XElement(ConfigurationXmlContract.Converter, item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.DllOrListCommands);

                foreach (var item in arranjos.listLISPCommand)
                {
                    x.Add(new XElement(ConfigurationXmlContract.Command, item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement(ConfigurationXmlContract.BlockConfig);
                x.Add(new XAttribute(ConfigurationXmlContract.DirectoryTeklaConversion, PROGRAMblockFormatoCaminho));

                string removelayer = "";
                foreach (var item in arranjos.allExplodeLayers)
                {
                    removelayer = removelayer + item + ";";
                }
                removelayer = removelayer.Trim(';');

                x.Add(new XAttribute(ConfigurationXmlContract.DirectoryCadConversion, EXTCONFCaminhoBlocoInv));
                x.Add(new XAttribute(ConfigurationXmlContract.LayerExplode, removelayer));
                foreach (Block item in blocks)
                {
                    XElement x1 = new XElement(ConfigurationXmlContract.BlockAtt);
                    x1.Add(new XAttribute(ConfigurationXmlContract.Nome, item.blockName));

                    foreach (TagBlock tag in item.listTags)
                    {
                        x1.Add(new XElement(ConfigurationXmlContract.Tag, tag.GetConjuntoString()));
                    }
                    x.Add(x1);
                }

                foreach (Block item in blocosi)
                {
                    XElement x1 = new XElement(ConfigurationXmlContract.BlockAttCad);
                    x1.Add(new XAttribute(ConfigurationXmlContract.Nome, item.blockName + ";" + item.blockNameRelacao + ";" + item.cor.ToArgb()));

                    foreach (TagBlock tag in item.listTags)
                    {
                        x1.Add(new XElement(ConfigurationXmlContract.Tag, tag.GetConjuntoString() + "@" + tag.indiceRelacao + "@" + tag.isSociate));
                    }
                    x.Add(x1);
                }

                foreach (Block item in blocoso)
                {
                    XElement x1 = new XElement(ConfigurationXmlContract.BlockAttOrig);
                    x1.Add(new XAttribute(ConfigurationXmlContract.Nome, item.blockName + ";" + item.blockNameRelacao + ";" + item.cor.ToArgb()));

                    foreach (TagBlock tag in item.listTags)
                    {
                        x1.Add(new XElement(ConfigurationXmlContract.Tag, tag.GetConjuntoString() + "@" + tag.indiceRelacao + "@" + tag.isSociate));

                    }
                    x.Add(x1);
                }
                xml.Add(x);
            }
            xml.Save(arquivo);
        }


        public static string LoadConfigDLL()
        {
            return DllConfigFile.Load();
        }

        public static void SaveConfigDLL()
        {
            DllConfigFile.SaveIfMissing();
        }
    }
}

