using System.Globalization;
using System.Text;
using System.Xml.Linq;
using ConversorDrawind;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ConversorDrawind.Tests;

public sealed class ConfigurationCharacterizationTests
{
    [Fact]
    public void Configuration_LoadXML_DeveLerTodosOsCamposBasicos()
    {
        using var workspace = TestWorkspace.Create();
        var state = ConfigurationFixture.CreatePopulatedState();

        state.Configuration.SaveXML(
            "COMPLETE",
            state.Arranjos,
            state.Blocks,
            state.BlocksInv,
            state.BlocksOrig,
            workspace.Status);

        var loaded = new Configuration();
        var arranjos = new Arranjos();
        var blocks = new List<Block>();
        var blocksInv = new List<Block>();
        var blocksOrig = new List<Block>();

        loaded.LoadXML("COMPLETE", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);

        Assert.Equal(ConfigurationSnapshot.Create(state.Configuration, state.Arranjos, state.Blocks, state.BlocksInv, state.BlocksOrig),
            ConfigurationSnapshot.Create(loaded, arranjos, blocks, blocksInv, blocksOrig));
    }

    [Fact]
    public void Configuration_SaveXML_DeveGerarContratoEstruturadoVersion2()
    {
        using var workspace = TestWorkspace.Create();
        var state = ConfigurationFixture.CreatePopulatedState();

        state.Configuration.SaveXML("BASELINE", state.Arranjos, state.Blocks, state.BlocksInv, state.BlocksOrig, workspace.Status);

        var loaded = new Configuration();
        var arranjos = new Arranjos();
        var blocks = new List<Block>();
        var blocksInv = new List<Block>();
        var blocksOrig = new List<Block>();
        loaded.LoadXML("BASELINE", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);
        loaded.SaveXML("ROUNDTRIP", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);

        var baselineXml = XDocument.Load(workspace.GetFile("BASELINE.txml"));
        var roundtripXml = XDocument.Load(workspace.GetFile("ROUNDTRIP.txml"));

        Assert.Equal("2", baselineXml.Root!.Attribute("VERSION")!.Value);
        Assert.Equal("2", roundtripXml.Root!.Attribute("VERSION")!.Value);
        Assert.NotNull(roundtripXml.Descendants("NEW_LAYER").Single(layer => layer.Attribute("NAME")!.Value == "NEW_A").Attribute("COLOR"));
        Assert.NotNull(roundtripXml.Descendants("CONVERSION_RULE").Single().Element("SOURCE"));
        Assert.NotNull(roundtripXml.Descendants("CONVERSION_RULE").Single().Element("TARGET"));

        Assert.Equal(
            ConfigurationSnapshot.Create(state.Configuration, state.Arranjos, state.Blocks, state.BlocksInv, state.BlocksOrig),
            ConfigurationSnapshot.Create(loaded, arranjos, blocks, blocksInv, blocksOrig));
    }

    [Fact]
    public void Configuration_LoadXML_DeveLerTemplateAtivoReal()
    {
        const string source = @"C:\0_Programas\Converter Framework para NET\AC_ConversorDrawind\AC_ConversorDrawind-Cad2026\ConversorDrawind\ConversorDrawind\bin\Debug\TemplatesAtivos\AMG_003_A1.txml";
        using var workspace = TestWorkspace.Create();
        File.Copy(source, workspace.GetFile("AMG_003_A1.txml"), overwrite: true);

        var configuration = new Configuration();
        var arranjos = new Arranjos();
        var blocks = new List<Block>();
        var blocksInv = new List<Block>();
        var blocksOrig = new List<Block>();

        configuration.LoadXML("AMG_003_A1", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);

        Assert.Equal(0, configuration.EXTCONFOrigem);
        Assert.True(configuration.EXTCONFIsExchangeFormat);
        Assert.Equal("DWI-DIM", configuration.EXTDIMlayer);
        Assert.Equal("DWI-100", configuration.EXTTEXTStyleName);
        Assert.Contains("DWI01:RED:CONTINUOUS", arranjos.allNewLayerComposition);
        Assert.NotEmpty(arranjos.allBaseLayer);
        Assert.NotEmpty(arranjos.allLineType1);
    }

    [Fact]
    public void Configuration_SaveConfigDLL_DeveCriarArquivoEsperadoNoTemp()
    {
        string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");
        string? backup = File.Exists(arquivo) ? File.ReadAllText(arquivo, Encoding.UTF8) : null;

        try
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }

            Configuration.SaveConfigDLL();

