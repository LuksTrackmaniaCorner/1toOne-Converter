using gbx.parser.info;
using gbx.parser.visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.core
{
    public class GbxNode : GbxComposite<GbxChunk>
    {
        public GbxClassInfo ClassInfo { get; }

        private readonly SortedSet<GbxChunk> _chunks;

        public GbxNode(GbxClassInfo classInfo)
        {
            ClassInfo = classInfo;

            _chunks = new SortedSet<GbxChunk>();
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
