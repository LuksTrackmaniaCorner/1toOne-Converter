using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxFloat : GbxPrimitive<float>
    {
        public GbxFloat(float value = 0, Predicate<float>? constraint = null) : base(value, constraint)
        {
        }

        public override void FromString(string value)
        {
            Value = float.Parse(value);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public override void FromStream(BinaryReader reader)
        {
            Value = reader.ReadSingle();
        }
    }
}
