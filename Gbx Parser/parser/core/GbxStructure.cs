using gbx.parser.visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.core
{
    public abstract class GbxStructure : GbxComposite<IGbxComponent>
    {
        private bool _cacheIsValid;
        private readonly List<string> _nameCache;
        private readonly List<GbxComponent> _childrenCache;

        public GbxStructure()
        {
            _cacheIsValid = false;
            _nameCache = new List<string>();
            _childrenCache = new List<GbxComponent>();
        }

        public void InvalidateChildrenCache()
        {
            _cacheIsValid = false;
        }

        private void ClearCache()
        {
            _cacheIsValid = false;
            _nameCache.Clear();
            _childrenCache.Clear();
        }

        public sealed override IEnumerable<IGbxComponent> GetChildren()
        {
            if (_cacheIsValid)
                return _childrenCache;
            else
                return GetChildrenBuildCache();
        }

        /// <summary>
        /// This Helper method rebuilds the childrenCache
        /// </summary>
        /// <returns>All children of the structure with name in order</returns>
        private IEnumerable<GbxComponent> GetChildrenBuildCache()
        {
            ClearCache();

            foreach (var (name, child) in GenerateChildren())
            {
                _nameCache.Add(name);
                _childrenCache.Add(child);
                yield return child;
            }

            _cacheIsValid = true;
            NotifyChange();
        }

        public sealed override IEnumerable<(string, IGbxComponent)> GetNamedChildren()
        {
            if (!_cacheIsValid)
            {
                ClearCache();

                foreach ((string name, GbxComponent child) namedChild in GenerateChildren())
                {
                    _nameCache.Add(namedChild.name);
                    _childrenCache.Add(namedChild.child);
                    yield return namedChild;
                }

                _cacheIsValid = true;
                NotifyChange();
            }
            else
            {
                //Cache data is Valid
                var nameEnumerator = _nameCache.GetEnumerator();
                var childEnumerator = _childrenCache.GetEnumerator();

                while (nameEnumerator.MoveNext() && childEnumerator.MoveNext())
                {
                    yield return (nameEnumerator.Current, childEnumerator.Current);
                }
            }
        }

        protected abstract IEnumerable<(string, GbxComponent)> GenerateChildren();

        public override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
