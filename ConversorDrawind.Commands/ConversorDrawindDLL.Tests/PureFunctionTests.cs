namespace ConversorDrawindDLL.Tests;

public sealed class PureFunctionTests
{
    [Fact]
    public void ReplaceComma_DeveTrocarSeparadorConformeCulturaAtual()
    {
        using var _ = new CultureScope("en-US");
        Assert.Equal("1.25", "1,25".ReplaceComma());

        using var __ = new CultureScope("pt-BR");
        Assert.Equal("1,25", "1.25".ReplaceComma());
    }

    [Fact]
    public void ToDouble_DevePreservarConversaoAtual()
    {
        using var _ = new CultureScope("pt-BR");
        Assert.Equal(1.25, ((object)"1.25").ToDouble(), 6);
    }

    [Fact]
    public void CheckPoint_DeveIdentificarPontoDentroDaJanela()
    {
        var p1 = new Point3d(0, 0, 0);
        var p2 = new Point3d(10, 10, 0);

        Assert.True(ConvertBlocks.CheckPoint(new Point3d(5, 5, 0), p1, p2));
        Assert.True(BlockMatcher.CheckPoint(new Point3d(5, 5, 0), p2, p1));
        Assert.False(ConvertBlocks.CheckPoint(new Point3d(11, 5, 0), p1, p2));
    }

    [Fact]
    public void GetPTReal_ComReferenciaExplicita_DeveIndependerDoEstadoGlobal()
    {
        var scaleReference = new BlockScaleReference(2, 841, new Point3d(10, 20, 30));

        Point3d result = ConvertBlocks.GetPTReal(new Point3d(1, 2, 3), scaleReference);

        Assert.Equal(12, result.X);
        Assert.Equal(24, result.Y);
        Assert.Equal(36, result.Z);
    }

    [Fact]
    public void FormatStartPointService_DeveAtualizarMenorPonto()
    {
        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double minZ = double.MaxValue;

        FormatStartPointService.UpdateMinimumPoint(new Point3d(10, -2, 3), ref minX, ref minY, ref minZ);
        FormatStartPointService.UpdateMinimumPoint(new Point3d(4, 5, -1), ref minX, ref minY, ref minZ);

        Assert.Equal(4, minX);
        Assert.Equal(-2, minY);
        Assert.Equal(-1, minZ);
    }

    [Theory]
    [InlineData("Layer_A", "layer_a", true)]
    [InlineData("Layer_A", "Layer_B", false)]
    public void FormatStartPointService_DeveCompararLayerCaseInsensitive(string entityLayer, string layerName, bool expected)
    {
        Assert.Equal(expected, FormatStartPointService.IsSameLayer(entityLayer, layerName));
    }

    [Fact]
    public void FormatStartPointService_DeveValidarPonto()
    {
        Assert.True(FormatStartPointService.IsValidPoint(new Point3d(0, 1, 2)));
        Assert.False(FormatStartPointService.IsValidPoint(new Point3d(double.NaN, 1, 2)));
        Assert.False(FormatStartPointService.IsValidPoint(new Point3d(0, double.PositiveInfinity, 2)));
        Assert.False(FormatStartPointService.IsValidPoint(new Point3d(0, 1, double.MaxValue)));
    }

    [Fact]
    public void FillDictionary_DeveMapearTagsEValores()
    {
        var dictionary = ConvertBlocks.FillDictionary(new[] { "A", "B" }, new[] { "1", "2" });
        var extractedDictionary = BlockAttributeWriter.FillDictionary(new[] { "A", "B" }, new[] { "1", "2" });

        Assert.Equal("1", dictionary["A"]);
        Assert.Equal("2", dictionary["B"]);
        Assert.Equal(dictionary, extractedDictionary);
    }

    [Fact]
    public void FillDictionary_DeveFalharQuandoTamanhosDiferentes()
    {
        var ex = Assert.Throws<Exception>(() => ConvertBlocks.FillDictionary(new[] { "A" }, Array.Empty<string>()));

        Assert.Contains("número diferente de elementos", ex.Message);
    }

