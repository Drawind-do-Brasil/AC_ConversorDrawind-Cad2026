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
[CommandMethod("CDwi_ScaleBlock")]
        public static void CDwi_ConvertToScaleInv()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Document document = documentContext.Document;
            Database database = documentContext.Database;
            Editor editor = documentContext.Editor;
            ISystemVariableService systemVariables = new AcadSystemVariableService();
            ScaleWorkflow scaleWorkflow = new ScaleWorkflow(systemVariables);
            ConversionStepRunner stepRunner = new ConversionStepRunner(
                new AcadEditorMessenger(editor),
                Conversor.EscreverLog,
                ConversionMessages.ShowWarningIfEnabled);



            stepRunner.Run(
                "Colocando o formato na escala real... ",
                () =>
                {
                    string newdate = editor.GetString("Digite o nome do bloco: ").StringResult.Replace("*******", " ");
                    double scale = escalaFinal = scaleWorkflow.ReadLineTypeScale();
                    ConvertLayer.ScaleDrawingInv(scale, new List<Block>() { new Block(newdate) });
                    object ptMax2 = GetNewMax();
                    object ptMin2 = GetNewMin();
                    ConvertLayer.Zoom((Point3d)ptMin2, (Point3d)ptMax2);
                },
                LogContext.DefinirEscalaDoBloco,
                "Não foi possível colocar o formato na escala real!",
                "Descrição: Erro ao tentar colocar o formato na escala real...\n",
                "... Completado.\n");
        }
    }
}