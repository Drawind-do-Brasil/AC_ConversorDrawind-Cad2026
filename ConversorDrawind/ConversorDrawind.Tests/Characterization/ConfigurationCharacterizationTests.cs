using System.Globalization;
using System.Xml.Linq;
using ConversorDrawind;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ConversorDrawind.Tests;

public sealed class ConfigurationCharacterizationTests
{
    [Fact]
    public void ConfigurationReader_DeveLerTodosOsCamposBasicos()
    {
        using var workspace = TestWorkspace.Create();
        Configuration expected = ConfigurationFixture.CreatePopulatedConfiguration();
        string file = workspace.GetFile("COMPLETE.txml");

        StructuredConfigurationXmlWriter.Save(file, expected);

        Configuration loaded = ConfigurationReader.Load(file);

        Assert.Equal(ConfigurationSnapshot.Create(expected), ConfigurationSnapshot.Create(loaded));
    }

    [Fact]
    public void StructuredWriter_DeveGerarContratoVersion2()
    {
        using var workspace = TestWorkspace.Create();
        Configuration expected = ConfigurationFixture.CreatePopulatedConfiguration();
        string baseline = workspace.GetFile("BASELINE.txml");
        string roundtrip = workspace.GetFile("ROUNDTRIP.txml");

        StructuredConfigurationXmlWriter.Save(baseline, expected);
        StructuredConfigurationXmlWriter.Save(roundtrip, ConfigurationReader.Load(baseline));

        var baselineXml = XDocument.Load(baseline);
        var roundtripXml = XDocument.Load(roundtrip);

        Assert.Equal("2", baselineXml.Root!.Attribute("VERSION")!.Value);
        Assert.Equal("2", roundtripXml.Root!.Attribute("VERSION")!.Value);
        Assert.NotNull(roundtripXml.Descendants("NEW_LAYER").Single(layer => layer.Attribute("NAME")!.Value == "NEW_A").Attribute("COLOR"));
        Assert.NotNull(roundtripXml.Descendants("CONVERSION_RULE").Single().Element("SOURCE"));
        Assert.NotNull(roundtripXml.Descendants("CONVERSION_RULE").Single().Element("TARGET"));
        Assert.Equal(
            ConfigurationSnapshot.Create(expected),
            ConfigurationSnapshot.Create(ConfigurationReader.Load(roundtrip)));
    }

    [Fact]
    public void ConfigurationReader_DeveLerTemplateAtivoReal()
    {
        const string source = @"C:\0_Programas\Converter Framework para NET\AC_ConversorDrawind\AC_ConversorDrawind-Cad2026\ConversorDrawind\ConversorDrawind\bin\Debug\TemplatesAtivos\AMG_003_A1.txml";
        using var workspace = TestWorkspace.Create();
        string file = workspace.GetFile("AMG_003_A1.txml");
        File.Copy(source, file, overwrite: true);

        Configuration configuration = ConfigurationReader.Load(file);

        Assert.Equal(0, configuration.General.SourceMode);
        Assert.True(configuration.General.ExchangeFormat);
        Assert.Equal("DWI-DIM", configuration.Dimensions.Layer);
        Assert.Equal("DWI-100", configuration.Text.DefaultStyleName);
        Assert.Contains(configuration.Layers.NewLayers, layer => layer.Name == "DWI01" && layer.Color == "RED" && layer.LineType == "CONTINUOUS");
        Assert.NotEmpty(configuration.Layers.BaseLayers);
        Assert.NotEmpty(configuration.Lines.BaseLineTypes);
    }

}

internal sealed class TestWorkspace : IDisposable
{
    private TestWorkspace(string folderName)
    {
        Root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
        Directory.CreateDirectory(Root);
        Status = new StatusConversorItem("Teste", folderName);
    }

    public string Root { get; }

    public StatusConversorItem Status { get; }

    public static TestWorkspace Create()
    {
        return new TestWorkspace("TestData_" + Guid.NewGuid().ToString("N"));
    }

    public string GetFile(string fileName)
    {
        return Path.Combine(Root, fileName);
    }

    public void Dispose()
    {
        if (!Directory.Exists(Root))
            return;

        for (int attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                Directory.Delete(Root, recursive: true);
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(50);
            }
        }
    }
}

