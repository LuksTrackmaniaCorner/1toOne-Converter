using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.info
{
    public sealed class GbxChunkInfo
    {

        public uint ChunkID { get; }
        public string Description { get; }
        public bool IsHeaderChunk { get; }
        public bool IsSkippable { get; }
        private readonly GbxChunkConstructor _constructor;

        public GbxChunkInfo(uint chunkID, string description, bool isHeaderChunk, bool isSkippable,
            GbxChunkConstructor constructor)
        {
            ChunkID = chunkID & GbxInfo.ChunkMask;
            Description = description;
            IsHeaderChunk = isHeaderChunk;
            IsSkippable = isSkippable;
            _constructor = constructor;
        }

        public GbxChunk CreateChunk()
        {
            return _constructor(this);
        }
    }

    public delegate GbxChunk GbxChunkConstructor(GbxChunkInfo chunkInfo);
}
