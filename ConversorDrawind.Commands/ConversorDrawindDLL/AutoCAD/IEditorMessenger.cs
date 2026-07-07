using Autodesk.AutoCAD.EditorInput;

namespace ConversorDrawindDLL
{
    internal interface IEditorMessenger
    {
        void WriteMessage(string message);
    }

    internal sealed class AcadEditorMessenger : IEditorMessenger
    {
        private readonly Editor editor;

        internal AcadEditorMessenger(Editor editor)
        {
            this.editor = editor;
        }

        public void WriteMessage(string message)
        {
            editor.WriteMessage(message);
        }
    }
}
