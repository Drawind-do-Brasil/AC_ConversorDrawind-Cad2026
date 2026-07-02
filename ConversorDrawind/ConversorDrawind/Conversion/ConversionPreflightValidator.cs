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
            if (!configuration.EXTCONFIsExchangeFormat)
                return ConversionPreflightResult.Success;

            string formatPath = configuration.EXTCONFOrigem == 1
                ? configuration.EXTCONFCaminhoBlocoInv
                : configuration.PROGRAMblockFormatoCaminho;

            if (!File.Exists(formatPath))
                return new ConversionPreflightResult(false, formatPath);

            return ConversionPreflightResult.Success;
        }
    }
}
