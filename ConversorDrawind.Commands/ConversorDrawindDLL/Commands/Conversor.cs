using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace ConversorDrawindDLL
{
    public partial class Conversor
    {
        private static ConversorDrawind.Point NewMin = new ConversorDrawind.Point(double.MaxValue, double.MaxValue, double.MaxValue);
        private static ConversorDrawind.Point NewMax = new ConversorDrawind.Point(double.MinValue, double.MinValue, double.MinValue);
        public static string LOG_Diretorio = "";
        public static string LOG_FileName = "";
        private static double escalaCapiturada = -1;
        private static double escalaFinal = 1;
        static DateTime timeini = new DateTime();
        static string conversor = "";

        public Conversor()
        {
        }

        public static void EscreverLog(string context, string detail)
        {
            ConversionLogger.Write(LOG_Diretorio, LOG_FileName, context, detail);
        }

        public static void EscreverLog(string context, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            EscreverLog(context, exception.Message);
        }

        public static void MoveElements(Point3d startPoint, Point3d endPoint)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityMoveService(documentContext, new AcadEntitySelector(documentContext.Editor))
                .MoveAll(startPoint, endPoint);
        }

        public static void MoveToOrigin()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new EntityMoveService(documentContext, new AcadEntitySelector(documentContext.Editor))
                .MoveAllToOrigin(GetNewMin());
        }

        public static void UPDATE_DIMENSTION(double dscale)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new DimensionScaleService(ConvertLayer.Filter, documentContext, Conversor.EscreverLog)
                .ApplyScale(dscale);
        }

        public static void GETREALMAXMIN()
        {
            CreateDrawingExtentsService().Refresh();
        }

        public static bool GETREALMAXMINTEKLA()
        {
            return CreateDrawingExtentsService().TryRefreshTeklaDrawingSheet();
        }

        public static void GETREALMAXMINGENERAL()
        {
            CreateDrawingExtentsService().RefreshGeneral();
        }

        public static void GETSCALE()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            double? scale = new DrawingScaleDetectionService(documentContext, new AcadEntitySelector(documentContext.Editor))
                .CaptureScale();

            if (scale.HasValue)
                escalaCapiturada = scale.Value;
        }

        private static void InitialConversionLayer()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            new InitialBlockLayerService(documentContext, new AcadEntitySelector(documentContext.Editor), Conversor.EscreverLog)
                .ConvertBlockLayers();
        }

        public static Point3d GetNewMin()
        {
            return CreateDrawingExtentsService().GetMinPoint();
        }

        public static Point3d GetNewMax()
        {
            return CreateDrawingExtentsService().GetMaxPoint();
        }

        private static DrawingExtentsService CreateDrawingExtentsService()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            return new DrawingExtentsService(
                documentContext,
                new AcadEntitySelector(documentContext.Editor),
                NewMin,
                NewMax);
        }

        private ObjectId[] Filter1(Editor editor)
        {
            return new BlockSelectionService(new AcadEntitySelector(editor)).SelectBlockReferences();
        }

        public static void CDwi_ConsertarSetaSeta()
        {
            try
            {
                FixArrow.ConsetaSetaSeta(Configuration.Config.Dimensions.FixArrowType, escalaFinal, Configuration.Config.Dimensions.FixArrowFactor);
            }
            catch (Exception e)
            {
                Conversor.EscreverLog(LogContext.FixarSetaDaCota, e.Message);
            }
        }
    }
}