    [Theory]
    [InlineData(new[] { 0.0, 0.0 }, false)]
    [InlineData(new[] { 0.0, 1.5707963267948966 }, true)]
    [InlineData(new double[0], false)]
    public void DimensionTextAnalyzer_DeveDetectarRotacoesDiferentes(double[] rotations, bool expected)
    {
        Assert.Equal(expected, DimensionTextAnalyzer.HasDifferentTextRotations(rotations));
    }

    [Fact]
    public void ConversionLog_DevePreservarFormatoAtual()
    {
        string oldDirectory = ConversionSession.LogDirectory;
        string oldFileName = ConversionSession.LogFileName;

        using var workspace = TestWorkspace.Create();
        string logFile = workspace.GetFile("Conversor.log");

        try
        {
            ConversionSession.SetLogFile(workspace.Root, logFile);

            ConversionLog.Write("Operacao de teste", "Detalhe");

            Assert.Equal("Operacao de teste : Detalhe", File.ReadAllText(logFile, Encoding.UTF8).Trim());
        }
        finally
        {
            ConversionSession.SetLogFile(oldDirectory, oldFileName);
        }
    }

    [Fact]
    public void ConversionLog_ComExcecao_DeveRegistrarContextoLegivel()
    {
        string oldDirectory = ConversionSession.LogDirectory;
        string oldFileName = ConversionSession.LogFileName;

        using var workspace = TestWorkspace.Create();
        string logFile = workspace.GetFile("Conversor.log");

        try
        {
            ConversionSession.SetLogFile(workspace.Root, logFile);

            ConversionLog.Write(LogContext.CapturarTextosDoFormato, new InvalidOperationException("Detalhe"));

            Assert.Equal("Capturar textos do formato : Detalhe", File.ReadAllText(logFile, Encoding.UTF8).Trim());
        }
        finally
        {
            ConversionSession.SetLogFile(oldDirectory, oldFileName);
        }
    }

    [Fact]
    public void ConversionPreflight_SemArquivoTemporario_DevePreservarNomeAtual()
    {
        string tempConfigurationPath = ConversionPreflight.GetTempConfigurationPath();
        string? backup = File.Exists(tempConfigurationPath) ? File.ReadAllText(tempConfigurationPath, Encoding.UTF8) : null;

        try
        {
            if (File.Exists(tempConfigurationPath))
            {
                File.Delete(tempConfigurationPath);
            }

            string converterName = "atual";

            ConversionPreflight.LoadTempConfiguration(new Configuration(), ref converterName);

            Assert.Equal("atual", converterName);
        }
        finally
        {
            if (backup is not null)
            {
                File.WriteAllText(tempConfigurationPath, backup, Encoding.UTF8);
            }
        }
    }

    [Theory]
    [InlineData("RED", 1)]
    [InlineData("YELLOW", 2)]
    [InlineData("GREEN", 3)]
    [InlineData("CYAN", 4)]
    [InlineData("BLUE", 5)]
    [InlineData("MAGENTA", 6)]
    [InlineData("WHITE", 7)]
    [InlineData("BYBLOCK", 0)]
    [InlineData("BYLAYER", 256)]
    [InlineData("42", 42)]
    [InlineData("red", 1)]
    public void ColorResolver_DevePreservarCoresAci(string colorName, short expectedIndex)
    {
        var color = ColorResolver.Resolve(colorName);

        Assert.Equal(expectedIndex, color.ColorIndex);
    }

    [Fact]
    public void ColorResolver_DeveRetornarNullParaAll()
    {
        Assert.Null(ColorResolver.Resolve("ALL"));
    }

    [Fact]
    public void ColorResolver_DevePreservarRgb()
    {
        var color = ColorResolver.Resolve("10,20,30");

        Assert.Equal(10, color.Red);
        Assert.Equal(20, color.Green);
        Assert.Equal(30, color.Blue);
    }

