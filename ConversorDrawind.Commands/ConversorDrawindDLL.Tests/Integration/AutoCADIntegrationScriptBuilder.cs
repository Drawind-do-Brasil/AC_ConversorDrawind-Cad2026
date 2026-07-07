namespace ConversorDrawindDLL.Tests;

internal static class AutoCADIntegrationScriptBuilder
{
    internal static string Build(string dllPath, IEnumerable<string> commands)
    {
        if (string.IsNullOrWhiteSpace(dllPath))
            throw new ArgumentException("DLL path is required.", nameof(dllPath));

        string[] commandList = commands
            .Where(command => !string.IsNullOrWhiteSpace(command))
            .ToArray();

        if (commandList.Length == 0)
            throw new ArgumentException("At least one AutoCAD command is required.", nameof(commands));

        List<string> lines = new List<string>
        {
            "FILEDIA",
            "0",
            "CMDECHO",
            "1",
            "SECURELOAD",
            "0",
            "NETLOAD",
            Quote(dllPath)
        };

        lines.AddRange(commandList);
        lines.Add("QSAVE");
        lines.Add("QUIT");
        lines.Add("N");

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string Quote(string value)
    {
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }
}
