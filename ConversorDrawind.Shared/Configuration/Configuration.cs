using System;
using System.Collections.Generic;
using System.IO;

namespace ConversorDrawind
{
    public class Configuration
    {
        public static Configuration Config = new Configuration();
        public static double INTREFTamFormato = 841;

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
        public double EXTTEXTSize = 2.5;
        
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
        public string EXTSCALETextSizeString = "2,5";

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
        public int ConvTekla0ConvInv1 = 0;
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
        public void LoadXML(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            string arquivo = ConfigurationPaths.TxmlPath(file, statusConversorItem);
            ConfigurationXmlDocument.Load(arquivo).ApplyTo(this, arranjos, blocks, blocosi, blocoso);
        }

        public void SaveXML(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            string arquivo = ConfigurationPaths.TxmlPath(file, statusConversorItem);
            ConfigurationXmlDocument.From(this, arranjos, blocks, blocosi, blocoso).Save(arquivo);
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






