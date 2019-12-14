using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.primitives
{
    public class GBXULong : Primitive<ulong>
    {
        public GBXULong(ulong l) => Value = l;

        public GBXULong(Stream s)
        {
            Value = BitConverter.ToUInt64(s.SimpleRead(8), 0);
        }

        public override void WriteBack(Stream s)
        {
            var srcdata = BitConverter.GetBytes(Value);
            s.SimpleWrite(srcdata);
        }
    }
}
