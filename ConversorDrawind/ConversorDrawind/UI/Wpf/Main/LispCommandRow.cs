using ConversorDrawind.UI.Wpf.LispDll;

namespace ConversorDrawind.UI.Wpf.Main
{
    public sealed class LispCommandRow
    {
        public LispCommandRow(string name, string path, bool runOnlyAtEnd)
        {
            Name = name;
            Path = path;
            RunOnlyAtEnd = runOnlyAtEnd;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public bool RunOnlyAtEnd { get; set; }

        public static LispCommandRow FromCommandEntry(string commandEntry)
        {
            LispDllCommandEntry entry = LispDllCommandEntry.Parse(commandEntry);
            return new LispCommandRow(entry.Name, entry.Path, entry.RunOnlyAtEnd);
        }
    }
}
