using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeLapCount : GbxChunk
    {
        public GbxUInt Unknown { get; }
        public GbxUInt LapCount { get; }

        public ChallengeLapCount(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Unknown = new GbxUInt();
            LapCount = new GbxUInt();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Unknown), Unknown);
            yield return (nameof(LapCount), LapCount);
        }
    }
}
