using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043049 : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public Challenge03043049(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            //TODO Figure out what this is. Mediatracker maybe?
            var unknown = new Unread(s, 36);
            AddChildDeprevated(unknownKey, unknown);
        }
    }
}
