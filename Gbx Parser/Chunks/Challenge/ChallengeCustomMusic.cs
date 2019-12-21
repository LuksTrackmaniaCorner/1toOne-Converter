using Gbx.Parser.Core;
using Gbx.Parser.info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeCustomMusic : GbxChunk
    {
        public GbxFileReference CustomMusic { get; }

        public ChallengeCustomMusic(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            CustomMusic = new GbxFileReference();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(CustomMusic), CustomMusic);
        }
    }
}
