using Autodesk.AutoCAD.DatabaseServices;

namespace ConversorDrawind.Commands
{
    public static class TransactionAnalizer
    {
        public static Transaction MyStartTransaction(this TransactionManager transactionManager)
        {
            return transactionManager.StartTransaction();
        }

        public static void MyCommit(this Transaction transaction)
        {
            transaction.Commit();
        }
    }
}
