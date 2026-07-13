using System.Globalization;
using System.Linq;

namespace ConversorDrawind
{
    public static class LegacyConfigurationParsers
    {
        public static LayerDefinition ParseLayerDefinition(string value)
        {
            string[] parts = SplitFixed(value, ':', 3);
            return new LayerDefinition
            {
                Name = parts[0],
                Color = parts[1],
                LineType = parts[2]
            };
        }

        public static string FormatLayerDefinition(LayerDefinition value)
        {
            if (value == null)
                return string.Empty;

            return Join(":", value.Name, value.Color, value.LineType);
        }

        public static TextStyleDefinition ParseTextStyleDefinition(string value)
        {
            string[] parts = SplitFixed(value, ':', 7);
            return new TextStyleDefinition
            {
                Name = parts[0],
                Font = parts[1],
                Italic = ParseBool(parts[2]),
                Bold = ParseBool(parts[3]),
                Size = ParseDouble(parts[4]),
                WidthFactor = ParseDouble(parts[5], 1),
                ObliqueAngle = ParseDouble(parts[6])
            };
        }

        public static string FormatTextStyleDefinition(TextStyleDefinition value)
        {
            if (value == null)
                return string.Empty;

            return Join(":",
                value.Name,
                value.Font,
                FormatBool(value.Italic),
                FormatBool(value.Bold),
                FormatDouble(value.Size),
                FormatDouble(value.WidthFactor),
                FormatDouble(value.ObliqueAngle));
        }

        public static EntityFilter ParseEntityFilter(string value)
        {
            string[] parts = SplitFixed(value, ':', 6);
            return new EntityFilter
            {
                ObjectType = parts[0],
                Color = parts[1],
                LineType = parts[2],
                TextContent = parts[3],
                TextHeight = parts[4],
                Orientation = string.IsNullOrEmpty(parts[5]) ? "ALL" : parts[5]
            };
        }

        public static string FormatEntityFilter(EntityFilter value)
        {
            if (value == null)
                return string.Empty;

            return Join(":",
                value.ObjectType,
                value.Color,
                value.LineType,
                value.TextContent,
                value.TextHeight,
                string.IsNullOrEmpty(value.Orientation) ? "ALL" : value.Orientation);
        }

        public static LayerRemoveRule ParseLayerRemoveRule(string value)
        {
            string normalized = value == null ? string.Empty : value.Split('$').Last();
            string[] parts = SplitFixed(normalized, ';', 2);
            EntityFilter filter = ParseEntityFilter(parts[1]);
            filter.BaseLayer = parts[0];
            return new LayerRemoveRule { Filter = filter };
        }

        public static string FormatLayerRemoveRule(LayerRemoveRule value)
        {
            if (value == null || value.Filter == null)
                return string.Empty;

            return value.Filter.BaseLayer + ";" + FormatEntityFilter(value.Filter);
        }

        public static LayerConversionRule ParseLayerConversionRule(string value)
        {
            string[] parts = SplitFixed(value, ';', 3);
            EntityFilter source = ParseEntityFilter(parts[1]);
            source.BaseLayer = parts[0];

            string[] targetParts = SplitFixed(parts[2], ':', 6);
            return new LayerConversionRule
            {
                Source = source,
                Target = new LayerOutput
                {
                    LayerName = targetParts[0],
                    Color = targetParts[1],
                    LineType = targetParts[2],
                    TextContent = targetParts[3],
                    TextHeight = targetParts[4],
                    TextStyle = targetParts[5]
                }
            };
        }

        public static string FormatLayerConversionRule(LayerConversionRule value)
        {
            if (value == null)
                return string.Empty;

            EntityFilter source = value.Source ?? new EntityFilter();
            LayerOutput target = value.Target ?? new LayerOutput();

            string formattedTarget = string.IsNullOrEmpty(target.TextStyle)
                ? Join(":", target.LayerName, target.Color, target.LineType, target.TextContent, target.TextHeight)
                : Join(":", target.LayerName, target.Color, target.LineType, target.TextContent, target.TextHeight, target.TextStyle);

            return source.BaseLayer + ";" + FormatEntityFilter(source) + ";" + formattedTarget;
        }

        public static string FormatDouble(double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture);
        }

        public static double ParseDouble(string value, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return NumericTextParser.ToDouble(value);
        }

        public static bool ParseBool(string value)
        {
            bool result;
            return bool.TryParse(value, out result) && result;
        }

        public static string FormatBool(bool value)
        {
            return value ? "true" : "false";
        }

        private static string[] SplitFixed(string value, char separator, int length)
        {
            string[] raw = (value ?? string.Empty).Split(separator);
            string[] result = new string[length];
            for (int index = 0; index < length; index++)
                result[index] = index < raw.Length ? raw[index] : string.Empty;

            return result;
        }

        private static string Join(string separator, params string[] values)
        {
            return string.Join(separator, values.Select(item => item ?? string.Empty));
        }
    }
}
