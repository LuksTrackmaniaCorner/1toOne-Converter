using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class ChallengeAuthor : Chunk
    {
        public static readonly string versionKey = "Version";
        public static readonly string authorVersionKey = "Author Version";
        public static readonly string authorLoginKey = "Author Login";
        public static readonly string authorNickKey = "Author Nickname";
        public static readonly string authorZoneKey = "Author Zone";
        public static readonly string authorExtraInfoKey = " Author Extra Info";

        public readonly GBXUInt version;
        public readonly GBXUInt authorVersion;
        public readonly GBXString authorLogin;
        public readonly GBXString authorNick;
        public readonly GBXString authorZone;
        public readonly GBXString authorExtraInfo;

        public ChallengeAuthor(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            version = new GBXUInt(s);
            AddChildDeprecated(versionKey, version);

            authorVersion = new GBXUInt(s);
            AddChildDeprecated(authorVersionKey, authorVersion);

            authorLogin = new GBXString(s);
            AddChildDeprecated(authorLoginKey, authorLogin);

            authorNick = new GBXString(s);
            AddChildDeprecated(authorNickKey, authorNick);

            authorZone = new GBXString(s);
            AddChildDeprecated(authorZoneKey, authorZone);

            authorExtraInfo = new GBXString(s);
            AddChildDeprecated(authorExtraInfoKey, authorExtraInfo);
        }
    }
}
