namespace ConversorDrawindDLL.Tests;

public sealed class ConfigurationCharacterizationTests
{
    [Fact]
    public void LoadXML_DevePopularEstadoGlobalESnapshot()
    {
        TestState.Reset();

        using var workspace = TestWorkspace.Create();
        string xmlPath = workspace.GetFile("sample.txml");
        File.WriteAllText(xmlPath, CreateXml(), Encoding.UTF8);

        Configuration.Config.LoadXML(xmlPath);

        Assert.Equal(ExpectedSnapshot(), Snapshot());
        Assert.Equal(2, Arranjos.Arrj.AllNewLayerComposition.Count);
        Assert.Single(Arranjos.Arrj.AllTextSyles);
        Assert.Single(Arranjos.ListBlocks);
        Assert.Single(Arranjos.ListBlocksInv);
        Assert.Single(Arranjos.ListBlocksOrig);
        Assert.Equal(2, InstanciaConversor.ConversorInstancias.Count);
    }

    [Fact]
    public void LoadXML_DeveRecriarInstanciasDeConversor()
    {
        TestState.Reset();

        using var workspace = TestWorkspace.Create();
        string xmlPath = workspace.GetFile("sample.txml");
        File.WriteAllText(xmlPath, CreateXml(), Encoding.UTF8);

        Configuration.Config.LoadXML(xmlPath);

        Assert.Collection(InstanciaConversor.ConversorInstancias,
            first =>
            {
                Assert.Equal("LAYER_A", first.BaseLayerName);
                Assert.Equal("TEXT", first.BaseObjectType);
                Assert.Equal("NEW_A", first.NewLayerName);
            },
            second =>
            {
                Assert.Equal("LAYER_B", second.BaseLayerName);
                Assert.Equal("LINE", second.BaseObjectType);
                Assert.Equal("NEW_B", second.NewLayerName);
            });
    }

    [Fact]
    public void ConversionContext_FromCurrentState_DeveCriarSnapshotIndependente()
    {
        TestState.Reset();

        using var workspace = TestWorkspace.Create();
        string xmlPath = workspace.GetFile("sample.txml");
        File.WriteAllText(xmlPath, CreateXml(), Encoding.UTF8);

        Configuration.Config.LoadXML(xmlPath);

        var scaleReference = new BlockScaleReference(2, 841, new Point3d(10, 20, 30));
        ConversionContext context = ConversionContext.From(Configuration.Config, Arranjos.Arrj, scaleReference);

        Arranjos.Arrj.AllNewLayerComposition.Clear();
        Arranjos.ListBlocks[0].blockName = "ALTERADO";
        Configuration.Config.EXTDIMStyleName = "ALTERADO";

        Assert.Equal("NEW_A:WHITE:CONTINUOUS", context.Layers.NewLayerCompositions[0]);
        Assert.Equal("BLOCK_A", context.Blocks.TeklaBlocks[0].blockName);
        Assert.Equal("COTAS", context.Dimensions.StyleName);
        Assert.Equal(2, context.BlockScale.Scale);
        Assert.Equal(10, context.BlockScale.StartPoint.X);
    }

    [Fact]
    public void LoadConfigDLL_DeveRetornarDefaultQuandoArquivoNaoExiste()
    {
        string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");
        string? backup = File.Exists(arquivo) ? File.ReadAllText(arquivo, Encoding.UTF8) : null;

        try
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }

            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");

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

