using System;
using System.IO;

namespace ConversorDrawind
{
    public partial class Configuration
    {
        private static readonly string DefaultTempDirectory = Path.Combine(Path.GetTempPath(), "ConversorDrawindTemp") + Path.DirectorySeparatorChar;

        public Configuration()
        {
            EnsureRuntimeDefaults();
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

            EnsureRuntimeDefaults();
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

        public string GetTempDirectory()
        {
            return string.IsNullOrWhiteSpace(Runtime.TempDirectory) ? DefaultTempDirectory : Runtime.TempDirectory;
        }

        public ConverterConfiguration ToConverterConfiguration()
        {
            SyncCompatibilityState();
            return new ConverterConfiguration(this);
        }

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

            EnsureRuntimeDefaults();
            InitializeCompatibilityState();
        }

        partial void InitializeCompatibilityState();

        partial void SyncCompatibilityState();

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
    }
}
