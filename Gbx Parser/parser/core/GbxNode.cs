using Gbx.Parser.Info;
using Gbx.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxNode : GbxComposite<GbxChunk>
    {
        public GbxClassInfo ClassInfo { get; }

        private readonly SortedSet<GbxChunk> _chunks;

        /// <summary>
        /// Here are all NodeReferences stored that point to this node.
        /// </summary>
        private readonly SortedSet<GbxNodeReference> _nodeReferences;

        public GbxNode(GbxClassInfo classInfo)
        {
            ClassInfo = classInfo;

            _chunks = new SortedSet<GbxChunk>();
            _nodeReferences = new SortedSet<GbxNodeReference>();
        }

        public void Add(GbxChunk chunk)
        {
            //Test if this chunk is a part of this chunk
            if (!ClassInfo.CanContain(chunk.ChunkInfo))
                throw new Exception();

            _chunks.Add(chunk);
        }

        public void Remove(GbxChunk chunk)
        {
            _chunks.Remove(chunk);
        }

        public void Clear()
        {
            _chunks.Clear();
        }

        internal void AddNodeReference(GbxNodeReference nodeReference)
        {
            Trace.Assert(_nodeReferences.Add(nodeReference));
        }

        internal void RemoveNodeReference(GbxNodeReference nodeReference)
        {
            Trace.Assert(_nodeReferences.Remove(nodeReference));
        }

        public override IEnumerable<GbxChunk> GetChildren()
        {
            return _chunks;
        }

        public override IEnumerable<(string, GbxChunk)> GetNamedChildren()
        {
            foreach(var child in _chunks)
            {
                yield return (child.ChunkInfo.Description, child);
            }
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
