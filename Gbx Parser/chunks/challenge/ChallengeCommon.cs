using Gbx.Parser.Core;
using Gbx.Parser.info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeCommon : GbxChunk
    {
        public GbxByte Version { get; }
        public GbxMeta TrackMeta { get; }
        public GbxString TrackName { get; }
        public GbxByte Kind { get; }
        public GbxBool Locked { get; }
        public GbxString Password { get; }
        public GbxMeta DecorationMeta { get; }
        public GbxVec2 MapOrigin { get; }
        public GbxVec2 MapTarget { get; }
        public GbxUnread UnknownUInt128 { get; } //Todo replace with GbxULong128
        public GbxString MapType { get; }
        public GbxString MapStyle { get; }
        public GbxBool UnknownBool { get; }
        public GbxUnread LightmapCacheUID { get; } //TODO replace with GbxULong
        public GbxByte LightmapVersion { get; }
        public GbxLookBackString TitleUID { get; }

        public ChallengeCommon(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Version = new GbxByte();
            TrackMeta = new GbxMeta();
            TrackName = new GbxString();
            Kind = new GbxByte();
            Locked = new GbxBool();
            Password = new GbxString();
            DecorationMeta = new GbxMeta();
            MapOrigin = new GbxVec2();
            MapTarget = new GbxVec2();
            UnknownUInt128 = new GbxUnread(16);
            MapType = new GbxString();
            MapStyle = new GbxString();
            UnknownBool = new GbxBool();
            LightmapCacheUID = new GbxUnread(8);
            LightmapVersion = new GbxByte();
            TitleUID = new GbxLookBackString();

            Version.OnChange += (x) => NotifyChange();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Version), Version);
            yield return (nameof(TrackMeta), TrackMeta);
            yield return (nameof(TrackName), TrackName);
            yield return (nameof(Kind), Kind);

            if (Version == 0)
                yield break;

            yield return (nameof(Locked), Locked);
            yield return (nameof(Password), Password);

            if (Version == 1)
                yield break;

            yield return (nameof(DecorationMeta), DecorationMeta);

            if (Version == 2)
                yield break;

            yield return (nameof(MapOrigin), MapOrigin);

            if (Version == 3)
                yield break;

            yield return (nameof(MapTarget), MapTarget);

            if (Version == 4)
                yield break;

            yield return (nameof(UnknownUInt128), UnknownUInt128);

            if (Version == 5)
                yield break;

            yield return (nameof(MapType), MapType);
            yield return (nameof(MapStyle), MapStyle);

            if (Version == 6)
                yield break;

            if (Version == 7)
            {
                yield return (nameof(UnknownBool), UnknownBool);
                yield break;
            }

            yield return (nameof(LightmapCacheUID), LightmapCacheUID);

            if (Version == 8)
                yield break;

            yield return (nameof(LightmapVersion), LightmapVersion);

            if (Version <= 10)
                yield break;

            yield return (nameof(TitleUID), TitleUID);
        }
    }
}
