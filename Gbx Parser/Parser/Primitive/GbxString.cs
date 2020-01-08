using Gbx.Parser.Core;
using Gbx.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Primitive
{
    public class GbxString : GbxPrimitive<string>
    {
        public GbxString(string value = "", Predicate<string>? valueChecker = null) : base(value, valueChecker)
        {
        }

        public override void FromString(string value)
        {
            TrySetValue(value);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.WriteUTF8String(Value);
        }

        public override void FromStream(BinaryReader reader)
        {
            Value = reader.ReadUTF8String();
        }
    }
}
