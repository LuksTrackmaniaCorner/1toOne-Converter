using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Primitive
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
