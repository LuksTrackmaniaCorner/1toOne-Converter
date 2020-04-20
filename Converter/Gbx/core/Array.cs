using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using Converter.Gbx.core.primitives;
using Converter.util;

namespace Converter.Gbx.core
{
    public delegate T Producer<T>();
    public delegate bool ContinueCondition();

    public class Array<T> : FileComponent, IEnumerable<T>
        where T : FileComponent
    {
        private readonly List<T> _children;

        public T[] Value
        {
            get => _children.ToArray();
            private set
            {
                _children.Clear();
                foreach (var child in value)
                    AddChild(child);
                UpdateSize();
            }
        }

        private GBXUInt _size;

        private Array() => _children = new List<T>();

        public Array(uint size, Producer<T> producer) : this()
        {
            for (uint i = 0; i < size; i++) {
                AddChild(producer());
            }
        }

        public Array(Producer<T> producer, ContinueCondition continueCondition) : this()
        {
            while(continueCondition())
            {
                AddChild(producer());
            }
        }

        public void LinkSize(GBXUInt size) => _size = size;

        private void UpdateSize()
        {
            if(_size != null)
            {
                _size.Value = (uint)_children.Count;
            }
        }

        public void Add(T newValue)
        {
            AddChild(newValue);
            UpdateSize();
        }

        public void AddAll(IEnumerable<T> newValues)
        {
            foreach(var newValue in newValues)
            {
                AddChild(newValue);
            }
            UpdateSize();
        }

        public void Clear()
        {
            _children.Clear();
            UpdateSize();
        }

        private void AddChild(T child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        public T Get(int i)
        {
            return _children[i];
        }

        public override void WriteBack(Stream fs)
        {
            foreach(var child in _children) {
                child.WriteBack(fs);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            foreach (var child in this) {
                foreach (var s in child.Dump()) {
                    result.AddLast(s);
                }
            }
            return result;
        }
    }
}
