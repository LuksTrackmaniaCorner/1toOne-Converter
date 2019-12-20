using Gbx.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxArray<T> : GbxComposite<T> /*, IReadOnlyList<T>*/ where T : GbxComponent
    {
        private readonly List<T> _children;
        private readonly Func<T> _producer;

        public int Count
        {
            get => _children.Count;
        }

        public T this[int index]
        {
            get => _children[index];
        }

        public GbxArray(Func<T> producer)
        {
            _children = new List<T>();
            _producer = producer;
        }

        public T AddNew()
        {
            var child = _producer();
            _children.Add(child);
            return child;
        }

        public void InsertNew(int index)
        {
            _children.Insert(index, _producer());
        }

        public void RemoveAt(int index)
        {
            _children.RemoveAt(index);
        }

        public void Clear()
        {
            _children.Clear();
        }

        public override IEnumerable<T> GetChildren()
        {
            return _children.AsReadOnly();
        }

        public override IEnumerable<(string, T)> GetNamedChildren()
        {
            for(int i = 0; i < Count; i++)
            {
                yield return (i.ToString(), _children[i]);
            }
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
