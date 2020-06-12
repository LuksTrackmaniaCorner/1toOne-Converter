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
        protected readonly GBXFile _file;

        public Converter(string inputFilePath)
        {
            using var fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _file = new GBXFile(fs);
        }

        public void WriteBack(Stream outputFile)
        {
            _file.WriteBack(outputFile);
        }

        public LinkedList<string> Dump()
        {
            return _file.Dump();
        }

        public void Convert(Conversion conversion)
        {
            conversion.Convert(_file);
        }

        public string GetStatistics()
        {
            return _file.GetStatistics();
        }

        public string GetOutputPath(string mapPath)
        {
            return Settings.GetSettings().GetOutputPath(_file, mapPath);
        }
    }
}
