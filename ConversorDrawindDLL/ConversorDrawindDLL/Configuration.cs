using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;

namespace ConversorDrawindDLL
{
    public class Configuration
    {
        public static double INTREFTamFormato = 841;
        public static Configuration Config = new Configuration();
        /// <summary>
        /// INT CONVERT DIMENSION
        /// </summary>

        public double INTFactorLengthChar = 1.5;
        public double INTDIMTextOffset = 0.6100;

        /// <summary>
        /// EXT CONVERT DIMENSION
        /// </summary>
        public string EXTDIMStyleName = "COTAS";
        public double EXTDIMScale = 1; //Escala da dimensão
        public int EXTDIMPrecision = 0; //Precisão da dimensão 
        public int EXTDIMAngularPrecision = 1; //Precisão da dimensão algular
        public int EXTDIMUnit = 2;//Unidade da dimensão 
        public int EXTDIMAngularUnit = 2;//Unidade da dimensão algular
        public string EXTDIMSeta = "OBLIQUE";
        public string EXTDIMSeta1 = "OBLIQUE";
        public string EXTDIMSeta2 = "OBLIQUE";
        public double EXTDIMSizeSeta = 0.15;
        public int EXTDIMTad = 1; //Posição do texto na dimensão verticalmente
        public bool EXTDIMDimensionPosition = false; //Posição do texto em relação a dimensão
        public bool EXTDIMTextForced = true; //Forçar o texto a permanecer alinhado com a dimensão, caso o espaço seja curto.
        public bool EXTDIMLineForced = true;
        public string EXTDIMColorText = "3";
        public string EXTDIMColorLine = "1";
        public double EXTDIMOffsetLineFromRefPoint = 0;
        public int EXTDIMTextMove = 2;
        public bool EXTDIMOutsideAlign = false;
        public string EXTDIMlayer = "0";
      public string EXTTEXTStyleName = "TEXTO";
       public double EXTTEXTSize = 2.5;
        // double EXTTEXTFactor = 1;
       // public string EXTTEXTFont = "RomanS";
        //public bool EXTTEXTFontItalico = false;
       // public bool EXTTEXTFontNegrito = false;
        public bool EXTDIMGERALHabilit = false;
        public double EXTDIMDIMEX = 1;
        public string EXTDIMBaseLayer = "DIMENSION";
        //public double EXTTEXTObliqueAngle = 1;
        /// <summary>
        /// EXT CONFIGURATION
        /// </summary> 
        public string EXTCONFComments;
        public bool EXTCONFIsConvertDimension;
        public bool EXTCONFIsConvertLayer;
        public bool EXTCONFIsExchangeFormat;
        public bool EXTCONFIsExchangeLM;
        public bool EXTCONFIsPutOnTheScaleDrawing;
        public bool EXTCONFIsExecuteLISP;
        public bool EXTCONFIsExecuteDLL;
        public bool EXTCONFIsDeleteTeklaStructures = true;
        public string EXTCONFIsFirstRum = "LISP";
        public bool EXTCONFIsPurge = true;
        public bool DMBlock = true;
        /// <summary>
        /// EXT SCALE
        /// </summary>
        public bool EXTSCALEManual = true;
        public PointEspecial2 EXTSCALEMp1 = new PointEspecial2(0, 0, 0);
        public PointEspecial2 EXTSCALEMp2 = new PointEspecial2(0, 0, 0);
        public PointEspecial2 EXTSCALEAp1 = new PointEspecial2(0, 0, 0);
        public PointEspecial2 EXTSCALEAp2 = new PointEspecial2(0, 0, 0);
        public string EXTSCALELayer = "0";
        public double EXTSCALETextSize = 2.5;
        public string EXTSCALETextSizeString = "2.5";
        /// <summary>
        /// LINE CONFIGURATION
        /// </summary> 
        public double EXTLINELtscale = 10;

        /// <summary>
        /// ROGRAM CONFIGURATION
        /// </summary> 
        static private string PROGRAMDirectoryTemp = Path.GetTempPath() + "\\ConversorDrawindTemp\\";

        public bool PROGRAMMessage = true;

        public bool EXTDIMCorrigeSeta = false;
        public string EXTDIMCorrigeSetaTipoSeta = "Oblique";
        public double EXTDIMCorrigeSetaFactor = 7.23;

        public string PROGRAMblockFormatoCaminho = " ";

        public int ConvTekla0ConvInv1 = 0;
        public bool EXTCONFInventorExplode = false;
        public string EXTCONFCaminhoBlocoInv = "";

        public bool ExplodeBlocks = false;
        public string LayerTeklaString = "DRAWING SHEET";
        public string LayerBlockAttribute = "OTHER OBJECT TYPE";

        public string GetPROGRAMDirectoryTemp()
        {
            return PROGRAMDirectoryTemp;
        }

