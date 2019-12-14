using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043040 : Chunk
    {
        public static readonly string versionKey = "Version?";
        public static readonly string unknownKey = "Unknown";
        public static readonly string sizeKey = "Size";
        public static readonly string aKey = "A";
        public static readonly string numItemsKey = "Number Of Items";
        public static readonly string itemsKey = "Items";
        public static readonly string alwaysZeroKey = "Zero";

        private GBXUInt version;
        private GBXUInt unknown;
        private GBXUInt size;
        private GBXUInt a;
        private GBXUInt numItems;
        private Array<Node> items;
        private GBXUInt alwaysZero;

        public GBXUInt Version { get => version; set { version = value; AddChildNew(value); } }
        public GBXUInt Unknown { get => unknown; set { unknown = value; AddChildNew(value); } }
        public GBXUInt Size { get => size; set { size = value; AddChildNew(value); } }
        public GBXUInt A { get => a; set { a = value; AddChildNew(value); } }
        public GBXUInt NumItems { get => numItems; set { numItems = value; AddChildNew(value); } }
        public Array<Node> Items { get => items; set { items = value; AddChildNew(value); value.LinkSize(NumItems); } }
        public GBXUInt AlwaysZero { get => alwaysZero; set { alwaysZero = value; AddChildNew(value); } }

        public Challenge03043040(bool dummy) : base(null, null)
        {
            this.ChunkID = 0x03043040;

            dummy = !dummy;
            Version = new GBXUInt(4);
            Unknown = new GBXUInt(0);
            Size = new GBXUInt(8);
            A = new GBXUInt(0xA);
            NumItems = new GBXUInt(0);
            Items = new Array<Node>(0, null);
            AlwaysZero = new GBXUInt(0);
        }

        public Challenge03043040(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            Version = new GBXUInt(s);
            Unknown = new GBXUInt(s);
            Size = new GBXUInt(s);
            A = new GBXUInt(s);
            NumItems = new GBXUInt(s);
            Items = new Array<Node>(NumItems.Value, () => new Node(s, context, null));
            AlwaysZero = new GBXUInt(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(versionKey, Version);
            result.AddChild(unknownKey, Unknown);
            result.AddChild(sizeKey, Size);
            result.AddChild(aKey, A);
            result.AddChild(numItemsKey, NumItems);
            result.AddChild(itemsKey, Items);
            result.AddChild(alwaysZeroKey, AlwaysZero);
            return result;
        }

        protected override void WriteBackChunk(Stream s)
        {
            Version.WriteBack(s);
            Unknown.WriteBack(s);

            var sizePos = s.Position;
            s.Position += 4; //skip Size for now
            var sizeStartPos = s.Position;

            A.WriteBack(s);
            NumItems.WriteBack(s);
            Items.WriteBack(s);
            alwaysZero.WriteBack(s);

            var sizeEndPos = s.Position;
            s.Position = sizePos;
            size.Value = (uint) (sizeEndPos - sizeStartPos);
            Size.WriteBack(s); //Write size back after everything else
            s.Position = sizeEndPos;
        }
    }
}
