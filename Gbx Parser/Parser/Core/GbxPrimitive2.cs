using Gbx.Parser.Visit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Core
{
    public abstract class GbxPrimitive2<T> : GbxComposite<GbxPrimitive<T>> where T : IEquatable<T>
    {
        public GbxPrimitive<T> X { get; }
        public GbxPrimitive<T> Y { get; }

        protected GbxPrimitive2(GbxPrimitive<T> x, GbxPrimitive<T> y)
        {
            X = x;
            Y = y;
        }

        public override IEnumerable<GbxPrimitive<T>> GetChildren()
        {
            yield return X;
            yield return Y;
        }

        public override IEnumerable<(string, GbxPrimitive<T>)> GetNamedChildren()
        {
            yield return (nameof(X), X);
            yield return (nameof(Y), Y);
        }

        internal override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
