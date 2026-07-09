namespace ConversorDrawind.UI.Wpf.Main.Rows
{
    public sealed class BlockRelationRow
    {
        public BlockRelationRow(BlockDefinition cadBlock, BlockDefinition originalBlock)
        {
            CadBlock = cadBlock;
            OriginalBlock = originalBlock;
        }

        public BlockDefinition CadBlock { get; }
        public BlockDefinition OriginalBlock { get; }
        public string CadName => CadBlock?.Name ?? string.Empty;
        public string OriginalName => OriginalBlock?.Name ?? string.Empty;

        public override string ToString()
        {
            return CadName + "    = >    " + OriginalName;
        }
    }
}
