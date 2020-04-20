using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.Gbx.core
{
    public class Unread : FileComponent
    {
        [XmlAttribute(DataType = "hexBinary")]
        public byte[] Value { get; set; }

        private Unread()
        {

        }

        public Unread(byte[] bytes) => Value = bytes;

        public Unread(Stream s, int numBytes)
        {
            Value = s.SimpleRead(numBytes);
        }

        public Unread(int i)
        {
            Value = new byte[i];
        }

        public byte[] Get() => Value;

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();

            result.AddLast(Value.Length + " Unread Bytes");

            return result;
        }

        public override void WriteBack(Stream fs)
        {
            fs.SimpleWrite(Value);
        }
    }
}
