using Gbx.Parser.Core;
using Gbx.Parser.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeBlocks : GbxChunk
    {
        public ChallengeBlocks(GbxChunkInfo chunkInfo, bool is013 = false) : base(chunkInfo)
        {
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            throw new NotImplementedException();
        }
    }
}
