namespace ConversorDrawindDLL
{
    public class Configuration : global::ConversorDrawind.Configuration
    {
        public static double INTREFTamFormato = 841;
        public static Configuration Config = new Configuration();

        public double EXTTEXTSize = 2.5;
        public string EXTSCALETextSizeString = "2.5";

        public int ConvTekla0ConvInv1
        {
            get { return EXTCONFOrigem; }
            set { EXTCONFOrigem = value; }
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
            EXTSCALEMp1 = ToSharedPoint(data.EXTSCALEMp1);
            EXTSCALEMp2 = ToSharedPoint(data.EXTSCALEMp2);
            EXTSCALEAp1 = ToSharedPoint(data.EXTSCALEAp1);
            EXTSCALEAp2 = ToSharedPoint(data.EXTSCALEAp2);
            EXTSCALELayer = data.EXTSCALELayer;
            EXTSCALETextSize = data.EXTSCALETextSize;
            EXTSCALETextSizeString = EXTSCALETextSize.ToString();
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

        public static new string LoadConfigDLL()
        {
            return global::ConversorDrawind.Configuration.LoadConfigDLL();
        }

        private static global::ConversorDrawind.PointEspecial ToSharedPoint(PointEspecial2 point)
        {
            return new global::ConversorDrawind.PointEspecial(point.X, point.Y, point.Z);
        }
    }
}
