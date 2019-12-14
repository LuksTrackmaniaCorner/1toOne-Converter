using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043019 : Chunk
    {
        public static readonly string modRefKey = "Mod Reference";

        public readonly GBXFileRef modRef;

        public Challenge03043019(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            modRef = new GBXFileRef(s);
            AddChildDeprevated(modRefKey, modRef);
        }
    }
}