        public void LoadXML(string file)
        {

            Arranjos.Arrj.AllNewLayerComposition.Clear();

            Arranjos.Arrj.Conversor.Clear();
            Arranjos.Arrj.LayerRemove.Clear();
            Arranjos.Arrj.ListLISPCommand.Clear();
            Arranjos.ListBlocks.Clear();
            Arranjos.ListBlocksInv.Clear();
            Arranjos.ListBlocksOrig.Clear();
            Arranjos.Arrj.AllExplodeLayers.Clear();



            XElement importXML = XElement.Load(file);

            EXTCONFComments = importXML.Element("COMMENTS").Attribute("TEXT").Value;


            ConvTekla0ConvInv1 = Convert.ToInt32(importXML.Element("BASIC_CONFIG").Attribute("TEKLAORCAD").Value);
            EXTCONFIsConvertDimension = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("CONVERT_DIMENSIONS").Value);
            EXTCONFIsConvertLayer = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("CONVERT_LAYERS").Value);
            EXTCONFIsExchangeFormat = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("EXCHANGE_FORMAT").Value);
            EXTCONFIsPutOnTheScaleDrawing = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("SCALE").Value);
            EXTCONFIsExecuteLISP = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("LISPORDLL").Value);
            EXTCONFIsPurge = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("PURGE").Value);
            PROGRAMMessage = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("MESSAGE").Value);
            EXTCONFIsDeleteTeklaStructures = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("DELETE_TEKLA_STRUCTURES").Value);
            ExplodeBlocks = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("EXPLOD_BLOCKS").Value);
            LayerTeklaString = importXML.Element("BASIC_CONFIG").Attribute("LAYER_TEKLA_STRING").Value;
            LayerBlockAttribute = importXML.Element("BASIC_CONFIG").Attribute("LAYER_BLOCK_ATTRIBUTE").Value;
            EXTCONFInventorExplode = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("CAD_EXPLODE").Value);

            EXTDIMGERALHabilit = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_GERAL_HABILIT").Value);
            EXTDIMlayer = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_LAYER").Value;
            EXTDIMColorLine = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_LINE_COLOR").Value;
            EXTDIMColorText = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_TEXT_COLOR").Value;
            EXTDIMStyleName = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_STYLE").Value;
            EXTDIMSeta = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_TYPE").Value;
            EXTDIMScale = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_SCALE").Value.ReplaceComma());
            EXTDIMPrecision = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_PRECISION").Value);
            EXTDIMAngularPrecision = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ANGULAR_PRECISION").Value);
            EXTDIMUnit = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_UNIT").Value);
            EXTDIMAngularUnit = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ANGULAR_UNIT").Value);
            EXTDIMSizeSeta = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_SIZE").Value.ReplaceComma());
            EXTDIMOffsetLineFromRefPoint = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_OFFSET").Value.ReplaceComma());
            EXTDIMOutsideAlign = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_OUTSIDE_ALING").Value);
            EXTDIMTad = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_TAD").Value);
            EXTDIMDimensionPosition = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_POSITION").Value);
            EXTDIMTextForced = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_TEXT_FORCED").Value);
            EXTDIMLineForced = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_LINE_FORCED").Value);
            EXTDIMDIMEX = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_DIMEX").Value.ReplaceComma());
            EXTDIMBaseLayer = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_BASE_LAYER").Value;
            EXTDIMCorrigeSeta = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_FIX").Value);
            EXTDIMCorrigeSetaTipoSeta = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_FIX_TYPE").Value;
            EXTDIMCorrigeSetaFactor = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_FACTOR").Value.ReplaceComma());
           EXTTEXTStyleName = importXML.Element("TEXT_CONFIG").Attribute("TEXT_STYPE").Value;

            EXTSCALEManual = Convert.ToBoolean(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MODE").Value);
            EXTSCALEMp1.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P1_X").Value.ReplaceComma());
            EXTSCALEMp1.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P1_Y").Value.ReplaceComma());
            EXTSCALEMp1.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P1_Z").Value.ReplaceComma());
            EXTSCALEMp2.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P2_X").Value.ReplaceComma());
            EXTSCALEMp2.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P2_Y").Value.ReplaceComma());
            EXTSCALEMp2.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P2_Z").Value.ReplaceComma());
            EXTSCALEAp1.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P1_X").Value.ReplaceComma());
            EXTSCALEAp1.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P1_Y").Value.ReplaceComma());
            EXTSCALEAp1.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P1_Z").Value.ReplaceComma());
            EXTSCALEAp2.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P2_X").Value.ReplaceComma());
            EXTSCALEAp2.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P2_Y").Value.ReplaceComma());
            EXTSCALEAp2.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P2_Z").Value.ReplaceComma());
            EXTSCALELayer = importXML.Element("SCALE_CONFIG").Attribute("SCALE_LAYER").Value;
            EXTSCALETextSize = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_TEXT_SIZE").Value.ReplaceComma());

            EXTLINELtscale = Convert.ToDouble(importXML.Element("BASIC_LAYERS").Attribute("LTSCALE").Value.ReplaceComma());


            foreach (var item in importXML.Element("NEW_LAYERS").Elements("NEW_LAYER"))
            {
                Arranjos.Arrj.AllNewLayerComposition.Add(item.Value);
            }


            try
            {
                Arranjos.Arrj.AllTextSyles.Clear();
                foreach (var item in importXML.Element("NEW_TEXTSTYLES").Elements("TEXT_STYLE"))
                {
                    Arranjos.Arrj.AllTextSyles.Add(item.Value);
                }
                if (Arranjos.Arrj.AllTextSyles.Count == 0)
                    Arranjos.Arrj.AllTextSyles.Add(Arranjos.defaultTextStyle);
            }
            catch (Exception)
            {
                Arranjos.Arrj.AllTextSyles.Add(Arranjos.defaultTextStyle);
            }


            string line;
            foreach (var item in importXML.Element("REMOVE_LAYERS").Elements("REMOVE_LAYER"))
            {
                Filter f = new Filter();
                line = item.Value;
                string[] treatment = line.Split('$');
                string[] st = treatment.Last().Split(';');
                f.layerBase = st[0];
                f.SetConjunto(st[1]);
                Arranjos.Arrj.LayerRemove.Add(f);
            }

            foreach (var item in importXML.Element("CONVERTERS").Elements("CONVERTER"))
            {
                Arranjos.Arrj.Conversor.Add(item.Value);
            }
            foreach (var item in importXML.Element("DLL_OR_LIST_COMMANDS").Elements("COMMAND"))
            {
                Arranjos.Arrj.ListLISPCommand.Add(item.Value);
            }

            PROGRAMblockFormatoCaminho = importXML.Element("BLOCK_CONFIG").Attribute("DIRECTORY_TEKLA_CONVERSION").Value;
            EXTCONFCaminhoBlocoInv = importXML.Element("BLOCK_CONFIG").Attribute("DIRECTORY_CAD_CONVERSION").Value;


            string[] allexplodelayers = importXML.Element("BLOCK_CONFIG").Attribute("LAYER_EXPLODE").Value.Split(';');
            Arranjos.Arrj.AllExplodeLayers.AddRange(allexplodelayers);

            foreach (var item in importXML.Element("BLOCK_CONFIG").Elements("BLOCK_ATT"))
            {
                BlockClass blockClass = new BlockClass();
                blockClass.blockName = item.Attribute("NOME").Value;
                foreach (var tag in item.Elements("TAG"))
                {
                    TagBlockClass tagTemp = new TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    blockClass.listTags.Add(tagTemp);
                }
                Arranjos.ListBlocks.Add(blockClass);
            }
            foreach (var item in importXML.Element("BLOCK_CONFIG").Elements("BLOCK_ATT_CAD"))
            {
                BlockClass blockClass = new BlockClass();

                string[] linesplit = item.Attribute("NOME").Value.Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements("TAG"))
                {
                    TagBlockClass tagTemp = new TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                Arranjos.ListBlocksInv.Add(blockClass);
            }
            foreach (var item in importXML.Element("BLOCK_CONFIG").Elements("BLOCK_ATT_ORIG"))
            {
                BlockClass blockClass = new BlockClass();

                string[] linesplit = item.Attribute("NOME").Value.Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements("TAG"))
                {
                    TagBlockClass tagTemp = new TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                Arranjos.ListBlocksOrig.Add(blockClass);
            }
            try
            {
                DMBlock = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("DMBLOCK").Value);
            }
            catch (Exception)
            {

            }
           List<string> estiloAtual =  Arranjos.Arrj.AllTextSyles.Where(a => a.Split(':').First().ToUpper() == EXTTEXTStyleName.ToUpper()).ToList();
            if (estiloAtual.Count == 0)
                estiloAtual.Add(Arranjos.defaultTextStyle);
            string[] estiloAtualSplit = estiloAtual.First().Split(':');

            EXTTEXTSize = estiloAtualSplit[4].ToDouble();

            InstanciaConversor.ConversorInstancias.Clear();
            InstanciaConversor.ConversorInstancias.AddRange(Arranjos.Arrj.Conversor.Select(p => new InstanciaConversor(p)));

        }

        public static string LoadConfigDLL()
        {
            string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");
            if (!File.Exists(arquivo))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");

            XElement importXML = XElement.Load(arquivo);


            return importXML.Element("configurations").Attribute("BlocoDM").Value;
        }

    }
}
