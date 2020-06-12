using Converter.Gbx.core.primitives;
using Converter.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class ChallengeCommon : Chunk
    {
        public static readonly string versionKey = "Version";
        public static readonly string trackMetaKey = "Meta";
        public static readonly string trackNameKey = "Track Name";
        public static readonly string kindKey = "Track Kind";
        public static readonly string lockedKey = "Locked";
        public static readonly string passwordKey = "Password";
        public static readonly string decorationMetaKey = "Decoration Meta";
        public static readonly string mapOriginKey = "Map Origin";
        public static readonly string mapTargetKey = "Map Target";
        public static readonly string unknownUInt128Key = "unknown uint128";
        public static readonly string mapTypeKey = "Map Type";
        public static readonly string mapStyleKey = "Map Style";
        public static readonly string unknownBoolKey = "unknown Bool";
        public static readonly string lightmapCacheUIDKey = "Lightmap Cache UID";
        public static readonly string lightmapVersionKey = "Lightmap Version";
        public static readonly string titleUIDKey = "Title UID";

        private GBXByte version;
        private Meta trackMeta;
        private GBXString trackName;
        private GBXByte kind;
        private GBXBool locked;
        private GBXString password;
        private Meta decorationMeta;
        private GBXVec2 mapOrigin;
        private GBXVec2 mapTarget;
        private Unread unknownUInt128;
        private GBXString mapType;
        private GBXString mapStyle;
        private GBXBool unknownBool;
        private GBXULong lightmapCacheUID;
        private GBXByte lightmapVersion;
        private GBXLBS titleUID;

        public GBXByte Version { get => version; set { version = value; AddChildNew(value); } }
        public Meta TrackMeta { get => trackMeta; set { trackMeta = value; AddChildNew(value); } }
        public GBXString TrackName { get => trackName; set { trackName = value; AddChildNew(value); } }
        public GBXByte Kind { get => kind; set { kind = value; AddChildNew(value); } }
        public GBXBool Locked { get => locked; set { locked = value; AddChildNew(value); } }
        public GBXString Password { get => password; set { password = value; AddChildNew(value); } }
        public Meta DecorationMeta { get => decorationMeta; set { decorationMeta = value; AddChildNew(value); } }
        public GBXVec2 MapOrigin { get => mapOrigin; set { mapOrigin = value; AddChildNew(value); } }
        public GBXVec2 MapTarget { get => mapTarget; set { mapTarget = value; AddChildNew(value); } }
        public Unread UnknownUInt128 { get => unknownUInt128; set { unknownUInt128 = value; AddChildNew(value); } }
        public GBXString MapType { get => mapType; set { mapType = value; AddChildNew(value); } }
        public GBXString MapStyle { get => mapStyle; set { mapStyle = value; AddChildNew(value); } }
        public GBXBool UnknownBool { get => unknownBool; set { unknownBool = value; AddChildNew(value); } }
        public GBXULong LightmapCacheUID { get => lightmapCacheUID; set { lightmapCacheUID = value; AddChildNew(value); } }
        public GBXByte LightmapVersion { get => lightmapVersion; set { lightmapVersion = value; AddChildNew(value); } }
        public GBXLBS TitleUID { get => titleUID; set { titleUID = value; AddChildNew(value); } }

        public ChallengeCommon(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            Version = new GBXByte(s);

            TrackMeta = new Meta(s, context);

            TrackName = new GBXString(s);

            Kind = new GBXByte(s);

            if (Version.Value < 1)
                return;
            //version >= 1
            Locked = new GBXBool(s);
            Locked.Value = false; //TODO replace with constant

            Password = new GBXString(s);

            if (Version.Value < 2)
                return;
            //version >= 2
            DecorationMeta = new Meta(s, context);

            if (Version.Value< 3)
                return;
            //version >= 3
            MapOrigin = new GBXVec2(s);

            if (Version.Value< 4)
                return;
            //version >= 4
            MapTarget = new GBXVec2(s);

            if (Version.Value< 5)
                return;
            //version >= 5
            UnknownUInt128 = new Unread(s, 16);

            if (Version.Value< 6)
                return;
            //version >= 6
            MapType = new GBXString(s);

            MapStyle = new GBXString(s);

            if(Version.Value== 7)
            {
                //version == 7
                UnknownBool = new GBXBool(s);
                return;
            }

            //version >= 8
            LightmapCacheUID = new GBXULong(s);

            if (Version.Value< 9)
                return;
            //version >= 9
            LightmapVersion = new GBXByte(s);

            if (Version.Value< 11)
                return;
            //version >= 11
            TitleUID = context.ReadLookBackString(s);
        }

        public void UpdateToVersion11()
        {
            Version.Value = 11;

            if (MapOrigin == null)
                MapOrigin = new GBXVec2(0, 0);
            if (MapTarget == null)
                MapTarget = new GBXVec2(0, 0);
            if (UnknownUInt128 == null)
                UnknownUInt128 = new Unread(16);
            if (MapType == null)
                MapType = new GBXString("Race");
            if (MapStyle == null)
                MapStyle = new GBXString("");
            unknownBool = null;
            MarkAsChanged();
            if (LightmapCacheUID == null)
                LightmapCacheUID = new GBXULong(new Random().NextULong());
            if (LightmapVersion == null)
                LightmapVersion = new GBXByte(7);
            if (TitleUID == null)
                TitleUID = new GBXLBS("");
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(versionKey, Version);
            result.AddChild(trackMetaKey, TrackMeta);
            result.AddChild(trackNameKey, TrackName);
            result.AddChild(kindKey, Kind);
            result.AddChild(lockedKey, Locked);
            result.AddChild(passwordKey, Password);
            result.AddChild(decorationMetaKey, DecorationMeta);
            result.AddChild(mapOriginKey, MapOrigin);
            result.AddChild(mapTargetKey, MapTarget);
            result.AddChild(unknownUInt128Key, UnknownUInt128);
            result.AddChild(mapTypeKey, MapType);
            result.AddChild(mapStyleKey, MapStyle);
            result.AddChild(unknownBoolKey, UnknownBool);
            result.AddChild(lightmapCacheUIDKey, LightmapCacheUID);
            result.AddChild(lightmapVersionKey, LightmapVersion);
            result.AddChild(titleUIDKey, TitleUID);
            return result;
        }
    }
}
