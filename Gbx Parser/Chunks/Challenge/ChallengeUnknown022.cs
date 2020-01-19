using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeUnknown022 : GbxChunk
    {
        public GbxUInt Unknown { get; }

        public ChallengeUnknown022(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Unknown = new GbxUInt();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Unknown), Unknown);
        }
    }
}