            Assert.True(File.Exists(arquivo));
            var xml = XDocument.Load(arquivo);
            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");
            Assert.Equal(expected, xml.Root!.Element("configurations")!.Attribute("BlocoDM")!.Value);
            Assert.Equal(expected, Configuration.LoadConfigDLL());
        }
        finally
        {
            if (backup is null)
            {
                if (File.Exists(arquivo))
                {
                    File.Delete(arquivo);
                }
            }
            else
            {
                File.WriteAllText(arquivo, backup, Encoding.UTF8);
            }
        }
    }

    [Fact]
    public void Configuration_CheckFileTxmlExist_DeveUsarPastaDoStatus()
    {
        using var workspace = TestWorkspace.Create();
        File.WriteAllText(workspace.GetFile("EXISTS.txml"), "<CONVERSOR />", Encoding.UTF8);

        var configuration = new Configuration();

        Assert.True(configuration.CheckFileTxmlExist("EXISTS", workspace.Status));
        Assert.False(configuration.CheckFileTxmlExist("MISSING", workspace.Status));
    }
}

internal sealed class TestWorkspace : IDisposable
{
    private TestWorkspace(string folderName)
    {
        FolderName = folderName;
        Root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
        Directory.CreateDirectory(Root);
        Status = new StatusConversorItem("Tests", folderName);
    }

    public string FolderName { get; }
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
        string baseDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        string root = Path.GetFullPath(Root);

        if (root.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase) && Directory.Exists(root))
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    Directory.Delete(root, recursive: true);
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
}

internal sealed class ConfigurationFixture
{
    public required Configuration Configuration { get; init; }
    public required Arranjos Arranjos { get; init; }
    public required List<Block> Blocks { get; init; }
    public required List<Block> BlocksInv { get; init; }
    public required List<Block> BlocksOrig { get; init; }

    public static ConfigurationFixture CreatePopulatedState()
    {
        var configuration = new Configuration
        {
            EXTCONFComments = "Comentario de teste",
            EXTCONFOrigem = 1,
            EXTCONFIsConvertDimension = true,
            EXTCONFIsConvertLayer = false,
            EXTCONFIsExchangeFormat = true,
            EXTCONFIsPutOnTheScaleDrawing = true,
            EXTCONFIsExecuteLISP = true,
            EXTCONFIsPurge = false,
            PROGRAMMessage = false,
            EXTCONFIsDeleteTeklaStructures = false,
            ExplodeBlocks = true,
            LayerTeklaString = "TEKLA_LAYER",
            LayerBlockAttribute = "ATTR_LAYER",
            EXTCONFInventorExplode = true,
            EXTDIMGERALHabilit = true,
            EXTDIMlayer = "DIM_TEST",
            EXTDIMColorLine = "RED",
            EXTDIMColorText = "GREEN",
            EXTDIMStyleName = "COTAS_TESTE",
            EXTDIMSeta = "Oblique",
            EXTDIMScale = 2.5,
            EXTDIMPrecision = 2,
            EXTDIMAngularPrecision = 3,
            EXTDIMUnit = 4,
            EXTDIMAngularUnit = 5,
            EXTDIMSizeSeta = 1.75,
            EXTDIMOffsetLineFromRefPoint = 0.25,
            EXTDIMOutsideAlign = true,
            EXTDIMTad = 2,
            EXTDIMDimensionPosition = true,
            EXTDIMTextForced = false,
            EXTDIMLineForced = false,
            EXTDIMDIMEX = 3.25,
            EXTDIMBaseLayer = "DIMENSION_BASE",
            EXTDIMCorrigeSeta = true,
            EXTDIMCorrigeSetaTipoSeta = "Closed",
            EXTDIMCorrigeSetaFactor = 8.5,
            EXTTEXTStyleName = "TEXTO_TESTE",
            EXTSCALEManual = false,
            EXTSCALEp1 = new PointEspecial(7, 8, 9),
            EXTSCALEp2 = new PointEspecial(10, 11, 12),
            EXTSCALELayer = "SCALE_LAYER",
            EXTSCALETextSize = 4.5,
            EXTLINELtscale = 12.5,
            PROGRAMblockFormatoCaminho = @"C:\Formato\Tekla.dwg",
            EXTCONFCaminhoBlocoInv = @"C:\Formato\Cad.dwg",
            DMBlock = false
        };

        var arranjos = new Arranjos();
        arranjos.allBaseLayer.Clear();
        arranjos.allLineType1.Clear();
        arranjos.allNewLayer.Clear();
        arranjos.allNewLayerComposition.Clear();
        arranjos.allTextSyles.Clear();
        arranjos.conversor.Clear();
        arranjos.layerRemove.Clear();
        arranjos.listLISPCommand.Clear();
        arranjos.allExplodeLayers.Clear();

        arranjos.allBaseLayer.AddRange(new[] { "LAYER_A", "LAYER_B" });
        arranjos.allLineType1.AddRange(new[] { "CONTINUOUS", "HIDDEN" });
        arranjos.allNewLayerComposition.AddRange(new[] { "NEW_A:WHITE:CONTINUOUS", "NEW_B:RED:HIDDEN" });
        arranjos.allNewLayer.AddRange(new[] { "NEW_A", "NEW_B" });
        arranjos.allTextSyles.AddRange(new[] { "TEXTO_TESTE:RomanS:false:false:2.5:1:0" });
        arranjos.conversor.Add("LAYER_A;TEXT:ALL:ALL:::ALL;NEW_A:BYLAYER:BYLAYER:::TEXTO_TESTE");
        arranjos.listLISPCommand.Add("(command \"zoom\" \"e\")");
        arranjos.allExplodeLayers.AddRange(new[] { "EXPLODE_A", "EXPLODE_B" });

        var remove = new Filter(arranjos) { layerBase = "REMOVE_BASE" };
        remove.SetConjunto("TEXT:RED:HIDDEN:ABC:2.5:ALL");
        arranjos.layerRemove.Add(remove);

        return new ConfigurationFixture
        {
            Configuration = configuration,
            Arranjos = arranjos,
            Blocks = new List<Block> { CreateBlock("BLOCK_A") },
            BlocksInv = new List<Block> { CreateRelatedBlock("BLOCK_INV", "BLOCK_ORIG", -65536, 0, true) },
            BlocksOrig = new List<Block> { CreateRelatedBlock("BLOCK_ORIG", "BLOCK_INV", -16776961, 1, false) }
        };
    }
    private static Block CreateBlock(string name)
    {
        return new Block
        {
            blockName = name,
            listTags = new List<TagBlock> { CreateTag(-1, false) }
        };
    }

