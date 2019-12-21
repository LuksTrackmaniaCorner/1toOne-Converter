using Gbx.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gbx.Parser.Core
{
    public abstract class GbxStructure : GbxComposite<GbxComponent>
    {
        public sealed override IEnumerable<GbxComponent> GetChildren()
        {
            foreach (var (_, child) in GetNamedChildren())
                yield return child;
        }

        public abstract override IEnumerable<(string, GbxComponent)> GetNamedChildren();

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}


