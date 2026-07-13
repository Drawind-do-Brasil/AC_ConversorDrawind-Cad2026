using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawind.Commands
{
    static class SymbolTableCleanup
    {
        private static void PurgeSymbolTableRecords(
            Func<Database, ObjectId> getTableId,
            string message,
            string eraseLogContext,
            string purgeLogContext)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEditorMessenger messenger = new AcadEditorMessenger(documentContext.Editor);
            new SymbolTablePurgeService(documentContext, messenger, ConversionLog.Write)
                .PurgeSymbolTableRecords(getTableId, message, eraseLogContext, purgeLogContext);
        }

        public static void PurgeDimensionSyles()
        {
            PurgeSymbolTableRecords(
                database => database.DimStyleTableId,
                Localization.MessagePurgingDimensionStyles,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeTextSyles()
        {
            PurgeSymbolTableRecords(
                database => database.TextStyleTableId,
                Localization.MessagePurgingTextStyles,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeUnreferencedLineTypes()
        {
            PurgeSymbolTableRecords(
                database => database.LinetypeTableId,
                Localization.MessagePurgingLineTypes,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeUnreferencedLayers()
        {
            PurgeSymbolTableRecords(
                database => database.LayerTableId,
                Localization.MessagePurgingLayers,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }

        public static void PurgeUnreferencedBlocks()
        {
            PurgeSymbolTableRecords(
                database => database.BlockTableId,
                Localization.MessagePurgingBlocks,
                LogContext.RemoverReferenciasDeTabela,
                LogContext.RemoverReferenciasDeTabela);
        }
    }
}
