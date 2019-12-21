using Gbx.Parser.Core;
using Gbx.Parser.info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeAuthor : GbxChunk
    {
        public GbxUInt Version { get; }
        public GbxUInt AuthorVersion { get; }
        public GbxString AuthorLogin { get; }
        public GbxString AuthorNick { get; }
        public GbxString AuthorZone { get; }
        public GbxString AuthorExtraInfo { get; }

        public ChallengeAuthor(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Version = new GbxUInt();
            AuthorVersion = new GbxUInt();
            AuthorLogin = new GbxString();
            AuthorNick = new GbxString();
            AuthorZone = new GbxString();
            AuthorExtraInfo = new GbxString();

            AuthorLogin.MakeConstAfterNextSet();
            AuthorNick.MakeConstAfterNextSet();
            AuthorZone.MakeConstAfterNextSet();
            AuthorExtraInfo.MakeConstAfterNextSet();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Version), Version);
            yield return (nameof(AuthorVersion), AuthorVersion);
            yield return (nameof(AuthorLogin), AuthorLogin);
            yield return (nameof(AuthorNick), AuthorNick);
            yield return (nameof(AuthorZone), AuthorZone);
            yield return (nameof(AuthorExtraInfo), AuthorExtraInfo);
        }
    }
}
