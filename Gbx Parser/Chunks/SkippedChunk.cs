using Gbx.Parser.Core;
using Gbx.Parser.info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks
{
    public class SkippedChunk : GbxChunk
    {
        public GbxUnread Bytes { get; }

        public SkippedChunk(GbxChunkInfo chunkInfo, int length) : base(chunkInfo)
        {
            Bytes = new GbxUnread(length);
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Bytes), Bytes);
        }
    }
}
