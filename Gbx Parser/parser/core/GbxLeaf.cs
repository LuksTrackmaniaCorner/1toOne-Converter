using System;
using System.Collections.Generic;
using System.Text;
using gbx.parser.visitor;

namespace gbx.parser.core
{
    public abstract class GbxLeaf : GbxComponent, IGbxLeaf
    {
        public abstract override string ToString();

        public abstract void FromString(string value);

        public override abstract TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg);
    }
}
