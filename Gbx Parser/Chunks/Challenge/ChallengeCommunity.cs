using Gbx.Parser.Core;
using Gbx.Parser.info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeCommunity : GbxChunk
    {
        GbxString Xml { get; }

        public ChallengeCommunity(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Xml = new GbxString();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Xml), Xml);
        }
    }
}
