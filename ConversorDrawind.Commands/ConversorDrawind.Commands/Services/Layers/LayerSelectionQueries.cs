using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace ConversorDrawind.Commands
{
    static class LayerSelectionQueries
    {
        public static ObjectId[] Filter(string LayerName, string Start, string ColorName, string LinetypeName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            return new LayerSelectionService(entitySelector, ConversionLog.Write).Filter(LayerName, Start, ColorName, LinetypeName);
        }

        public static ObjectId[] FilterLayers(params string[] layers)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            IEntitySelector entitySelector = new AcadEntitySelector(documentContext.Editor);
            return new LayerSelectionService(entitySelector, ConversionLog.Write).FilterLayers(layers);
        }

        public static SelectionFilter FilterText(string LayerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextAndMTextOnLayer(LayerName));
        }

        public static SelectionFilter FilterText2(params string[] LayerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextAndMTextOnLayers(LayerName));
        }

        public static SelectionFilter FilterTextTeste(string LayerName)
        {
            return new SelectionFilter(LayerFilterFactory.TextAndInsertOnLayer(LayerName));
        }
    }
}
