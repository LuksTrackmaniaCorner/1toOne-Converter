using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class CollectorList0301B000 : Chunk
    {
        public static string archiveCountKey = "Archive Count";

        public readonly GBXUInt archiveCount;

        public CollectorList0301B000(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            archiveCount = new GBXUInt(s);
            Trace.Assert(archiveCount.Value == 0, "Unsupported Archive count, probably because you try to convert a puzzle map."); //TODO support more
            AddChildDeprecated(archiveCountKey, archiveCount);
        }
    }
}
