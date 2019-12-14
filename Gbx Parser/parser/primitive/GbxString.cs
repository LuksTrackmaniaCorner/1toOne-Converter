using gbx.parser.core;
using gbx.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxString : GbxPrimitive<string>
    {
        public GbxString(string value = "", Func<string, bool>? valueChecker = null) : base(value, valueChecker)
        {
        }

        public override void FromString(string value)
        {
            TrySetValue(value);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.WriteUTF8(Value);
        }

        public override void FromStream(BinaryReader reader)
        {
            Value = reader.ReadUTF8();
        }
    }
}
