using gbx.parser.visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.core
{
    public abstract class GbxChunk : GbxStructure, IComparable<GbxChunk>
    {
        public static uint Facade = 0xFACADE01;

        //Only relevant for chunks of the Main Node
        public bool IsHeaderChunk { get; set; }
        public bool IsSkippable { get; set; }
        public uint ChunkID { get; }

        public GbxChunk(uint chunkID)
        {
            ChunkID = chunkID;
        }

        public override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }

        public int CompareTo(GbxChunk other)
        {
            return this.ChunkID.CompareTo(other.ChunkID);
        }
    }
}
