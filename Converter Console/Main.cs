using Converter.Conversion;
using Converter.Gbx;
using Converter.Gbx.Core;
using Converter.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter
{
    public static class Program
    {
        [STAThread] //Required for OpenFileDialog
        public static void Main(string[] args)
        {
#if DEBUG
            SetInvariantCulture();
            ConvertMaps(args);
#else
            try
            {
                SetInvariantCulture();
                ConvertMaps(args);
            }
            catch(Exception e)
            {
                Console.WriteLine("An unexpected error occured:");
                Console.WriteLine(e);
                Console.ReadKey();
            }
#endif
        }

        public static void SetInvariantCulture()
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.CurrentCulture = culture;
            System.Globalization.CultureInfo.CurrentUICulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public static void ConvertMaps(string[] args)
        {
            const string DefaultXmlFilePath = @"Default.xml";

            var gbxFiles = new List<string>();

            //Getting the files from the command line arguments
            foreach(var file in args)
            {
                if(file.EndsWith(".Challenge.Gbx", true, null))
                {
                    gbxFiles.Add(file);
                }
                else
                {
                    //TODO add warning.
                }
            }

            Directory.SetCurrentDirectory(FileHelper.ProgramPath);
            var xmlFile = DefaultXmlFilePath;

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
            var conversion = Conversion.Conversion.LoadConversion<SwitchConversion>(xmlFile);

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

            Task.WaitAll(tasks.ToArray());

            ioManager.KeepConsoleOpen();
            ioManager.OpenFolders();
        }

        public static void Convert(string mapFile, Conversion.Conversion conversion, IOManager ioManager)
        {
            string errorMessage = null;

            var fileName = Path.GetFileName(mapFile);
            var outputName = fileName.Replace(".Challenge.Gbx", ".Map.Gbx");
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

                var outputPath = converter.GetOutputPath(filePath);
                var outputMapFile = Path.Combine(outputPath, outputName);

#if DEBUG
                //Optional Tasks
                //CreateLog(converter.File, mapFile);
                //PropagateClips(converter.file);
#endif

                converter.Convert(conversion);

                var createFileMode = Settings.GetSettings().OverwriteExistingFiles ? FileMode.Create : FileMode.CreateNew;
                try
                {
                    using var fs = new FileStream(outputMapFile, createFileMode, FileAccess.Write);
                    converter.WriteBack(fs);
                }
                catch(IOException e)
                {
                    errorMessage = "File Error: Could not store file, usually because a file with this name already exists. Delete the existing file or change the OverwriteExistingFiles setting in Settings.xml" + "\n" +
                        "Details: " + e.Message;
                    goto error;
                }

                ioManager.Success(outputName, outputPath, converter.GetStatistics());
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
                ioManager.Error(outputName, errorMessage);
            }
        }

        public static void CreateLog(GBXFile file, string filePath)
        {
            string logFilePath = filePath.Replace(".Challenge.Gbx", ".txt").Replace(".Gbx", ".txt");

            using var fs = new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            using var sw = new StreamWriter(fs);
            file.DumpToFile(sw);
        }
    }
}