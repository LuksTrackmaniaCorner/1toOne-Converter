using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gbx.Parser.Visit;

namespace Gbx.Parser.Core
{
    public abstract class GbxLeaf : GbxComponent
    {
        public override abstract string ToString();

        public abstract void FromString(string value);

        public abstract void ToStream(BinaryWriter writer);

        public abstract void FromStream(BinaryReader reader);

        internal override abstract void Accept(Visitor visitor);
    }
}
