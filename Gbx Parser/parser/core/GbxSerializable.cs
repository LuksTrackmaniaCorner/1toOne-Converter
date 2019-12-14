using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.parser.visitor;

namespace gbx.parser.core
{
    public abstract class GbxSerializable : GbxLeaf
    {
        public abstract void ToStream(BinaryWriter writer);

        public abstract void FromStream(BinaryReader reader);

        public override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