    private static string CreateXml()
    {
        XDocument document = new(
            new XElement("CONVERSOR",
                new XElement("COMMENTS", new XAttribute("TEXT", "Comentario de teste")),
                new XElement("BASIC_CONFIG",
                    new XAttribute("TEKLAORCAD", "1"),
                    new XAttribute("CONVERT_DIMENSIONS", "true"),
                    new XAttribute("CONVERT_LAYERS", "false"),
                    new XAttribute("EXCHANGE_FORMAT", "true"),
                    new XAttribute("SCALE", "false"),
                    new XAttribute("LISPORDLL", "true"),
                    new XAttribute("PURGE", "false"),
                    new XAttribute("MESSAGE", "true"),
                    new XAttribute("DELETE_TEKLA_STRUCTURES", "true"),
                    new XAttribute("EXPLOD_BLOCKS", "false"),
                    new XAttribute("LAYER_TEKLA_STRING", "DRAWING SHEET"),
                    new XAttribute("LAYER_BLOCK_ATTRIBUTE", "OTHER OBJECT TYPE"),
                    new XAttribute("CAD_EXPLODE", "true"),
                    new XAttribute("DMBLOCK", "false")),
                new XElement("DIMENSION_CONFIG",
                    new XAttribute("DIM_GERAL_HABILIT", "true"),
                    new XAttribute("DIM_LAYER", "DWI-DIM"),
                    new XAttribute("DIM_LINE_COLOR", "1"),
                    new XAttribute("DIM_TEXT_COLOR", "3"),
                    new XAttribute("DIM_STYLE", "COTAS"),
                    new XAttribute("DIM_ARROW_TYPE", "OBLIQUE"),
                    new XAttribute("DIM_SCALE", "2,5"),
                    new XAttribute("DIM_PRECISION", "2"),
                    new XAttribute("DIM_ANGULAR_PRECISION", "3"),
                    new XAttribute("DIM_UNIT", "4"),
                    new XAttribute("DIM_ANGULAR_UNIT", "5"),
                    new XAttribute("DIM_ARROW_SIZE", "1,75"),
                    new XAttribute("DIM_OFFSET", "0,25"),
                    new XAttribute("DIM_OUTSIDE_ALING", "true"),
                    new XAttribute("DIM_TAD", "2"),
                    new XAttribute("DIM_POSITION", "true"),
                    new XAttribute("DIM_TEXT_FORCED", "false"),
                    new XAttribute("DIM_LINE_FORCED", "false"),
                    new XAttribute("DIM_DIMEX", "3,25"),
                    new XAttribute("DIM_BASE_LAYER", "DIMENSION_BASE"),
                    new XAttribute("DIM_ARROW_FIX", "true"),
                    new XAttribute("DIM_ARROW_FIX_TYPE", "Closed"),
                    new XAttribute("DIM_ARROW_FACTOR", "8,5")),
                new XElement("TEXT_CONFIG", new XAttribute("TEXT_STYPE", "TEXTO_TESTE")),
                new XElement("SCALE_CONFIG",
                    new XAttribute("SCALE_MODE", "true"),
                    new XAttribute("SCALE_MANUAL_P1_X", "1"),
                    new XAttribute("SCALE_MANUAL_P1_Y", "2"),
                    new XAttribute("SCALE_MANUAL_P1_Z", "3"),
                    new XAttribute("SCALE_MANUAL_P2_X", "4"),
                    new XAttribute("SCALE_MANUAL_P2_Y", "5"),
                    new XAttribute("SCALE_MANUAL_P2_Z", "6"),
                    new XAttribute("SCALE_AUTO_P1_X", "7"),
                    new XAttribute("SCALE_AUTO_P1_Y", "8"),
                    new XAttribute("SCALE_AUTO_P1_Z", "9"),
                    new XAttribute("SCALE_AUTO_P2_X", "10"),
                    new XAttribute("SCALE_AUTO_P2_Y", "11"),
                    new XAttribute("SCALE_AUTO_P2_Z", "12"),
                    new XAttribute("SCALE_LAYER", "SCALE_LAYER"),
                    new XAttribute("SCALE_TEXT_SIZE", "4,5")),
                new XElement("BASIC_LAYERS", new XAttribute("LTSCALE", "12,5")),
                new XElement("NEW_LAYERS",
                    new XElement("NEW_LAYER", "NEW_A:WHITE:CONTINUOUS"),
                    new XElement("NEW_LAYER", "NEW_B:RED:HIDDEN")),
                new XElement("NEW_TEXTSTYLES",
                    new XElement("TEXT_STYLE", "TEXTO_TESTE:Arial:false:false:4.5:1:0")),
                new XElement("REMOVE_LAYERS",
                    new XElement("REMOVE_LAYER", "IGNORE$REMOVE_BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL")),
                new XElement("CONVERTERS",
                    new XElement("CONVERTER", "LAYER_A;TEXT:WHITE:CONTINUOUS::1,25:ALL;NEW_A:RED:CONTINUOUS:3,5:1,1:TEXTO_TESTE"),
                    new XElement("CONVERTER", "LAYER_B;LINE:GREEN:HIDDEN::0:HOR;NEW_B:BLUE:HIDDEN:0:0")),
                new XElement("DLL_OR_LIST_COMMANDS",
                    new XElement("COMMAND", "(command \"zoom\" \"e\")")),
                new XElement("BLOCK_CONFIG",
                    new XAttribute("DIRECTORY_TEKLA_CONVERSION", @"C:\Formato\Tekla.dwg"),
                    new XAttribute("DIRECTORY_CAD_CONVERSION", @"C:\Formato\Cad.dwg"),
                    new XAttribute("LAYER_EXPLODE", "EXPLODE_A;EXPLODE_B"),
                    new XElement("BLOCK_ATT",
                        new XAttribute("NOME", "BLOCK_A"),
                        new XElement("TAG", "TAG_A@false@1,2,3;4,5,6@BASE;TEXT:WHITE:CONTINUOUS::2,5:ALL@1,0")),
                    new XElement("BLOCK_ATT_CAD",
                        new XAttribute("NOME", "BLOCK_INV;BLOCK_ORIG;-65536"),
                        new XElement("TAG", "TAG_B@true@1,2,3;4,5,6@BASE;TEXT:WHITE:CONTINUOUS::2,5:ALL@0,8@0@True")),
                    new XElement("BLOCK_ATT_ORIG",
                        new XAttribute("NOME", "BLOCK_ORIG;BLOCK_INV;-16776961"),
                        new XElement("TAG", "TAG_C@false@1,2,3;4,5,6@BASE;TEXT:WHITE:CONTINUOUS::2,5:ALL@0,9@0@False")))));

        return document.ToString(SaveOptions.DisableFormatting);
    }

