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
    public class GBXNat3 : PrimitiveVec3<uint>
    {
        private GBXNat3()
        {

        }

        public GBXNat3(Stream s)
        {
            X = s.ReadUInt();
            Y = s.ReadUInt();
            Z = s.ReadUInt();
        }

        public GBXNat3(uint x, uint y, uint z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override void WriteBack(Stream s)
        {
            s.WriteUInt(X);
            s.WriteUInt(Y);
            s.WriteUInt(Z);
        }

        public override FileComponent DeepClone()
        {
            return new GBXNat3(X, Y, Z);
        }
    }
}
