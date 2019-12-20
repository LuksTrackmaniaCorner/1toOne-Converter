using Gbx.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxWrapper<T> : GbxComposite<T> where T : GbxComponent
    {
        public GbxComposite<T> Wrapped { get; }

        public GbxWrapper(GbxComposite<T> wrapped)
        {
            Wrapped = wrapped;
        }

        public sealed override IEnumerable<T> GetChildren()
        {
            return Wrapped.GetChildren();
        }

        public sealed override IEnumerable<(string, T)> GetNamedChildren()
        {
            return Wrapped.GetNamedChildren();
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
