namespace ConversorDrawind.Commands
{
    internal static class TestHooks
    {
        internal static void ResetGlobalState()
        {
            Configuration.ReferenceFormatSize = 841;
            Configuration.Config = new Configuration();

            RuntimeConfigurationState.ResetWorkingStateFromConfiguration();

            InstanciaConversor.ConversorInstancias.Clear();

            ConvertBlocks.ResetForTests();
            FixArrow.ResetForTests();
        }
    }
}
