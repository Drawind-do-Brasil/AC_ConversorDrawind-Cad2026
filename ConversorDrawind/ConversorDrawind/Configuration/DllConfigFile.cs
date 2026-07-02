using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ConversorDrawind
{
    internal static class DllConfigFile
    {
        private static string ConfigPath
        {
            get { return Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config"); }
        }

        public static string Load()
        {
            string arquivo = ConfigPath;
            if (!File.Exists(arquivo))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");

            XElement importXML = XElement.Load(arquivo);

            return importXML.Element(ConfigurationXmlContract.DllConfigurations).Attribute(ConfigurationXmlContract.BlocoDm).Value;
        }

        public static void SaveIfMissing()
        {
            string arquivo = ConfigPath;

            if (File.Exists(arquivo))
                return;

            var escritor = new XmlTextWriter(arquivo, Encoding.UTF8) { Formatting = Formatting.Indented };
            escritor.WriteStartDocument();
            escritor.WriteStartElement(ConfigurationXmlContract.ConfigurationRoot);
            escritor.WriteEndElement();
            escritor.WriteEndDocument();
            escritor.Close();

            XElement xml = XElement.Load(arquivo);

            XElement configurations = new XElement(ConfigurationXmlContract.DllConfigurations);
            configurations.Add(new XAttribute(ConfigurationXmlContract.BlocoDm, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg")));
            xml.Add(configurations);

            xml.Save(arquivo);
        }
    }
}
