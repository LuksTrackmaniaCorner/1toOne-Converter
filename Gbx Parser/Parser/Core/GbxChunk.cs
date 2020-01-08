using Gbx.Parser.Info;
using Gbx.Parser.Visit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    public abstract class GbxChunk : GbxStructure, IComparable<GbxChunk>
    {
        public static uint Facade = 0xFACADE01;

        //All the metadata of the chunk is stored here
        public GbxChunkInfo ChunkInfo { get; }

        public GbxChunk(GbxChunkInfo chunkInfo)
        {
            ChunkInfo = chunkInfo;
        }

        internal sealed override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }

        public int CompareTo(GbxChunk other)
        {
            return this.ChunkInfo.ChunkID.CompareTo(other.ChunkInfo.ChunkID);
        }
    }
}
