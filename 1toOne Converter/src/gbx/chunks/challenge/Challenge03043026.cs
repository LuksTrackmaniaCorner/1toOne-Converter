using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043026 : Chunk
    {
        public static readonly string clipGlobalKey = "Clip Global";

        public readonly GBXNodeRef clipGlobal;

        public Challenge03043026(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            clipGlobal = list.ReadGBXNodeRef(s, context);
            AddChildDeprecated(clipGlobalKey, clipGlobal);
        }
    }
}
