namespace ConversorDrawindDLL
{
    internal static class TestHooks
    {
        internal static void ResetGlobalState()
        {
            Configuration.INTREFTamFormato = 841;
            Configuration.Config = new Configuration();

            RuntimeConfigurationState.ResetWorkingStateFromConfiguration();

            InstanciaConversor.ConversorInstancias.Clear();

            ConvertBlocks.ResetForTests();
            FixArrow.ResetForTests();
        }
    }
}
