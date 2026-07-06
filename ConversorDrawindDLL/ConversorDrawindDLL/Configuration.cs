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
            ConfigurationXmlData data = ConfigurationXmlReader.Read(file);
            Arranjos.Arrj.AllNewLayerComposition.Clear();
            Arranjos.Arrj.Conversor.Clear();
            Arranjos.Arrj.LayerRemove.Clear();
            Arranjos.Arrj.ListLISPCommand.Clear();
            Arranjos.ListBlocks.Clear();
            Arranjos.ListBlocksInv.Clear();
            Arranjos.ListBlocksOrig.Clear();
            Arranjos.Arrj.AllExplodeLayers.Clear();

            EXTCONFComments = data.EXTCONFComments;
            ConvTekla0ConvInv1 = data.ConvTekla0ConvInv1;
            EXTCONFIsConvertDimension = data.EXTCONFIsConvertDimension;
            EXTCONFIsConvertLayer = data.EXTCONFIsConvertLayer;
            EXTCONFIsExchangeFormat = data.EXTCONFIsExchangeFormat;
            EXTCONFIsPutOnTheScaleDrawing = data.EXTCONFIsPutOnTheScaleDrawing;
            EXTCONFIsExecuteLISP = data.EXTCONFIsExecuteLISP;
            EXTCONFIsPurge = data.EXTCONFIsPurge;
            PROGRAMMessage = data.PROGRAMMessage;
            EXTCONFIsDeleteTeklaStructures = data.EXTCONFIsDeleteTeklaStructures;
            ExplodeBlocks = data.ExplodeBlocks;
            LayerTeklaString = data.LayerTeklaString;
            LayerBlockAttribute = data.LayerBlockAttribute;
            EXTCONFInventorExplode = data.EXTCONFInventorExplode;
            EXTDIMGERALHabilit = data.EXTDIMGERALHabilit;
            EXTDIMlayer = data.EXTDIMlayer;
            EXTDIMColorLine = data.EXTDIMColorLine;
            EXTDIMColorText = data.EXTDIMColorText;
            EXTDIMStyleName = data.EXTDIMStyleName;
            EXTDIMSeta = data.EXTDIMSeta;
            EXTDIMScale = data.EXTDIMScale;
            EXTDIMPrecision = data.EXTDIMPrecision;
            EXTDIMAngularPrecision = data.EXTDIMAngularPrecision;
            EXTDIMUnit = data.EXTDIMUnit;
            EXTDIMAngularUnit = data.EXTDIMAngularUnit;
            EXTDIMSizeSeta = data.EXTDIMSizeSeta;
            EXTDIMOffsetLineFromRefPoint = data.EXTDIMOffsetLineFromRefPoint;
            EXTDIMOutsideAlign = data.EXTDIMOutsideAlign;
            EXTDIMTad = data.EXTDIMTad;
            EXTDIMDimensionPosition = data.EXTDIMDimensionPosition;
            EXTDIMTextForced = data.EXTDIMTextForced;
            EXTDIMLineForced = data.EXTDIMLineForced;
            EXTDIMDIMEX = data.EXTDIMDIMEX;
            EXTDIMBaseLayer = data.EXTDIMBaseLayer;
            EXTDIMCorrigeSeta = data.EXTDIMCorrigeSeta;
            EXTDIMCorrigeSetaTipoSeta = data.EXTDIMCorrigeSetaTipoSeta;
            EXTDIMCorrigeSetaFactor = data.EXTDIMCorrigeSetaFactor;
            EXTTEXTStyleName = data.EXTTEXTStyleName;
            EXTSCALEManual = data.EXTSCALEManual;
            EXTSCALEMp1 = data.EXTSCALEMp1;
            EXTSCALEMp2 = data.EXTSCALEMp2;
            EXTSCALEAp1 = data.EXTSCALEAp1;
            EXTSCALEAp2 = data.EXTSCALEAp2;
            EXTSCALELayer = data.EXTSCALELayer;
            EXTSCALETextSize = data.EXTSCALETextSize;
            EXTLINELtscale = data.EXTLINELtscale;
            PROGRAMblockFormatoCaminho = data.PROGRAMblockFormatoCaminho;
            EXTCONFCaminhoBlocoInv = data.EXTCONFCaminhoBlocoInv;
            if (data.DMBlock.HasValue)
                DMBlock = data.DMBlock.Value;
            EXTTEXTSize = data.TextSizeFromStyle;

            Arranjos.Arrj.AllNewLayerComposition.AddRange(data.AllNewLayerComposition);
            Arranjos.Arrj.AllTextSyles.Clear();
            Arranjos.Arrj.AllTextSyles.AddRange(data.AllTextSyles);
            Arranjos.Arrj.LayerRemove.AddRange(data.LayerRemove);
            Arranjos.Arrj.Conversor.AddRange(data.Conversor);
            Arranjos.Arrj.ListLISPCommand.AddRange(data.ListLISPCommand);
            Arranjos.Arrj.AllExplodeLayers.AddRange(data.AllExplodeLayers);
            Arranjos.ListBlocks.AddRange(data.ListBlocks);
            Arranjos.ListBlocksInv.AddRange(data.ListBlocksInv);
            Arranjos.ListBlocksOrig.AddRange(data.ListBlocksOrig);

            InstanciaConversor.ConversorInstancias.Clear();
            InstanciaConversor.ConversorInstancias.AddRange(ConfigurationXmlReader.CreateConverterInstances(Arranjos.Arrj.Conversor));

        }

        public static string LoadConfigDLL()
        {
            string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");
            return ConfigurationXmlReader.ReadLoadConfigDll(arquivo);
        }

    }
}
