using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengePasswordOld : GbxChunk
    {
        public GbxUInt Unknown { get; }
        public GbxString Password { get; }

        public ChallengePasswordOld(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Unknown = new GbxUInt();
            Password = new GbxString();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Unknown), Unknown);
            yield return (nameof(Password), Password);
        }
    }
}
