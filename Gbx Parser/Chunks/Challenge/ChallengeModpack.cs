using Gbx.Parser.Core;
using Gbx.Parser.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeModpack : GbxChunk
    {
        public GbxFileReference Modpack { get; }

        public ChallengeModpack(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Modpack = new GbxFileReference();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Modpack), Modpack);
        }
    }
}
