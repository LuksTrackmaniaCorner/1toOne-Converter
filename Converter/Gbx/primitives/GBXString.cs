using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Text.Encoding;

namespace Converter.Gbx.core.primitives
{
    public class GBXString : FileComponent , IEquatable<GBXString>
    {
        public static readonly string lengthKey = "Length";
        public static readonly string stringKey = "String";

        [XmlAttribute]
        public string Value;

        private GBXString()
        {

        }

        public GBXString(Stream s)
        {
            uint length = s.ReadUInt();
            byte[] srcdata = s.SimpleRead((int) length);
            Value = UTF8.GetString(srcdata);
        }

        public GBXString(string s) => Value = s;

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(Value);
            return result;
        }

        public override void WriteBack(Stream s)
        {
            s.WriteUInt((uint) UTF8.GetByteCount(Value));
            s.SimpleWrite(UTF8.GetBytes(Value));
        }

        public bool Equals(GBXString other)
        {
            return this.Value == other.Value;
        }
    }
}