internal static class ConfigurationFixture
{
    public static Configuration CreatePopulatedConfiguration()
    {
        var configuration = new Configuration
        {
            Comments = "Comentario de teste",
            General =
            {
                SourceMode = 1,
                ConverterType = 1,
                ConvertDimensions = true,
                ConvertLayers = false,
                ExchangeFormat = true,
                ApplyDrawingScale = true,
                ExecuteLisp = true,
                Purge = false,
                ShowMessages = false,
                DeleteTeklaStructures = false,
                ExplodeBlocks = true,
                InventorExplode = true
            },
            Dimensions =
            {
                Enabled = true,
                Layer = "DIM_TEST",
                LineColor = "RED",
                TextColor = "GREEN",
                StyleName = "COTAS_TESTE",
                ArrowType = "Oblique",
                Scale = 2.5,
                Precision = 2,
                AngularPrecision = 3,
                Unit = 4,
                AngularUnit = 5,
                ArrowSize = 1.75,
                OffsetLineFromReferencePoint = 0.25,
                OutsideAlign = true,
                TextVerticalPosition = 2,
                TextRelativeToDimensionLine = true,
                ForceTextInside = false,
                ForceDimensionLine = false,
                ExtensionLineOffset = 3.25,
                BaseLayer = "DIMENSION_BASE",
                FixArrow = true,
                FixArrowType = "Closed",
                FixArrowFactor = 8.5
            },
            Text =
            {
                DefaultStyleName = "TEXTO_TESTE"
            },
            Scale =
            {
                Manual = false,
                Point1 = new Point3DConfiguration { X = 7, Y = 8, Z = 9 },
                Point2 = new Point3DConfiguration { X = 10, Y = 11, Z = 12 },
                Layer = "SCALE_LAYER",
                TextSize = 4.5
            },
            Lines =
            {
                LineTypeScale = 12.5,
                BaseLineTypes = new List<string> { "CONTINUOUS", "HIDDEN" }
            },
            Layers =
            {
                TeklaDrawingSheetLayer = "TEKLA_LAYER",
                BlockAttributeLayer = "ATTR_LAYER",
                BaseLayers = new List<string> { "LAYER_A", "LAYER_B" },
                NewLayers = new List<LayerDefinition>
                {
                    new LayerDefinition { Name = "NEW_A", Color = "WHITE", LineType = "CONTINUOUS" },
                    new LayerDefinition { Name = "NEW_B", Color = "RED", LineType = "HIDDEN" }
                },
                ConversionRules = new List<LayerConversionRule>
                {
                    LegacyConfigurationParsers.ParseLayerConversionRule("LAYER_A;TEXT:ALL:ALL:::ALL;NEW_A:BYLAYER:BYLAYER:::TEXTO_TESTE")
                },
                ExplodeLayers = new List<string> { "EXPLODE_A", "EXPLODE_B" }
            },
            Catalogs =
            {
                FilterLineTypes = new List<string> { "CONTINUOUS", "HIDDEN" }
            },
            Blocks =
            {
                TeklaBlockPath = @"C:\Formato\Tekla.dwg",
                CadBlockPath = @"C:\Formato\Cad.dwg",
                DimensionBlockEnabled = false,
                TeklaBlocks = new List<BlockDefinition> { CreateBlock("BLOCK_A") },
                CadBlocks = new List<BlockDefinition> { CreateRelatedBlock("BLOCK_INV", "BLOCK_ORIG", -65536, 0, true) },
                OriginalBlocks = new List<BlockDefinition> { CreateRelatedBlock("BLOCK_ORIG", "BLOCK_INV", -16776961, 1, false) }
            },
            Commands =
            {
                LispCommands = new List<string> { "(command \"zoom\" \"e\")" }
            }
        };

        configuration.Text.Styles = new List<TextStyleDefinition>
        {
            LegacyConfigurationParsers.ParseTextStyleDefinition("TEXTO_TESTE:RomanS:false:false:2.5:1:0")
        };

        configuration.Layers.RemoveRules = new List<LayerRemoveRule>
        {
            LegacyConfigurationParsers.ParseLayerRemoveRule("REMOVE_BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL")
        };

        return configuration;
    }

    private static BlockDefinition CreateBlock(string name)
    {
        return new BlockDefinition
        {
            Name = name,
            Tags = new List<BlockTagDefinition> { CreateTag(-1, false) }
        };
    }

    private static BlockDefinition CreateRelatedBlock(string name, string relatedName, int argb, int index, bool isAssociated)
    {
        return new BlockDefinition
        {
            Name = name,
            RelatedName = relatedName,
            ColorArgb = argb,
            Tags = new List<BlockTagDefinition> { CreateTag(index, isAssociated) }
        };
    }