    [Fact]
    public void ScaleDetector_GetPointDifference_DevePreservarCalculoAtual()
    {
        Point3d result = ScaleDetector.GetPointDifference(
            new Point3d(10, 20, 30),
            new Point3d(1, 2, 3),
            2);

        Assert.Equal(12, result.X);
        Assert.Equal(24, result.Y);
        Assert.Equal(36, result.Z);
    }

    [Fact]
    public void ScaleDetector_IsOrientation_DevePreservarArredondamentoAtual()
    {
        Assert.True(ScaleDetector.IsOrientation(new Point3d(0, 1.0004, 0), new Point3d(10, 1.0001, 0), "HORIZONTAL"));
        Assert.True(ScaleDetector.IsOrientation(new Point3d(2.0004, 0, 0), new Point3d(2.0001, 10, 0), "VERTICAL"));
        Assert.False(ScaleDetector.IsOrientation(new Point3d(0, 1, 0), new Point3d(10, 2, 0), "HORIZONTAL"));
    }

    [Fact]
    public void DegreeToRadian_DeveManterResultadoAtual()
    {
        Assert.Equal(Math.PI, ConvertDimension.DegreeToRadian(180), 12);
        Assert.Equal(Math.PI / 2, ConvertDimension.DegreeToRadian(90), 12);
        Assert.Equal(Math.PI, DimensionGeometry.DegreeToRadian(180), 12);
        Assert.Equal(Math.PI / 2, DimensionGeometry.DegreeToRadian(90), 12);
    }

    [Fact]
    public void DimensionGeometry_RadianToDegree_DevePreservarCalculoAtual()
    {
        Assert.Equal(180, DimensionGeometry.RadianToDegree(Math.PI), 12);
        Assert.Equal(90, DimensionGeometry.RadianToDegree(Math.PI / 2), 12);
    }

    [Fact]
    public void DimensionGeometry_IsOnLine_DevePreservarCasosConhecidos()
    {
        Assert.True(DimensionGeometry.IsOnLine(new Point3d(0, 2, 0), new Point3d(10, 2, 0), new Point3d(5, 2, 0)));
        Assert.True(DimensionGeometry.IsOnLine(new Point3d(3, 0, 0), new Point3d(3, 10, 0), new Point3d(3, 5, 0)));
        Assert.True(DimensionGeometry.IsOnLine(new Point3d(0, 0, 0), new Point3d(10, 10, 0), new Point3d(5, 5, 0)));
        Assert.False(DimensionGeometry.IsOnLine(new Point3d(0, 0, 0), new Point3d(10, 10, 0), new Point3d(5, 6, 0)));
    }

    [Fact]
    public void DimensionGeometry_CheckParallelLine_DevePreservarCasosConhecidos()
    {
        Assert.True(DimensionGeometry.CheckParallelLine(0, Math.PI));
        Assert.True(DimensionGeometry.CheckParallelLine(Math.PI / 4, Math.PI / 4));
        Assert.False(DimensionGeometry.CheckParallelLine(0, Math.PI / 2));
    }

    [Fact]
    public void DimensionGeometry_SlopeTwoPoints_DevePreservarCalculoAtual()
    {
        Assert.Equal(Math.PI / 4, DimensionGeometry.SlopeTwoPoints(new Point3d(0, 0, 0), new Point3d(10, 10, 0)), 12);
    }

    [Fact]
    public void ArrowFixService_GetDimensionDistance_DevePreservarCalculoAtual()
    {
        double distance = ArrowFixService.GetDimensionDistance(
            new Point3d(0, 0, 0),
            new Point3d(10, 0, 0),
            new Point3d(10, 5, 0));

        Assert.Equal(10, distance, 12);
    }

    [Fact]
    public void ArrowFixService_ProjectFirstExtensionPoint_DeveProjetarNoPlanoDaLinhaDeCota()
    {
        Point3d projected = ArrowFixService.ProjectFirstExtensionPoint(
            new Point3d(0, 0, 0),
            new Point3d(10, 0, 0),
            new Point3d(10, 5, 0));

        Assert.Equal(0, projected.X, 12);
        Assert.Equal(5, projected.Y, 12);
        Assert.Equal(0, projected.Z, 12);
    }

