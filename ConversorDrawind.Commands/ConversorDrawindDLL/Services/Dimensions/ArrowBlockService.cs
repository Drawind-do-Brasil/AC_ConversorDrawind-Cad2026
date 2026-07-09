using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace ConversorDrawind.Commands
{
    internal static class ArrowBlockService
    {
        private static readonly IReadOnlyDictionary<string, string> ArrowBlockNames =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Closed Filled", "." },
                { "Dot", "_DOT" },
                { "Dot Small", "_DOTSMALL" },
                { "Dot Blank", "_DOTBLANK" },
                { "Origin Indicator", "_ORIGIN" },
                { "Origin Indicator 2", "_ORIGIN2" },
                { "Open", "_OPEN" },
                { "Right Angle", "_OPEN90" },
                { "Open 30", "_OPEN30" },
                { "Closed", "_CLOSED" },
                { "Dot Small Blank", "_SMALL" },
                { "None", "_NONE" },
                { "Oblique", "_OBLIQUE" },
                { "Box Filled", "_BOXFILLED" },
                { "Box", "_BOXBLANK" },
                { "Closed Blank", "_CLOSEDBLANK" },
                { "Datum Triangle Filled", "_DATUMFILLED" },
                { "Datum Triangle", "_DATUMBLANK" },
                { "Integral", "_INTEGRAL" },
                { "Architectural Tick", "_ARCHTICK" }
            };

        private static readonly IReadOnlyDictionary<string, string> ArrowBlockNameStrings =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Closed Filled", "" },
                { "Dot", "DOT" },
                { "Dot Small", "DOTSMALL" },
                { "Dot Blank", "DOTBLANK" },
                { "Origin Indicator", "ORIGIN" },
                { "Origin Indicator 2", "ORIGIN2" },
                { "Open", "OPEN" },
                { "Right Angle", "OPEN90" },
                { "Open 30", "OPEN30" },
                { "Closed", "CLOSED" },
                { "Dot Small Blank", "SMALL" },
                { "None", "NONE" },
                { "Oblique", "OBLIQUE" },
                { "Box Filled", "BOXFILLED" },
                { "Box", "BOXBLANK" },
                { "Closed Blank", "CLOSEDBLANK" },
                { "Datum Triangle Filled", "DATUMFILLED" },
                { "Datum Triangle", "DATUMBLANK" },
                { "Integral", "INTEGRAL" },
                { "Architectural Tick", "ARCHTICK" }
            };

        internal static string GetArrowBlockName(string name)
        {
            if (name == null)
                throw new NullReferenceException();

            return ArrowBlockNames.TryGetValue(name, out string blockName) ? blockName : ".";
        }

        internal static string GetArrowBlockNameString(string name)
        {
            if (name == null)
                throw new NullReferenceException();

            return ArrowBlockNameStrings.TryGetValue(name, out string blockName) ? blockName : "";
        }

        internal static ObjectId GetArrowObjectId(
            string newArrowName,
            string systemVariableName,
            IAcadDocumentContext documentContext,
            ISystemVariableService systemVariables,
            Action<string, string> logError,
            string logContext)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                ObjectId arrowObjectId = ObjectId.Null;
                try
                {
                    string oldArrowName = GetArrowBlockName(newArrowName);
                    systemVariables.Set(systemVariableName, oldArrowName);

                    if (oldArrowName != ".")
                    {
                        BlockTable blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                        arrowObjectId = blockTable[oldArrowName];
                    }
                }
                catch (Exception e)
                {
                    logError(logContext, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }

                return arrowObjectId;
            }
        }
    }
}
