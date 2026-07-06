using System;

namespace ConversorDrawindDLL
{
    internal sealed class ScaleWorkflow
    {
        private readonly ISystemVariableService systemVariables;

        internal ScaleWorkflow(ISystemVariableService systemVariables)
        {
            this.systemVariables = systemVariables ?? throw new ArgumentNullException(nameof(systemVariables));
        }

        internal void ApplyLineTypeScale(double lineTypeScale)
        {
            systemVariables.Set("LTSCALE", lineTypeScale);
        }

        internal void ApplyDimensionScale(double dimensionScale)
        {
            systemVariables.Set("DIMSCALE", dimensionScale);
        }

        internal void ApplyDrawingScale(double lineTypeScale, double dimensionScale, double scale)
        {
            ApplyLineTypeScale(lineTypeScale * scale);
            ApplyDimensionScale(dimensionScale * scale);
        }

        internal double ReadLineTypeScale()
        {
            return Convert.ToDouble(systemVariables.Get("LTSCALE"));
        }
    }
}