    private static string Snapshot()
    {
        var lines = new List<string>
        {
            "EXTCONFComments=" + Configuration.Config.EXTCONFComments,
            "ConvTekla0ConvInv1=" + Configuration.Config.ConvTekla0ConvInv1.ToString(CultureInfo.InvariantCulture),
            "EXTCONFIsConvertDimension=" + Configuration.Config.EXTCONFIsConvertDimension,
            "EXTCONFIsConvertLayer=" + Configuration.Config.EXTCONFIsConvertLayer,
            "EXTCONFIsExchangeFormat=" + Configuration.Config.EXTCONFIsExchangeFormat,
            "EXTCONFIsPutOnTheScaleDrawing=" + Configuration.Config.EXTCONFIsPutOnTheScaleDrawing,
            "EXTCONFIsExecuteLISP=" + Configuration.Config.EXTCONFIsExecuteLISP,
            "EXTCONFIsPurge=" + Configuration.Config.EXTCONFIsPurge,
            "PROGRAMMessage=" + Configuration.Config.PROGRAMMessage,
            "EXTCONFIsDeleteTeklaStructures=" + Configuration.Config.EXTCONFIsDeleteTeklaStructures,
            "ExplodeBlocks=" + Configuration.Config.ExplodeBlocks,
            "LayerTeklaString=" + Configuration.Config.LayerTeklaString,
            "LayerBlockAttribute=" + Configuration.Config.LayerBlockAttribute,
            "EXTCONFInventorExplode=" + Configuration.Config.EXTCONFInventorExplode,
            "EXTDIMGERALHabilit=" + Configuration.Config.EXTDIMGERALHabilit,
            "EXTDIMlayer=" + Configuration.Config.EXTDIMlayer,
            "EXTDIMColorLine=" + Configuration.Config.EXTDIMColorLine,
            "EXTDIMColorText=" + Configuration.Config.EXTDIMColorText,
            "EXTDIMStyleName=" + Configuration.Config.EXTDIMStyleName,
            "EXTDIMSeta=" + Configuration.Config.EXTDIMSeta,
            "EXTDIMScale=" + Configuration.Config.EXTDIMScale.ToString("R", CultureInfo.InvariantCulture),
            "EXTDIMPrecision=" + Configuration.Config.EXTDIMPrecision.ToString(CultureInfo.InvariantCulture),
            "EXTDIMAngularPrecision=" + Configuration.Config.EXTDIMAngularPrecision.ToString(CultureInfo.InvariantCulture),
            "EXTDIMUnit=" + Configuration.Config.EXTDIMUnit.ToString(CultureInfo.InvariantCulture),
            "EXTDIMAngularUnit=" + Configuration.Config.EXTDIMAngularUnit.ToString(CultureInfo.InvariantCulture),
            "EXTDIMSizeSeta=" + Configuration.Config.EXTDIMSizeSeta.ToString("R", CultureInfo.InvariantCulture),
            "EXTDIMOffsetLineFromRefPoint=" + Configuration.Config.EXTDIMOffsetLineFromRefPoint.ToString("R", CultureInfo.InvariantCulture),
            "EXTDIMOutsideAlign=" + Configuration.Config.EXTDIMOutsideAlign,
            "EXTDIMTad=" + Configuration.Config.EXTDIMTad.ToString(CultureInfo.InvariantCulture),
            "EXTDIMDimensionPosition=" + Configuration.Config.EXTDIMDimensionPosition,
            "EXTDIMTextForced=" + Configuration.Config.EXTDIMTextForced,
            "EXTDIMLineForced=" + Configuration.Config.EXTDIMLineForced,
            "EXTDIMDIMEX=" + Configuration.Config.EXTDIMDIMEX.ToString("R", CultureInfo.InvariantCulture),
            "EXTDIMBaseLayer=" + Configuration.Config.EXTDIMBaseLayer,
            "EXTDIMCorrigeSeta=" + Configuration.Config.EXTDIMCorrigeSeta,
            "EXTDIMCorrigeSetaTipoSeta=" + Configuration.Config.EXTDIMCorrigeSetaTipoSeta,
            "EXTDIMCorrigeSetaFactor=" + Configuration.Config.EXTDIMCorrigeSetaFactor.ToString("R", CultureInfo.InvariantCulture),
            "EXTTEXTStyleName=" + Configuration.Config.EXTTEXTStyleName,
            "EXTTEXTSize=" + Configuration.Config.EXTTEXTSize.ToString("R", CultureInfo.InvariantCulture),
            "EXTSCALEManual=" + Configuration.Config.EXTSCALEManual,
            "EXTSCALEp1=" + Point(Configuration.Config.EXTSCALEp1),
            "EXTSCALEp2=" + Point(Configuration.Config.EXTSCALEp2),
            "EXTSCALELayer=" + Configuration.Config.EXTSCALELayer,
            "EXTSCALETextSize=" + Configuration.Config.EXTSCALETextSize.ToString("R", CultureInfo.InvariantCulture),
            "EXTLINELtscale=" + Configuration.Config.EXTLINELtscale.ToString("R", CultureInfo.InvariantCulture),
            "PROGRAMblockFormatoCaminho=" + Configuration.Config.PROGRAMblockFormatoCaminho,
            "EXTCONFCaminhoBlocoInv=" + Configuration.Config.EXTCONFCaminhoBlocoInv,
            "DMBlock=" + Configuration.Config.DMBlock,
            "allNewLayerComposition=" + string.Join("|", Arranjos.Arrj.AllNewLayerComposition),
            "allTextSyles=" + string.Join("|", Arranjos.Arrj.AllTextSyles),
            "conversor=" + string.Join("|", Arranjos.Arrj.Conversor),
            "listLISPCommand=" + string.Join("|", Arranjos.Arrj.ListLISPCommand),
            "allExplodeLayers=" + string.Join("|", Arranjos.Arrj.AllExplodeLayers),
            "layerRemove=" + string.Join("|", Arranjos.Arrj.LayerRemove.Select(Filter)),
            "blocks=" + string.Join("|", Arranjos.ListBlocks.Select(Block)),
            "blocksInv=" + string.Join("|", Arranjos.ListBlocksInv.Select(Block)),
            "blocksOrig=" + string.Join("|", Arranjos.ListBlocksOrig.Select(Block))
        };

        return string.Join(Environment.NewLine, lines);
    }

