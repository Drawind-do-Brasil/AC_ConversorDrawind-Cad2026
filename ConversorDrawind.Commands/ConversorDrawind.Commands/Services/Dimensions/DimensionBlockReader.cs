using Autodesk.AutoCAD.DatabaseServices;

namespace ConversorDrawind.Commands
{
    internal static class DimensionBlockReader
    {
        internal static ObjectsInBlock Read(BlockTableRecord blockTableRecord, BlockReference blockReference, ObjectId dimStyle)
        {
            ObjectsInBlock objectsInBlock = ReadEntities(blockTableRecord);
            objectsInBlock.matrix3d = blockReference.BlockTransform;
            objectsInBlock.textStyle = DrawingStyleOperations.GetTextSyleByName(Configuration.Config.Text.DefaultStyleName);
            objectsInBlock.dimStyle = dimStyle;
            return objectsInBlock;
        }

        private static ObjectsInBlock ReadEntities(BlockTableRecord blockTableRecord)
        {
            ObjectsInBlock objectsInBlock = new ObjectsInBlock();
            foreach (ObjectId item in blockTableRecord)
            {
                DBObject dBObject = (DBObject)item.GetObject(OpenMode.ForRead);
                if (dBObject.GetType() == typeof(Line))
                    objectsInBlock.lineList.Add((Line)dBObject);
                else if (dBObject.GetType() == typeof(DBText))
                    objectsInBlock.dBTextList.Add((DBText)dBObject);
                else if (dBObject.GetType() == typeof(Arc))
                    objectsInBlock.arcList.Add((Arc)dBObject);
                else if (dBObject.GetType() == typeof(Hatch))
                    objectsInBlock.hatchList.Add((Hatch)dBObject);
            }

            return objectsInBlock;
        }
    }
}
