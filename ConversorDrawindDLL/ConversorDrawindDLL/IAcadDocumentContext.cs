using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace ConversorDrawindDLL
{
    internal interface IAcadDocumentContext
    {
        Document Document { get; }
        Database Database { get; }
        Editor Editor { get; }
    }

    internal sealed class AcadDocumentContext : IAcadDocumentContext
    {
        private readonly Document document;

        internal AcadDocumentContext()
            : this(Application.DocumentManager.MdiActiveDocument)
        {
        }

        internal AcadDocumentContext(Document document)
        {
            this.document = document;
        }

        public Document Document
        {
            get { return document; }
        }

        public Database Database
        {
            get { return document.Database; }
        }

        public Editor Editor
        {
            get { return document.Editor; }
        }
    }
}
