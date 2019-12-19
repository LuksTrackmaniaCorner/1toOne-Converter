using System;
using System.Collections.Generic;
using System.Text;
using gbx.parser.visitor;

namespace gbx.parser.core
{
    public class GbxMeta : GbxComposite<GbxLookBackString>
    {
        public override IEnumerable<GbxLookBackString> GetChildren()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(string, GbxLookBackString)> GetNamedChildren()
        {
            throw new NotImplementedException();
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
