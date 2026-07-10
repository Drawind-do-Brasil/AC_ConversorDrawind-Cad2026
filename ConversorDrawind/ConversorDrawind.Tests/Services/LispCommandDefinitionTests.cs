using ConversorDrawind;

namespace ConversorDrawind.Tests;

public sealed class LispCommandDefinitionTests
{
    [Fact]
    public void TryParse_DeveDistinguirComandosAntesEDepoisDaConversao()
    {
        Assert.True(LispCommandDefinition.TryParse("PRE@C:\\Rotinas\\pre.lsp", out var before));
        Assert.True(LispCommandDefinition.TryParse("POS@C:\\Rotinas\\pos.lsp@LAST", out var after));

        Assert.False(before.ExecuteAfterConversion);
        Assert.True(after.ExecuteAfterConversion);
        Assert.Equal("PRE", before.Command);
        Assert.Equal(@"C:\Rotinas\pos.lsp", after.SourceFile);
    }

    [Theory]
    [InlineData("")]
    [InlineData("COMANDO")]
    [InlineData("@arquivo.lsp")]
    [InlineData("COMANDO@@LAST")]
    [InlineData("COMANDO@arquivo@LAST@EXTRA")]
    public void TryParse_DeveRejeitarDefinicoesInvalidas(string definition)
    {
        Assert.False(LispCommandDefinition.TryParse(definition, out _));
    }
}
