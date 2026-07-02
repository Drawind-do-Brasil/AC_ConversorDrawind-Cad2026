using ConversorDrawind;

namespace ConversorDrawind.Tests;

public sealed class FrontConfigurationServiceTests
{
    [Fact]
    public void ConverterFileService_ListConverterNames_DeveListarTemplateETxml()
    {
        using var workspace = TestWorkspace.Create();
        File.WriteAllText(workspace.GetFile("A.template"), string.Empty);
        File.WriteAllText(workspace.GetFile("B.txml"), "<CONVERSOR />");

        var names = ConverterFileService.ListConverterNames(workspace.Status);

        Assert.Contains("A", names);
        Assert.Contains("B", names);
    }

    [Fact]
    public void ConversionPreflightValidator_DeveBloquearFormatoAusenteQuandoTrocaFormato()
    {
        var configuration = new Class_Configuration
        {
            EXTCONFIsExchangeFormat = true,
            EXTCONFOrigem = 0,
            PROGRAMblockFormatoCaminho = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dwg")
        };

        var result = ConversionPreflightValidator.ValidateFormatPath(configuration);

        Assert.False(result.CanConvert);
        Assert.Equal(configuration.PROGRAMblockFormatoCaminho, result.MissingFormatPath);
    }

    [Fact]
    public void ConversionPreflightValidator_DevePermitirQuandoNaoTrocaFormato()
    {
        var configuration = new Class_Configuration
        {
            EXTCONFIsExchangeFormat = false,
            PROGRAMblockFormatoCaminho = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dwg")
        };

        var result = ConversionPreflightValidator.ValidateFormatPath(configuration);

        Assert.True(result.CanConvert);
        Assert.Equal(string.Empty, result.MissingFormatPath);
    }

    [Fact]
    public void NumericTextParser_DevePreservarConversaoComPontoDecimal()
    {
        Assert.Equal(1.25, NumericTextParser.ToDouble("1.25"));
    }
}
