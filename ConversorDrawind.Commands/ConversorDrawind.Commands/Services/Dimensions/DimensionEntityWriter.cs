using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawind.Commands
{
    internal sealed class DimensionEntityWriter
    {
        private readonly Database database;
        private readonly Transaction transaction;

        internal DimensionEntityWriter(Database database, Transaction transaction)
        {
            this.database = database;
            this.transaction = transaction;
        }

        internal BlockTableRecord GetModelSpaceForWrite()
        {
            BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
            return transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        }

        internal void Append(Entity entity)
        {
            BlockTableRecord blockTableRecord = GetModelSpaceForWrite();
            blockTableRecord.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);
        }

        internal void CreateLine(Point3d p1, Point3d p2)
        {
            Line line = DimensionEntityFactory.CreateLine(
                p1,
                p2,
                Configuration.Config.Dimensions.Layer,
                LayerSetupOperations.GetColorForName(Configuration.Config.Dimensions.LineColor));
            Append(line);
        }

        internal void CreateRotatedDimension(DimensionProperties dimensionProperties, ObjectId dimStyle)
        {
            RotatedDimension rotatedDimension = DimensionEntityFactory.CreateRotatedDimension(
                dimensionProperties,
                dimStyle,
                Configuration.Config.Dimensions.Layer,
                LayerSetupOperations.GetColorForName(Configuration.Config.Dimensions.LineColor));
            Append(rotatedDimension);
        }

        internal void CreateAngularDimension(DimensionProperties dimensionProperties, ObjectId dimStyle)
        {
            Point3AngularDimension dimension = DimensionEntityFactory.CreateAngularDimension(
                dimensionProperties,
                dimStyle,
                Configuration.Config.Dimensions.Layer,
                LayerSetupOperations.GetColorForName(Configuration.Config.Dimensions.LineColor));
            Append(dimension);
        }

        internal void CreateAngularDimensionWithLargeGap(DimensionProperties dimensionProperties, ObjectId dimStyle)
        {
            Point3AngularDimension dimension = DimensionEntityFactory.CreateAngularDimensionWithLargeGap(
                dimensionProperties,
                dimStyle,
                Configuration.Config.Dimensions.Layer,
                LayerSetupOperations.GetColorForName(Configuration.Config.Dimensions.LineColor),
                Configuration.Config.Dimensions.ArrowSize);
            Append(dimension);
        }

        internal void EraseBlockReferences(BlockTableRecord blockTableRecord)
        {
            ObjectIdCollection blockReferenceIds = blockTableRecord.GetBlockReferenceIds(true, true);
            foreach (ObjectId item in blockReferenceIds)
            {
                BlockReference blockReference = item.GetObject(OpenMode.ForWrite) as BlockReference;
                blockReference.Erase();
            }
        }
    }
}
