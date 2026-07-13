using System.Collections.Generic;

namespace ConversorDrawind
{
    public sealed class LegacyConfigurationState
    {
        public List<string> BaseLayers { get; } = Defaults.DefaultBaseLayers();
        public List<string> NewLayerNames { get; } = Defaults.DefaultNewLayerNames();
        public List<string> NewLayerDefinitions { get; } = Defaults.DefaultLegacyNewLayers();
        public List<string> ExplodeLayers { get; } = new List<string>();
        public List<string> TextStyles { get; } = new List<string> { Defaults.LegacyTextStyle() };
        public List<string> ConversionRules { get; } = new List<string>();
        public List<Filter> RemoveRules { get; } = new List<Filter>();
        public List<string> LispCommands { get; } = new List<string>();
        public List<string> DllCommands { get; } = new List<string>();

        public void Clear()
        {
            BaseLayers.Clear();
            NewLayerNames.Clear();
            NewLayerDefinitions.Clear();
            ExplodeLayers.Clear();
            TextStyles.Clear();
            ConversionRules.Clear();
            RemoveRules.Clear();
            LispCommands.Clear();
            DllCommands.Clear();
        }
    }
}
