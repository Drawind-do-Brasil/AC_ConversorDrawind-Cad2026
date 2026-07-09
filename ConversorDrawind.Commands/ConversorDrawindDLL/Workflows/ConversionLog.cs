using System;

namespace ConversorDrawind.Commands
{
    internal static class ConversionLog
    {
        internal static void Write(string context, string detail)
        {
            ConversionLogger.Write(
                ConversionSession.LogDirectory,
                ConversionSession.LogFileName,
                context,
                detail);
        }

        internal static void Write(string context, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            Write(context, exception.Message);
        }
    }
}
