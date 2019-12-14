using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.primitives
{
    public class GBXMat3 : Structure
    {
        private GBXVec3 x;
        private GBXVec3 y;
        private GBXVec3 z;

        public GBXVec3 X { get => x; set { x = value; AddChildNew(value); } }
        public GBXVec3 Y { get => y; set { y = value; AddChildNew(value); } }
        public GBXVec3 Z { get => z; set { z = value; AddChildNew(value); } }

        public GBXMat3(Stream s)
        {
            X = new GBXVec3(s);
            Y = new GBXVec3(s);
            Z = new GBXVec3(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild("X", X);
            result.AddChild("Y", Y);
            result.AddChild("Z", Z);
            return result;
        }
    }
}
