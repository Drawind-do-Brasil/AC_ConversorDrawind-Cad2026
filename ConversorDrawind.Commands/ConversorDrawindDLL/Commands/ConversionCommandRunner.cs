using Autodesk.AutoCAD.ApplicationServices;
using System;

namespace ConversorDrawindDLL
{
    internal sealed class ConversionCommandRunner
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly Action<string, string> logError;
        private readonly Action<string> showWarning;

        internal ConversionCommandRunner(
            IAcadDocumentContext documentContext,
            Action<string, string> logError,
            Action<string> showWarning)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
            this.showWarning = showWarning ?? throw new ArgumentNullException(nameof(showWarning));
        }

        internal ConversionCommandContext CreateContext()
        {
            IEditorMessenger messenger = new AcadEditorMessenger(documentContext.Editor);
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            ScaleWorkflow scaleWorkflow = new ScaleWorkflow(systemVariables);
            ConversionStepRunner stepRunner = new ConversionStepRunner(messenger, logError, showWarning);

            return new ConversionCommandContext(
                documentContext,
                messenger,
                systemVariables,
                scaleWorkflow,
                stepRunner);
        }

        internal void WriteStartupBanner(IEditorMessenger messenger)
        {
            messenger.WriteMessage("\n" + Localization.AppCopyright + "\n");
            messenger.WriteMessage(Localization.AppDevelopedBy + "\n");
            messenger.WriteMessage(Localization.AppCompatibilityAutoCad2023 + "\n");
        }

        internal void InitializeLogger(Document document, ref string logDirectory, ref string logFileName)
        {
            ConversionLogger.InitializeForDocument(document, ref logDirectory, ref logFileName);
        }

        internal void LoadTempConfiguration(Configuration configuration, ref string converterName)
        {
            ConversionPreflight.LoadTempConfiguration(configuration, ref converterName);
        }
    }

    internal sealed class ConversionCommandContext
    {
        internal ConversionCommandContext(
            IAcadDocumentContext documentContext,
            IEditorMessenger messenger,
            ISystemVariableService systemVariables,
            ScaleWorkflow scaleWorkflow,
            ConversionStepRunner stepRunner)
        {
            DocumentContext = documentContext;
            Messenger = messenger;
            SystemVariables = systemVariables;
            ScaleWorkflow = scaleWorkflow;
            StepRunner = stepRunner;
        }

        internal IAcadDocumentContext DocumentContext { get; }

        internal IEditorMessenger Messenger { get; }

        internal ISystemVariableService SystemVariables { get; }

        internal ScaleWorkflow ScaleWorkflow { get; }

        internal ConversionStepRunner StepRunner { get; }
    }
}
