using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FORMS = System.Windows.Forms;

namespace ConversorDrawindDLL
{
    public partial class Conversor
    {
[CommandMethod("CDwi_Save")]
        public static void CDwi_Save()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Editor editor = documentContext.Editor;
            Database database = documentContext.Database;
            {

                PromptKeywordOptions pko = new PromptKeywordOptions(Localization.PromptType);
                pko.AllowNone = true;
                pko.Keywords.Add("DWG");
                pko.Keywords.Add("DXF");
                pko.Keywords.Default = "DWG";
                PromptResult tipoOperacao = editor.GetKeywords(pko);
                if (tipoOperacao.Status == PromptStatus.Cancel)
                {
                    return;
                }
                if (tipoOperacao.StringResult == "DWG")
                    database.Save();
                else
                    SaveDXF();

            }
            {
                PromptKeywordOptions pko = new PromptKeywordOptions(Localization.PromptClose);
                pko.AllowNone = true;
                pko.Keywords.Add(Localization.KeywordYes);
                pko.Keywords.Add(Localization.KeywordNo);
                pko.Keywords.Default = Localization.KeywordYes;
                PromptResult tipoOperacao = editor.GetKeywords(pko);
                if (tipoOperacao.Status == PromptStatus.Cancel)
                {
                    return;
                }
                if (tipoOperacao.StringResult == Localization.KeywordYes)
                {
                    document.CloseAndDiscard();
                    document.Dispose();
                }

            }

        }
    }
}