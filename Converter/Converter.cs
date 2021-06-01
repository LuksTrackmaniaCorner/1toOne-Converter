using Converter.Conversion;
using Converter.Gbx;
using Converter.Gbx.Core;
using Converter.Util;
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

        public void Convert(Conversion.Conversion conversion)
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
