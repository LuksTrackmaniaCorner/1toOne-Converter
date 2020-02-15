using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.src.util;
using System.IO;

namespace _1toOne_Converter.src.gbx.chunks
{
    public class Challenge03043051 : Chunk
    {
        public GBXUInt AlwaysZero { get; }
        public GBXLBS TitlePack { get; }
        public GBXString Version { get; }

        public Challenge03043051(Stream s, GBXLBSContext c) : base(c, null)
        {
            AlwaysZero = new GBXUInt(s);
            this.LBSContext.SkipVersion();
            TitlePack = c.ReadLookBackString(s);
            Version = new GBXString(s);
        }

        public Challenge03043051(bool dummy) : base(null, null)
        {
            this.ChunkID = 0x03043051;
            AlwaysZero = new GBXUInt(0);
            TitlePack = new GBXLBS("");
            TitlePack.Parent = this;
            Version = new GBXString("date=2019-07-03_10_37 Svn=93842 GameVersion=3.3.0"); //Good Default
            this.LBSContext.SkipVersion();
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
