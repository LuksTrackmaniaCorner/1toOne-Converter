using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeParameters0305B005 : Chunk
    {
        public readonly GBXUInt ignored1;
        public readonly GBXUInt ignored2;
        public readonly GBXUInt ignored3;

        //TODO improve names

        public ChallengeParameters0305B005(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            ignored1 = new GBXUInt(s);
            AddChildDeprecated("Ignored 1", ignored1);

            ignored2 = new GBXUInt(s);
            AddChildDeprecated("Ignored 2", ignored2);

            ignored3 = new GBXUInt(s);
            AddChildDeprecated("Ignored 3", ignored3);
        }
    }
}
