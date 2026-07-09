using System;

namespace ConversorDrawindDLL
{
    internal sealed class ConversionStepRunner
    {
        private readonly IEditorMessenger messenger;
        private readonly Action<string, string> logError;
        private readonly Action<string> showWarning;

        internal ConversionStepRunner(
            IEditorMessenger messenger,
            Action<string, string> logError,
            Action<string> showWarning)
        {
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
            this.showWarning = showWarning ?? throw new ArgumentNullException(nameof(showWarning));
        }

        internal void Run(
            string startMessage,
            Action action,
            string logContext,
            string warningMessage,
            string errorDescription,
            string completedMessage = null)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                messenger.WriteMessage(startMessage);
                action();
                messenger.WriteMessage(completedMessage ?? Localization.MessageCompletedLowercase + "\n");
            }
            catch (Exception e)
            {
                logError(logContext, e.Message);

                if (!string.IsNullOrEmpty(warningMessage))
                    showWarning(warningMessage);

                messenger.WriteMessage(Localization.MessageFailedPrefix + " \n" + errorDescription);
            }
        }
    }
}