    private static string ExpectedSnapshot()
    {
        return string.Join(Environment.NewLine, new[]
        {
            "EXTCONFComments=Comentario de teste",
            "ConvTekla0ConvInv1=1",
            "EXTCONFIsConvertDimension=True",
            "EXTCONFIsConvertLayer=False",
            "EXTCONFIsExchangeFormat=True",
            "EXTCONFIsPutOnTheScaleDrawing=False",
            "EXTCONFIsExecuteLISP=True",
            "EXTCONFIsPurge=False",
            "PROGRAMMessage=True",
            "EXTCONFIsDeleteTeklaStructures=True",
            "ExplodeBlocks=False",
            "LayerTeklaString=DRAWING SHEET",
            "LayerBlockAttribute=OTHER OBJECT TYPE",
            "EXTCONFInventorExplode=True",
            "EXTDIMGERALHabilit=True",
            "EXTDIMlayer=DWI-DIM",
            "EXTDIMColorLine=1",
            "EXTDIMColorText=3",
            "EXTDIMStyleName=COTAS",
            "EXTDIMSeta=OBLIQUE",
            "EXTDIMScale=2.5",
            "EXTDIMPrecision=2",
            "EXTDIMAngularPrecision=3",
            "EXTDIMUnit=4",
            "EXTDIMAngularUnit=5",
            "EXTDIMSizeSeta=1.75",
            "EXTDIMOffsetLineFromRefPoint=0.25",
            "EXTDIMOutsideAlign=True",
            "EXTDIMTad=2",
            "EXTDIMDimensionPosition=True",
            "EXTDIMTextForced=False",
            "EXTDIMLineForced=False",
            "EXTDIMDIMEX=3.25",
            "EXTDIMBaseLayer=DIMENSION_BASE",
            "EXTDIMCorrigeSeta=True",
            "EXTDIMCorrigeSetaTipoSeta=Closed",
            "EXTDIMCorrigeSetaFactor=8.5",
            "EXTTEXTStyleName=TEXTO_TESTE",
            "EXTTEXTSize=4.5",
            "EXTSCALEManual=True",
            "EXTSCALEp1=1;2;3",
            "EXTSCALEp2=4;5;6",
            "EXTSCALELayer=SCALE_LAYER",
            "EXTSCALETextSize=4.5",
            "EXTLINELtscale=12.5",
            "PROGRAMblockFormatoCaminho=C:\\Formato\\Tekla.dwg",
            "EXTCONFCaminhoBlocoInv=C:\\Formato\\Cad.dwg",
            "DMBlock=False",
            "allNewLayerComposition=NEW_A:WHITE:CONTINUOUS|NEW_B:RED:HIDDEN",
            "allTextSyles=TEXTO_TESTE:Arial:false:false:4.5:1:0",
            "conversor=LAYER_A;TEXT:WHITE:CONTINUOUS::1,25:ALL;NEW_A:RED:CONTINUOUS:3,5:1,1:TEXTO_TESTE|LAYER_B;LINE:GREEN:HIDDEN::0:HOR;NEW_B:BLUE:HIDDEN:0:0",
            "listLISPCommand=(command \"zoom\" \"e\")",
            "allExplodeLayers=EXPLODE_A|EXPLODE_B",
            "layerRemove=REMOVE_BASE;TEXT:RED:HIDDEN:ABC:2.5:ALL",
            "blocks=BLOCK_A,,-16777216,TAG_A,False,1;2;3,4;5;6,BASE:TEXT:WHITE:CONTINUOUS::2,5:ALL,1,-1,False",
            "blocksInv=BLOCK_INV,BLOCK_ORIG,-65536,TAG_B,True,1;2;3,4;5;6,BASE:TEXT:WHITE:CONTINUOUS::2,5:ALL,0.8,0,True",
            "blocksOrig=BLOCK_ORIG,BLOCK_INV,-16776961,TAG_C,False,1;2;3,4;5;6,BASE:TEXT:WHITE:CONTINUOUS::2,5:ALL,0.9,0,False"
        });
    }

