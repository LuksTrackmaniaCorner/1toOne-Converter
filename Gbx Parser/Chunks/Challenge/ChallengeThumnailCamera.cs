using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeThumnailCamera : GbxChunk
    {
        public GbxBool HasCamera { get; }
        public GbxByte Unknown { get; }
        //TODO create matrix class

        public ChallengeThumnailCamera(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            HasCamera = new GbxBool();
            Unknown = new GbxByte();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(HasCamera), HasCamera);

            if (HasCamera == false)
                yield break;
        }
    }
}
