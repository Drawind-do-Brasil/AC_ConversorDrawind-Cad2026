using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConversorDrawind
{
    public partial class Configuration
    {
        private static readonly string DefaultTempDirectory = Path.Combine(Path.GetTempPath(), "ConversorDrawindTemp") + Path.DirectorySeparatorChar;

        public Configuration()
        {
            EnsureDefaults();
            InitializeCompatibilityState();
        }

        public Configuration(Configuration source)
        {
            if (source == null)
            {
                EnsureRuntimeDefaults();
                InitializeCompatibilityState();
                return;
            }

            Comments = source.Comments;
            General = source.General ?? new GeneralConfiguration();
            Dimensions = source.Dimensions ?? new DimensionConfiguration();
            Text = source.Text ?? new TextConfiguration();
            Scale = source.Scale ?? new ScaleConfiguration();
            Layers = source.Layers ?? new LayerConfiguration();
            Lines = source.Lines ?? new LineConfiguration();
            Commands = source.Commands ?? new CommandConfiguration();
            Blocks = source.Blocks ?? new BlockConfiguration();
            Runtime = source.Runtime ?? new RuntimeConfiguration();
            Catalogs = source.Catalogs ?? new CatalogConfiguration();

            EnsureDefaults();
            InitializeCompatibilityState();
        }

        public static Configuration Config { get; set; } = new Configuration();

        public string Comments { get; set; } = string.Empty;
        public GeneralConfiguration General { get; set; } = new GeneralConfiguration();
        public DimensionConfiguration Dimensions { get; set; } = new DimensionConfiguration();
        public TextConfiguration Text { get; set; } = new TextConfiguration();
        public ScaleConfiguration Scale { get; set; } = new ScaleConfiguration();
        public LayerConfiguration Layers { get; set; } = new LayerConfiguration();
        public LineConfiguration Lines { get; set; } = new LineConfiguration();
        public CommandConfiguration Commands { get; set; } = new CommandConfiguration();
        public BlockConfiguration Blocks { get; set; } = new BlockConfiguration();
        public RuntimeConfiguration Runtime { get; set; } = new RuntimeConfiguration();
        public CatalogConfiguration Catalogs { get; set; } = new CatalogConfiguration();

        public string GetTempDirectory()
        {
            return string.IsNullOrWhiteSpace(Runtime.TempDirectory) ? DefaultTempDirectory : Runtime.TempDirectory;
        }

#pragma warning disable CS0618
        public ConverterConfiguration ToConverterConfiguration()
        {
            SyncCompatibilityState();
            return new ConverterConfiguration(this);
        }
#pragma warning restore CS0618

        public void Apply(Configuration source)
        {
            source = source ?? new Configuration();

            Comments = source.Comments;
            General = source.General ?? new GeneralConfiguration();
            Dimensions = source.Dimensions ?? new DimensionConfiguration();
            Text = source.Text ?? new TextConfiguration();
            Scale = source.Scale ?? new ScaleConfiguration();
            Layers = source.Layers ?? new LayerConfiguration();
            Lines = source.Lines ?? new LineConfiguration();
            Commands = source.Commands ?? new CommandConfiguration();
            Blocks = source.Blocks ?? new BlockConfiguration();
            Runtime = source.Runtime ?? new RuntimeConfiguration();
            Catalogs = source.Catalogs ?? new CatalogConfiguration();

            EnsureDefaults();
            InitializeCompatibilityState();
        }

        partial void InitializeCompatibilityState();

        partial void SyncCompatibilityState();

        public void EnsureDefaults()
        {
            if (General == null) General = new GeneralConfiguration();
            if (Dimensions == null) Dimensions = new DimensionConfiguration();
            if (Text == null) Text = new TextConfiguration();
            if (Scale == null) Scale = new ScaleConfiguration();
            if (Layers == null) Layers = new LayerConfiguration();
            if (Lines == null) Lines = new LineConfiguration();
            if (Commands == null) Commands = new CommandConfiguration();
            if (Blocks == null) Blocks = new BlockConfiguration();
            if (Runtime == null) Runtime = new RuntimeConfiguration();
            if (Catalogs == null) Catalogs = new CatalogConfiguration();

            EnsureRuntimeDefaults();
            EnsureCatalogDefaults();
        }

        private void EnsureRuntimeDefaults()
        {
            if (string.IsNullOrWhiteSpace(Runtime.DbLineTypePath))
            {
                string applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Runtime.DbLineTypePath = Path.Combine(applicationData, @"Autodesk\AutoCAD 2026\R25.1\enu\support\") + @"acad.lin";
            }

            if (string.IsNullOrWhiteSpace(Runtime.TempDirectory))
                Runtime.TempDirectory = DefaultTempDirectory;
        }

        private void EnsureCatalogDefaults()
        {
            if (Catalogs.Colors == null) Catalogs.Colors = new List<string>();
            if (Catalogs.ObjectTypes == null) Catalogs.ObjectTypes = new List<string>();
            if (Catalogs.FilterLineTypes == null) Catalogs.FilterLineTypes = new List<string>();
            if (Catalogs.LayerLineTypes == null) Catalogs.LayerLineTypes = new List<string>();
            if (Catalogs.RemovedLineTypes == null) Catalogs.RemovedLineTypes = new List<string>();
            if (Text.Styles == null) Text.Styles = new List<TextStyleDefinition>();
            if (Layers.BaseLayers == null) Layers.BaseLayers = new List<string>();
            if (Lines.BaseLineTypes == null) Lines.BaseLineTypes = new List<string>();

            FillIfEmpty(Catalogs.Colors, Defaults.DefaultColors());
            FillIfEmpty(Catalogs.ObjectTypes, Defaults.DefaultObjectTypes());
            FillIfEmpty(Catalogs.FilterLineTypes, Defaults.DefaultFilterLineTypes());
            FillIfEmpty(Catalogs.LayerLineTypes, BuildDefaultLayerLineTypes());
            FillIfEmpty(Catalogs.RemovedLineTypes, Defaults.DefaultRemovedLineTypes());

            FillIfEmpty(Text.Styles, new List<TextStyleDefinition>
            {
                Defaults.TextStyle()
            });
            FillIfEmpty(Layers.BaseLayers, Defaults.DefaultBaseLayers());
            FillIfEmpty(Lines.BaseLineTypes, Catalogs.FilterLineTypes);

            if (Layers.NewLayers == null)
                Layers.NewLayers = new List<LayerDefinition>();

            FillIfEmpty(Layers.NewLayers, Defaults.DefaultNewLayers());
        }

        private List<string> BuildDefaultLayerLineTypes()
        {
            List<string> result = Defaults.DefaultLayerLineTypes();
            foreach (string lineType in ReadLineTypeNames(GetLineTypeCatalogPath()))
            {
                if (!result.Contains(lineType))
                    result.Add(lineType);
            }

            return result;
        }

        private string GetLineTypeCatalogPath()
        {
            string linPackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LinPack.nfj");
            if (File.Exists(linPackPath))
            {
                string configuredPath = File.ReadLines(linPackPath).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(configuredPath))
                    return configuredPath;
            }

            return Runtime.DbLineTypePath;
        }

        private static IEnumerable<string> ReadLineTypeNames(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
                yield break;

            foreach (string line in File.ReadLines(file))
            {
                if (line.Length > 0 && line[0] == '*')
                    yield return line.Remove(0, 1);
            }
        }

        private static void FillIfEmpty(List<string> target, IEnumerable<string> values)
        {
            if (target == null || target.Count > 0)
                return;

            target.AddRange(values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static void FillIfEmpty(List<TextStyleDefinition> target, IEnumerable<TextStyleDefinition> values)
        {
            if (target == null || target.Count > 0)
                return;

            target.AddRange(values);
        }

        private static void FillIfEmpty(List<LayerDefinition> target, IEnumerable<LayerDefinition> values)
        {
            if (target == null || target.Count > 0)
                return;

            target.AddRange(values);
        }
    }
}
