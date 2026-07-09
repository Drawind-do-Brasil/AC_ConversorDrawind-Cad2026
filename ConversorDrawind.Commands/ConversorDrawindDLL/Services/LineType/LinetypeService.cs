using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawindDLL
{
    internal static class LinetypeService
    {
        internal static bool ShouldSkipLinetype(string linetypeName)
        {
            return string.Equals(linetypeName, "ALL", StringComparison.OrdinalIgnoreCase);
        }

        internal static ObjectId LoadLinetype(Database database, string linetypeName)
        {
            if (ShouldSkipLinetype(linetypeName))
                return ObjectId.Null;

            LinetypeTable linetypeTable = database.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
            if (linetypeTable.Has(linetypeName) == false)
            {
                database.LoadLineTypeFile(linetypeName, CadPaths.FindFileNetload("acad.lin"));
            }

            if (linetypeTable.Has(linetypeName))
            {
                return linetypeTable[linetypeName];
            }

            return ObjectId.Null;
        }
    }
}
