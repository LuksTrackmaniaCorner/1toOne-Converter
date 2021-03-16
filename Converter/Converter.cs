using Converter.Converion;
using Converter.Gbx;
using Converter.Gbx.core;
using Converter.Gbx.core.chunks;
using Converter.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class Converter : IDumpable
    {
        public GBXFile File { get; }

        public Converter(string inputFilePath)
        {
            using var fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            File = new GBXFile(fs);
        }

        public void WriteBack(Stream outputFile)
        {
            File.WriteBack(outputFile);
        }

        public LinkedList<string> Dump()
        {
            return File.Dump();
        }

        public void Convert(Conversion conversion)
        {
            conversion.Convert(File);
        }

        public string GetStatistics()
        {
            return File.GetStatistics();
        }

        public string GetOutputPath(string mapPath)
        {
            return Settings.GetSettings().GetOutputPath(File, mapPath);
        }
    }
}
