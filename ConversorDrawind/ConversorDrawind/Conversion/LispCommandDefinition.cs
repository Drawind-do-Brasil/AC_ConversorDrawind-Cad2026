using System;
using System.Collections.Generic;

namespace ConversorDrawind
{
    internal sealed class LispCommandDefinition
    {
        private LispCommandDefinition(string command, string sourceFile, bool executeAfterConversion)
        {
            Command = command;
            SourceFile = sourceFile;
            ExecuteAfterConversion = executeAfterConversion;
        }

        internal string Command { get; }

        internal string SourceFile { get; }

        internal bool ExecuteAfterConversion { get; }

        internal static IReadOnlyList<LispCommandDefinition> ParseAll(IEnumerable<string> definitions)
        {
            List<LispCommandDefinition> commands = new List<LispCommandDefinition>();

            if (definitions == null)
                return commands;

            foreach (string definition in definitions)
            {
                LispCommandDefinition command;
                if (!TryParse(definition, out command))
                    throw new ArgumentException("Comando LISP inválido: " + definition);

                commands.Add(command);
            }

            return commands;
        }

        internal static bool TryParse(string definition, out LispCommandDefinition command)
        {
            command = null;

            if (string.IsNullOrWhiteSpace(definition))
                return false;

            string[] parts = definition.Split('@');
            if (parts.Length != 2 && parts.Length != 3)
                return false;

            string commandName = parts[0].Trim();
            string sourceFile = parts[1].Trim();
            if (string.IsNullOrWhiteSpace(commandName) || string.IsNullOrWhiteSpace(sourceFile))
                return false;

            command = new LispCommandDefinition(commandName, sourceFile, parts.Length == 3);
            return true;
        }
    }
}
