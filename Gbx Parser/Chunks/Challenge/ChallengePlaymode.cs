using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengePlaymode : GbxChunk
    {
        public GbxUInt Playmode { get; }

        public ChallengePlaymode(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Playmode = new GbxUInt();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Playmode), Playmode);
        }
    }
}
