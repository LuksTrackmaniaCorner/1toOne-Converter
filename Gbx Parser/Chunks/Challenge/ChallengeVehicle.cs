using Gbx.Parser.Core;
using Gbx.Parser.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeVehicle : GbxChunk
    {
        public GbxMeta Vehicle { get;}

        public ChallengeVehicle(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Vehicle = new GbxMeta();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Vehicle), Vehicle);
        }
    }
}
