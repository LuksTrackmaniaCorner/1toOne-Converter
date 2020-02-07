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
        private static readonly object ConsoleLock;

        static Converter()
        {
            ConsoleLock = new object();
        }


        private readonly GBXFile file;
        private Converter(string inputFileName)
        {
            using var fs = new FileStream(inputFileName, FileMode.Open);
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

        private void PrintStatistics()
        {
            file.PrintStatistics();
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
            string DefaultXmlFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\AlpineConversion.xml";

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
            xmlFile ??= DefaultXmlFilePath;

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
            //TODO check if file exists
            Conversion conversion;
            var xmls = new XmlSerializer(typeof(ComplexConversion));
            using (var fs = new FileStream(xmlFile, FileMode.Open))
                conversion = (Conversion)xmls.Deserialize(fs);
            conversion.Initialize();

            //Loading the Settings
            Settings.GenerateSettings();

            //Starting the converters on multiple Threads
            var tasks = new List<Task>();

            int numberOfFiles = gbxFiles.Count;
            int currentFileNumber = 0;

            foreach(var gbxFile in gbxFiles)
            {
                tasks.Add(Task.Run(() => Convert(gbxFile, conversion, ref currentFileNumber, numberOfFiles, ref keepConsoleOpen)));
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

            var settings = Settings.GetSettings();
            if(settings.OpenFolderAfterFinished)
            {
                if(settings.OutputFolder != Settings.RelativeDirectory)
                    Process.Start(settings.OutputFolder);
                else
                    Process.Start(Path.GetDirectoryName(gbxFiles[0]));
            }
        }

        public static void Convert(string filePath, Conversion conversion, ref int currentFileNumber, int numberOfFiles, ref bool keepConsoleOpen)
        {
            string errorMessage = null;

            string fileName = Path.GetFileName(filePath).Replace(".Challenge.Gbx", ".Map.Gbx");
            try
            {
                string outputPath;

                if (Settings.GetSettings().OutputFolder == Settings.RelativeDirectory)
                {
                    outputPath = filePath.Replace(".Challenge.Gbx", ".Map.Gbx");
                }
                else
                {
                    outputPath = Settings.GetSettings().OutputFolder + fileName;
                }

                var converter = new Converter(filePath);

#if DEBUG
                //Optional Tasks
                //CreateLog(converter.file, filePath);
                //PropagateClips(converter.file);
#endif

                conversion.Convert(converter.file);

                var createFileMode = Settings.GetSettings().OverwriteExistingFiles ? FileMode.Create : FileMode.CreateNew;
                using (var fs = new FileStream(outputPath, createFileMode))
                    converter.WriteBack(fs);

                switch (Settings.GetSettings().DisplayMode)
                {
                    case DisplayMode.None:
                        break;
                    case DisplayMode.TracksOnly:
                        lock (ConsoleLock)
                        {
                            currentFileNumber++;
                            Console.WriteLine(currentFileNumber + " / " + numberOfFiles + " converted: " + fileName);
                        }
                        break;
                    case DisplayMode.Full:
                        lock (ConsoleLock)
                        {
                            currentFileNumber++;
                            Console.WriteLine(currentFileNumber + " / " + numberOfFiles + " converted: " + fileName);
                            converter.PrintStatistics();
                            Console.WriteLine();
                        }
                        break;
                }
            }
            catch (UnsupportedMapBaseException)
            {
                errorMessage = "Map Error: Your map uses an unsupported oversized base, converting not possible";
            }
            catch(IOException)
            {
                errorMessage = "File Error: Could not open/store file, usually because a file with this name already exists. Delete the existing file or change the OverwriteExistingFiles setting in Settings.xml";
            }
#if !DEBUG
            catch(Exception e)
            {
                errorMessage = "Unexpected Error: " + e.Message;
            }
#endif

            if(errorMessage != null)
            {
                lock (ConsoleLock)
                {
                    currentFileNumber++;
                    Console.WriteLine(currentFileNumber + " / " + numberOfFiles + " failed: " + fileName);
                    Console.WriteLine(errorMessage);
                    Console.WriteLine();
                }

                keepConsoleOpen = true;
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