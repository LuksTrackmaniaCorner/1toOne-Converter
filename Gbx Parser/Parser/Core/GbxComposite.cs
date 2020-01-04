using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gbx.Parser.Visit;

namespace Gbx.Parser.Core
{
    public abstract class GbxComposite<T> : GbxComponent, IEnumerable<T> where T : GbxComponent
    {
        public abstract IEnumerable<T> GetChildren();

        public abstract IEnumerable<(string, T)> GetNamedChildren();

        public IEnumerator<T> GetEnumerator()
        {
            return GetChildren().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal override abstract void Accept(Visitor visitor);
    }
}
