using System.Collections.Generic;
using ACAD = Autodesk.AutoCAD.Interop;

namespace ConversorDrawind
{
    internal sealed class AutoCadSession
    {
        internal ACAD.AcadApplication Application { get; set; }

        internal ACAD.AcadDocument CurrentDocument { get; set; }

        internal ACAD.AcadDocument AttributeDocument { get; set; }

        internal List<ACAD.AcadDocument> OpenedDocuments { get; } = new List<ACAD.AcadDocument>();

        internal void Reset()
        {
            OpenedDocuments.Clear();
            CurrentDocument = null;
            AttributeDocument = null;
            Application = null;
        }
    }
}
