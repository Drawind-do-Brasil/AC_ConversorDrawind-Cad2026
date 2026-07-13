using Autodesk.AutoCAD.DatabaseServices;

namespace ConversorDrawind.Commands.Services.Styles
{
    static class StyleOperations
    {
        public static Autodesk.AutoCAD.GraphicsInterface.FontDescriptor UpdateTextFont(string font, bool italic, bool negrito)
        {
            return TextStyleService.CreateFontDescriptor(font, italic, negrito);
        }

        public static ObjectId GetTextSyleByName(string name = null)
        {
            name = TextStyleService.ResolveStyleNameOrDefault(name, RuntimeConfigurationState.TextStyles);
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return TextStyleService.GetStyleIdByName(documentContext, name);
        }

        public static void CreateTextSyles()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            TextStyleService.CreateStyles(documentContext, RuntimeConfigurationState.TextStyles, ConversionLog.Write);
        }

        public static ObjectId CreateDimstyle()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return DimensionStyleService.CreateCurrentStyle(documentContext, Configuration.Config);
        }

        public static ObjectId CreateDimstyle2()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return DimensionStyleService.UpdateAllStyles(documentContext, Configuration.Config);
        }

        public static void UpdateDimensionPrecision()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new DimensionPrecisionService(LayerSelectionQueries.Filter, documentContext, ConversionLog.Write).UpdateDimensionPrecision();
        }

        public static ObjectId GetArrowObjectId(string newArrName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            return ArrowBlockService.GetArrowObjectId(
                newArrName,
                "DIMBLK",
                documentContext,
                systemVariables,
                ConversionLog.Write,
                LogContext.CriarEstilosDeTexto);
        }

        public static ObjectId GetArrowObjectId(string newArrName, string tipo)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            return ArrowBlockService.GetArrowObjectId(
                newArrName,
                tipo,
                documentContext,
                systemVariables,
                ConversionLog.Write,
                LogContext.CriarEstiloDeCota);
        }

        public static string GetArrowBlockName(string name)
        {
            return ArrowBlockService.GetArrowBlockName(name);
        }

        public static string GetArrowBlockNameString(string name)
        {
            return ArrowBlockService.GetArrowBlockNameString(name);
        }
    }
}