    private static BlockTagDefinition CreateTag(int index, bool isAssociated)
    {
        return new BlockTagDefinition
        {
            Name = "TAG_A",
            Modify = true,
            Point1 = new Point3DConfiguration { X = 1.1, Y = 2.2, Z = 3.3 },
            Point2 = new Point3DConfiguration { X = 4.4, Y = 5.5, Z = 6.6 },
            Filter = LegacyConfigurationParsers.ParseLayerRemoveRule("BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL").Filter,
            WidthFactor = 0.8,
            RelatedIndex = index,
            IsAssociated = isAssociated
        };
    }
}

internal static class ConfigurationSnapshot
{
    public static string Create(Configuration configuration)
    {
        var lines = new List<string>
        {
            "Comments=" + configuration.Comments,
            "SourceMode=" + configuration.General.SourceMode.ToString(CultureInfo.InvariantCulture),
            "ConvertDimensions=" + configuration.General.ConvertDimensions,
            "ConvertLayers=" + configuration.General.ConvertLayers,
            "ExchangeFormat=" + configuration.General.ExchangeFormat,
            "ApplyDrawingScale=" + configuration.General.ApplyDrawingScale,
            "ExecuteLisp=" + configuration.General.ExecuteLisp,
            "Purge=" + configuration.General.Purge,
            "ShowMessages=" + configuration.General.ShowMessages,
            "DeleteTeklaStructures=" + configuration.General.DeleteTeklaStructures,
            "ExplodeBlocks=" + configuration.General.ExplodeBlocks,
            "TeklaDrawingSheetLayer=" + configuration.Layers.TeklaDrawingSheetLayer,
            "BlockAttributeLayer=" + configuration.Layers.BlockAttributeLayer,
            "InventorExplode=" + configuration.General.InventorExplode,
            "DimensionLayer=" + configuration.Dimensions.Layer,
            "DimensionStyle=" + configuration.Dimensions.StyleName,
            "ScalePoint1=" + Point(configuration.Scale.Point1),
            "ScalePoint2=" + Point(configuration.Scale.Point2),
            "LineTypeScale=" + Format(configuration.Lines.LineTypeScale),
            "TeklaBlockPath=" + configuration.Blocks.TeklaBlockPath,
            "CadBlockPath=" + configuration.Blocks.CadBlockPath,
            "DimensionBlockEnabled=" + configuration.Blocks.DimensionBlockEnabled,
            "BaseLayers=" + Join(configuration.Layers.BaseLayers),
            "BaseLineTypes=" + Join(configuration.Lines.BaseLineTypes),
            "NewLayers=" + Join(configuration.Layers.NewLayers.Select(LegacyConfigurationParsers.FormatLayerDefinition)),
            "TextStyles=" + Join(configuration.Text.Styles.Select(LegacyConfigurationParsers.FormatTextStyleDefinition)),
            "ConversionRules=" + Join(configuration.Layers.ConversionRules.Select(LegacyConfigurationParsers.FormatLayerConversionRule)),
            "LispCommands=" + Join(configuration.Commands.LispCommands),
            "ExplodeLayers=" + Join(configuration.Layers.ExplodeLayers),
            "RemoveRules=" + Join(configuration.Layers.RemoveRules.Select(LegacyConfigurationParsers.FormatLayerRemoveRule)),
            "TeklaBlocks=" + Join(configuration.Blocks.TeklaBlocks.Select(Block)),
            "CadBlocks=" + Join(configuration.Blocks.CadBlocks.Select(Block)),
            "OriginalBlocks=" + Join(configuration.Blocks.OriginalBlocks.Select(Block))
        };

        return string.Join(Environment.NewLine, lines);
    }

    private static string Block(BlockDefinition block)
    {
        return string.Join(",", new[]
        {
            block.Name,
            block.RelatedName,
            block.ColorArgb.ToString(CultureInfo.InvariantCulture),
            Join(block.Tags.Select(Tag))
        });
    }

    private static string Tag(BlockTagDefinition tag)
    {
        return string.Join(",", new[]
        {
            tag.Name,
            tag.Modify.ToString(),
            Point(tag.Point1),
            Point(tag.Point2),
            LegacyConfigurationParsers.FormatEntityFilter(tag.Filter),
            Format(tag.WidthFactor),
            tag.RelatedIndex.ToString(CultureInfo.InvariantCulture),
            tag.IsAssociated.ToString()
        });
    }

    private static string Point(Point3DConfiguration point)
    {
        return string.Join(",", new[] { Format(point.X), Format(point.Y), Format(point.Z) });
    }

    private static string Join(IEnumerable<string> items)
    {
        return string.Join("|", items);
    }

    private static string Format(double value)
    {
        return value.ToString("G17", CultureInfo.InvariantCulture);
    }
}
