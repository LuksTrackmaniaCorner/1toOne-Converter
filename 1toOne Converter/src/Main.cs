using _1toOne_Converter.src.conversion;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src
{
    public class Converter : IDumpable
    {
        private readonly GBXFile file;
        private Converter(string inputFileName)
        {
            using var fs = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            file = new GBXFile(fs);
        }

        public void WriteBack(Stream outputFile)
        {
            file.WriteBack(outputFile);
        }

        public LinkedList<string> Dump()
        {
            return file.Dump();
        }

        public void Convert(Conversion conversion)
        {
            conversion.Convert(file);
        }

        private string GetStatistics()
        {
            return file.GetStatistics();
        }

        [STAThread] //Required for OpenFileDialog
        public static void Main(string[] args)
        {
            try
            {
                ConvertMaps(args);
            }
            catch(Exception e)
            {
                Console.WriteLine("An unexpected error occured:");
                Console.WriteLine(e);
                Console.ReadKey();
            }

            //AddPylonsToGround();
        }

        public static void ConvertMaps(string[] args)
        {
            string DefaultXmlFilePath = @"Default.xml";

            string xmlFile = null;
            var gbxFiles = new List<string>();

#if DEBUG
            bool keepConsoleOpen = true;
#else
            bool keepConsoleOpen = false;
#endif
            //Getting the files from the command line arguments
            foreach(var file in args)
            {
                if(file.EndsWith(".xml", true, null))
                {
                    if (xmlFile != null)
                        throw new Exception("Invalid command line arguments. You can only specify one xml file");

                    xmlFile = file;
                }
                else if(file.EndsWith(".Challenge.Gbx"))
                {
                    gbxFiles.Add(file);
                }
                else
                {
                    //TODO add warning.
                }
            }

            //Setting to default conversion if nothing has been specified.
            if(xmlFile == null)
            {
                Directory.SetCurrentDirectory(FileHelper.ProgramPath);
                xmlFile = DefaultXmlFilePath;
            }

            //Getting the files manually, if no command line arguments have been found.
            if (gbxFiles.Count == 0)
            {
                var files = FileHelper.GetFilePaths(".gbx");

                //No Files selected
                if(files == null)
                {
                    //End Program
                    Environment.Exit(Environment.ExitCode);
                }

                gbxFiles.AddRange(files);
            }

            //Generating the Conversion
            var conversion = Conversion.LoadConversion<SwitchConversion>(xmlFile); //TODO change to switch  

            //Loading the Settings
            Settings.GenerateSettings();

            //Creating IOManager
            using var ioManager = new IOManager(gbxFiles.Count);

            //Starting the converters on multiple Threads
            var tasks = new List<Task>();

            foreach(var gbxFile in gbxFiles)
            {
                tasks.Add(Task.Run(() => Convert(gbxFile, conversion, ioManager)));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch(AggregateException e)
            {
                foreach(var innerException in e.InnerExceptions)
                {
                    Console.WriteLine(innerException);
                }
                keepConsoleOpen = true;
            }

            if (Settings.GetSettings().DisplayMode == DisplayMode.Full)
            {
                Console.WriteLine("Remember to calculate shadows for the converted maps!");
                keepConsoleOpen = true;
            }

            if (keepConsoleOpen)
                Console.ReadKey();
        }

        public static void Convert(string mapFile, Conversion conversion, IOManager ioManager)
        {
            string errorMessage = null;

            var fileName = Path.GetFileName(mapFile).Replace(".Challenge.Gbx", ".Map.Gbx");
            var filePath = Path.GetDirectoryName(mapFile);
            try
            {
                Converter converter;
                try
                {
                    converter = new Converter(mapFile);
                }
                catch(IOException e)
                {
                    errorMessage += "File Error: Could not open file" +
                        "Details: " + e.Message;
                    goto error;
                }

                var outputPath = Settings.GetSettings().GetOutputPath(converter.file, filePath);
                outputPath = Path.Combine(outputPath, fileName);

#if DEBUG
                //Optional Tasks
                //CreateLog(converter.file, filePath);
                //PropagateClips(converter.file);
#endif

                conversion.Convert(converter.file);

                var createFileMode = Settings.GetSettings().OverwriteExistingFiles ? FileMode.Create : FileMode.CreateNew;
                try
                {
                    using var fs = new FileStream(outputPath, createFileMode, FileAccess.Write);
                    converter.WriteBack(fs);
                }
                catch(IOException e)
                {
                    errorMessage = "File Error: Could not store file, usually because a file with this name already exists. Delete the existing file or change the OverwriteExistingFiles setting in Settings.xml" + "\n" +
                        "Details: " + e.Message;
                    goto error;
                }

                ioManager.Success(fileName, filePath, converter.GetStatistics());
            }
            catch (UnsupportedMapBaseException)
            {
                errorMessage = "Map Error: Your map uses an unsupported oversized base, converting not possible";
            }
#if !DEBUG
            catch(Exception e)
            {
                errorMessage = "Unexpected Error: " + e.Message;
            }
#endif

         error:
            if(errorMessage != null)
            {
                ioManager.Error(fileName, errorMessage);
            }
        }

        public static void CreateLog(GBXFile file, string filePath)
        {
            string logFilePath = filePath.Replace(".Challenge.Gbx", ".txt").Replace(".Gbx", ".txt");

            using var fs = new FileStream(logFilePath, FileMode.Create);
            using var sw = new StreamWriter(fs);
            file.DumpToFile(sw);
        }

        public static object ClipLock = new object();

        public static void PropagateClips(GBXFile file)
        {
            lock(ClipLock)
            {
                string defaultXmlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\complex.xml";

                ComplexConversion complexConversion;
                var xmls = new XmlSerializer(typeof(ComplexConversion));

                using (var fs = new FileStream(defaultXmlFilePath, FileMode.Open))
                    complexConversion = (ComplexConversion)xmls.Deserialize(fs);

                //The Values in this loop can be adjusted
                foreach (var conversion in complexConversion.Conversions)
                {
                    if(conversion is BlockToItemConversion blockToItemConversion)
                        blockToItemConversion.PropagateClips(file, "AlpineRoadChaletSlopePillar", "AlpineChaletPillarClip");
                }

                using (var fs = new FileStream(defaultXmlFilePath + "l", FileMode.Create))
                    xmls.Serialize(fs, complexConversion);
            }
        }

        public static void AddPylonsToClip()
        {
            lock (ClipLock)
            {
                string defaultXmlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\complex.xml";

                ComplexConversion complexConversion;
                var xmls = new XmlSerializer(typeof(ComplexConversion));

                using (var fs = new FileStream(defaultXmlFilePath, FileMode.Open))
                    complexConversion = (ComplexConversion)xmls.Deserialize(fs);

                //The Values in this loop can be adjusted
                foreach (var conversion in complexConversion.Conversions)
                {
                    if (conversion is BlockToItemConversion blockToItemConversion)
                        blockToItemConversion.AddPylonsToClips("AlpineTubeClip", 1);
                }

                using (var fs = new FileStream(defaultXmlFilePath + "l", FileMode.Create))
                    xmls.Serialize(fs, complexConversion);
            }
        }

        public static void AddPylonsToGround()
        {
            lock (ClipLock)
            {
                string defaultXmlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\complex.xml";

                ComplexConversion complexConversion;
                var xmls = new XmlSerializer(typeof(ComplexConversion));

                using (var fs = new FileStream(defaultXmlFilePath, FileMode.Open))
                    complexConversion = (ComplexConversion)xmls.Deserialize(fs);

                //The Values in this loop can be adjusted
                foreach (var conversion in complexConversion.Conversions)
                {
                    if (conversion is BlockToItemConversion blockToItemConversion)
                        blockToItemConversion.AddPylonsToGround("NoGround");
                }

                using (var fs = new FileStream(defaultXmlFilePath + "l", FileMode.Create))
                    xmls.Serialize(fs, complexConversion);
            }
        }
    }
}