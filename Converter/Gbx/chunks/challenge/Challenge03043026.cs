using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.Challenge
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
