using System;
using System.Collections.Generic;

namespace ConversorDrawind
{
    [Obsolete("Use Configuration. ConverterConfiguration remains only as a compatibility alias.")]
    public sealed class ConverterConfiguration : Configuration
    {
        public ConverterConfiguration()
        {
        }

        public ConverterConfiguration(Configuration source)
            : base(source)
        {
        }
    }

    public sealed class GeneralConfiguration
    {
        public int SourceMode { get; set; }
        public bool ConvertDimensions { get; set; } = true;
        public bool ConvertLayers { get; set; } = true;
        public bool ExchangeFormat { get; set; }
        public bool ExchangeLm { get; set; }
        public bool ApplyDrawingScale { get; set; }
        public bool ExecuteLisp { get; set; }
        public bool ExecuteDll { get; set; }
        public string FirstRunMode { get; set; } = "LISP";
        public bool DeleteTeklaStructures { get; set; } = true;
        public bool Purge { get; set; } = true;
        public bool ShowMessages { get; set; } = true;
        public bool ExplodeBlocks { get; set; }
        public bool InventorExplode { get; set; }
        public int ConverterType { get; set; }
    }

    public sealed class DimensionConfiguration
    {
        public double ReferenceFormatSize { get; set; } = 841;
        public double InternalLengthCharFactor { get; set; } = 1.5;
        public double InternalTextOffset { get; set; } = 0.6100;
        public bool Enabled { get; set; } = true;
        public string Layer { get; set; } = "0";
        public string BaseLayer { get; set; } = "DIMENSION";
        public string LineColor { get; set; } = "RED";
        public string TextColor { get; set; } = "GREEN";
        public string StyleName { get; set; } = "COTAS";
        public string ArrowType { get; set; } = "Oblique";
        public string ArrowType1 { get; set; } = "Oblique";
        public string ArrowType2 { get; set; } = "Oblique";
        public double Scale { get; set; } = 1;
        public int Precision { get; set; }
        public int AngularPrecision { get; set; } = 1;
        public int Unit { get; set; } = 2;
        public int AngularUnit { get; set; } = 2;
        public double ArrowSize { get; set; } = 1.25;
        public int TextVerticalPosition { get; set; } = 1;
        public bool TextRelativeToDimensionLine { get; set; }
        public bool ForceTextInside { get; set; } = true;
        public bool ForceDimensionLine { get; set; } = true;
        public double OffsetLineFromReferencePoint { get; set; }
        public int TextMove { get; set; } = 2;
        public bool OutsideAlign { get; set; }
        public double ExtensionLineOffset { get; set; } = 1.25;
        public bool FixArrow { get; set; }
        public string FixArrowType { get; set; } = "Oblique";
        public double FixArrowFactor { get; set; } = 7.23;
    }

    public sealed class TextConfiguration
    {
        public string DefaultStyleName { get; set; } = "TEXTO";
        public double DefaultSize { get; set; } = 2.5;
        public List<TextStyleDefinition> Styles { get; set; } = new List<TextStyleDefinition>();
    }

    public sealed class ScaleConfiguration
    {
        public bool Manual { get; set; } = true;
        public Point3DConfiguration Point1 { get; set; } = new Point3DConfiguration();
        public Point3DConfiguration Point2 { get; set; } = new Point3DConfiguration();
        public string Layer { get; set; } = "0";
        public double TextSize { get; set; } = 2.5;
    }

    public sealed class LayerConfiguration
    {
        public string TeklaDrawingSheetLayer { get; set; } = "DRAWING SHEET";
        public string BlockAttributeLayer { get; set; } = "OTHER OBJECT TYPE";
        public List<string> BaseLayers { get; set; } = new List<string>();
        public List<LayerDefinition> NewLayers { get; set; } = new List<LayerDefinition>();
        public List<LayerRemoveRule> RemoveRules { get; set; } = new List<LayerRemoveRule>();
        public List<LayerConversionRule> ConversionRules { get; set; } = new List<LayerConversionRule>();
        public List<string> ExplodeLayers { get; set; } = new List<string>();
    }

    public sealed class LineConfiguration
    {
        public double LineTypeScale { get; set; } = 10;
        public List<string> BaseLineTypes { get; set; } = new List<string>();
    }

    public sealed class CatalogConfiguration
    {
        public List<string> Colors { get; set; } = new List<string>();
        public List<string> ObjectTypes { get; set; } = new List<string>();
        public List<string> FilterLineTypes { get; set; } = new List<string>();
        public List<string> LayerLineTypes { get; set; } = new List<string>();
        public List<string> RemovedLineTypes { get; set; } = new List<string>();
    }

    public sealed class CommandConfiguration
    {
        public List<string> LispCommands { get; set; } = new List<string>();
        public List<string> DllCommands { get; set; } = new List<string>();
    }

    public sealed class BlockConfiguration
    {
        public string TeklaBlockPath { get; set; } = " ";
        public string CadBlockPath { get; set; } = string.Empty;
        public bool DimensionBlockEnabled { get; set; } = true;
        public List<BlockDefinition> TeklaBlocks { get; set; } = new List<BlockDefinition>();
        public List<BlockDefinition> CadBlocks { get; set; } = new List<BlockDefinition>();
        public List<BlockDefinition> OriginalBlocks { get; set; } = new List<BlockDefinition>();
    }

    public sealed class RuntimeConfiguration
    {
        public string DbLineTypePath { get; set; } = string.Empty;
        public string TempDirectory { get; set; } = string.Empty;
    }

    public sealed class Point3DConfiguration
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public sealed class LayerDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string LineType { get; set; } = string.Empty;
    }

    public sealed class TextStyleDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Font { get; set; } = string.Empty;
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public double Size { get; set; }
        public double WidthFactor { get; set; } = 1;
        public double ObliqueAngle { get; set; }
    }

    public sealed class EntityFilter
    {
        public string BaseLayer { get; set; } = "ALL";
        public string ObjectType { get; set; } = "ALL";
        public string Color { get; set; } = "ALL";
        public string LineType { get; set; } = "ALL";
        public string TextContent { get; set; } = string.Empty;
        public string TextHeight { get; set; } = string.Empty;
        public string Orientation { get; set; } = "ALL";
    }

    public sealed class LayerRemoveRule
    {
        public EntityFilter Filter { get; set; } = new EntityFilter();
    }

    public sealed class LayerConversionRule
    {
        public EntityFilter Source { get; set; } = new EntityFilter();
        public LayerOutput Target { get; set; } = new LayerOutput();
    }

    public sealed class LayerOutput
    {
        public string LayerName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string LineType { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
        public string TextHeight { get; set; } = string.Empty;
        public string TextStyle { get; set; } = string.Empty;
    }

    public sealed class BlockDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string RelatedName { get; set; } = string.Empty;
        public int ColorArgb { get; set; }
        public List<BlockTagDefinition> Tags { get; set; } = new List<BlockTagDefinition>();
    }

    public sealed class BlockTagDefinition
    {
        public string Name { get; set; } = string.Empty;
        public bool Modify { get; set; }
        public Point3DConfiguration Point1 { get; set; } = new Point3DConfiguration();
        public Point3DConfiguration Point2 { get; set; } = new Point3DConfiguration();
        public EntityFilter Filter { get; set; } = new EntityFilter();
        public double WidthFactor { get; set; } = 1;
        public int RelatedIndex { get; set; } = -1;
        public bool IsAssociated { get; set; }
    }
}
