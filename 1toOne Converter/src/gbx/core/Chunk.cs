using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.gbx.core
{
    public class Chunk : Structure
    {
        internal static readonly Dictionary<uint, ChunkInfo> chunkInfos;

        public static readonly string anchoredObject03101002Key = "Anchored Obj 030101002";

        public static readonly string challengeDescKey = "Challenge Desc";
        public static readonly string challengeCommonKey = "Challenge Common";
        public static readonly string challengeVersionKey = "Challenge Version";
        public static readonly string challengeCommunityKey = "Challenge Community";
        public static readonly string challengeThumbnailKey = "Challenge Thumbnail";
        public static readonly string challengeAuthorKey = "Challenge Author";
        public static readonly string challenge0304300DKey = "Challenge 0304300D";
        public static readonly string challenge03043011Key = "Challenge 03043011";
        public static readonly string challenge03043013Key = "Challenge 03043013";
        public static readonly string challenge03043014Key = "Challenge 03043014";
        public static readonly string challenge03043017Key = "Challenge 03043017";
        public static readonly string challenge03043018Key = "Challenge 03043018";
        public static readonly string challenge03043019Key = "Challenge 03043019";
        public static readonly string challenge0304301CKey = "Challenge 0304301C";
        public static readonly string challenge0304301FKey = "Challenge 0304301F";
        public static readonly string challenge03043021Key = "Challenge 03043021";
        public static readonly string challenge03043022Key = "Challenge 03043022";
        public static readonly string challenge03043024Key = "Challenge 03043024";
        public static readonly string challenge03043025Key = "Challenge 03043025";
        public static readonly string challenge03043026Key = "Challenge 03043026";
        public static readonly string challenge03043028Key = "Challenge 03043028";
        public static readonly string challenge0304302AKey = "Challenge 0304302A";
        public static readonly string challenge03043040Key = "Challenge 03043040";
        public static readonly string challenge03043049Key = "Challenge 03043049";

        public static readonly string collectorList0301B000Key = "Collector List 0301B000";

        public static readonly string blockSkin03059002Key = "Block Skin 03059002";

        public static readonly string challengeParameters0305B001Key = "Challenge Params 0305B001";
        public static readonly string challengeParameters0305B004Key = "Challenge Params 0305B004";
        public static readonly string challengeParameters0305B005Key = "Challenge Params 0305B005";
        public static readonly string challengeParameters0305B006Key = "Challenge Params 0305B006";
        public static readonly string challengeParameters0305B008Key = "Challenge Params 0305B008";
        public static readonly string challengeParameters0305B00AKey = "Challenge Params 0305B00A";
        public static readonly string challengeParameters0305B00DKey = "Challenge Params 0305B00D";
        public static readonly string challengeParameters0305B00EKey = "Challenge Params 0305B00E";

        public static readonly string blockParameters2E009000Key = "Block Params 2E009000";

        public static readonly string skippedChunkKey = "Skipped";

        static Chunk()
        {
            chunkInfos = new Dictionary<uint, ChunkInfo>();

            //AnchoredObject
            var anchoredObject03101002Info = new ChunkInfo(anchoredObject03101002Key, false, (s, c, l) => new AnchoredObject03101002(s, c, l));
            chunkInfos.Add(0x03101002, anchoredObject03101002Info);


            //Challenge
            var challengeDescInfo = new ChunkInfo(challengeDescKey, false , (s, c, l) => new ChallengeDesc(s, c, l));
            chunkInfos.Add(0x03043002, challengeDescInfo);
            chunkInfos.Add(0x24003002, challengeDescInfo);

            var challengeCommonInfo = new ChunkInfo(challengeCommonKey, false, (s, c, l) => new ChallengeCommon(s, c, l));
            chunkInfos.Add(0x03043003, challengeCommonInfo);
            chunkInfos.Add(0x24003003, challengeCommonInfo);

            var challengeVersionInfo = new ChunkInfo(challengeVersionKey, false, (s, c, l) => new ChallengeVersion(s, c, l));
            chunkInfos.Add(0x03043004, challengeVersionInfo);
            chunkInfos.Add(0x24003004, challengeVersionInfo);

            var challengeCommunityInfo = new ChunkInfo(challengeCommunityKey, true, (s, c, l) => new ChallengeCommunity(s, c, l));
            chunkInfos.Add(0x03043005, challengeCommunityInfo);
            chunkInfos.Add(0x24003005, challengeCommunityInfo);

            var challengeThumbnailInfo = new ChunkInfo(challengeThumbnailKey, true, (s, c, l) => new ChallengeThumbnail(s, c, l));
            chunkInfos.Add(0x03043007, challengeThumbnailInfo);
            chunkInfos.Add(0x24003007, challengeThumbnailInfo);

            var challengeAuthorInfo = new ChunkInfo(challengeAuthorKey, false, (s, c, l) => new ChallengeAuthor(s, c, l));
            chunkInfos.Add(0x03043008, challengeAuthorInfo);
            chunkInfos.Add(0x24003008, challengeAuthorInfo);

            var challenge0304300DInfo = new ChunkInfo(challenge0304300DKey, false, (s, c, l) => new Challenge0304300D(s, c, l));
            chunkInfos.Add(0x0304300D, challenge0304300DInfo);
            chunkInfos.Add(0x2400300D, challenge0304300DInfo);

            var challenge03043011Info = new ChunkInfo(challenge03043011Key, false, (s, c, l) => new Challenge03043011(s, c, l));
            chunkInfos.Add(0x03043011, challenge03043011Info);
            chunkInfos.Add(0x24003011, challenge03043011Info);

            //03043013 is redirected and updated to 0304301F
            var challenge03043013Info = new ChunkInfo(challenge0304301FKey, false, (s, c, l) => new Challenge0304301F(s, c, l, true));
            chunkInfos.Add(0x03043013, challenge03043013Info);
            chunkInfos.Add(0x24003013, challenge03043013Info);

            var challenge03043014Info = new ChunkInfo(challenge03043014Key, true, (s, c, l) => new Challenge03043014(s, c, l));
            chunkInfos.Add(0x03043014, challenge03043014Info);
            chunkInfos.Add(0x24003014, challenge03043014Info);

            var challenge03043017Info = new ChunkInfo(challenge03043017Key, true, (s, c, l) => new Challenge03043017(s, c, l));
            chunkInfos.Add(0x03043017, challenge03043017Info);
            chunkInfos.Add(0x24003017, challenge03043017Info);

            var challenge03043018Info = new ChunkInfo(challenge03043018Key, true, (s, c, l) => new Challenge03043018(s, c, l));
            chunkInfos.Add(0x03043018, challenge03043018Info);
            chunkInfos.Add(0x24003018, challenge03043018Info);

            var challenge03043019Info = new ChunkInfo(challenge03043019Key, true, (s, c, l) => new Challenge03043019(s, c, l));
            chunkInfos.Add(0x03043019, challenge03043019Info);
            chunkInfos.Add(0x24003019, challenge03043019Info);

            var challenge0304301CInfo = new ChunkInfo(challenge0304301CKey, true, (s, c, l) => new Challenge0304301C(s, c, l));
            chunkInfos.Add(0x0304301C, challenge0304301CInfo);
            chunkInfos.Add(0x2400301C, challenge0304301CInfo);

            var challenge0304301FInfo = new ChunkInfo(challenge0304301FKey, false, (s, c, l) => new Challenge0304301F(s, c, l, false));
            chunkInfos.Add(0x0304301F, challenge0304301FInfo);
            chunkInfos.Add(0x2400301F, challenge0304301FInfo);

            var challenge03043021Info = new ChunkInfo(challenge03043021Key, false, (s, c, l) => new Challenge03043021(s, c, l));
            chunkInfos.Add(0x03043021, challenge03043021Info);
            chunkInfos.Add(0x24003021, challenge03043021Info);

            var challenge03043022Info = new ChunkInfo(challenge03043022Key, false, (s, c, l) => new Challenge03043022(s, c, l));
            chunkInfos.Add(0x03043022, challenge03043022Info);
            chunkInfos.Add(0x24003022, challenge03043022Info);

            var challenge03043024Info = new ChunkInfo(challenge03043024Key, false, (s, c, l) => new Challenge03043024(s, c, l));
            chunkInfos.Add(0x03043024, challenge03043024Info);
            chunkInfos.Add(0x24003024, challenge03043024Info);

            var challenge03043025Info = new ChunkInfo(challenge03043025Key, false, (s, c, l) => new Challenge03043025(s, c, l));
            chunkInfos.Add(0x03043025, challenge03043025Info);
            chunkInfos.Add(0x24003025, challenge03043025Info);

            var challenge03043026Info = new ChunkInfo(challenge03043026Key, false, (s, c, l) => new Challenge03043026(s, c, l));
            chunkInfos.Add(0x03043026, challenge03043026Info);
            chunkInfos.Add(0x24003026, challenge03043026Info);

            var challenge03043028Info = new ChunkInfo(challenge03043028Key, false, (s, c, l) => new Challenge03043028(s, c, l));
            chunkInfos.Add(0x03043028, challenge03043028Info);
            chunkInfos.Add(0x24003028, challenge03043028Info);

            var challenge0304302AInfo = new ChunkInfo(challenge0304302AKey, false, (s, c, l) => new Challenge0304302A(s, c, l));
            chunkInfos.Add(0x0304302A, challenge0304302AInfo);
            chunkInfos.Add(0x2400302A, challenge0304302AInfo);

            var challenge03043040Info = new ChunkInfo(challenge03043040Key, true, (s, c, l) => new Challenge03043040(s, c, l));
            chunkInfos.Add(0x03043040, challenge03043040Info);
            chunkInfos.Add(0x24003040, challenge03043040Info);

            var challenge03043049Info = new ChunkInfo(challenge03043049Key, false, (s, c, l) => new Challenge03043049(s, c, l));
            chunkInfos.Add(0x03043049, challenge03043049Info);
            chunkInfos.Add(0x24003049, challenge03043049Info);


            //CollectorList
            var collectorList0301B000Info = new ChunkInfo(collectorList0301B000Key, false, (s, c, l) => new CollectorList0301B000(s, c, l));
            chunkInfos.Add(0x0301B000, collectorList0301B000Info);
            chunkInfos.Add(0x2403C000, collectorList0301B000Info);


            //BlockSkin
            var blockSkin03059002Info = new ChunkInfo(blockSkin03059002Key, false, (s, c, l) => new BlockSkin03059002(s, c, l));
            chunkInfos.Add(0x03059002, blockSkin03059002Info);
            chunkInfos.Add(0x2403A002, blockSkin03059002Info);


            //ChallengeParameters
            var challengeParameters0305B001Info = new ChunkInfo(challengeParameters0305B001Key, false, (s, c, l) => new ChallengeParameters0305B001(s, c, l));
            chunkInfos.Add(0x0305B001, challengeParameters0305B001Info);
            chunkInfos.Add(0x2400C001, challengeParameters0305B001Info);

            var challengeParameters0305B004Info = new ChunkInfo(challengeParameters0305B004Key, false, (s, c, l) => new ChallengeParameters0305B004(s, c, l));
            chunkInfos.Add(0x0305B004, challengeParameters0305B004Info);
            chunkInfos.Add(0x2400C004, challengeParameters0305B004Info);

            var challengeParameters0305B005Info = new ChunkInfo(challengeParameters0305B005Key, false, (s, c, l) => new ChallengeParameters0305B005(s, c, l));
            chunkInfos.Add(0x0305B005, challengeParameters0305B005Info);
            chunkInfos.Add(0x2400C005, challengeParameters0305B005Info);

            var challengeParameters0305B006Info = new ChunkInfo(challengeParameters0305B006Key, false, (s, c, l) => new ChallengeParameters0305B006(s, c, l));
            chunkInfos.Add(0x0305B006, challengeParameters0305B006Info);
            chunkInfos.Add(0x2400C006, challengeParameters0305B006Info);

            var challengeParameters0305B008Info = new ChunkInfo(challengeParameters0305B008Key, false, (s, c, l) => new ChallengeParameters0305B008(s, c, l));
            chunkInfos.Add(0x0305B008, challengeParameters0305B008Info);
            chunkInfos.Add(0x2400C008, challengeParameters0305B008Info);

            var challengeParameters0305B00AInfo = new ChunkInfo(challengeParameters0305B00AKey, true, (s, c, l) => new ChallengeParameters0305B00A(s, c, l));
            chunkInfos.Add(0x0305B00A, challengeParameters0305B00AInfo);
            chunkInfos.Add(0x2400C00A, challengeParameters0305B00AInfo);

            var challengeParameters0305B00DInfo = new ChunkInfo(challengeParameters0305B00DKey, false, (s, c, l) => new ChallengeParameters0305B00D(s, c, l));
            chunkInfos.Add(0x0305B00D, challengeParameters0305B00DInfo);
            chunkInfos.Add(0x2400C00D, challengeParameters0305B00DInfo);

            var challengeParameters0305B00EInfo = new ChunkInfo(challengeParameters0305B00EKey, true, (s, c, l) => new ChallengeParameters0305B00E(s, c, l));
            chunkInfos.Add(0x0305B00E, challengeParameters0305B00EInfo);
            chunkInfos.Add(0x2400C00E, challengeParameters0305B00EInfo);

            //Block Parameters
            var blockParameters2E009000Info = new ChunkInfo(blockParameters2E009000Key, false, (s, c, l) => new BlockParameters2E009000(s, c, l));
            chunkInfos.Add(0x2E009000, blockParameters2E009000Info);
        }

        public const uint skip = 0x534B4950;
        public const uint facade = 0xFACADE01;

        private bool? isSkippable;

        public bool IsSkippable
        {
            get
            {
                try
                {
                    return isSkippable ?? IsChunkSkippable(ChunkID);
                }
                catch(KeyNotFoundException)
                {
                    isSkippable = true;
                    return true;
                }
            }
        }

        [XmlIgnore]
        public bool IsHeaderChunk { get; private set; }

        public uint ChunkID { get; set; }

        private GBXLBSContext _context;

        public override GBXLBSContext LBSContext
        {
            get
            {
                if (IsHeaderChunk)
                    return _context;
                if (!IsSkippable)
                    return Parent.LBSContext;
                if (_context == null)
                    _context = new GBXLBSContext();
                return _context;
            }
        }

        public override GBXNodeRefList NodeRefList
        {
            get
            {
                if (!IsSkippable)
                    return Parent.NodeRefList;
                throw new ParsingException("Noderefs can't be in skippable chunks. Consider using nodes instead.");
            }
        }
        protected Chunk(GBXLBSContext context, GBXNodeRefList list)
        {
            _context = context;
            IsHeaderChunk = false;
        }

        public sealed override void WriteBack(Stream s)
        {
            if (IsHeaderChunk)
            {
                WriteBackChunk(s);
            }
            else
            {
                s.WriteUInt(ChunkID);

                if(IsSkippable)
                {
                    s.WriteUInt(skip);

                    //Skip Length
                    s.Position += 4;

                    //Write Chunk
                    long startPos = s.Position;
                    WriteBackChunk(s);
                    long endPos = s.Position;

                    //Write Length
                    s.Position = startPos - 4;
                    s.WriteUInt((uint)(endPos - startPos));
                    s.Position = endPos;
                }
                else
                {
                    base.WriteBack(s);
                }
            }
        }

        protected virtual void WriteBackChunk(Stream s)
        {
            base.WriteBack(s);
        }

        public static Chunk ReadHeaderChunk(Stream s, uint chunkID, out string key)
        {
            Chunk newChunk = ReadChunk(s, chunkID, new GBXLBSContext(), null, out key);

            newChunk.IsHeaderChunk = true;
            return newChunk;
        }

        public static Chunk ReadBodyChunk(Stream s, GBXLBSContext context, GBXNodeRefList list, out string key)
        {
            uint chunkID = s.ReadUInt();

            if (chunkID == facade)
            {
                key = null;
                return null;
            }

            Chunk newChunk;

            try
            {
                if(IsChunkSkippable(chunkID))
                {
                    Trace.Assert(s.ReadUInt() == skip, "Error reading chunk. ChunkID:" + chunkID.ToString("X"));

                    long chunkEndPos = s.ReadUInt() + s.Position;

                    newChunk = ReadChunk(s, chunkID, new GBXLBSContext(), list, out key);

                    Trace.Assert(s.Position == chunkEndPos, "Error reading chunk. ChunkID:" + chunkID.ToString("X"));
                }
                else
                {
                    newChunk = ReadChunk(s, chunkID, context, list, out key);
                }
            }
            catch(KeyNotFoundException)
            {
                //Chunk not in database, testing if it can be skipped
                uint testSkip = BitConverter.ToUInt32(s.SimplePeek(4), 0);

                if(testSkip == skip) //Chunk can be skipped, chunk info not necessary
                {
                    s.Position += 4;
                    uint length = s.ReadUInt();

                    newChunk = new SkippedChunk(s, (int)length, new GBXLBSContext(), new GBXNodeRefList())
                    {
                        ChunkID = chunkID,
                        isSkippable = true
                    };
                    key = skippedChunkKey + " " + chunkID.ToString("X");
                }
                else
                {
                    //This exeption will cause the read to end.
                    throw new UnknownChunkException("Chunk could not be read ChunkID: " + chunkID.ToString("X"));
                }
                
            }

            return newChunk;
        }

        private static Chunk ReadChunk(Stream s, uint chunkID, GBXLBSContext context, GBXNodeRefList list, out string key)
        {
            var info = chunkInfos[chunkID];
            key = info.key;
            var newChunk = info.constructor(s, context, list);
            if(newChunk.ChunkID == 0)
            {
                newChunk.ChunkID = chunkID; //ChunkID has not been set manually.
            }
            newChunk.isSkippable = info.skippable;
            return newChunk;
        }

        public static bool IsChunkSkippable(uint chunkID)
        {
            return chunkInfos[chunkID].skippable;
        }
    }

    internal class ChunkInfo
    {
        internal string key;
        internal bool skippable;
        internal ChunkConstructor constructor;

        internal ChunkInfo(string key, bool skippable, ChunkConstructor constructor)
        {
            this.key = key;
            this.skippable = skippable;
            this.constructor = constructor;
        }
    }

    internal delegate Chunk ChunkConstructor(Stream s, GBXLBSContext context, GBXNodeRefList list);
}
