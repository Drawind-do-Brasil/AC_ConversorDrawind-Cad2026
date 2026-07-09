namespace ConversorDrawindDLL.Tests
{
    public class ConversionStepRunnerTests
    {
        [Fact]
        public void Run_WhenActionSucceeds_WritesStartAndCompletedMessages()
        {
            var messenger = new FakeEditorMessenger();
            var logs = new List<string>();
            var warnings = new List<string>();
            var runner = new ConversionStepRunner(
                messenger,
                (context, message) => logs.Add(context + ":" + message),
                warnings.Add);

            runner.Run(
                "Iniciando ",
                () => messenger.Events.Add("action"),
                "Contexto de teste",
                "Aviso",
                "Descrição de erro\n");

            Assert.Equal(new[] { "Iniciando ", "action", Localization.MessageCompletedLowercase + "\n" }, messenger.Events);
            Assert.Empty(logs);
            Assert.Empty(warnings);
        }

        [Fact]
        public void Run_WhenActionFails_LogsWarnsAndWritesReadableContext()
        {
            var messenger = new FakeEditorMessenger();
            var logs = new List<string>();
            var warnings = new List<string>();
            var runner = new ConversionStepRunner(
                messenger,
                (context, message) => logs.Add(context + ":" + message),
                warnings.Add);

            runner.Run(
                "Iniciando ",
                () => throw new InvalidOperationException("Falhou"),
                "Contexto de teste",
                "Aviso",
                "Descrição de erro\n");

            Assert.Equal(new[] { "Iniciando ", Localization.MessageFailedPrefix + " \nDescrição de erro\n" }, messenger.Events);
            Assert.Equal(new[] { "Contexto de teste:Falhou" }, logs);
            Assert.Equal(new[] { "Aviso" }, warnings);
        }

        private sealed class FakeEditorMessenger : IEditorMessenger
        {
            internal List<string> Events { get; } = new List<string>();

            public void WriteMessage(string message)
            {
                Events.Add(message);
            }
        }
    }
}
