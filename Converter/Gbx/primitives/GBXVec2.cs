using Converter.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Primitives
{
    public class GBXVec2 : PrimitiveVec2<float>
    {
        public GBXVec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public GBXVec2(Stream s)
        {
            X = s.ReadFloat();
            Y = s.ReadFloat();
        }

        public override void WriteBack(Stream s)
        {
            s.WriteFloat(X);
            s.WriteFloat(Y);
        }
    }
}
