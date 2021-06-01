using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.BlockParameters
{
    public class BlockParameters2E009000 : Chunk
    {
        public static readonly string referenceIDKey = "Reference ID";
        public static readonly string tagKey = "Tag";
        public static readonly string orderKey = "Order";

        private GBXUInt referenceID;
        private GBXString tag;
        private GBXUInt order;

        public GBXUInt ReferenceID { get => referenceID; set { referenceID = value; AddChildNew(value); } }
        public GBXString Tag { get => tag; set { tag = value; AddChildNew(value); } }
        public GBXUInt Order { get => order; set { order = value; AddChildNew(value); } }

        public BlockParameters2E009000(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            ReferenceID = new GBXUInt(s);
            Tag = new GBXString(s);
            Order = new GBXUInt(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(referenceIDKey, ReferenceID);
            result.AddChild(tagKey, Tag);
            result.AddChild(orderKey, Order);
            return result;
        }
    }
}
