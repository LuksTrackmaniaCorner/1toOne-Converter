using Gbx.Parser.Core;
using Gbx.Parser.info;
using Gbx.Parser.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Challenge
{
    public class ChallengeDescription : GbxChunk
    {
        public GbxByte Version { get; }
        public GbxBool AlwaysFalse { get; }
        public GbxUInt BronzeTime { get; }
        public GbxUInt SilverTime { get; }
        public GbxUInt GoldTime { get; }
        public GbxUInt AuthorTime { get; }
        public GbxUInt DisplayCost { get; }
        public GbxBool Multilap { get; }
        public GbxBool Unknown { get; }
        public GbxUInt TrackType { get; }
        public GbxUInt AlwaysZero { get; }
        public GbxUInt AuthorScore { get; }
        public GbxUInt EditMode { get; }
        public GbxBool AlwaysFalseToo { get; }
        public GbxUInt NumCPs { get; }
        public GbxUInt NumLaps { get; }

        public ChallengeDescription(GbxChunkInfo chunkInfo) : base(chunkInfo)
        {
            Version = new GbxByte(3, (x) => x >= 3);
            Version.OnChange += (x) => NotifyChange();
            AlwaysFalse = new GbxBool();
            AlwaysFalse.MakeConst();
            BronzeTime = new GbxUInt();
            SilverTime = new GbxUInt();
            GoldTime = new GbxUInt();
            AuthorTime = new GbxUInt();
            DisplayCost = new GbxUInt();
            Multilap = new GbxBool();
            Unknown = new GbxBool();
            TrackType = new GbxUInt();
            AlwaysZero = new GbxUInt(0);
            AlwaysZero.MakeConst();
            AuthorScore = new GbxUInt();
            EditMode = new GbxUInt();
            AlwaysFalseToo = new GbxBool();
            AlwaysFalseToo.MakeConst();
            NumCPs = new GbxUInt();
            NumLaps = new GbxUInt();
        }

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Version), Version);
            yield return (nameof(AlwaysFalse), AlwaysFalse);
            yield return (nameof(BronzeTime), BronzeTime);
            yield return (nameof(SilverTime), SilverTime);
            yield return (nameof(GoldTime), GoldTime);
            yield return (nameof(AuthorTime), AuthorTime);

            if (Version == 3)
                yield break;

            yield return (nameof(DisplayCost), DisplayCost);

            if (Version == 4)
                yield break;

            yield return (nameof(Multilap), Multilap);

            if(Version == 6)
            {
                yield return (nameof(Unknown), Unknown);
            }

            yield return (nameof(TrackType), TrackType);

            if (Version <= 8)
                yield break;

            yield return (nameof(AlwaysZero), AlwaysZero);

            if (Version == 9)
                yield break;

            yield return (nameof(AuthorScore), AuthorScore);

            if (Version == 10)
                yield break;

            yield return (nameof(EditMode), EditMode);

            if (Version == 11)
                yield break;

            yield return (nameof(AlwaysFalseToo), AlwaysFalseToo);

            if (Version == 12)
                yield break;

            yield return (nameof(NumCPs), NumCPs);
            yield return (nameof(NumLaps), NumLaps);
        }
    }
}
