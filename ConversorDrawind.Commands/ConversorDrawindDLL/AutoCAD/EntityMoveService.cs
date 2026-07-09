using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawindDLL
{
    internal sealed class EntityMoveService
    {
        private readonly IAcadDocumentContext documentContext;
        private readonly IEntitySelector entitySelector;

        internal EntityMoveService(IAcadDocumentContext documentContext, IEntitySelector entitySelector)
        {
            this.documentContext = documentContext ?? throw new ArgumentNullException(nameof(documentContext));
            this.entitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
        }

        internal void MoveAll(Point3d startPoint, Point3d endPoint)
        {
            MoveAllBy(endPoint.GetVectorTo(startPoint));
        }

        internal void MoveAllToOrigin(Point3d minPoint)
        {
            MoveAllBy(new Vector3d(-minPoint.X, -minPoint.Y, -minPoint.Z));
        }

        private void MoveAllBy(Vector3d displacement)
        {
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                PromptSelectionResult selectionResult = entitySelector.SelectAll();
                if (selectionResult.Status == PromptStatus.OK)
                {
                    foreach (ObjectId id in selectionResult.Value.GetObjectIds())
                    {
                        Entity entity = transaction.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (entity != null)
                        {
                            entity.TransformBy(Matrix3d.Displacement(displacement));
                        }
                    }
                }

                transaction.Commit();
            }

            editor.Regen();
        }
    }
}
