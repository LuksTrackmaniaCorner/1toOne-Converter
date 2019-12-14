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
using System.Xml.Serialization;

namespace _1toOne_Converter.src
{
    public class Converter : IDumpable
    {
        public readonly GBXFile file;
        private Converter(string inputFileName)
        {
            using (var fs = new FileStream(inputFileName, FileMode.Open)) {
                file = new GBXFile
                    (fs);
            }
        }

        public void WriteBack(Stream outputFile)
        {
            file.WriteBack(outputFile);
        }

        public LinkedList<string> Dump()
        {
            return file.Dump();
        }

        [STAThread]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //Get a gbxFile
            var gbxFilePath = FileHelper.GetFilePath(".gbx");

            if (gbxFilePath == null) {
                //End Program
                Environment.Exit(Environment.ExitCode);
            }

#if !DEBUG
            Trace.Assert(gbxFilePath.EndsWith(".Challenge.Gbx"), "Please select a .Challenge.Gbx File.");
#endif

            var converter = new Converter(gbxFilePath);

#if DEBUG
            Console.WriteLine(gbxFilePath);

            Console.ReadKey();

            Console.BufferHeight = Int16.MaxValue - 1;
            converter.DumpToConsole();
#endif

            //CreateComplexConversion(converter.file);
            ApplyComplexConversion(converter.file);


#region WriteBack
            string outputPath = gbxFilePath.Replace(".Challenge.Gbx", ".Map.Gbx");

            using (var fs = new FileStream(outputPath, FileMode.Create))
                converter.WriteBack(fs);
            #endregion

            #region FileDump
            //string logFilePath = gbxFilePath.Replace(".Challenge.Gbx", ".txt");

            //using (var fs = new FileStream(logFilePath, FileMode.Create))
            //{
            //    using (var sw = new StreamWriter(fs))
            //        converter.DumpToFile(sw);
            //}
            #endregion

#if DEBUG
            Console.ReadKey();
#endif
        }

        public static void CreateComplexConversion(GBXFile file)
        {
            var xmlPath = @".\TerrainConversion.xml";

            using (var fs = new FileStream(xmlPath, FileMode.CreateNew))
            {
                var xmls = new XmlSerializer(typeof(ComplexConversion));

                var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
                var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

                var blockConv = new BlockAddConversion
                {
                    ExtraBlocks = new List<Block>()
                };
                foreach (var block in blockChunk.Blocks)
                {
                    blockConv.ExtraBlocks.Add(block);
                }


                var complexConv = new ComplexConversion
                {
                    Conversions = new List<Conversion>()
                };
                complexConv.Conversions.Add(new BlockClearConversion());
                complexConv.Conversions.Add(blockConv);
                xmls.Serialize(fs, complexConv);
            }
        }

        public static void ApplyComplexConversion(GBXFile file)
        {
            var xmlPath = @".\TerrainConversion.xml";

            using (var fs = new FileStream(xmlPath, FileMode.Open))
            {
                var xmls = new XmlSerializer(typeof(ComplexConversion));
                var test = (Conversion)xmls.Deserialize(fs);
                test.Convert(file);
            }
        }
    }
}
