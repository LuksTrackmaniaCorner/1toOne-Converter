using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace _1toOne_Converter.src
{
    public class Settings : IXmlSerializable
    {
        private static Settings settings;

        public static Settings GetSettings() => settings;

        /// <summary>
        /// Generates the settings object from the settings file.
        /// If the settings file is missing or corrupt, a default settings file will be generated.
        /// After the settings object has been generated, it can be read by multiple Threads simultaniously.
        /// </summary>
        public static void GenerateSettings()
        {
            string defaultPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @".\Settings.xml";

            if (File.Exists(defaultPath))
            {
                try {
                    using var fs = new FileStream(defaultPath, FileMode.Open);
                    var xmls = new XmlSerializer(typeof(Settings));
                    settings = (Settings)xmls.Deserialize(fs);
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("Settings could not be read. The settings will be restored to the original values.");
                }
            }
            
            {
                //Create new Settings file
                settings = new Settings
                {
                    OutputFolder = RelativeDirectory,
                    OpenFolderAfterFinished = true,
                    OverwriteExistingFiles = false,
                    DisplayMode = DisplayMode.Full
                };

                using var fs = new FileStream(defaultPath, FileMode.Create);
                var xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(fs, settings);
            }
        }

        public const string RelativeDirectory = " ";

        public string OutputFolder { get; private set; }
        public bool OpenFolderAfterFinished { get; private set; }
        public bool OverwriteExistingFiles { get; private set; }
        public DisplayMode DisplayMode { get; private set; }

        private Settings() { }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (isEmpty)
                throw new XmlException("Settings were empty");

            OutputFolder = reader.ReadElementString(nameof(OutputFolder));
            if (string.IsNullOrWhiteSpace(OutputFolder))
                OutputFolder = RelativeDirectory;
            else if(!Directory.Exists(OutputFolder))
            {
                OutputFolder = RelativeDirectory;
                Console.WriteLine("Settings directory does not exist. Resetting to relative Path.");
            }
            else if (!OutputFolder.EndsWith("\\"))
                OutputFolder += "\\";
            OpenFolderAfterFinished = bool.Parse(reader.ReadElementString(nameof(OpenFolderAfterFinished)));
            OverwriteExistingFiles = bool.Parse(reader.ReadElementString(nameof(OverwriteExistingFiles)));
            Enum.TryParse(reader.ReadElementString(nameof(DisplayMode)), out DisplayMode displayMode);
            DisplayMode = displayMode;
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteComment("Definines the output directory of the converter.");
            writer.WriteComment(@"Enter a file path, e.g: C:\Users\[Username]\Documents\Maniaplanet\Maps\MyMaps");
            writer.WriteComment("Leave this option empty to create the new files in the same Folder as the old ones.");
            writer.WriteElementString(nameof(OutputFolder), OutputFolder);

            writer.WriteComment("Definines whether the converter should open the output directory after it has finished.");
            writer.WriteComment("Allowed values are: True, False");
            writer.WriteElementString(nameof(OpenFolderAfterFinished), OpenFolderAfterFinished.ToString());

            writer.WriteComment("Definines whether the converter can override files, if they already exist in the output directory.");
            writer.WriteComment("Allowed values are: True, False");
            writer.WriteElementString(nameof(OverwriteExistingFiles), OverwriteExistingFiles.ToString());

            writer.WriteComment("Definines how much information the converter displays.");
            writer.WriteComment("Allowed values are: None, TracksOnly, Full");
            writer.WriteElementString(nameof(DisplayMode), DisplayMode.ToString());
        }
    }

    public enum DisplayMode
    {
        None,
        TracksOnly,
        Full
    }
}
