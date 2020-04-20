using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class ChallengeDesc : Chunk
    {
        public static readonly string versionKey = "Version";
        public static readonly string alwaysFalseKey = "Always False";
        public static readonly string bronzeTimeKey = "Bronze Time";
        public static readonly string silverTimeKey = "Silver Time";
        public static readonly string goldTimeKey = "Gold Time";
        public static readonly string authorTimeKey = "Author Time";
        public static readonly string displayCostKey = "Display Cost";
        public static readonly string multilapKey = "Multilap";
        public static readonly string unknownKey = "Unknown";
        public static readonly string trackTypeKey = "Track Type";
        public static readonly string alwaysZeroKey = "Always Zero";
        public static readonly string authorScoreKey = "Author Score";
        public static readonly string editModeKey = "Edit Mode";
        public static readonly string alwaysFalseTooKey = "Always false too";
        public static readonly string numCPsKey = "NumberOfCheckpoints";
        public static readonly string numLapsKey = "NumberOfLaps";

        private GBXByte _version;
        private GBXBool _alwaysFalse;
        private GBXUInt _bronzeTime;
        private GBXUInt _silverTime;
        private GBXUInt _goldTime;
        private GBXUInt _authorTime;
        private GBXUInt _displayCost;
        private GBXBool _multilap;
        private GBXBool _unknown;
        private GBXUInt _trackType;
        private GBXUInt _alwaysZero;
        private GBXUInt _authorScore;
        private GBXUInt _editMode;
        private GBXBool _alwaysFalseToo;
        private GBXUInt _numCPs;
        private GBXUInt _numLaps;

        
        public GBXByte Version { get => _version; private set { _version = value; AddChildNew(value); } }
        public GBXBool AlwaysFalse { get => _alwaysFalse; private set { _alwaysFalse = value; AddChildNew(value); } }
        public GBXUInt BronzeTime { get => _bronzeTime; private set { _bronzeTime = value; AddChildNew(value); } }
        public GBXUInt SilverTime { get => _silverTime; private set { _silverTime = value; AddChildNew(value); } }
        public GBXUInt GoldTime { get => _goldTime; private set { _goldTime = value; AddChildNew(value); } }
        public GBXUInt AuthorTime { get => _authorTime; private set { _authorTime = value; AddChildNew(value); } }
        public GBXUInt DisplayCost { get => _displayCost; private set { _displayCost = value; AddChildNew(value); } }
        public GBXBool Multilap { get => _multilap; private set { _multilap = value; AddChildNew(value); } }
        public GBXBool Unknown { get => _unknown; private set { _unknown = value; AddChildNew(value); } }
        public GBXUInt TrackType { get => _trackType; private set { _trackType = value; AddChildNew(value); } }
        public GBXUInt AlwaysZero { get => _alwaysZero; private set { _alwaysZero = value; AddChildNew(value); } }
        public GBXUInt AuthorScore { get => _authorScore; private set { _authorScore = value; AddChildNew(value); } }
        public GBXUInt EditMode { get => _editMode; private set { _editMode = value; AddChildNew(value); } }
        public GBXBool AlwaysFalseToo { get => _alwaysFalseToo; private set { _alwaysFalseToo = value; AddChildNew(value); } }
        public GBXUInt NumCPs { get => _numCPs; private set { _numCPs = value; AddChildNew(value); } }

        public GBXUInt NumLaps { get => _numLaps; private set { _numLaps = value; AddChildNew(value); } }

        public ChallengeDesc(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            Version = new GBXByte(s);

            if (Version.Value < 3) {
                throw new Exception("File is too old. ChallengeTMDesc version is not supported.");
            }

            AlwaysFalse = new GBXBool(s);
            BronzeTime = new GBXUInt(s);
            SilverTime = new GBXUInt(s);
            GoldTime = new GBXUInt(s);
            AuthorTime = new GBXUInt(s);

            if(Version.Value < 4)
                return;
            //version >= 4
            DisplayCost = new GBXUInt(s);

            if (Version.Value < 5)
                return;
            //version >= 5
            Multilap = new GBXBool(s);

            if (Version.Value == 6)
            {
                //version == 6;
                Unknown = new GBXBool(s);
            }

            //version >= 7
            TrackType = new GBXUInt(s);

            if(Version.Value < 9)
                return;
            //version >= 9
            AlwaysZero = new GBXUInt(s);

            if(Version.Value < 10)
                return;
            //version >= 10
            AuthorScore = new GBXUInt(s);

            if (Version.Value < 11)
                return;
            //version >= 11
            EditMode = new GBXUInt(s);

            if(Version.Value< 12)
            {
                return;
            }
            //version >= 12
            AlwaysFalseToo = new GBXBool(s);

            if(Version.Value< 13)
                return;
            //version >= 13
            NumCPs = new GBXUInt(s);
            NumLaps = new GBXUInt(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(versionKey, Version);
            result.AddChild(alwaysFalseKey, AlwaysFalse);
            result.AddChild(bronzeTimeKey, BronzeTime);
            result.AddChild(silverTimeKey, SilverTime);
            result.AddChild(goldTimeKey, GoldTime);
            result.AddChild(authorTimeKey, AuthorTime);
            result.AddChild(displayCostKey, DisplayCost);
            result.AddChild(multilapKey, Multilap);
            result.AddChild(unknownKey, Unknown);
            result.AddChild(trackTypeKey, TrackType);
            result.AddChild(alwaysZeroKey, AlwaysZero);
            result.AddChild(authorScoreKey, AuthorScore);
            result.AddChild(editModeKey, EditMode);
            result.AddChild(alwaysFalseTooKey, AlwaysFalseToo);
            result.AddChild(numCPsKey, NumCPs);
            result.AddChild(numLapsKey, NumLaps);
            return result;
        }
    }
}
