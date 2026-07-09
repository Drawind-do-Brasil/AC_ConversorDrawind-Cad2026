namespace ConversorDrawindDLL.Tests;

public sealed class SharedConverterConfigurationTests
{
    private const string TemplatesAtivosPath =
        @"C:\0_Programas\Converter Framework para NET\AC_ConversorDrawind\AC_ConversorDrawind-Cad2026\ConversorDrawind\ConversorDrawind\bin\Debug\TemplatesAtivos";

    [Fact]
    public void LegacyReader_DeveCarregarTodosTemplatesAtivos()
    {
        string[] files = Directory.GetFiles(TemplatesAtivosPath, "*.txml");

        Assert.NotEmpty(files);

        foreach (string file in files)
        {
            global::ConversorDrawind.Configuration configuration =
                global::ConversorDrawind.ConverterConfigurationReader.Load(file);

            Assert.NotNull(configuration);
            Assert.NotNull(configuration.Layers);
            Assert.NotNull(configuration.Blocks);
        }
    }

    [Fact]
    public void StructuredWriter_DeveSalvarVersion2ELerNovamente()
    {
        string source = Path.Combine(TemplatesAtivosPath, "GERDAU_DWI-024.txml");
        global::ConversorDrawind.Configuration original =
            global::ConversorDrawind.ConverterConfigurationReader.Load(source);

        using TestWorkspace workspace = TestWorkspace.Create();
        string output = workspace.GetFile("structured.txml");

        global::ConversorDrawind.StructuredConfigurationXmlWriter.Save(output, original);

        XDocument saved = XDocument.Load(output);
        Assert.Equal("2", (string?)saved.Root?.Attribute("VERSION"));
        Assert.Null(saved.Descendants("NEW_LAYER").FirstOrDefault()?.Attribute("NOME"));
        Assert.All(saved.Descendants("NEW_LAYER"), item =>
        {
            Assert.NotNull(item.Attribute("NAME"));
            Assert.NotNull(item.Attribute("COLOR"));
            Assert.NotNull(item.Attribute("LINE_TYPE"));
            Assert.True(string.IsNullOrWhiteSpace(item.Value));
        });
        Assert.All(saved.Descendants("TAG"), item =>
        {
            Assert.NotNull(item.Element("POINT1"));
            Assert.NotNull(item.Element("POINT2"));
            Assert.NotNull(item.Element("FILTER"));
            Assert.True(string.IsNullOrWhiteSpace(item.Value));
        });

        global::ConversorDrawind.Configuration roundtrip =
            global::ConversorDrawind.ConverterConfigurationReader.Load(output);

        Assert.Equal(original.Layers.NewLayers.Count, roundtrip.Layers.NewLayers.Count);
        Assert.Equal(original.Layers.RemoveRules.Count, roundtrip.Layers.RemoveRules.Count);
        Assert.Equal(original.Layers.ConversionRules.Count, roundtrip.Layers.ConversionRules.Count);
        Assert.Equal(original.Blocks.TeklaBlocks.Count, roundtrip.Blocks.TeklaBlocks.Count);
        Assert.Equal(original.Blocks.CadBlocks.Count, roundtrip.Blocks.CadBlocks.Count);
        Assert.Equal(original.Blocks.OriginalBlocks.Count, roundtrip.Blocks.OriginalBlocks.Count);
        Assert.Equal(original.Commands.LispCommands.Count, roundtrip.Commands.LispCommands.Count);
        Assert.Equal(original.Text.Styles.Count, roundtrip.Text.Styles.Count);
    }

    [Fact]
    public void ConfigurationSaveXml_DeveUsarFormatoEstruturadoVersion2()
    {
        using TestWorkspace workspace = TestWorkspace.Create();
        var status = new global::ConversorDrawind.StatusConversorItem("Teste", Path.GetFileName(workspace.Root));
        var configuration = new global::ConversorDrawind.Configuration();
        var arranjos = new global::ConversorDrawind.Arranjos();
        var blocks = new List<global::ConversorDrawind.Block>();
        var cadBlocks = new List<global::ConversorDrawind.Block>();
        var originalBlocks = new List<global::ConversorDrawind.Block>();

        arranjos.allNewLayerComposition.Clear();
        arranjos.allNewLayerComposition.Add("DWI01:RED:CONTINUOUS");
        arranjos.conversor.Add("ALL;TEXT:ALL:ALL:::ALL;DWI01:BYLAYER:BYLAYER:::TEXTO");

        configuration.SaveXML("sample", arranjos, blocks, cadBlocks, originalBlocks, status);

        string output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, status.Pasta, "sample.txml");
        XDocument saved = XDocument.Load(output);

        Assert.Equal("2", (string?)saved.Root?.Attribute("VERSION"));
        Assert.NotNull(saved.Descendants("CONVERSION_RULE").Single().Element("SOURCE"));
        Assert.NotNull(saved.Descendants("CONVERSION_RULE").Single().Element("TARGET"));
    }

    [Fact]
    public void LegacyParsers_DeveConverterStringsCompostasParaObjetos()
    {
        global::ConversorDrawind.LayerDefinition layer =
            global::ConversorDrawind.LegacyConfigurationParsers.ParseLayerDefinition("0:WHITE:CONTINUOUS");
        global::ConversorDrawind.TextStyleDefinition style =
            global::ConversorDrawind.LegacyConfigurationParsers.ParseTextStyleDefinition("TEXTO:RomanS:false:false:2.5:1:0");
        global::ConversorDrawind.LayerRemoveRule remove =
            global::ConversorDrawind.LegacyConfigurationParsers.ParseLayerRemoveRule("REMOVER_WHITE;ALL:ALL:ALL:::ALL");
        global::ConversorDrawind.LayerConversionRule converter =
            global::ConversorDrawind.LegacyConfigurationParsers.ParseLayerConversionRule("ALL;TEXT:ALL:ALL:::ALL;DWI02TX:BYLAYER:BYLAYER:::TEXTO");

        Assert.Equal("0", layer.Name);
        Assert.Equal("WHITE", layer.Color);
        Assert.Equal("CONTINUOUS", layer.LineType);
        Assert.Equal("TEXTO", style.Name);
        Assert.Equal(2.5, style.Size);
        Assert.Equal("REMOVER_WHITE", remove.Filter.BaseLayer);
        Assert.Equal("ALL", remove.Filter.Orientation);
        Assert.Equal("ALL", converter.Source.BaseLayer);
        Assert.Equal("TEXT", converter.Source.ObjectType);
        Assert.Equal("DWI02TX", converter.Target.LayerName);
        Assert.Equal("TEXTO", converter.Target.TextStyle);
    }
}
