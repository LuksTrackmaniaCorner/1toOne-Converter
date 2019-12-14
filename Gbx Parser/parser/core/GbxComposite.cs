using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gbx.parser.visitor;

namespace gbx.parser.core
{
    public abstract class GbxComposite<T> : GbxComponent, IEnumerable<T> where T : GbxComponent
    {
        public abstract IEnumerable<T> GetChildren();

        public abstract IEnumerable<(string, T)> GetNamedChildren();

        //TODO check if this is lazily evaluated.
        public IEnumerator<T> GetEnumerator()
        {
            return GetChildren().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override abstract TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg);
    }
}
