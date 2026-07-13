using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawind.Commands
{
    internal sealed class DrawingExtentsService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly IEntitySelector entitySelector;
        private readonly Point minPoint;
        private readonly Point maxPoint;

        internal DrawingExtentsService(
            IAcadDocumentContext documentContext,
            IEntitySelector entitySelector,
            Point minPoint,
            Point maxPoint)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
            this.minPoint = minPoint ?? throw new ArgumentNullException(nameof(minPoint));
            this.maxPoint = maxPoint ?? throw new ArgumentNullException(nameof(maxPoint));
        }

        internal void Refresh()
        {
            if (!TryRefreshTeklaDrawingSheet())
            {
                RefreshGeneral();
            }

            if (IsEmpty())
            {
                RefreshGeneral();
            }
        }

        internal bool TryRefreshTeklaDrawingSheet()
        {
            return Refresh(new SelectionFilter(LayerFilterFactory.TeklaDrawingSheetInsert()), includeBlockLinesOnly: true);
        }

        internal void RefreshGeneral()
        {
            Refresh(new SelectionFilter(LayerFilterFactory.GeneralDrawingEntities()), includeBlockLinesOnly: false);
        }

        internal Point3d GetMinPoint()
        {
            return new Point3d(minPoint.X, minPoint.Y, minPoint.Z);
        }

        internal Point3d GetMaxPoint()
        {
            return new Point3d(maxPoint.X, maxPoint.Y, maxPoint.Z);
        }

        private bool Refresh(SelectionFilter filter, bool includeBlockLinesOnly)
        {
            Database database = documentContext.Database;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                PromptSelectionResult selectionResult = entitySelector.SelectAll(filter);
                if (selectionResult.Status != PromptStatus.OK)
                {
                    transaction.Commit();
                    return false;
                }

                foreach (ObjectId id in selectionResult.Value.GetObjectIds())
                {
                    Entity entity = transaction.GetObject(id, OpenMode.ForRead) as Entity;
                    if (entity == null)
                    {
                        continue;
                    }

                    BlockReference blockReference = entity as BlockReference;
                    if (includeBlockLinesOnly && blockReference != null)
                    {
                        IncludeBlockLineExtents(transaction, blockReference);
                    }
                    else
                    {
                        IncludeEntityExtents(entity);
                    }
                }

                transaction.Commit();
                return true;
            }
        }

        private void IncludeBlockLineExtents(Transaction transaction, BlockReference blockReference)
        {
            BlockTableRecord blockDefinition = transaction.GetObject(blockReference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            foreach (ObjectId objectId in blockDefinition)
            {
                Entity entity = transaction.GetObject(objectId, OpenMode.ForRead) as Entity;
                if (entity is Line)
                {
                    IncludeEntityExtents(entity, blockReference.BlockTransform);
                }
            }
        }

        private void IncludeEntityExtents(Entity entity)
        {
            IncludeEntityExtents(entity, Matrix3d.Identity);
        }

        private void IncludeEntityExtents(Entity entity, Matrix3d transform)
        {
            try
            {
                Extents3d extents = (Extents3d)entity.Bounds;
                Include(extents.MinPoint.TransformBy(transform), extents.MaxPoint.TransformBy(transform));
            }
            catch (Exception)
            {
            }
        }

        private void Include(Point3d min, Point3d max)
        {
            if (min.X < minPoint.X) minPoint.X = min.X;
            if (min.Y < minPoint.Y) minPoint.Y = min.Y;
            if (min.Z < minPoint.Z) minPoint.Z = min.Z;

            if (max.X > maxPoint.X) maxPoint.X = max.X;
            if (max.Y > maxPoint.Y) maxPoint.Y = max.Y;
            if (max.Z > maxPoint.Z) maxPoint.Z = max.Z;
        }

        private bool IsEmpty()
        {
            return minPoint.X == double.MaxValue ||
                   minPoint.Y == double.MaxValue ||
                   minPoint.Z == double.MaxValue ||
                   maxPoint.X == double.MinValue ||
                   maxPoint.Y == double.MinValue ||
                   maxPoint.Z == double.MinValue;
        }
    }
}
