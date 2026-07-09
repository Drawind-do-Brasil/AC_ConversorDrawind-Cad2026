using System.Collections.Generic;

namespace ConversorDrawind
{
    [System.Obsolete("Use Defaults for template values and Configuration for runtime state. Arranjos remains only for legacy XML compatibility.")]
    public class Arranjos
    {
        public List<string> allBaseLayer = new List<string>();
        public List<string> allNewLayer = new List<string>();
        public List<string> allNewLayerComposition = new List<string>();
        public List<string> allExplodeLayers = new List<string>();
        public List<string> allTextSyles = new List<string>();

        public List<string> conversor = new List<string>();
        public List<Filter> layerRemove = new List<Filter>();
        public List<string> listLISPCommand = new List<string>();
        public List<string> listDLLCommand = new List<string>();


        public Arranjos()
        {
            Load();
        }

        private void Load()
        {
            allTextSyles.Add(Defaults.LegacyTextStyle());
            allNewLayer.AddRange(Defaults.DefaultNewLayerNames());
            allNewLayerComposition.AddRange(Defaults.DefaultLegacyNewLayers());
            allBaseLayer.AddRange(Defaults.DefaultBaseLayers());
        }
    }
}





