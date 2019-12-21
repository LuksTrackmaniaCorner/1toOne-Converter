using Gbx.Parser.Core;
using Gbx.Parser.info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeParameter : GbxChunk
    {
        public GbxNodeReference CollectorList { get; }
        public GbxNodeReference ChallengeParameters { get; }
        public GbxUInt Kind { get; } //TODO implement enum

        public ChallengeParameter(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            CollectorList = new GbxNodeReference(); //TODO specify type
            ChallengeParameters = new GbxNodeReference(); //TODO specify type
            Kind = new GbxUInt();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(CollectorList), CollectorList);
            yield return (nameof(ChallengeParameters), ChallengeParameters);
            yield return (nameof(Kind), Kind);
        }
    }
}
