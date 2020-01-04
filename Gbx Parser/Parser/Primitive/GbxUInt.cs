using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Primitive
{
    public class GbxUInt : GbxPrimitive<uint>
    {
        public GbxUInt(uint initialValue = 0, Predicate<uint>? constraint = null) : base(initialValue, constraint)
        {
        }

        public override void FromString(string value)
        {
            Value = uint.Parse(value);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public override void FromStream(BinaryReader reader)
        {
            Value = reader.ReadUInt32();
        }
    }
}
