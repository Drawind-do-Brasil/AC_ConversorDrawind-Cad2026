namespace ConversorDrawindDLL.Tests;

public sealed class AutoCADIntegrationScriptBuilderTests
{
    [Fact]
    public void Build_DeveGerarScriptComNetloadComandosSalvarESair()
    {
        string script = AutoCADIntegrationScriptBuilder.Build(
            @"C:\build\ConversorDrawind.dll",
            new[] { "DRAWINDCAD_Convert", "DRAWINDCAD_Finalize" });

        Assert.Equal(
            string.Join(Environment.NewLine, new[]
            {
                "FILEDIA",
                "0",
                "CMDECHO",
                "1",
                "SECURELOAD",
                "0",
                "NETLOAD",
                @"""C:\build\ConversorDrawind.dll""",
                "DRAWINDCAD_Convert",
                "DRAWINDCAD_Finalize",
                "QSAVE",
                "QUIT",
                "N",
                ""
            }),
            script);
    }

    [Fact]
    public void Build_DeveExigirPeloMenosUmComando()
    {
        Assert.Throws<ArgumentException>(() =>
            AutoCADIntegrationScriptBuilder.Build(@"C:\build\ConversorDrawind.dll", Array.Empty<string>()));
    }
}