    private static string Point(PointEspecial2 point)
    {
        return point.X.ToString("R", CultureInfo.InvariantCulture) + ";" +
               point.Y.ToString("R", CultureInfo.InvariantCulture) + ";" +
               point.Z.ToString("R", CultureInfo.InvariantCulture);
    }

    private static string Point(global::ConversorDrawind.PointEspecial point)
    {
        return point.X.ToString("R", CultureInfo.InvariantCulture) + ";" +
               point.Y.ToString("R", CultureInfo.InvariantCulture) + ";" +
               point.Z.ToString("R", CultureInfo.InvariantCulture);
    }

    private static string Filter(Filter filter)
    {
        return filter.layerBase + ";" + filter.GetConjunto();
    }

    private static string Block(BlockClass block)
    {
        return string.Join(",", new[]
        {
            block.blockName,
            block.blockNameRelacao,
            block.cor.ToArgb().ToString(CultureInfo.InvariantCulture),
            string.Join("|", block.listTags.Select(Tag))
        });
    }

    private static string Tag(TagBlockClass tag)
    {
        return string.Join(",", new[]
        {
            tag.tag,
            tag.modify.ToString(),
            Point(tag.p1),
            Point(tag.p2),
            tag.filtro.layerBase + ":" + tag.filtro.GetConjunto(),
            tag.widthfactor.ToString("R", CultureInfo.InvariantCulture),
            tag.indiceRelacao.ToString(CultureInfo.InvariantCulture),
            tag.isSociate.ToString()
        });
    }
}
