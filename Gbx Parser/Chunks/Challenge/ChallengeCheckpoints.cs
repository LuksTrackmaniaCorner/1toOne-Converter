using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeCheckpoints : GbxChunk
    {
        public GbxArray<GbxUInt3> Checkpoints { get; }

        public ChallengeCheckpoints(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Checkpoints = new GbxArray<GbxUInt3>(() => new GbxUInt3());
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Checkpoints), Checkpoints);
        }
    }
}
