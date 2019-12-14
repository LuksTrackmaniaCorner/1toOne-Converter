using gbx.parser.visitor;
using System;
using System.Collections.Generic;

namespace gbx.parser.core
{
    public abstract class GbxComponent : IGbxComponent
    {
        public event Action<IGbxComponent>? OnChange;

        private Dictionary<string, object>? _componentData;

        void IGbxComponent.AddComponentData(string key, object data)
        {
            _componentData ??= new Dictionary<string, object>();
            _componentData.Add(key, data);
        }

        void IGbxComponent.UpdateComponentData(string key, object data)
        {
            _componentData ??= new Dictionary<string, object>();
            _componentData[key] = data;
        }

        object? IGbxComponent.GetComponentData(string key)
        {
            if (_componentData == null)
                return null;

            _componentData.TryGetValue(key, out object? result);
            return result;
        }

        void IGbxComponent.RemoveComponentData(string key)
        {
            _componentData?.Remove(key);
        }

        protected void NotifyChange()
        {
            OnChange?.Invoke(this);
        }

        //public GbxComponent DeepClone();

        public abstract TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg);
    }
}
