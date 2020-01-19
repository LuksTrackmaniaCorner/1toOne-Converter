using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeMapCoordinates : GbxChunk
    {
        public GbxVec2 MapOrigin { get; }
        public GbxVec2 MapTarget { get; }

        public ChallengeMapCoordinates(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            MapOrigin = new GbxVec2();
            MapTarget = new GbxVec2();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(MapOrigin), MapOrigin);
            yield return (nameof(MapTarget), MapTarget);
        }
    }
}
