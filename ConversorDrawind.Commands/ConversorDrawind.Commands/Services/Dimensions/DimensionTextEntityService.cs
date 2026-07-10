using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawind.Commands
{
    internal sealed class DimensionTextEntityService
    {
        private readonly Database database;
        private readonly Transaction transaction;
        private readonly Configuration configuration;

        internal DimensionTextEntityService(Database database, Transaction transaction, Configuration configuration)
        {
            this.database = database;
            this.transaction = transaction;
            this.configuration = configuration;
        }

        internal Point3d CalculateAlignedTextPosition(DBText sourceText, Matrix3d transform, BlockTableRecord modelSpace)
        {
            Point3d textPositionPoint = sourceText.Position.TransformBy(transform);
            DBText positionText = CreateAlignmentProbeText(sourceText, textPositionPoint);

            positionText.AdjustAlignment(database);
            double difX = positionText.AlignmentPoint.X - positionText.Position.X;
            double difY = positionText.AlignmentPoint.Y - positionText.Position.Y;

            modelSpace.AppendEntity(positionText);
            transaction.AddNewlyCreatedDBObject(positionText, true);
            positionText.Erase();

            return new Point3d(textPositionPoint.X + difX, textPositionPoint.Y + difY, textPositionPoint.Z);
        }

        internal void CopyAdditionalTextEntities(ObjectsInBlock objectsInBlock, double rotation, BlockTableRecord modelSpace)
        {
            for (int i = 1; i < objectsInBlock.dBTextList.Count; i++)
            {
                DBText sourceText = objectsInBlock.dBTextList[i];
                DBText copiedText = CreateAdditionalText(sourceText, objectsInBlock.matrix3d, objectsInBlock.textStyle, rotation);

                copiedText.AdjustAlignment(database);
                modelSpace.AppendEntity(copiedText);
                transaction.AddNewlyCreatedDBObject(copiedText, true);
            }
        }

        private DBText CreateAlignmentProbeText(DBText sourceText, Point3d position)
        {
            DBText positionText = new DBText();

            positionText.SetDatabaseDefaults();
            positionText.Justify = sourceText.Justify;
            positionText.TextString = sourceText.TextString;
            positionText.TextStyleId = sourceText.TextStyleId;
            positionText.Height = sourceText.Height;
            positionText.Rotation = sourceText.Rotation;
            positionText.WidthFactor = sourceText.WidthFactor;
            positionText.HorizontalMode = TextHorizontalMode.TextCenter;
            positionText.VerticalMode = TextVerticalMode.TextVerticalMid;
            positionText.AlignmentPoint = position;
            positionText.Position = position;

            return positionText;
        }

        private DBText CreateAdditionalText(DBText sourceText, Matrix3d transform, ObjectId textStyle, double rotation)
        {
            DBText copiedText = new DBText();

            copiedText.SetDatabaseDefaults();
            copiedText.Justify = sourceText.Justify;
            copiedText.TextString = sourceText.TextString;
            copiedText.TextStyleId = sourceText.TextStyleId;
            copiedText.Height = sourceText.Height;
            copiedText.Rotation = rotation;
            copiedText.WidthFactor = sourceText.WidthFactor;
            copiedText.TextStyleId = textStyle;
            copiedText.Layer = configuration.Dimensions.Layer;
            copiedText.Color = LayerSetupOperations.GetColorForName(configuration.Dimensions.TextColor);
            copiedText.Position = sourceText.Position.TransformBy(transform);

            return copiedText;
        }
    }
}
