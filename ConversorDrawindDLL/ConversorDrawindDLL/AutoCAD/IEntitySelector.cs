using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace ConversorDrawindDLL
{
    internal interface IEntitySelector
    {
        PromptSelectionResult SelectAll();

        PromptSelectionResult SelectAll(SelectionFilter filter);

        PromptSelectionResult SelectCrossingWindow(Point3d firstPoint, Point3d secondPoint, SelectionFilter filter);

        PromptSelectionResult SelectWindow(Point3d firstPoint, Point3d secondPoint, SelectionFilter filter);
    }

    internal sealed class AcadEntitySelector : IEntitySelector
    {
        private readonly Editor editor;

        internal AcadEntitySelector(Editor editor)
        {
            this.editor = editor;
        }

        public PromptSelectionResult SelectAll(SelectionFilter filter)
        {
            return editor.SelectAll(filter);
        }

        public PromptSelectionResult SelectAll()
        {
            return editor.SelectAll();
        }

        public PromptSelectionResult SelectCrossingWindow(Point3d firstPoint, Point3d secondPoint, SelectionFilter filter)
        {
            return editor.SelectCrossingWindow(firstPoint, secondPoint, filter);
        }

        public PromptSelectionResult SelectWindow(Point3d firstPoint, Point3d secondPoint, SelectionFilter filter)
        {
            return editor.SelectWindow(firstPoint, secondPoint, filter);
        }
    }
}
