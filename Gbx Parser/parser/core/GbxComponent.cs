using gbx.parser.visitor;
using System;
using System.Collections.Generic;

namespace gbx.parser.core
{
    public abstract class GbxComponent
    {
        public event Action<GbxComponent>? OnChange;

        private Dictionary<string, object>? _componentData;

        internal void AddComponentData(string key, object data)
        {
            _componentData ??= new Dictionary<string, object>();
            _componentData.Add(key, data);
        }

        internal void UpdateComponentData(string key, object data)
        {
            _componentData ??= new Dictionary<string, object>();
            _componentData[key] = data;
        }

        internal object? GetComponentData(string key)
        {
            if (_componentData == null)
                return null;

            _componentData.TryGetValue(key, out object? result);
            return result;
        }

        internal void RemoveComponentData(string key)
        {
            _componentData?.Remove(key);
        }

        protected void NotifyChange()
        {
            OnChange?.Invoke(this);
        }

        //public GbxComponent DeepClone();

        internal abstract TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg);
    }
}
