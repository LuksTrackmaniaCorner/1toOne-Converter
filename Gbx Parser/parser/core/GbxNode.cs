using gbx.parser.visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.core
{
    public class GbxNode : GbxComposite<GbxChunk>
    {
        public uint ClassID { get; }

        public SortedSet<GbxChunk> Children { get; }

        public GbxNode()
        {
            Children = new SortedSet<GbxChunk>();
        }

        public override IEnumerable<GbxChunk> GetChildren()
        {
            return Children;
        }

        public override IEnumerable<(string, GbxChunk)> GetNamedChildren()
        {
            foreach(var child in Children)
            {
                yield return (child.ChunkID.ToString("x"), child);
            }
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
