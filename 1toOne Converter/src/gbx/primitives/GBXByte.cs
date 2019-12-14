using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.primitives
{
    public class GBXByte : Primitive<byte>
    {
        private GBXByte()
        {

        }

        public GBXByte(byte b)
        {
            Value = b;
        }

        public GBXByte(Stream s)
        {
            Value = s.ReadAByte();
        }

        public override void WriteBack(Stream s)
        {
            s.WriteByte(Value);
        }
    }
}
