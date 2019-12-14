using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.primitives
{
    public class GBXFloat : Primitive<float>
    {
        private GBXFloat()
        {

        }

        public GBXFloat(float f)
        {
            Value = f;
        }

        public GBXFloat(Stream s)
        {
            Value = s.ReadFloat();
        }

        public override void WriteBack(Stream s)
        {
            s.WriteFloat(Value);
        }
    }
}
