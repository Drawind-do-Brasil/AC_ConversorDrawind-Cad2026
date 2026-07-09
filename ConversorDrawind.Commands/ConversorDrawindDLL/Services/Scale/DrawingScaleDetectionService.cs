using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Linq;

namespace ConversorDrawindDLL
{
    internal sealed class DrawingScaleDetectionService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly IEntitySelector entitySelector;

        internal DrawingScaleDetectionService(IAcadDocumentContext documentContext, IEntitySelector entitySelector)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
        }

        internal double? CaptureScale()
        {
            TypedValue[] filterValues =
            {
                new TypedValue((int)DxfCode.Start, "INSERT, TEXT, MTEXT")
            };

            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                PromptSelectionResult selectionResult = entitySelector.SelectAll(new SelectionFilter(filterValues));
                if (selectionResult.Status != PromptStatus.OK)
                {
                    transaction.Commit();
                    return null;
                }

                foreach (ObjectId id in selectionResult.Value.GetObjectIds())
                {
                    Entity entity = transaction.GetObject(id, OpenMode.ForRead) as Entity;
                    double? scale = TryCaptureScale(transaction, entity);
                    if (scale.HasValue)
                    {
                        transaction.Commit();
                        return scale.Value;
                    }
                }

                transaction.Commit();
                return null;
            }
        }

        private double? TryCaptureScale(Transaction transaction, Entity entity)
        {
            try
            {
                DBText text = entity as DBText;
                if (text != null)
                {
                    return TryReadScale(text, text.Position);
                }

                BlockReference blockReference = entity as BlockReference;
                if (blockReference == null)
                {
                    return null;
                }

                BlockTableRecord blockDefinition = transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId objectId in blockDefinition)
                {
                    if (!objectId.ObjectClass.DxfName.Equals("TEXT", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    text = transaction.GetObject(objectId, OpenMode.ForRead) as DBText;
                    double? scale = TryReadScale(text, text.Position.TransformBy(blockReference.BlockTransform));
                    if (scale.HasValue)
                    {
                        return scale.Value;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static double? TryReadScale(DBText text, Point3d positionInSpace)
        {
            if (!string.Equals(text.Layer, Configuration.Config.Scale.Layer, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!ConvertBlocks.CheckPoint(
                    positionInSpace,
                    ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.Scale.Point1.X, Configuration.Config.Scale.Point1.Y, Configuration.Config.Scale.Point1.Z)),
                    ConvertBlocks.GetPTReal(new Point3d(Configuration.Config.Scale.Point2.X, Configuration.Config.Scale.Point2.Y, Configuration.Config.Scale.Point2.Z))))
            {
                return null;
            }

            if (text.Height <= Configuration.Config.Scale.TextSize - 0.2 ||
                text.Height >= Configuration.Config.Scale.TextSize + 0.2)
            {
                return null;
            }

            string[] parts = text.TextString.Split(':');
            double scale;
            if (double.TryParse(parts.Last().ReplaceComma(), out scale))
            {
                return scale;
            }

            return null;
        }
    }
}
