namespace ConversorDrawindDLL
{
    internal static class TestHooks
    {
        internal static void ResetGlobalState()
        {
            Configuration.INTREFTamFormato = 841;
            Configuration.Config = new Configuration();

            Arranjos.Arrj = new Arranjos();
            Arranjos.ListBlocks.Clear();
            Arranjos.ListBlocksInv.Clear();
            Arranjos.ListBlocksOrig.Clear();

            InstanciaConversor.ConversorInstancias.Clear();

            ConvertBlocks.ResetForTests();
            FixArrow.ResetForTests();
        }
    }
}
