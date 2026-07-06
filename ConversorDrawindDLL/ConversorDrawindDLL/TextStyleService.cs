using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConversorDrawindDLL
{
    internal static class TextStyleService
    {
        internal static FontDescriptor CreateFontDescriptor(string font, bool italic, bool bold)
        {
            return new FontDescriptor(font, bold, italic, 0, 0);
        }

        internal static string ResolveStyleNameOrDefault(string name, IEnumerable<string> textStyles)
        {
            if (name == null || !textStyles.Contains(name))
                return textStyles.First().Split(':').First();

            return name;
        }

        internal static TextStyleDefinition ParseDefinition(string textStyle)
        {
            string[] itemSplit = textStyle.Split(':');
            return new TextStyleDefinition(
                itemSplit[0],
                itemSplit[1],
                itemSplit[2].ToBollean(),
                itemSplit[3].ToBollean(),
                itemSplit[5].ToDouble(),
                itemSplit[6].ToDouble());
        }

        internal static ObjectId GetStyleIdByName(IAcadDocumentContext documentContext, string name)
        {
            Database database = documentContext.Database;
            TextStyleTable textStyleTable = (TextStyleTable)database.TextStyleTableId.GetObject(OpenMode.ForWrite);
            TextStyleTableRecord textStyleTableRecord = null;
            ObjectId id = ObjectId.Null;
            if (textStyleTable.Has(name))
            {
                textStyleTableRecord = textStyleTable[name].GetObject(OpenMode.ForWrite) as TextStyleTableRecord;
            }

            id = textStyleTableRecord.ObjectId;

            return id;
        }

        internal static void CreateStyles(
            IAcadDocumentContext documentContext,
            IEnumerable<string> textStyles,
            Action<string, string> logError)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                TextStyleTable textStyleTable = (TextStyleTable)transaction.GetObject(database.TextStyleTableId, OpenMode.ForWrite);

                try
                {
                    foreach (string item in textStyles)
                    {
                        TextStyleDefinition textStyleDefinition = ParseDefinition(item);
                        TextStyleTableRecord textStyleTableRecord = GetOrCreateTextStyleRecord(
                            textStyleTable,
                            transaction,
                            textStyleDefinition.Name);

                        ApplyDefinition(textStyleTableRecord, textStyleDefinition);
                    }
                }
                catch (Exception e)
                {
                    logError("Erro 72", e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }
            }
        }

        private static TextStyleTableRecord GetOrCreateTextStyleRecord(
            TextStyleTable textStyleTable,
            Transaction transaction,
            string name)
        {
            if (textStyleTable.Has(name))
            {
                return transaction.GetObject(textStyleTable[name], OpenMode.ForWrite) as TextStyleTableRecord;
            }

            if (textStyleTable.IsWriteEnabled == false)
                textStyleTable.UpgradeOpen();

            TextStyleTableRecord textStyleTableRecord = new TextStyleTableRecord
            {
                Name = name
            };
            textStyleTable.Add(textStyleTableRecord);
            transaction.AddNewlyCreatedDBObject(textStyleTableRecord, true);

            return textStyleTableRecord;
        }

        private static void ApplyDefinition(
            TextStyleTableRecord textStyleTableRecord,
            TextStyleDefinition textStyleDefinition)
        {
            textStyleTableRecord.XScale = textStyleDefinition.XScale;
            textStyleTableRecord.Font = CreateFontDescriptor(
                textStyleDefinition.Font,
                textStyleDefinition.Italic,
                textStyleDefinition.Bold);

            string file = CadPaths.FindFileNetload(textStyleDefinition.Font + ".shx");
            if (File.Exists(file))
                textStyleTableRecord.FileName = file;

            textStyleTableRecord.ObliquingAngle = ConvertDimension.DegreeToRadian(textStyleDefinition.ObliquingAngleDegrees);
            textStyleTableRecord.TextSize = 0;
        }
    }

    internal sealed class TextStyleDefinition
    {
        internal TextStyleDefinition(
            string name,
            string font,
            bool italic,
            bool bold,
            double xScale,
            double obliquingAngleDegrees)
        {
            Name = name;
            Font = font;
            Italic = italic;
            Bold = bold;
            XScale = xScale;
            ObliquingAngleDegrees = obliquingAngleDegrees;
        }

        internal string Name { get; }

        internal string Font { get; }

        internal bool Italic { get; }

        internal bool Bold { get; }

        internal double XScale { get; }

        internal double ObliquingAngleDegrees { get; }
    }
}
