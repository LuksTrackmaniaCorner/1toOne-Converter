using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gbx.Parser.Visitor;

namespace Gbx.Parser.Core
{
    public abstract class GbxPrimitive3<T> : GbxComposite<GbxPrimitive<T>> where T : IEquatable<T>
    {
        public GbxPrimitive<T> X { get; }
        public GbxPrimitive<T> Y { get; }
        public GbxPrimitive<T> Z { get; }

        protected GbxPrimitive3(GbxPrimitive<T> x, GbxPrimitive<T> y, GbxPrimitive<T> z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override IEnumerable<GbxPrimitive<T>> GetChildren()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        public override IEnumerable<(string, GbxPrimitive<T>)> GetNamedChildren()
        {
            yield return (nameof(X), X);
            yield return (nameof(Y), Y);
            yield return (nameof(Z), Z);
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
