using Converter.Gbx.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Util;
using System.IO;
using Converter.Gbx.Primitives;

namespace Converter.Gbx.Chunks.Challenge
{
    public class Challenge03043051 : Chunk
    {
        public GBXUInt AlwaysZero { get; }
        public GBXLBS TitlePack { get; }
        public GBXString Version { get; }

        public Challenge03043051(Stream s, GBXLBSContext c) : base(c, null)
        {
            AlwaysZero = new GBXUInt(s);
            LBSContext.SkipVersion();
            TitlePack = c.ReadLookBackString(s);
            Version = new GBXString(s);
        }

        public Challenge03043051(bool dummy) : base(null, null)
        {
            ChunkID = 0x03043051;
            AlwaysZero = new GBXUInt(0);
            TitlePack = new GBXLBS("");
            TitlePack.Parent = this;
            Version = new GBXString("date=2019-07-03_10_37 Svn=93842 GameVersion=3.3.0"); //Good Default
            LBSContext.SkipVersion();
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(nameof(AlwaysZero), AlwaysZero);
            result.AddChild(nameof(TitlePack), TitlePack);
            result.AddChild(nameof(Version), Version);
            return result;
        }
    }
}
