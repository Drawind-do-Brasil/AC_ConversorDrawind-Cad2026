using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ConversorDrawind.Commands
{
    static class LayerSetupOperations
    {
        public static void CreateAndAssignALayer()
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            LayerRepository.CreateAndAssignAll(documentContext, RuntimeConfigurationState.NewLayerCompositions);
        }

        public static Color GetColorForName(string color)
        {
            return ColorResolver.Resolve(color);
        }

        public static ObjectId LoadLinetype(string sLineTypName)
        {
            IAcadDocumentContext documentContext = new AcadDocumentContext();
            Database database = documentContext.Database;
            ObjectId id = ObjectId.Null;

            try
            {
                id = LinetypeService.LoadLinetype(database, sLineTypName);
            }
            catch (Exception e)
            {
                ConversionLog.Write(LogContext.ConverterEntidadePorLayer, e.Message);
            }

            return id;
        }
    }
}
