using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxBool : GbxPrimitive<bool>
    {
        public GbxBool(bool initialValue = false, Predicate<bool>? constraint = null) : base(initialValue, constraint)
        {
        }

        public override void FromString(string value)
        {
            Value = bool.Parse(value);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.Write(Value ? 1u : 0u);
        }

        public override void FromStream(BinaryReader reader)
        {
            Value = reader.ReadUInt32() != 0;
        }
    }
}
