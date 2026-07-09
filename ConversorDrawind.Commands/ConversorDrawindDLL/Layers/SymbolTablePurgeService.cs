using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawindDLL
{
    internal sealed class SymbolTablePurgeService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly IEditorMessenger messenger;
        private readonly Action<string, string> logError;

        internal SymbolTablePurgeService(
            IAcadDocumentContext documentContext,
            IEditorMessenger messenger,
            Action<string, string> logError)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            this.logError = logError ?? throw new ArgumentNullException(nameof(logError));
        }

        internal void PurgeSymbolTableRecords(
            Func<Database, ObjectId> getTableId,
            string message,
            string eraseLogContext,
            string purgeLogContext)
        {
            Database database = documentContext.Database;

            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                try
                {
                    SymbolTable symbolTable = transaction.GetObject(getTableId(database), OpenMode.ForRead) as SymbolTable;
                    ObjectIdCollection objectIds = new ObjectIdCollection();
                    foreach (ObjectId objectId in symbolTable)
                    {
                        objectIds.Add(objectId);
                    }
                    database.Purge(objectIds);
                    messenger.WriteMessage(message);
                    foreach (ObjectId objectId in objectIds)
                    {
                        SymbolTableRecord symbolTableRecord = transaction.GetObject(objectId, OpenMode.ForWrite) as SymbolTableRecord;

                        try
                        {
                            symbolTableRecord.Erase(true);
                            messenger.WriteMessage(symbolTableRecord.Name.ToUpper() + "  ");
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            logError(eraseLogContext, ex.Message);
                            Application.ShowAlertDialog("Erro:\n" + ex.Message);
                        }
                    }
                    messenger.WriteMessage("\n");
                }
                catch (Exception e)
                {
                    logError(purgeLogContext, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }
    }
}
