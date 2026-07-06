namespace ConversorDrawindDLL.Tests
{
    public class ScaleWorkflowTests
    {
        [Fact]
        public void ApplyDrawingScale_WritesLineTypeAndDimensionScale()
        {
            var variables = new FakeSystemVariableService();
            var workflow = new ScaleWorkflow(variables);

            workflow.ApplyDrawingScale(lineTypeScale: 2.5, dimensionScale: 4, scale: 3);

            Assert.Equal(7.5, variables.Values["LTSCALE"]);
            Assert.Equal(12.0, variables.Values["DIMSCALE"]);
        }

        [Fact]
        public void ReadLineTypeScale_UsesCurrentSystemVariableValue()
        {
            var variables = new FakeSystemVariableService();
            variables.Set("LTSCALE", 2.75);
            var workflow = new ScaleWorkflow(variables);

            double result = workflow.ReadLineTypeScale();

            Assert.Equal(2.75, result);
        }

        private sealed class FakeSystemVariableService : ISystemVariableService
        {
            internal Dictionary<string, object> Values { get; } = new();

            public void Set(string name, object value)
            {
                Values[name] = value;
            }

            public object Get(string name)
            {
                return Values[name];
            }
        }
    }
}
