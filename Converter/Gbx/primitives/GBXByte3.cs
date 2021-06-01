using Converter.Gbx.Core;
using Converter.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Primitives
{
    public class GBXByte3 : PrimitiveVec3<byte>
    {
        private GBXByte3()
        {

        }

        public GBXByte3(Stream s)
        {
            X = s.ReadAByte();
            Y = s.ReadAByte();
            Z = s.ReadAByte();
        }

        public GBXByte3(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public GBXByte3((byte x, byte y, byte z) coords) : this(coords.x, coords.y, coords.z) { }

        public override void WriteBack(Stream s)
        {
            s.WriteByte(X);
            s.WriteByte(Y);
            s.WriteByte(Z);
        }

        public override FileComponent DeepClone()
        {
            return new GBXByte3(X, Y, Z);
        }
    }
}
