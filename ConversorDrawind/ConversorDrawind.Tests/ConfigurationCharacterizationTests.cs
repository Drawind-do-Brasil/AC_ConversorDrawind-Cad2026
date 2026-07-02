using System.Globalization;
using System.Text;
using System.Xml.Linq;
using ConversorDrawind;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ConversorDrawind.Tests;

public sealed class ConfigurationCharacterizationTests
{
    [Fact]
    public void Class_Configuration_LoadXML_DeveLerTodosOsCamposBasicos()
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

        var loaded = new Class_Configuration();
        var arranjos = new Class_Arranjos();
        var blocks = new List<Class_BlockClass>();
        var blocksInv = new List<Class_BlockClass>();
        var blocksOrig = new List<Class_BlockClass>();

        loaded.LoadXML("COMPLETE", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);

        Assert.Equal(ConfigurationSnapshot.Create(state.Configuration, state.Arranjos, state.Blocks, state.BlocksInv, state.BlocksOrig),
            ConfigurationSnapshot.Create(loaded, arranjos, blocks, blocksInv, blocksOrig));
    }

    [Fact]
    public void Class_Configuration_SaveXML_DeveGerarMesmoContratoXml()
    {
        using var workspace = TestWorkspace.Create();
        var state = ConfigurationFixture.CreatePopulatedState();

        state.Configuration.SaveXML("BASELINE", state.Arranjos, state.Blocks, state.BlocksInv, state.BlocksOrig, workspace.Status);

        var loaded = new Class_Configuration();
        var arranjos = new Class_Arranjos();
        var blocks = new List<Class_BlockClass>();
        var blocksInv = new List<Class_BlockClass>();
        var blocksOrig = new List<Class_BlockClass>();
        loaded.LoadXML("BASELINE", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);
        loaded.SaveXML("ROUNDTRIP", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);

        Assert.Equal(
            XmlAssert.Normalize(workspace.GetFile("BASELINE.txml")),
            XmlAssert.Normalize(workspace.GetFile("ROUNDTRIP.txml")));
    }

    [Fact]
    public void Class_Configuration_LoadTemplate_DeveMigrarParaTxmlSemPerderDados()
    {
        using var workspace = TestWorkspace.Create();
        File.WriteAllText(workspace.GetFile("LEGACY.Template"), ConfigurationFixture.LegacyTemplateText(), Encoding.UTF8);

        var configuration = new Class_Configuration();
        var arranjos = new Class_Arranjos();
        var blocks = new List<Class_BlockClass>();
        var blocksInv = new List<Class_BlockClass>();
        var blocksOrig = new List<Class_BlockClass>();

        configuration.Load("LEGACY", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);
        configuration.SaveXML("MIGRATED", arranjos, blocks, blocksInv, blocksOrig, workspace.Status);

        Assert.Equal("Comentario legado", configuration.EXTCONFComments);
        Assert.True(configuration.EXTCONFIsConvertDimension);
        Assert.False(configuration.EXTCONFIsConvertLayer);
        Assert.Equal("DIM_LEGACY", configuration.EXTDIMlayer);
        Assert.Equal("LAYER_BASE", arranjos.allBaseLayer.Single());
        Assert.Equal("CONTINUOUS", arranjos.allLineType1.Single());
        Assert.Equal("NEW_LAYER:WHITE:CONTINUOUS", arranjos.allNewLayerComposition.Single());
        Assert.Equal("OLD:NEW:TEXT:ALL:ALL::ALL", arranjos.conversor.Single());
        Assert.True(File.Exists(workspace.GetFile("MIGRATED.txml")));
    }

    [Fact]
    public void Class_Configuration_SaveConfigDLL_DeveCriarArquivoEsperadoNoTemp()
    {
        string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");
        string? backup = File.Exists(arquivo) ? File.ReadAllText(arquivo, Encoding.UTF8) : null;

        try
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }

            Class_Configuration.SaveConfigDLL();

            Assert.True(File.Exists(arquivo));
            var xml = XDocument.Load(arquivo);
            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");
            Assert.Equal(expected, xml.Root!.Element("configurations")!.Attribute("BlocoDM")!.Value);
            Assert.Equal(expected, Class_Configuration.LoadConfigDLL());
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
    public void Class_Configuration_CheckFileTxmlExist_DeveUsarPastaDoStatus()
    {
        using var workspace = TestWorkspace.Create();
        File.WriteAllText(workspace.GetFile("EXISTS.txml"), "<CONVERSOR />", Encoding.UTF8);

        var configuration = new Class_Configuration();

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
    public required Class_Configuration Configuration { get; init; }
    public required Class_Arranjos Arranjos { get; init; }
    public required List<Class_BlockClass> Blocks { get; init; }
    public required List<Class_BlockClass> BlocksInv { get; init; }
    public required List<Class_BlockClass> BlocksOrig { get; init; }

    public static ConfigurationFixture CreatePopulatedState()
    {
        var configuration = new Class_Configuration
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
            EXTSCALEMp1 = new Class_PointEspecial(1, 2, 3),
            EXTSCALEMp2 = new Class_PointEspecial(4, 5, 6),
            EXTSCALEAp1 = new Class_PointEspecial(7, 8, 9),
            EXTSCALEAp2 = new Class_PointEspecial(10, 11, 12),
            EXTSCALELayer = "SCALE_LAYER",
            EXTSCALETextSize = 4.5,
            EXTLINELtscale = 12.5,
            PROGRAMblockFormatoCaminho = @"C:\Formato\Tekla.dwg",
            EXTCONFCaminhoBlocoInv = @"C:\Formato\Cad.dwg",
            DMBlock = false
        };

        var arranjos = new Class_Arranjos();
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
        arranjos.conversor.Add("LAYER_A:NEW_A:TEXT:ALL:ALL::ALL");
        arranjos.listLISPCommand.Add("(command \"zoom\" \"e\")");
        arranjos.allExplodeLayers.AddRange(new[] { "EXPLODE_A", "EXPLODE_B" });

        var remove = new Class_Filter(arranjos) { layerBase = "REMOVE_BASE" };
        remove.SetConjunto("TEXT:RED:HIDDEN:ABC:2.5:ALL");
        arranjos.layerRemove.Add(remove);

        return new ConfigurationFixture
        {
            Configuration = configuration,
            Arranjos = arranjos,
            Blocks = new List<Class_BlockClass> { CreateBlock("BLOCK_A") },
            BlocksInv = new List<Class_BlockClass> { CreateRelatedBlock("BLOCK_INV", "BLOCK_ORIG", -65536, 0, true) },
            BlocksOrig = new List<Class_BlockClass> { CreateRelatedBlock("BLOCK_ORIG", "BLOCK_INV", -16776961, 1, false) }
        };
    }

    public static string LegacyTemplateText()
    {
        return string.Join(Environment.NewLine, new[]
        {
            "Coments:$Comentario legado",
            "EndComments:$",
            "ConfigBool:$True",
            "ConfigBool:$False",
            "ConfigBool:$True",
            "ConfigBool:$False",
            "ConfigBool:$True",
            "ConfigBool:$False",
            "ConfigBool:$True",
            "ConfigBool:$LISP",
            "ConfigBool:$True",
            "**********",
            "BaseLayer:$LAYER_BASE",
            "BaseLineType:$",
            "BaseLineType:$CONTINUOUS",
            "NewLayer:$",
            "NewLayer:$NEW_LAYER:WHITE:CONTINUOUS",
            "Converter:$",
            "Converter:$OLD:NEW:TEXT:ALL:ALL::ALL",
            "ConfDimension:$",
            "ConfDimension:$True",
            "ConfDimension:$DIM_LEGACY",
            "ConfDimension:$RED",
            "ConfDimension:$GREEN",
            "ConfDimension:$COTAS_LEGACY",
            "ConfDimension:$TEXTO_LEGACY",
            "ConfDimension:$Oblique",
            "ConfDimension:$1.5",
            "ConfDimension:$0",
            "ConfDimension:$1",
            "ConfDimension:$2",
            "ConfDimension:$2",
            "ConfDimension:$1.25",
            "ConfDimension:$0.5",
            "ConfDimension:$False",
            "ConfDimension:$1",
            "ConfDimension:$False",
            "ConfDimension:$True",
            "ConfDimension:$True",
            "ConfDimension:$1.25",
            "ConfDimension:$DIMENSION",
            "LineConf:$",
            "LineConf:$10",
            "ProgramConf:$",
            "ProgMessage:$True",
            "ScaleBlock:$",
            "ScaleConf:$True",
            "ScaleConf:$0;0;0",
            "ScaleConf:$1;1;0",
            "ScaleConf:$2;2;0",
            "ScaleConf:$3;3;0",
            "ScaleConf:$SCALE_LAYER",
            "ScaleConf:$2.5",
            "DeleteTeklaHeader:$",
            "DeleteTeklaStructures:$True",
            "BlockPathHeader:$",
            "BlockPath:$C:\\Formato\\Tekla.dwg",
            "BlockLayerRemove:$",
            "BlockLayerRemove:$REMOVE_BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL",
            "DLLCommand:$",
            "DLLCommand:$(command \"zoom\" \"e\")",
            "LISPCommand:$",
            "LISPCommand:$(command \"purge\" \"all\" \"\" \"n\")",
            "EndLISP:$",
            "NoBlocks:$",
            "ConfDimension:$False",
            "ConfDimension:$Oblique",
            "ConfDimension:$7.23"
        }) + Environment.NewLine;
    }

    private static Class_BlockClass CreateBlock(string name)
    {
        return new Class_BlockClass
        {
            blockName = name,
            listTags = new List<Class_TagBlockClass> { CreateTag(-1, false) }
        };
    }

    private static Class_BlockClass CreateRelatedBlock(string name, string relatedName, int argb, int index, bool isSociate)
    {
        return new Class_BlockClass
        {
            blockName = name,
            blockNameRelacao = relatedName,
            cor = System.Drawing.Color.FromArgb(argb),
            listTags = new List<Class_TagBlockClass> { CreateTag(index, isSociate) }
        };
    }

    private static Class_TagBlockClass CreateTag(int index, bool isSociate)
    {
        var tag = new Class_TagBlockClass();
        tag.SetConjunto("TAG_A@True@1.1,2.2,3.3;4.4,5.5,6.6@BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL@0.8");
        tag.indiceRelacao = index;
        tag.isSociate = isSociate;
        return tag;
    }
}