    [Theory]
    [InlineData("Closed Filled", ".", "")]
    [InlineData("Dot", "_DOT", "DOT")]
    [InlineData("Dot Small", "_DOTSMALL", "DOTSMALL")]
    [InlineData("Dot Blank", "_DOTBLANK", "DOTBLANK")]
    [InlineData("Origin Indicator", "_ORIGIN", "ORIGIN")]
    [InlineData("Origin Indicator 2", "_ORIGIN2", "ORIGIN2")]
    [InlineData("Open", "_OPEN", "OPEN")]
    [InlineData("Right Angle", "_OPEN90", "OPEN90")]
    [InlineData("Open 30", "_OPEN30", "OPEN30")]
    [InlineData("Closed", "_CLOSED", "CLOSED")]
    [InlineData("Dot Small Blank", "_SMALL", "SMALL")]
    [InlineData("None", "_NONE", "NONE")]
    [InlineData("Oblique", "_OBLIQUE", "OBLIQUE")]
    [InlineData("Box Filled", "_BOXFILLED", "BOXFILLED")]
    [InlineData("Box", "_BOXBLANK", "BOXBLANK")]
    [InlineData("Closed Blank", "_CLOSEDBLANK", "CLOSEDBLANK")]
    [InlineData("Datum Triangle Filled", "_DATUMFILLED", "DATUMFILLED")]
    [InlineData("Datum Triangle", "_DATUMBLANK", "DATUMBLANK")]
    [InlineData("Integral", "_INTEGRAL", "INTEGRAL")]
    [InlineData("Architectural Tick", "_ARCHTICK", "ARCHTICK")]
    [InlineData("unknown", ".", "")]
    [InlineData("dot", "_DOT", "DOT")]
    public void ConvertLayer_ArrowNames_DevePreservarMapeamentoCaseInsensitive(
        string input,
        string expectedBlockName,
        string expectedBlockNameString)
    {
        Assert.Equal(expectedBlockName, ConvertLayer.GetArrowBlockName(input));
        Assert.Equal(expectedBlockNameString, ConvertLayer.GetArrowBlockNameString(input));
    }

    [Fact]
    public void TextStyleResolver_DevePreservarBuscaCaseInsensitiveECalculos()
    {
        string[] textStyles =
        {
            "TEXTO_TESTE:Arial:false:false:4.5:1:30"
        };

        Assert.Equal("TEXTO_TESTE:Arial:false:false:4.5:1:30", TextStyleResolver.Resolve(textStyles, "texto_teste"));
        Assert.Equal(4.5, TextStyleResolver.ResolveTextSize(textStyles, "texto_teste"));
        Assert.Equal(Math.PI / 6, TextStyleResolver.ResolveOblique(textStyles, "texto_teste"), 12);
    }

    [Fact]
    public void TextStyleService_DevePreservarResolucaoHistoricaDeNomePadrao()
    {
        string[] textStyles =
        {
            "TEXTO_PADRAO:RomanS:false:false:2.5:1:0",
            "OUTRO:Arial:true:false:3.5:0.8:15"
        };

        Assert.Equal("TEXTO_PADRAO", TextStyleService.ResolveStyleNameOrDefault(null, textStyles));
        Assert.Equal("TEXTO_PADRAO", TextStyleService.ResolveStyleNameOrDefault("OUTRO", textStyles));
        Assert.Equal(textStyles[1], TextStyleService.ResolveStyleNameOrDefault(textStyles[1], textStyles));
    }

    [Fact]
    public void TextStyleService_DevePreservarParsingDaDefinicao()
    {
        TextStyleDefinition definition = TextStyleService.ParseDefinition("TEXTO:Arial:true:false:2.5:0.75:30");

        Assert.Equal("TEXTO", definition.Name);
        Assert.Equal("Arial", definition.Font);
        Assert.True(definition.Italic);
        Assert.False(definition.Bold);
        Assert.Equal(0.75, definition.XScale);
        Assert.Equal(30, definition.ObliquingAngleDegrees);
    }

