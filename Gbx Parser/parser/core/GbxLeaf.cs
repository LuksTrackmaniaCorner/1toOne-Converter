using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.parser.visitor;

namespace gbx.parser.core
{
    public abstract class GbxLeaf : GbxComponent
    {
        public override abstract string ToString();

        public abstract void FromString(string value);

        public abstract void ToStream(BinaryWriter writer);

        public abstract void FromStream(BinaryReader reader);

        internal override abstract TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg);
    }
}