internal static class ConfigurationSnapshot
{
    public static string Create(
        Class_Configuration configuration,
        Class_Arranjos arranjos,
        IReadOnlyList<Class_BlockClass> blocks,
        IReadOnlyList<Class_BlockClass> blocksInv,
        IReadOnlyList<Class_BlockClass> blocksOrig)
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
            "EXTSCALEMp1=" + Point(configuration.EXTSCALEMp1),
            "EXTSCALEMp2=" + Point(configuration.EXTSCALEMp2),
            "EXTSCALEAp1=" + Point(configuration.EXTSCALEAp1),
            "EXTSCALEAp2=" + Point(configuration.EXTSCALEAp2),
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

    private static string Block(Class_BlockClass block)
    {
        return string.Join(",", new[]
        {
            block.blockName,
            block.blockNameRelacao,
            block.cor.ToArgb().ToString(CultureInfo.InvariantCulture),
            Join(block.listTags.Select(Tag))
        });
    }

    private static string Tag(Class_TagBlockClass tag)
    {
        return string.Join(",", new[]
        {
            tag.GetConjuntoString(),
            tag.indiceRelacao.ToString(CultureInfo.InvariantCulture),
            tag.isSociate.ToString()
        });
    }

    private static string Filter(Class_Filter filter)
    {
        return filter.layerBase + ";" + filter.GetConjunto();
    }

    private static string Point(Class_PointEspecial point)
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

internal static class XmlAssert
{
    public static string Normalize(string file)
    {
        return XDocument.Load(file).ToString(SaveOptions.DisableFormatting);
    }
}
