using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxByte : GbxPrimitive<byte>
    {
        public GbxByte(byte initialValue = 0, Predicate<byte>? constraint = null) : base(initialValue, constraint)
        {
        }

        public override void FromString(string value)
        {
            Value = byte.Parse(value);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public override void FromStream(BinaryReader reader)
        {
            Value = reader.ReadByte();
        }
    }
}
