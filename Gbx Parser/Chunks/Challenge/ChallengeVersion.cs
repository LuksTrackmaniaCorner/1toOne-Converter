using Gbx.Parser.Core;
using Gbx.Parser.info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeVersion : GbxChunk
    {
        public GbxUInt Version { get; }

        public ChallengeVersion(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Version = new GbxUInt();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Version), Version);
        }
    }
}
