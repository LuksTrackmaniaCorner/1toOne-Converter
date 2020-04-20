using Converter.Gbx;
using Converter.Gbx.core;
using Converter.Gbx.core.chunks;
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

namespace Converter
{
    public class Settings : IXmlSerializable
    {
        public const bool DefaultOpenFolderAfterFinished = true;
        public const bool DefaultOverwriteExistingFiles = false;
        public const string RelativeDirectory = " ";
        public const string DefaultLogFilePath = @".\log.txt";
        public const bool DefaultAppendToLog = false;
        public const bool DefaultPlaceExtraPylons = false;

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

            var xmls = new XmlSerializer(typeof(Settings));
            if (File.Exists(defaultPath))
            {
                try {
                    using (var fs = new FileStream(defaultPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        settings = (Settings)xmls.Deserialize(fs);
                    }

                    try
                    {
                        //Update settings if changes have been made
                        using (var fs = new FileStream(defaultPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                        {
                            xmls.Serialize(fs, settings);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("You encountered an unexpected error, but not so unexpected that I didn't add a custom error message.\n");
                    }
                       
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("Settings could not be read. The settings will be restored to the original values.\n");
                }
            }
            
            //Create new Settings file
            settings = new Settings();
            using (var fs = new FileStream(defaultPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmls.Serialize(fs, settings);
            }
        }

        public string OutputFolder { get; private set; }
        public string AlpineOutputFolder { get; private set; }
        public string SpeedOutputFolder { get; private set; }
        public string RallyOutputFolder { get; private set; }
        public string BayOutputFolder { get; private set; }
        public string CoastOutputFolder { get; private set; }
        public string IslandOutputFolder { get; private set; }

        public bool OpenFolderAfterFinished { get; private set; }

        public bool OverwriteExistingFiles { get; private set; }

        public DisplayMode DisplayMode { get; private set; }

        public bool PlaceExtraPylons { get; private set; }

        public DisplayMode LogMode { get; private set; }
        public string LogFilePath { get; private set; }
        public bool AppendToLog { get; private set; }

        private Settings()
        {
            OutputFolder = RelativeDirectory;
            AlpineOutputFolder = RelativeDirectory;
            SpeedOutputFolder = RelativeDirectory;
            RallyOutputFolder = RelativeDirectory;
            BayOutputFolder = RelativeDirectory;
            CoastOutputFolder = RelativeDirectory;
            IslandOutputFolder = RelativeDirectory;
            OpenFolderAfterFinished = DefaultOpenFolderAfterFinished;
            OverwriteExistingFiles = DefaultOverwriteExistingFiles;
            DisplayMode = DisplayMode.Full;
            PlaceExtraPylons = DefaultPlaceExtraPylons;
            LogMode = DisplayMode.ErrorsOnly;
            LogFilePath = DefaultLogFilePath;
            AppendToLog = DefaultAppendToLog;
        }

        public string GetOutputPath(GBXFile file, string mapPath)
        {
            var commonChunk = (ChallengeCommon)file.GetChunk(Chunk.challengeCommonKey);
            var envi = commonChunk.TrackMeta.Collection.Content;

            return GetOutputPath(envi, mapPath);
        }

        public string GetOutputPath(string envi, string mapPath)
        {
            string outputFolder = envi switch
            {
                "Alpine" => AlpineOutputFolder,
                "Speed" => SpeedOutputFolder,
                "Rally" => RallyOutputFolder,
                "Bay" => BayOutputFolder,
                "Coast" => CoastOutputFolder,
                "Island" => IslandOutputFolder,
                _ => null
            };

            if (!string.IsNullOrWhiteSpace(outputFolder))
                return outputFolder;

            outputFolder = OutputFolder;

            if (!string.IsNullOrWhiteSpace(outputFolder))
                return outputFolder;
            else
                return mapPath;
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (isEmpty)
                throw new XmlException("Settings were empty");

            OutputFolder = ReadXmlPath(reader, nameof(OutputFolder));
            AlpineOutputFolder = ReadXmlPath(reader, nameof(AlpineOutputFolder));
            SpeedOutputFolder = ReadXmlPath(reader, nameof(SpeedOutputFolder));
            RallyOutputFolder = ReadXmlPath(reader, nameof(RallyOutputFolder));
            BayOutputFolder = ReadXmlPath(reader, nameof(BayOutputFolder));
            CoastOutputFolder = ReadXmlPath(reader, nameof(CoastOutputFolder));
            IslandOutputFolder = ReadXmlPath(reader, nameof(IslandOutputFolder));

            //Optional Element, to ensure backwards compatability
            reader.MoveToContent();
            if (reader.LocalName == nameof(PlaceExtraPylons))
                PlaceExtraPylons = ReadXmlBool(reader, nameof(PlaceExtraPylons), DefaultPlaceExtraPylons);

            OpenFolderAfterFinished = ReadXmlBool(reader, nameof(OpenFolderAfterFinished), DefaultOpenFolderAfterFinished);
            OverwriteExistingFiles = ReadXmlBool(reader, nameof(OverwriteExistingFiles), DefaultOverwriteExistingFiles);
            DisplayMode = ReadXmlEnum(reader, nameof(DisplayMode), DisplayMode.Full);

            LogMode = ReadXmlEnum(reader, nameof(LogMode), DisplayMode.None);
            LogFilePath = ReadXmlFile(reader, nameof(LogFilePath));
            AppendToLog = ReadXmlBool(reader, nameof(AppendToLog), DefaultAppendToLog);

            reader.ReadEndElement();
        }

        private static string ReadXmlPath(XmlReader reader, string elementName)
        {
            var path = reader.ReadElementString(elementName);
            if (string.IsNullOrWhiteSpace(path))
                path = RelativeDirectory;
            else if (!Directory.Exists(path))
            {
                path = RelativeDirectory;
                Console.WriteLine(elementName + "-Setting: Path does not exist. Resetting to relative Path.\n");
            }
            return path;
        }

        private static string ReadXmlFile(XmlReader reader, string elementName)
        {
            var file = reader.ReadElementString(elementName);
            if (string.IsNullOrWhiteSpace(file))
                file = null;
            else if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                file = null;
                Console.WriteLine(elementName + "-Setting: File does not exist. Resetting to Default.\n");
            }

            return file;
        }

        private static bool ReadXmlBool(XmlReader reader, string elementName, bool @default = default)
        {
            var boolString = reader.ReadElementString(elementName);
            if (bool.TryParse(boolString, out var result))
                return result;

            Console.WriteLine(elementName + "-Setting: Invalid input value.\n");
            return @default;
        }

        private static T ReadXmlEnum<T>(XmlReader reader, string elementName, T @default = default) where T : struct, Enum
        {
            var enumString = reader.ReadElementString(elementName);
            if (Enum.TryParse<T>(enumString, out T result))
                return result;

            Console.WriteLine(elementName + "-Setting: Invalid input value.\n");
            return @default;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteComment("Defines the output directory of the converter.");
            writer.WriteComment(@"Enter a file path, e.g: C:\Users\[Username]\Documents\Maniaplanet\Maps\MyMaps");
            writer.WriteComment("Leave this option empty to create the new files in the same folder as the old ones.");
            writer.WriteElementString(nameof(OutputFolder), OutputFolder);
            writer.WriteComment("You can also define the Output Path for every environment individually.");
            writer.WriteElementString(nameof(AlpineOutputFolder), AlpineOutputFolder);
            writer.WriteElementString(nameof(SpeedOutputFolder), SpeedOutputFolder);
            writer.WriteElementString(nameof(RallyOutputFolder), RallyOutputFolder);
            writer.WriteElementString(nameof(BayOutputFolder), BayOutputFolder);
            writer.WriteElementString(nameof(CoastOutputFolder), CoastOutputFolder);
            writer.WriteElementString(nameof(IslandOutputFolder), IslandOutputFolder);

            writer.WriteComment("If False, the converter will place the pylons in the same way as TMO would.");
            writer.WriteComment("If True, the converter will place pylons in more positions.");
            writer.WriteComment("Allowed values are: True, False");
            writer.WriteElementString(nameof(PlaceExtraPylons), PlaceExtraPylons.ToString());

            writer.WriteComment("Defines whether the converter should open the output directory after it has finished.");
            writer.WriteComment("Allowed values are: True, False");
            writer.WriteElementString(nameof(OpenFolderAfterFinished), OpenFolderAfterFinished.ToString());

            writer.WriteComment("Defines whether the converter can override files, if they already exist in the output directory.");
            writer.WriteComment("Allowed values are: True, False");
            writer.WriteElementString(nameof(OverwriteExistingFiles), OverwriteExistingFiles.ToString());

            writer.WriteComment("Defines how much information the converter displays.");
            writer.WriteComment("Allowed values are: None, TracksOnly, Full");
            writer.WriteElementString(nameof(DisplayMode), DisplayMode.ToString());

            writer.WriteComment("Defines how much information the converter stores in the log file");
            writer.WriteComment("Allowed values are: None, ErrorsOnly, TracksOnly, Full");
            writer.WriteElementString(nameof(LogMode), LogMode.ToString());

            writer.WriteComment("Specify a a path to a Log File");
            writer.WriteComment(@"Enter a file path, e.g: .\log.txt");
            writer.WriteElementString(nameof(LogFilePath), LogFilePath);

            writer.WriteComment("Determines whether to append to the log file or create the file new again.");
            writer.WriteComment("Allowed values are: True, False");
            writer.WriteElementString(nameof(AppendToLog), AppendToLog.ToString());
        }
    }

    public enum DisplayMode
    {
        None,
        ErrorsOnly,
        TracksOnly,
        Full
    }
}
