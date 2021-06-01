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
