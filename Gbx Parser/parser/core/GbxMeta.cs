using System;
using System.Collections.Generic;
using System.Text;
using Gbx.Parser.Visitor;

namespace Gbx.Parser.Core
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
