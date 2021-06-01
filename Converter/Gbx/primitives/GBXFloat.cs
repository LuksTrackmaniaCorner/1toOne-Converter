﻿using Converter.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Primitives
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
