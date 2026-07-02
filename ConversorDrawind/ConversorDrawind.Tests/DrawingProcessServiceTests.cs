using ConversorDrawind;

namespace ConversorDrawind.Tests;

public sealed class DrawingProcessServiceTests
{
    [Fact]
    public void DrawingCommandBuilder_DeveGerarNetloadParaDll()
    {
        string command = DrawingCommandBuilder.BuildLoadFileCommand(@"C:\Temp\Plugin.dll");

        Assert.Equal(@"NETLOAD C:\Temp\Plugin.dll" + "\n", command);
    }

    [Fact]
    public void DrawingCommandBuilder_DeveGerarLoadLispComBarrasEscapadas()
    {
        string command = DrawingCommandBuilder.BuildLoadFileCommand(@"C:\Temp\Rotina.lsp");

        Assert.Equal(@"(load  ""C:\\Temp\\Rotina.lsp"")" + "\n", command);
    }

    [Fact]
    public void DrawingProcessPaths_DeveResolverFormatoPelaOrigem()
    {
        var configuration = new Class_Configuration
        {
            EXTCONFOrigem = 0,
            PROGRAMblockFormatoCaminho = "tekla.dwg",
            EXTCONFCaminhoBlocoInv = "cad.dwg"
        };

        Assert.Equal("tekla.dwg", DrawingProcessPaths.GetExchangeFormatPath(configuration));

        configuration.EXTCONFOrigem = 1;

        Assert.Equal("cad.dwg", DrawingProcessPaths.GetExchangeFormatPath(configuration));
    }
}
