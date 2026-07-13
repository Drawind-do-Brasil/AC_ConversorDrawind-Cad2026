namespace ConversorDrawind.UI.Wpf.LispDll
{
    public sealed class LispDllCommandEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool RunOnlyAtEnd { get; set; }

        public static LispDllCommandEntry Parse(string commandEntry)
        {
            string[] parts = (commandEntry ?? string.Empty).Split(new[] { '@' }, 3);
            return new LispDllCommandEntry
            {
                Name = parts.Length > 0 ? parts[0] : string.Empty,
                Path = parts.Length > 1 ? parts[1] : string.Empty,
                RunOnlyAtEnd = parts.Length == 3
            };
        }

        public string ToCommandEntry()
        {
            string entry = (Name ?? string.Empty).Trim() + "@" + (Path ?? string.Empty).Trim();
            if (RunOnlyAtEnd)
            {
                entry += "@True";
            }

            return entry;
        }
    }
}
