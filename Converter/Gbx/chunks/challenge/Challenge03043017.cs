using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class Challenge03043017 : Chunk
    {
        public static readonly string checkPointCountKey = "Checkpoint count";
        public static readonly string checkPointArrayKey = "Checkpoints";

        public readonly GBXUInt checkPointCount;
        public readonly Array<GBXNat3> checkPointArray;

        public Challenge03043017(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            checkPointCount = new GBXUInt(s);
            AddChildDeprecated(checkPointCountKey, checkPointCount);

            checkPointArray = new Array<GBXNat3>(checkPointCount.Value, () => new GBXNat3(s));
            AddChildDeprecated(checkPointArrayKey, checkPointArray);
        }
    }
}
