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
            string errorCode,
            string warningMessage,
            string errorDescription,
            string completedMessage = "... completado.\n")
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                messenger.WriteMessage(startMessage);
                action();
                messenger.WriteMessage(completedMessage);
            }
            catch (Exception e)
            {
                logError(errorCode, e.Message);

                if (!string.IsNullOrEmpty(warningMessage))
                    showWarning(warningMessage);

                messenger.WriteMessage("... Erro. \n" + errorDescription);
            }
        }
    }
}
