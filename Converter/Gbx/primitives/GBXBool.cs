using Converter.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Primitives
{
    public class GBXBool : Primitive<bool>
    {
        public GBXBool(Stream s)
        {
            Value = BitConverter.ToUInt32(s.SimpleRead(4), 0) != 0;
        }

        public GBXBool(bool b) => Value = b;

        public override void WriteBack(Stream s)
        {
            int i;
            if (Value == true)
                i = 1;
            else
                i = 0;
            var srcdata = BitConverter.GetBytes(i);
            s.SimpleWrite(srcdata);
        }
    }
}
