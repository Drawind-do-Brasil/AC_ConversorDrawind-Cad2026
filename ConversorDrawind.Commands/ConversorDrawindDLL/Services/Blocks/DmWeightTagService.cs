using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Text.RegularExpressions;

namespace ConversorDrawindDLL
{
    internal sealed class DmWeightTagService
    {
        private static readonly Regex WeightTagPattern = new Regex(
            "%DM%[0-9 ]*[,.]{0,1}[0-9 ]*%DM%",
            RegexOptions.Compiled);

        internal string ExtractFirstWeightAndNormalize(Transaction transaction, ObjectId[] objectIds)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (objectIds == null)
                return string.Empty;

            foreach (ObjectId objectId in objectIds)
            {
                string weight = ExtractWeightAndNormalize(transaction, objectId);
                if (!string.IsNullOrWhiteSpace(weight))
                    return weight;
            }

            return string.Empty;
        }

        private string ExtractWeightAndNormalize(Transaction transaction, ObjectId objectId)
        {
            Entity entity = transaction.GetObject(objectId, OpenMode.ForRead) as Entity;
            if (entity == null)
                return null;

            DBText text = entity as DBText;
            if (text != null)
                return ExtractFromText(text);

            BlockReference blockReference = entity as BlockReference;
            if (blockReference != null)
                return ExtractFromBlock(transaction, blockReference);

            return null;
        }

        private string ExtractFromText(DBText text)
        {
            Match match = WeightTagPattern.Match(text.TextString);
            if (!match.Success)
                return null;

            string originalValue = match.Value;
            string weight = originalValue.Replace("%DM%", "").Trim();

            text.UpgradeOpen();
            text.TextString = text.TextString.Replace(originalValue, weight);
            text.DowngradeOpen();

            return weight;
        }

        private string ExtractFromBlock(Transaction transaction, BlockReference blockReference)
        {
            BlockTableRecord blockDefinition = transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            foreach (ObjectId nestedId in blockDefinition)
            {
                string weight = ExtractWeightAndNormalize(transaction, nestedId);
                if (!string.IsNullOrWhiteSpace(weight))
                    return weight;
            }

            return null;
        }
    }
}
