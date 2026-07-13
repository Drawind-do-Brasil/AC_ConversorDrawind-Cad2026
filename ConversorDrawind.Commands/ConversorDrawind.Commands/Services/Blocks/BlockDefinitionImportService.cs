using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.IO;

namespace ConversorDrawind.Commands
{
    internal sealed class BlockDefinitionImportService
    {
        private readonly Database database;

        internal BlockDefinitionImportService(Database database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        internal void ImportFromDrawing(string blockName, string sourceDrawingPath)
        {
            if (string.IsNullOrWhiteSpace(blockName))
                throw new InvalidOperationException(Localization.MessageBlockNameRequired);

            if (string.IsNullOrWhiteSpace(sourceDrawingPath))
                throw new InvalidOperationException(Localization.MessageSourceDrawingRequired);

            using (Database sourceDatabase = new Database(false, true))
            {
                sourceDatabase.ReadDwgFile(sourceDrawingPath, FileShare.Read, true, string.Empty);
                database.Insert(blockName, sourceDatabase, true);
            }
        }
    }
}
