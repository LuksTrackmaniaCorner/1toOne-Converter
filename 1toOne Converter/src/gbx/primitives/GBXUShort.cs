﻿using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.primitives
{
    public class GBXUShort : Primitive<ushort>
    {
        private GBXUShort()
        {

        }

        public GBXUShort(ushort s)
        {
            Value = s;
        }

        public GBXUShort(Stream s)
        {
            Value = BitConverter.ToUInt16(s.SimpleRead(2), 0);
        }

        public override void WriteBack(Stream s)
        {
            var srcdata = BitConverter.GetBytes(Value);
            s.SimpleWrite(srcdata);
        }
    }
}
