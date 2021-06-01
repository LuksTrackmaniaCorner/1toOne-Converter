using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Gbx.Core;
using Converter.Util;
using static System.Text.Encoding;

namespace Converter.Gbx.Primitives
{
    public class GBXFixedLengthString : FileComponent
    {
        private readonly byte[] _srcdata;
        private readonly string _string;

        //String of Fixed length, not terminated with \0
        public GBXFixedLengthString(Stream fs, int length)
        {
            _srcdata = fs.SimpleRead(length);
            _string = UTF8.GetString(_srcdata);
        }

        public string Get() => _string;

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(_string);
            return result;
        }

        public override void WriteBack(Stream s)
        {
            s.SimpleWrite(_srcdata);
        }
    }
}
