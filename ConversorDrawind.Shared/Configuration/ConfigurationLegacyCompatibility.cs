using System;
using System.Collections.Generic;

namespace ConversorDrawind
{
    public partial class Configuration
    {
        private static readonly IConfigurationRepository ConfigurationRepository = new TxmlConfigurationRepository();
        private static double referenceFormatSize = 841;

        private PointEspecial scalePoint1 = new PointEspecial(0, 0, 0);
        private PointEspecial scalePoint2 = new PointEspecial(0, 0, 0);
        private Point3DConfiguration syncedScalePoint1 = new Point3DConfiguration();
        private Point3DConfiguration syncedScalePoint2 = new Point3DConfiguration();
        private PointEspecial automaticScalePoint1 = new PointEspecial(0, 0, 0);
        private PointEspecial automaticScalePoint2 = new PointEspecial(0, 0, 0);

        public static double INTREFTamFormato
        {
            get { return referenceFormatSize; }
            set
            {
                referenceFormatSize = value;
                if (Config != null)
                    Config.Dimensions.ReferenceFormatSize = value;
            }
        }

        [Obsolete("Use Dimensions.InternalLengthCharFactor.")]
        public double INTFactorLengthChar { get { return Dimensions.InternalLengthCharFactor; } set { Dimensions.InternalLengthCharFactor = value; } }

        [Obsolete("Use Dimensions.InternalTextOffset.")]
        public double INTDIMTextOffset { get { return Dimensions.InternalTextOffset; } set { Dimensions.InternalTextOffset = value; } }

        [Obsolete("Use Blocks.DimensionBlockEnabled.")]
        public bool DMBlock { get { return Blocks.DimensionBlockEnabled; } set { Blocks.DimensionBlockEnabled = value; } }

