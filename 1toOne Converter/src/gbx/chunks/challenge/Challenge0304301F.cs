using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge0304301F : Chunk
    {
        public static readonly string metaKey = "Meta";
        public static readonly string trackNameKey = "Track Name";
        public static readonly string metaDecorationKey = "Meta Decoration";
        public static readonly string mapSizeKey = "Map Size";
        public static readonly string needUnlockKey = "Need Unlock";
        public static readonly string versionKey = "Version";
        public static readonly string numBlocksKey = "Number of Blocks";
        public static readonly string blocksKey = "Blocks";

        private Meta trackMeta;
        private GBXString trackName;
        private Meta decorationMeta;
        private GBXNat3 mapSize;
        private GBXBool needUnlock;
        private GBXUInt version;
        private GBXUInt numBlocks;
        private Array<Block> blocks;

        public Meta TrackMeta { get => trackMeta; set { trackMeta = value; AddChildNew(value); } }
        public GBXString TrackName { get => trackName; set { trackName = value; AddChildNew(value); } }
        public Meta DecorationMeta { get => decorationMeta; set { decorationMeta = value; AddChildNew(value); } }
        public GBXNat3 MapSize { get => mapSize; set { mapSize = value; AddChildNew(value); } }
        public GBXBool NeedUnlock { get => needUnlock; set { needUnlock = value; AddChildNew(value); } }
        public GBXUInt Version { get => version; set { version = value; AddChildNew(value); } }
        public GBXUInt NumBlocks { get => numBlocks; set { numBlocks = value; AddChildNew(value); } }
        public Array<Block> Blocks { get => blocks; set { blocks = value; AddChildNew(value); } }

        public Challenge0304301F(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            TrackMeta = new Meta(s, context);

            TrackName = new GBXString(s);

            DecorationMeta = new Meta(s, context);

            MapSize = new GBXNat3(s);

            NeedUnlock = new GBXBool(s);

            Version = new GBXUInt(s);
            Trace.Assert(Version.Value!= 0, "Unsupported version in the block chunk");

            NumBlocks = new GBXUInt(s);

            Blocks = new Array<Block>(() => new Block(s, context, list), () => (s.PeekUInt() & 0xC0_00_00_00) != 0);
            Blocks.LinkSize(NumBlocks);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(metaKey, TrackMeta);
            result.AddChild(trackNameKey, TrackName);
            result.AddChild(metaDecorationKey, DecorationMeta);
            result.AddChild(mapSizeKey, mapSize);
            result.AddChild(needUnlockKey, NeedUnlock);
            result.AddChild(versionKey, Version);
            result.AddChild(numBlocksKey, NumBlocks);
            result.AddChild(blocksKey, Blocks);
            return result;
        }
    }
    
    public class Block : Structure
    {
        public static readonly string blockNameKey = "Block Name";
        public static readonly string rotKey = "Rot";
        public static readonly string coordsKey = "Coords";
        public static readonly string flagsKey = "Flags";
        //optional
        public static readonly string authorKey = "Author";
        public static readonly string skinKey = "Skin";
        public static readonly string blockParametersKey = "Block Parameters";

        private GBXLBS blockName;
        private GBXByte rot;
        private GBXByte3 coords;
        private GBXUInt flags;
        //optional
        private GBXLBS author;
        private GBXNodeRef skin;
        private GBXNodeRef blockParameters;

        public GBXLBS BlockName { get => blockName; set { blockName = value; AddChildNew(value); } }
        public GBXByte Rot { get => rot; set { rot = value; AddChildNew(value); } }
        public GBXByte3 Coords { get => coords; set { coords = value; AddChildNew(value); } }
        public GBXUInt Flags { get => flags; set { flags = value; AddChildNew(value); } }
        //Optional
        public GBXLBS Author { get => author; set { author = value; AddChildNew(value); } }
        public GBXNodeRef Skin { get => skin; set { skin = value; AddChildNew(value); } }
        public GBXNodeRef BlockParameters { get => blockParameters; set { blockParameters = value; AddChildNew(value); } }

        private Block()
        {

        }

        public Block(Stream s, GBXLBSContext context, GBXNodeRefList list)
        {
            BlockName = context.ReadLookBackString(s);

            Rot = new GBXByte(s);

            if(Rot.Value>= 4)
            {
                throw new Exception("OOF");
            }

            Coords = new GBXByte3(s);

            Flags = new GBXUInt(s);
            Flags.SetBase(16);

            if(Flags.Value== 0xFFFFFFFF)
            {
                return;
            }

            if((Flags.Value & 0x8000) != 0) //Skinnable block
            {
                Author = context.ReadLookBackString(s);

                Skin = list.ReadGBXNodeRef(s, context);
            }

            if((Flags.Value & 0x100000) != 0) //CP or Finish (newer Versions only)
            {
                BlockParameters = list.ReadGBXNodeRef(s, context);
            }
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(blockNameKey, BlockName);
            result.AddChild(rotKey, Rot);
            result.AddChild(coordsKey, Coords);
            result.AddChild(flagsKey, Flags);
            result.AddChild(authorKey, Author);
            result.AddChild(skinKey, Skin);
            result.AddChild(blockParametersKey, BlockParameters);
            return result;
        }
    }
}