    private static Block CreateRelatedBlock(string name, string relatedName, int argb, int index, bool isSociate)
    {
        return new Block
        {
            blockName = name,
            blockNameRelacao = relatedName,
            cor = System.Drawing.Color.FromArgb(argb),
            listTags = new List<TagBlock> { CreateTag(index, isSociate) }
        };
    }

    private static TagBlock CreateTag(int index, bool isSociate)
    {
        var tag = new TagBlock();
        tag.SetConjunto("TAG_A@True@1.1,2.2,3.3;4.4,5.5,6.6@BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL@0.8");
        tag.indiceRelacao = index;
        tag.isSociate = isSociate;
        return tag;
    }
}

internal static class ConfigurationSnapshot
{
    public static string Create(
        Configuration configuration,
        Arranjos arranjos,
        IReadOnlyList<Block> blocks,
        IReadOnlyList<Block> blocksInv,
        IReadOnlyList<Block> blocksOrig)
    {
        var lines = new List<string>
        {
            "EXTCONFComments=" + configuration.EXTCONFComments,
            "EXTCONFOrigem=" + configuration.EXTCONFOrigem.ToString(CultureInfo.InvariantCulture),
            "EXTCONFIsConvertDimension=" + configuration.EXTCONFIsConvertDimension,
            "EXTCONFIsConvertLayer=" + configuration.EXTCONFIsConvertLayer,
            "EXTCONFIsExchangeFormat=" + configuration.EXTCONFIsExchangeFormat,
            "EXTCONFIsPutOnTheScaleDrawing=" + configuration.EXTCONFIsPutOnTheScaleDrawing,
            "EXTCONFIsExecuteLISP=" + configuration.EXTCONFIsExecuteLISP,
            "EXTCONFIsPurge=" + configuration.EXTCONFIsPurge,
            "PROGRAMMessage=" + configuration.PROGRAMMessage,
            "EXTCONFIsDeleteTeklaStructures=" + configuration.EXTCONFIsDeleteTeklaStructures,
            "ExplodeBlocks=" + configuration.ExplodeBlocks,
            "LayerTeklaString=" + configuration.LayerTeklaString,
            "LayerBlockAttribute=" + configuration.LayerBlockAttribute,
            "EXTCONFInventorExplode=" + configuration.EXTCONFInventorExplode,
            "EXTDIMGERALHabilit=" + configuration.EXTDIMGERALHabilit,
            "EXTDIMlayer=" + configuration.EXTDIMlayer,
            "EXTDIMColorLine=" + configuration.EXTDIMColorLine,
            "EXTDIMColorText=" + configuration.EXTDIMColorText,
            "EXTDIMStyleName=" + configuration.EXTDIMStyleName,
            "EXTDIMSeta=" + configuration.EXTDIMSeta,
            "EXTDIMScale=" + Format(configuration.EXTDIMScale),
            "EXTDIMPrecision=" + configuration.EXTDIMPrecision.ToString(CultureInfo.InvariantCulture),
            "EXTDIMAngularPrecision=" + configuration.EXTDIMAngularPrecision.ToString(CultureInfo.InvariantCulture),
            "EXTDIMUnit=" + configuration.EXTDIMUnit.ToString(CultureInfo.InvariantCulture),
            "EXTDIMAngularUnit=" + configuration.EXTDIMAngularUnit.ToString(CultureInfo.InvariantCulture),
            "EXTDIMSizeSeta=" + Format(configuration.EXTDIMSizeSeta),
            "EXTDIMOffsetLineFromRefPoint=" + Format(configuration.EXTDIMOffsetLineFromRefPoint),
            "EXTDIMOutsideAlign=" + configuration.EXTDIMOutsideAlign,
            "EXTDIMTad=" + configuration.EXTDIMTad.ToString(CultureInfo.InvariantCulture),
            "EXTDIMDimensionPosition=" + configuration.EXTDIMDimensionPosition,
            "EXTDIMTextForced=" + configuration.EXTDIMTextForced,
            "EXTDIMLineForced=" + configuration.EXTDIMLineForced,
            "EXTDIMDIMEX=" + Format(configuration.EXTDIMDIMEX),
            "EXTDIMBaseLayer=" + configuration.EXTDIMBaseLayer,
            "EXTDIMCorrigeSeta=" + configuration.EXTDIMCorrigeSeta,
            "EXTDIMCorrigeSetaTipoSeta=" + configuration.EXTDIMCorrigeSetaTipoSeta,
            "EXTDIMCorrigeSetaFactor=" + Format(configuration.EXTDIMCorrigeSetaFactor),
            "EXTTEXTStyleName=" + configuration.EXTTEXTStyleName,
            "EXTSCALEManual=" + configuration.EXTSCALEManual,
            "EXTSCALEp1=" + Point(configuration.EXTSCALEp1),
            "EXTSCALEp2=" + Point(configuration.EXTSCALEp2),
            "EXTSCALELayer=" + configuration.EXTSCALELayer,
            "EXTSCALETextSize=" + Format(configuration.EXTSCALETextSize),
            "EXTLINELtscale=" + Format(configuration.EXTLINELtscale),
            "PROGRAMblockFormatoCaminho=" + configuration.PROGRAMblockFormatoCaminho,
            "EXTCONFCaminhoBlocoInv=" + configuration.EXTCONFCaminhoBlocoInv,
            "DMBlock=" + configuration.DMBlock,
            "allBaseLayer=" + Join(arranjos.allBaseLayer),
            "allLineType1=" + Join(arranjos.allLineType1),
            "allNewLayer=" + Join(arranjos.allNewLayer),
            "allNewLayerComposition=" + Join(arranjos.allNewLayerComposition),
            "allTextSyles=" + Join(arranjos.allTextSyles),
            "conversor=" + Join(arranjos.conversor),
            "listLISPCommand=" + Join(arranjos.listLISPCommand),
            "allExplodeLayers=" + Join(arranjos.allExplodeLayers),
            "layerRemove=" + Join(arranjos.layerRemove.Select(Filter)),
            "blocks=" + Join(blocks.Select(Block)),
            "blocksInv=" + Join(blocksInv.Select(Block)),
            "blocksOrig=" + Join(blocksOrig.Select(Block))
        };

        return string.Join(Environment.NewLine, lines);
    }

    private static string Block(Block block)
    {
        return string.Join(",", new[]
        {
            block.blockName,
            block.blockNameRelacao,
            block.cor.ToArgb().ToString(CultureInfo.InvariantCulture),
            Join(block.listTags.Select(Tag))
        });
    }

    private static string Tag(TagBlock tag)
    {
        return string.Join(",", new[]
        {
            tag.GetConjuntoString(),
            tag.indiceRelacao.ToString(CultureInfo.InvariantCulture),
            tag.isSociate.ToString()
        });
    }

    private static string Filter(Filter filter)
    {
        return filter.layerBase + ";" + filter.GetConjunto();
    }

    private static string Point(PointEspecial point)
    {
        return Format(point.X) + ";" + Format(point.Y) + ";" + Format(point.Z);
    }

    private static string Join(IEnumerable<string> values)
    {
        return string.Join("|", values);
    }

    private static string Format(double value)
    {
        return value.ToString("R", CultureInfo.InvariantCulture);
    }
}


