using Gbx.Parser.Core;
using Gbx.Parser.info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeGlobalClip : GbxChunk
    {
        public GbxNodeReference GlobalClip { get; }

        public ChallengeGlobalClip(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            GlobalClip = new GbxNodeReference(); //TODO specify type
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(GlobalClip), GlobalClip);
        }
    }
}
