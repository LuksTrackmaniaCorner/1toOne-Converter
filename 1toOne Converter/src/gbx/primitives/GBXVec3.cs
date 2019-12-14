using _1toOne_Converter.src.gbx.primitives;
using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.primitives
{
    public class GBXVec3 : PrimitiveVec3<float>
    {
        private GBXVec3()
        {
            
        }

        public GBXVec3(Stream s)
        {
            X = s.ReadFloat();
            Y = s.ReadFloat();
            Z = s.ReadFloat();
        }

        public GBXVec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override void WriteBack(Stream s)
        {
            s.WriteFloat(X);
            s.WriteFloat(Y);
            s.WriteFloat(Z);
        }
    }
}
