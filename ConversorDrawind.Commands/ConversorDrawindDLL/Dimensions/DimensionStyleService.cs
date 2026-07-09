using Autodesk.AutoCAD.DatabaseServices;

namespace ConversorDrawindDLL
{
    internal static class DimensionStyleService
    {
        internal static ObjectId CreateCurrentStyle(IAcadDocumentContext documentContext, Configuration configuration)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                ObjectId id = ObjectId.Null;
                try
                {
                    DimStyleTable dimStyleTable = (DimStyleTable)transaction.GetObject(database.DimStyleTableId, OpenMode.ForWrite);
                    DimStyleTableRecord dimStyleTableRecord = GetOrCreateStyleRecord(
                        dimStyleTable,
                        transaction,
                        configuration.Dimensions.StyleName);

                    ApplyConfiguredStyle(dimStyleTableRecord, configuration);
                    id = dimStyleTableRecord.ObjectId;

                    if (dimStyleTableRecord.ObjectId != database.Dimstyle)
                    {
                        database.Dimstyle = dimStyleTableRecord.ObjectId;
                        database.SetDimstyleData(dimStyleTableRecord);
                    }
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.CriarEstiloDeCota, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }

                return id;
            }
        }

        internal static ObjectId UpdateAllStyles(IAcadDocumentContext documentContext, Configuration configuration)
        {
            Database database = documentContext.Database;
            using (Transaction transaction = database.TransactionManager.MyStartTransaction())
            {
                ObjectId id = ObjectId.Null;
                try
                {
                    DimStyleTable dimStyleTable = (DimStyleTable)transaction.GetObject(database.DimStyleTableId, OpenMode.ForRead);
                    foreach (ObjectId item in dimStyleTable)
                    {
                        DimStyleTableRecord dimStyleTableRecord = transaction.GetObject(
                            item,
                            OpenMode.ForWrite) as DimStyleTableRecord;

                        ApplyCommonStyleUpdates(dimStyleTableRecord, configuration);
                    }
                }
                catch (System.Exception e)
                {
                    ConversionLog.Write(LogContext.AplicarEstiloDeCota, e.Message);
                }
                finally
                {
                    transaction.MyCommit();
                }

                return id;
            }
        }

        private static DimStyleTableRecord GetOrCreateStyleRecord(
            DimStyleTable dimStyleTable,
            Transaction transaction,
            string styleName)
        {
            if (dimStyleTable.Has(styleName))
                return transaction.GetObject(dimStyleTable[styleName], OpenMode.ForWrite) as DimStyleTableRecord;

            if (dimStyleTable.IsWriteEnabled == false)
                dimStyleTable.UpgradeOpen();

            DimStyleTableRecord dimStyleTableRecord = new DimStyleTableRecord
            {
                Name = styleName
            };
            dimStyleTable.Add(dimStyleTableRecord);
            transaction.AddNewlyCreatedDBObject(dimStyleTableRecord, true);

            return dimStyleTableRecord;
        }

        private static void ApplyConfiguredStyle(DimStyleTableRecord dimStyleTableRecord, Configuration configuration)
        {
            dimStyleTableRecord.Dimtxsty = ConvertLayer.GetTextSyleByName(configuration.Text.DefaultStyleName);
            dimStyleTableRecord.Dimtxt = configuration.Text.DefaultSize;
            dimStyleTableRecord.Dimscale = configuration.Dimensions.Scale;
            dimStyleTableRecord.Dimdec = configuration.Dimensions.Precision;
            dimStyleTableRecord.Dimadec = configuration.Dimensions.AngularPrecision;
            dimStyleTableRecord.Dimlunit = configuration.Dimensions.Unit;
            dimStyleTableRecord.Dimaunit = configuration.Dimensions.AngularUnit;
            dimStyleTableRecord.Dimtad = configuration.Dimensions.TextVerticalPosition;
            dimStyleTableRecord.Dimtih = configuration.Dimensions.TextRelativeToDimensionLine;
            dimStyleTableRecord.Dimtix = configuration.Dimensions.ForceTextInside;
            dimStyleTableRecord.Dimtofl = configuration.Dimensions.ForceDimensionLine;
            dimStyleTableRecord.Dimblk = ConvertLayer.GetArrowObjectId(
                ConvertLayer.GetArrowBlockNameString(configuration.Dimensions.ArrowType));
            dimStyleTableRecord.Dimasz = configuration.Dimensions.ArrowSize;
            dimStyleTableRecord.Dimgap = configuration.Dimensions.InternalTextOffset;
            dimStyleTableRecord.Dimclrt = ConvertLayer.GetColorForName(configuration.Dimensions.TextColor);
            dimStyleTableRecord.Dimclre = ConvertLayer.GetColorForName(configuration.Dimensions.LineColor);
            dimStyleTableRecord.Dimclrd = ConvertLayer.GetColorForName(configuration.Dimensions.LineColor);
            dimStyleTableRecord.Dimexo = configuration.Dimensions.OffsetLineFromReferencePoint;
            dimStyleTableRecord.Dimtmove = configuration.Dimensions.TextMove;
            dimStyleTableRecord.Dimtoh = configuration.Dimensions.OutsideAlign;
            dimStyleTableRecord.Dimexe = configuration.Dimensions.ExtensionLineOffset;
        }

        private static void ApplyCommonStyleUpdates(DimStyleTableRecord dimStyleTableRecord, Configuration configuration)
        {
            dimStyleTableRecord.Dimclrt = ConvertLayer.GetColorForName(configuration.Dimensions.TextColor);
            dimStyleTableRecord.Dimclre = ConvertLayer.GetColorForName(configuration.Dimensions.LineColor);
            dimStyleTableRecord.Dimclrd = ConvertLayer.GetColorForName(configuration.Dimensions.LineColor);
            dimStyleTableRecord.Dimtxsty = ConvertLayer.GetTextSyleByName(configuration.Text.DefaultStyleName);
            dimStyleTableRecord.Dimatfit = 3;
        }
    }
}

