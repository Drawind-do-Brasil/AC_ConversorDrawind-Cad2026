using System.Linq;

namespace ConversorDrawindDLL
{
    internal static class DimensionBlockClassifier
    {
        internal static DimensionBlockKind Classify(ObjectsInBlock objectsInBlock)
        {
            if (objectsInBlock.dBTextList.Count == 0)
                return DimensionBlockKind.WithoutText;

            if (DimensionTextAnalyzer.HasDifferentTextRotations(objectsInBlock.dBTextList.Select(text => text.Rotation)))
                return DimensionBlockKind.Tangent;

            if (objectsInBlock.hatchList.Count > 0)
                return DimensionBlockKind.Radius;

            if (objectsInBlock.arcList.Count > 0)
                return DimensionBlockKind.Arc;

            return DimensionBlockKind.LinearOrElevation;
        }
    }
}
