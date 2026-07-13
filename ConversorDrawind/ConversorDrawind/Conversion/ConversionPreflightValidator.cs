using System.IO;

namespace ConversorDrawind
{
    internal sealed class ConversionPreflightResult
    {
        public static readonly ConversionPreflightResult Success = new ConversionPreflightResult(true, string.Empty);

        public ConversionPreflightResult(bool canConvert, string missingFormatPath)
        {
            CanConvert = canConvert;
            MissingFormatPath = missingFormatPath;
        }

        public bool CanConvert { get; private set; }
        public string MissingFormatPath { get; private set; }
    }

    internal static class ConversionPreflightValidator
    {
        public static ConversionPreflightResult ValidateFormatPath(Configuration configuration)
        {
            if (!configuration.General.ExchangeFormat)
                return ConversionPreflightResult.Success;

            string formatPath = configuration.General.SourceMode == 1
                ? configuration.Blocks.CadBlockPath
                : configuration.Blocks.TeklaBlockPath;

            if (!File.Exists(formatPath))
                return new ConversionPreflightResult(false, formatPath);

            return ConversionPreflightResult.Success;
        }
    }
}



