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
                        configuration.EXTDIMStyleName);

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
                    Conversor.EscreverLog("Erro 73", e.Message);
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
                    Conversor.EscreverLog("Erro 74", e.Message);
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
            dimStyleTableRecord.Dimtxsty = ConvertLayer.GetTextSyleByName(configuration.EXTTEXTStyleName);
            dimStyleTableRecord.Dimtxt = configuration.EXTTEXTSize;
            dimStyleTableRecord.Dimscale = configuration.EXTDIMScale;
            dimStyleTableRecord.Dimdec = configuration.EXTDIMPrecision;
            dimStyleTableRecord.Dimadec = configuration.EXTDIMAngularPrecision;
            dimStyleTableRecord.Dimlunit = configuration.EXTDIMUnit;
            dimStyleTableRecord.Dimaunit = configuration.EXTDIMAngularUnit;
            dimStyleTableRecord.Dimtad = configuration.EXTDIMTad;
            dimStyleTableRecord.Dimtih = configuration.EXTDIMDimensionPosition;
            dimStyleTableRecord.Dimtix = configuration.EXTDIMTextForced;
            dimStyleTableRecord.Dimtofl = configuration.EXTDIMLineForced;
            dimStyleTableRecord.Dimblk = ConvertLayer.GetArrowObjectId(
                ConvertLayer.GetArrowBlockNameString(configuration.EXTDIMSeta));
            dimStyleTableRecord.Dimasz = configuration.EXTDIMSizeSeta;
            dimStyleTableRecord.Dimgap = configuration.INTDIMTextOffset;
            dimStyleTableRecord.Dimclrt = ConvertLayer.GetColorForName(configuration.EXTDIMColorText);
            dimStyleTableRecord.Dimclre = ConvertLayer.GetColorForName(configuration.EXTDIMColorLine);
            dimStyleTableRecord.Dimclrd = ConvertLayer.GetColorForName(configuration.EXTDIMColorLine);
            dimStyleTableRecord.Dimexo = configuration.EXTDIMOffsetLineFromRefPoint;
            dimStyleTableRecord.Dimtmove = configuration.EXTDIMTextMove;
            dimStyleTableRecord.Dimtoh = configuration.EXTDIMOutsideAlign;
            dimStyleTableRecord.Dimexe = configuration.EXTDIMDIMEX;
        }

        private static void ApplyCommonStyleUpdates(DimStyleTableRecord dimStyleTableRecord, Configuration configuration)
        {
            dimStyleTableRecord.Dimclrt = ConvertLayer.GetColorForName(configuration.EXTDIMColorText);
            dimStyleTableRecord.Dimclre = ConvertLayer.GetColorForName(configuration.EXTDIMColorLine);
            dimStyleTableRecord.Dimclrd = ConvertLayer.GetColorForName(configuration.EXTDIMColorLine);
            dimStyleTableRecord.Dimtxsty = ConvertLayer.GetTextSyleByName(configuration.EXTTEXTStyleName);
            dimStyleTableRecord.Dimatfit = 3;
        }
    }
}
