using Autodesk.AutoCAD.ApplicationServices;

namespace ConversorDrawind.Commands
{
    internal interface ISystemVariableService
    {
        void Set(string name, object value);
        object Get(string name);
    }

    internal sealed class AcadSystemVariableService : ISystemVariableService
    {
        public void Set(string name, object value)
        {
            Application.SetSystemVariable(name, value);
        }

        public object Get(string name)
        {
            return Application.GetSystemVariable(name);
        }
    }
}
