using System.IO;

namespace ConversorDrawind
{
    internal static class DrawingCommandBuilder
    {
        public static string BuildLoadFileCommand(string file)
        {
            string ext = Path.GetExtension(file);
            if (ext.ToUpper() == ".DLL")
                return "NETLOAD " + file + "\n";

            return "(load  \"" + file.Replace("\\", "\\\\") + "\")\n";
        }

        public static string BuildCommandLineCommand()
        {
            return "._COMMANDLINE\n";
        }
    }
}
