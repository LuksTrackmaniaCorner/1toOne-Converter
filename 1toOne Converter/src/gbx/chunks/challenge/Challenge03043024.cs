using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043024 : Chunk
    {
        public static readonly string customMusicPackDescKey = "Custom Music Pack Desc";

        public readonly GBXFileRef customMusicPackDesc;

        public Challenge03043024(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            customMusicPackDesc = new GBXFileRef(s);
            AddChildDeprecated(customMusicPackDescKey, customMusicPackDesc);
        }
    }
}