        [Obsolete("Use Dimensions.StyleName.")]
        public string EXTDIMStyleName { get { return Dimensions.StyleName; } set { Dimensions.StyleName = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.Scale.")]
        public double EXTDIMScale { get { return Dimensions.Scale; } set { Dimensions.Scale = value; } }

        [Obsolete("Use Dimensions.Precision.")]
        public int EXTDIMPrecision { get { return Dimensions.Precision; } set { Dimensions.Precision = value; } }

        [Obsolete("Use Dimensions.AngularPrecision.")]
        public int EXTDIMAngularPrecision { get { return Dimensions.AngularPrecision; } set { Dimensions.AngularPrecision = value; } }

        [Obsolete("Use Dimensions.Unit.")]
        public int EXTDIMUnit { get { return Dimensions.Unit; } set { Dimensions.Unit = value; } }

        [Obsolete("Use Dimensions.AngularUnit.")]
        public int EXTDIMAngularUnit { get { return Dimensions.AngularUnit; } set { Dimensions.AngularUnit = value; } }

        [Obsolete("Use Dimensions.ArrowType.")]
        public string EXTDIMSeta { get { return Dimensions.ArrowType; } set { Dimensions.ArrowType = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.ArrowType1.")]
        public string EXTDIMSeta1 { get { return Dimensions.ArrowType1; } set { Dimensions.ArrowType1 = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.ArrowType2.")]
        public string EXTDIMSeta2 { get { return Dimensions.ArrowType2; } set { Dimensions.ArrowType2 = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.ArrowSize.")]
        public double EXTDIMSizeSeta { get { return Dimensions.ArrowSize; } set { Dimensions.ArrowSize = value; } }

        [Obsolete("Use Dimensions.TextVerticalPosition.")]
        public int EXTDIMTad { get { return Dimensions.TextVerticalPosition; } set { Dimensions.TextVerticalPosition = value; } }

        [Obsolete("Use Dimensions.TextRelativeToDimensionLine.")]
        public bool EXTDIMDimensionPosition { get { return Dimensions.TextRelativeToDimensionLine; } set { Dimensions.TextRelativeToDimensionLine = value; } }

        [Obsolete("Use Dimensions.ForceTextInside.")]
        public bool EXTDIMTextForced { get { return Dimensions.ForceTextInside; } set { Dimensions.ForceTextInside = value; } }

        [Obsolete("Use Dimensions.ForceDimensionLine.")]
        public bool EXTDIMLineForced { get { return Dimensions.ForceDimensionLine; } set { Dimensions.ForceDimensionLine = value; } }

        [Obsolete("Use Dimensions.TextColor.")]
        public string EXTDIMColorText { get { return Dimensions.TextColor; } set { Dimensions.TextColor = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.LineColor.")]
        public string EXTDIMColorLine { get { return Dimensions.LineColor; } set { Dimensions.LineColor = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.OffsetLineFromReferencePoint.")]
        public double EXTDIMOffsetLineFromRefPoint { get { return Dimensions.OffsetLineFromReferencePoint; } set { Dimensions.OffsetLineFromReferencePoint = value; } }

        [Obsolete("Use Dimensions.TextMove.")]
        public int EXTDIMTextMove { get { return Dimensions.TextMove; } set { Dimensions.TextMove = value; } }

        [Obsolete("Use Dimensions.OutsideAlign.")]
        public bool EXTDIMOutsideAlign { get { return Dimensions.OutsideAlign; } set { Dimensions.OutsideAlign = value; } }

        [Obsolete("Use Dimensions.Layer.")]
        public string EXTDIMlayer { get { return Dimensions.Layer; } set { Dimensions.Layer = value ?? string.Empty; } }

        [Obsolete("Use Text.DefaultStyleName.")]
        public string EXTTEXTStyleName { get { return Text.DefaultStyleName; } set { Text.DefaultStyleName = value ?? string.Empty; } }

        [Obsolete("Use Text.DefaultSize.")]
        public double EXTTEXTSize { get { return Text.DefaultSize; } set { Text.DefaultSize = value; } }

        [Obsolete("Use Dimensions.Enabled.")]
        public bool EXTDIMGERALHabilit { get { return Dimensions.Enabled; } set { Dimensions.Enabled = value; } }

        [Obsolete("Use Dimensions.ExtensionLineOffset.")]
        public double EXTDIMDIMEX { get { return Dimensions.ExtensionLineOffset; } set { Dimensions.ExtensionLineOffset = value; } }

        [Obsolete("Use Dimensions.BaseLayer.")]
        public string EXTDIMBaseLayer { get { return Dimensions.BaseLayer; } set { Dimensions.BaseLayer = value ?? string.Empty; } }

        [Obsolete("Use Layers.TeklaDrawingSheetLayer.")]
        public string LayerTeklaString { get { return Layers.TeklaDrawingSheetLayer; } set { Layers.TeklaDrawingSheetLayer = value ?? string.Empty; } }

        [Obsolete("Use Layers.BlockAttributeLayer.")]
        public string LayerBlockAttribute { get { return Layers.BlockAttributeLayer; } set { Layers.BlockAttributeLayer = value ?? string.Empty; } }

        [Obsolete("Use Scale.Manual.")]
        public bool EXTSCALEManual { get { return Scale.Manual; } set { Scale.Manual = value; } }

        [Obsolete("Use Scale.Point1.")]
        public PointEspecial EXTSCALEp1 { get { return scalePoint1; } set { SetScalePoint1(value); } }

        [Obsolete("Use Scale.Point2.")]
        public PointEspecial EXTSCALEp2 { get { return scalePoint2; } set { SetScalePoint2(value); } }

        [Obsolete("Use a dedicated automatic scale calibration model.")]
        public PointEspecial EXTSCALEAp1 { get { return automaticScalePoint1; } set { automaticScalePoint1 = value ?? new PointEspecial(0, 0, 0); } }

        [Obsolete("Use a dedicated automatic scale calibration model.")]
        public PointEspecial EXTSCALEAp2 { get { return automaticScalePoint2; } set { automaticScalePoint2 = value ?? new PointEspecial(0, 0, 0); } }

        [Obsolete("Use Scale.Layer.")]
        public string EXTSCALELayer { get { return Scale.Layer; } set { Scale.Layer = value ?? string.Empty; } }

        [Obsolete("Use Scale.TextSize.")]
        public double EXTSCALETextSize { get { return Scale.TextSize; } set { Scale.TextSize = value; } }

        [Obsolete("Use Scale.TextSize.")]
        public string EXTSCALETextSizeString { get; set; } = "2,5";

        [Obsolete("Use Comments.")]
        public string EXTCONFComments { get { return Comments; } set { Comments = value ?? string.Empty; } }

        [Obsolete("Use General.ConvertDimensions.")]
        public bool EXTCONFIsConvertDimension { get { return General.ConvertDimensions; } set { General.ConvertDimensions = value; } }

        [Obsolete("Use General.ConvertLayers.")]
        public bool EXTCONFIsConvertLayer { get { return General.ConvertLayers; } set { General.ConvertLayers = value; } }

        [Obsolete("Use General.ExchangeFormat.")]
        public bool EXTCONFIsExchangeFormat { get { return General.ExchangeFormat; } set { General.ExchangeFormat = value; } }

        [Obsolete("Use General.ExchangeLm.")]
        public bool EXTCONFIsExchangeLM { get { return General.ExchangeLm; } set { General.ExchangeLm = value; } }

        [Obsolete("Use General.ApplyDrawingScale.")]
        public bool EXTCONFIsPutOnTheScaleDrawing { get { return General.ApplyDrawingScale; } set { General.ApplyDrawingScale = value; } }

        [Obsolete("Use General.ExecuteLisp.")]
        public bool EXTCONFIsExecuteLISP { get { return General.ExecuteLisp; } set { General.ExecuteLisp = value; } }

        [Obsolete("Use General.ExecuteDll.")]
        public bool EXTCONFIsExecuteDLL { get { return General.ExecuteDll; } set { General.ExecuteDll = value; } }

        [Obsolete("Use General.DeleteTeklaStructures.")]
        public bool EXTCONFIsDeleteTeklaStructures { get { return General.DeleteTeklaStructures; } set { General.DeleteTeklaStructures = value; } }

        [Obsolete("Use General.FirstRunMode.")]
        public string EXTCONFIsFirstRum { get { return General.FirstRunMode; } set { General.FirstRunMode = value ?? string.Empty; } }

        [Obsolete("Use General.Purge.")]
        public bool EXTCONFIsPurge { get { return General.Purge; } set { General.Purge = value; } }

        [Obsolete("Use Lines.LineTypeScale.")]
        public double EXTLINELtscale { get { return Lines.LineTypeScale; } set { Lines.LineTypeScale = value; } }

        [Obsolete("Use Runtime.DbLineTypePath.")]
        public string PROGRAMDbLin { get { return Runtime.DbLineTypePath; } set { Runtime.DbLineTypePath = value ?? string.Empty; } }

        [Obsolete("Use General.ShowMessages.")]
        public bool PROGRAMMessage { get { return General.ShowMessages; } set { General.ShowMessages = value; } }

        [Obsolete("Use Dimensions.FixArrow.")]
        public bool EXTDIMCorrigeSeta { get { return Dimensions.FixArrow; } set { Dimensions.FixArrow = value; } }

        [Obsolete("Use Dimensions.FixArrowType.")]
        public string EXTDIMCorrigeSetaTipoSeta { get { return Dimensions.FixArrowType; } set { Dimensions.FixArrowType = value ?? string.Empty; } }

        [Obsolete("Use Dimensions.FixArrowFactor.")]
        public double EXTDIMCorrigeSetaFactor { get { return Dimensions.FixArrowFactor; } set { Dimensions.FixArrowFactor = value; } }

        [Obsolete("Use Blocks.TeklaBlockPath.")]
        public string PROGRAMblockFormatoCaminho { get { return Blocks.TeklaBlockPath; } set { Blocks.TeklaBlockPath = value ?? string.Empty; } }

        [Obsolete("Use General.SourceMode.")]
        public int EXTCONFOrigem { get { return General.SourceMode; } set { General.SourceMode = value; } }

        [Obsolete("Use General.InventorExplode.")]
        public bool EXTCONFInventorExplode { get { return General.InventorExplode; } set { General.InventorExplode = value; } }

        [Obsolete("Use General.ConverterType.")]
        public int ConvTekla0ConvInv1 { get { return General.ConverterType; } set { General.ConverterType = value; } }

        [Obsolete("Use Blocks.CadBlockPath.")]
        public string EXTCONFCaminhoBlocoInv { get { return Blocks.CadBlockPath; } set { Blocks.CadBlockPath = value ?? string.Empty; } }

        [Obsolete("Use General.ExplodeBlocks.")]
        public bool ExplodeBlocks { get { return General.ExplodeBlocks; } set { General.ExplodeBlocks = value; } }

        [Obsolete("Use GetTempDirectory.")]
        public string GetPROGRAMDirectoryTemp()
        {
            return GetTempDirectory();
        }

        public bool CheckFileTxmlExist(string file, StatusConversorItem statusConversorItem)
        {
            return ConfigurationRepository.Exists(file, statusConversorItem);
        }

        [Obsolete("Use ConverterFileService.LoadConverter or IConfigurationRepository.Load.")]
        public void Load(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            LoadXML(file, arranjos, blocks, blocosi, blocoso, statusConversorItem);
        }

        [Obsolete("Use ConverterFileService.LoadConverter or IConfigurationRepository.Load.")]
        public void LoadXML(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            Configuration configuration = ConfigurationRepository.Load(file, statusConversorItem);
            Apply(configuration);
            ConfigurationCompatibilityMapper.ApplyToLegacyState(configuration.ToConverterConfiguration(), this, arranjos, blocks, blocosi, blocoso);
        }

        [Obsolete("Use ConverterFileService.SaveConverter or IConfigurationRepository.Save.")]
        public void SaveXML(string file, Arranjos arranjos, List<Block> blocks, List<Block> blocosi, List<Block> blocoso, StatusConversorItem statusConversorItem)
        {
            Configuration configuration = ConfigurationCompatibilityMapper.FromLegacyState(this, arranjos, blocks, blocosi, blocoso);
            ConfigurationRepository.Save(file, statusConversorItem, configuration);
        }

        public static string LoadConfigDLL()
        {
            return DllConfigFile.Load();
        }

        public static void SaveConfigDLL()
        {
            DllConfigFile.SaveIfMissing();
        }

        partial void InitializeCompatibilityState()
        {
            referenceFormatSize = Dimensions.ReferenceFormatSize;
            scalePoint1 = ToPointEspecial(Scale.Point1);
            scalePoint2 = ToPointEspecial(Scale.Point2);
            syncedScalePoint1 = ToPointConfiguration(scalePoint1);
            syncedScalePoint2 = ToPointConfiguration(scalePoint2);
        }

        partial void SyncCompatibilityState()
        {
            SyncScalePoint(ref scalePoint1, ref syncedScalePoint1, value => Scale.Point1 = value, () => Scale.Point1);
            SyncScalePoint(ref scalePoint2, ref syncedScalePoint2, value => Scale.Point2 = value, () => Scale.Point2);
            Dimensions.ReferenceFormatSize = referenceFormatSize;
        }

        private void SetScalePoint1(PointEspecial value)
        {
            scalePoint1 = value ?? new PointEspecial(0, 0, 0);
            Scale.Point1 = ToPointConfiguration(scalePoint1);
            syncedScalePoint1 = ToPointConfiguration(scalePoint1);
        }

        private void SetScalePoint2(PointEspecial value)
        {
            scalePoint2 = value ?? new PointEspecial(0, 0, 0);
            Scale.Point2 = ToPointConfiguration(scalePoint2);
            syncedScalePoint2 = ToPointConfiguration(scalePoint2);
        }

        private static void SyncScalePoint(
            ref PointEspecial legacyPoint,
            ref Point3DConfiguration syncedPoint,
            Action<Point3DConfiguration> setModelPoint,
            Func<Point3DConfiguration> getModelPoint)
        {
            Point3DConfiguration currentLegacyPoint = ToPointConfiguration(legacyPoint);
            Point3DConfiguration currentModelPoint = getModelPoint() ?? new Point3DConfiguration();

            if (!PointsEqual(currentLegacyPoint, syncedPoint))
            {
                setModelPoint(currentLegacyPoint);
                syncedPoint = CopyPoint(currentLegacyPoint);
                return;
            }

            if (!PointsEqual(currentModelPoint, syncedPoint))
            {
                legacyPoint = ToPointEspecial(currentModelPoint);
                syncedPoint = CopyPoint(currentModelPoint);
            }
        }

        private static PointEspecial ToPointEspecial(Point3DConfiguration point)
        {
            point = point ?? new Point3DConfiguration();
            return new PointEspecial(point.X, point.Y, point.Z);
        }

        private static Point3DConfiguration ToPointConfiguration(PointEspecial point)
        {
            point = point ?? new PointEspecial(0, 0, 0);
            return new Point3DConfiguration { X = point.X, Y = point.Y, Z = point.Z };
        }

        private static Point3DConfiguration CopyPoint(Point3DConfiguration point)
        {
            point = point ?? new Point3DConfiguration();
            return new Point3DConfiguration { X = point.X, Y = point.Y, Z = point.Z };
        }

        private static bool PointsEqual(Point3DConfiguration first, Point3DConfiguration second)
        {
            first = first ?? new Point3DConfiguration();
            second = second ?? new Point3DConfiguration();
            return first.X == second.X && first.Y == second.Y && first.Z == second.Z;
        }
    }
}
