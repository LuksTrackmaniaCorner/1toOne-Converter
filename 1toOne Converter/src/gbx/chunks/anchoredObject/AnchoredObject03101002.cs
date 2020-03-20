using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.chunks
{
    public class AnchoredObject03101002 : Chunk
    {
        public readonly string always7Key = "Always 7";
        public readonly string metaKey = "Meta";
        public readonly string rotationKey = "Rotation";
        public readonly string blockUnitCoordsKey = "Block Unit Coords";
        public readonly string unknown1Key = "Unknown 1";
        public readonly string absCoordsKey = "AbsCoords";
        public readonly string unknown2Key = "Unknown 2";
        public readonly string unknown3Key = "Unknown 3";
        public readonly string unknownVecKey = "Unknown Vec";
        public readonly string scaleKey = "Scale";

        private GBXUInt always7;
        private Meta meta;
        private GBXVec3 rotation;
        private GBXByte3 blockUnitCoords;
        private GBXUInt unknown1;
        private GBXVec3 absCoords;
        private GBXUInt unknown2;
        private GBXUShort unknown3;
        private GBXVec3 unknownVec;
        private GBXFloat scale;

        public GBXUInt Always7 { get => always7; set { always7 = value; AddChildNew(value); } }
        public Meta Meta { get => meta; set { meta = value; AddChildNew(value); } }
        public GBXVec3 Rotation { get => rotation; set { rotation = value; AddChildNew(value); } }
        public GBXByte3 BlockUnitCoords { get => blockUnitCoords; set { blockUnitCoords = value; AddChildNew(value); } }
        public GBXUInt Unknown1 { get => unknown1; set { unknown1 = value; AddChildNew(value); } }
        public GBXVec3 AbsCoords { get => absCoords; set { absCoords = value; AddChildNew(value); } }
        public GBXUInt Unknown2 { get => unknown2; set { unknown2 = value; AddChildNew(value); } }
        public GBXUShort Unknown3 { get => unknown3; set { unknown3 = value; AddChildNew(value); } }
        public GBXVec3 UnknownVec { get => unknownVec; set { unknownVec = value; AddChildNew(value); } }
        public GBXFloat Scale { get => scale; set { scale = value; AddChildNew(value); } }

        public AnchoredObject03101002() : base(null, null)
        {

        }


        public AnchoredObject03101002(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            Always7 = new GBXUInt(s);
            Meta = new Meta(s, context);
            Rotation = new GBXVec3(s);
            BlockUnitCoords = new GBXByte3(s);
            Unknown1 = new GBXUInt(s);
            AbsCoords = new GBXVec3(s);
            Unknown2 = new GBXUInt(s);
            Unknown3 = new GBXUShort(s);
            UnknownVec = new GBXVec3(s);
            Scale = new GBXFloat(s);
        }

        public AnchoredObject03101002(
            GBXUInt always7,
            Meta meta,
            GBXVec3 rotation,
            GBXByte3 blockUnitCoords,
            GBXUInt unknown1,
            GBXVec3 absCoords,
            GBXUInt unknown2,
            GBXUShort unknown3,
            GBXVec3 unknownVec,
            GBXFloat scale) : base(null, null)
        {
            this.ChunkID = 0x03101002;

            Always7 = always7;
            Meta = meta;
            Rotation = rotation;
            BlockUnitCoords = blockUnitCoords;
            Unknown1 = unknown1;
            AbsCoords = absCoords;
            Unknown2 = unknown2;
            Unknown3 = unknown3;
            UnknownVec = unknownVec;
            Scale = scale;
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(always7Key, Always7);
            result.AddChild(metaKey, Meta);
            result.AddChild(rotationKey, Rotation);
            result.AddChild(blockUnitCoordsKey, BlockUnitCoords);
            result.AddChild(unknown1Key, Unknown1);
            result.AddChild(absCoordsKey, AbsCoords);
            result.AddChild(unknown2Key, Unknown2);
            result.AddChild(unknown3Key, Unknown3);
            result.AddChild(unknownVecKey, UnknownVec);
            result.AddChild(scaleKey, Scale);
            return result;
        }
    }
}
