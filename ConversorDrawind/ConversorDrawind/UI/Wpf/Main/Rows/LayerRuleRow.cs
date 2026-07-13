using System;
using System.Linq;

namespace ConversorDrawind.UI.Wpf.Main.Rows
{
    public sealed class LayerRuleRow
    {
        public LayerRuleRow(LayerConversionRule rule)
        {
            Rule = rule ?? new LayerConversionRule();
            if (Rule.Source == null) Rule.Source = new EntityFilter();
            if (Rule.Target == null) Rule.Target = new LayerOutput();
        }

        public LayerConversionRule Rule { get; }
        public string BaseLayer => Rule.Source.BaseLayer;
        public string Filter => FilterDisplay;
        public string NewLayer => NewLayerDisplay;
        public string FilterDisplay => LayerRuleDisplay.FormatFilter(Rule.Source);
        public string NewLayerDisplay => LayerRuleDisplay.FormatLayerOutput(Rule.Target);
    }

    public sealed class RemoveLayerRow
    {
        public RemoveLayerRow(LayerRemoveRule rule)
        {
            Rule = rule ?? new LayerRemoveRule();
            if (Rule.Filter == null) Rule.Filter = new EntityFilter();
        }

        public LayerRemoveRule Rule { get; }
        public string Layer => Rule.Filter.BaseLayer;
        public string Filter => FilterDisplay;
        public string FilterDisplay => LayerRuleDisplay.FormatFilter(Rule.Filter);
    }

    internal static class LayerRuleDisplay
    {
        public static string FormatFilter(EntityFilter filter)
        {
            filter = filter ?? new EntityFilter();

            string line1 = "Objeto: " + filter.ObjectType + "  |  Cor: " + filter.Color;
            string line2 = "Linha: " + filter.LineType + "  |  Orientacao: " + filter.Orientation;
            string line3 = JoinDisplayParts(
                DisplayPart("Texto", filter.TextContent),
                DisplayPart("Altura", filter.TextHeight));

            return JoinDisplayLines(line1, line2, line3);
        }

        public static string FormatLayerOutput(LayerOutput output)
        {
            output = output ?? new LayerOutput();

            string line1 = "Layer: " + output.LayerName + "  |  Cor: " + output.Color;
            string line2 = "Linha: " + output.LineType + "  |  Estilo: " + output.TextStyle;
            string line3 = JoinDisplayParts(
                DisplayPart("Altura", output.TextContent),
                DisplayPart("Largura", output.TextHeight));

            return JoinDisplayLines(line1, line2, line3);
        }

        private static string DisplayPart(string label, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : label + ": " + value;
        }

        private static string JoinDisplayParts(params string[] values)
        {
            return string.Join("  |  ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string JoinDisplayLines(params string[] values)
        {
            return string.Join(Environment.NewLine, values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