    [Fact]
    public void TextStyleService_DeveCriarFontDescriptorComoFachadaAtual()
    {
        var direct = ConvertLayer.UpdateTextFont("Arial", italic: true, negrito: false);
        var extracted = TextStyleService.CreateFontDescriptor("Arial", italic: true, bold: false);

        Assert.Equal(direct.TypeFace, extracted.TypeFace);
        Assert.Equal(direct.Italic, extracted.Italic);
        Assert.Equal(direct.Bold, extracted.Bold);
    }

    [Theory]
    [InlineData("ALL", true)]
    [InlineData("all", true)]
    [InlineData("Continuous", false)]
    public void LinetypeService_DevePreservarRegraDeIgnorarAll(string linetypeName, bool expected)
    {
        Assert.Equal(expected, LinetypeService.ShouldSkipLinetype(linetypeName));
    }

    [Fact]
    public void LayerFilterFactory_DevePreservarFiltrosDeTexto()
    {
        Assert.Equal(
            new[]
            {
                "-4:<and",
                "8:LAYER_A",
                "-4:<or",
                "0:TEXT",
                "0:MTEXT",
                "-4:or>",
                "-4:and>"
            },
            Serialize(LayerFilterFactory.TextAndMTextOnLayer("LAYER_A")));

        Assert.Equal(
            new[]
            {
                "-4:<and",
                "8:LAYER_A",
                "-4:<or",
                "0:TEXT",
                "0:INSERT",
                "-4:or>",
                "-4:and>"
            },
            Serialize(LayerFilterFactory.TextAndInsertOnLayer("LAYER_A")));
    }

    [Fact]
    public void LayerFilterFactory_DevePreservarFiltrosComMultiplasLayersEOpcionalAll()
    {
        Assert.Equal(
            new[]
            {
                "-4:<and",
                "-4:<or",
                "8:A",
                "8:B",
                "-4:or>",
                "-4:<or",
                "0:TEXT",
                "0:MTEXT",
                "-4:or>",
                "-4:and>"
            },
            Serialize(LayerFilterFactory.TextAndMTextOnLayers("A", "B")));

        Assert.Equal(
            new[]
            {
                "-4:<and",
                "0:TEXT",
                "-4:and>"
            },
            Serialize(LayerFilterFactory.TextOnOptionalLayer("ALL")));
    }

    [Fact]
    public void LayerFilterFactory_DevePreservarFiltrosDeInsert()
    {
        Assert.Equal(
            new[] { "0:INSERT", "8:LAYER_A" },
            Serialize(LayerFilterFactory.InsertOnLayer("LAYER_A")));

        Assert.Equal(
            new[] { "0:INSERT" },
            Serialize(LayerFilterFactory.InsertOnly()));

        Assert.Equal(
            new[] { "0:INSERT", "2:BLOCO_A" },
            Serialize(LayerFilterFactory.InsertBlockByName("BLOCO_A")));

        Assert.Equal(
            new[] { "-4:<and", "0:INSERT", "2:BLOCO_A", "-4:and>" },
            Serialize(LayerFilterFactory.InsertBlockByNameAnd("BLOCO_A")));
    }

    [Fact]
    public void LayerFilterFactory_DevePreservarFiltroTextOuInsert()
    {
        Assert.Equal(
            new[]
            {
                "-4:<and",
                "-4:<or",
                "0:TEXT",
                "0:INSERT",
                "-4:or>",
                "-4:and>"
            },
            Serialize(LayerFilterFactory.TextOrInsert()));
    }

    [Fact]
    public void LayerFilterFactory_DevePreservarFiltroPorTipos()
    {
        Assert.Equal(
            new[]
            {
                "-4:<or",
                "0:DIMENSION",
                "0:TEXT",
                "-4:or>"
            },
            Serialize(LayerFilterFactory.ObjectTypes("DIMENSION", "TEXT")));
    }

    private static string[] Serialize(TypedValue[] values)
    {
        return values.Select(value => value.TypeCode.ToString(CultureInfo.InvariantCulture) + ":" + value.Value).ToArray();
    }
}
