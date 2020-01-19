using Gbx.Parser.Core;
using Gbx.Parser.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeMediatracker : GbxChunk
    {
        public GbxNodeReference Intro { get; }
        public GbxNodeReference Ingame { get; }
        public GbxNodeReference RaceEnd { get; }

        public ChallengeMediatracker(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Intro = new GbxNodeReference(null);
            Ingame = new GbxNodeReference(null);
            RaceEnd = new GbxNodeReference(null);
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Intro), Intro);
            yield return (nameof(Ingame), Ingame);
            yield return (nameof(RaceEnd), RaceEnd);
        }
    }
}
