using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.ChallengeParameters
{
    //TODO rebuild properly
    public class ChallengeParameters0305B001 : Chunk
    {
        public readonly GBXString string1;
        public readonly GBXString string2;
        public readonly GBXString string3;
        public readonly GBXString string4;

        public ChallengeParameters0305B001(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            string1 = new GBXString(s);
            AddChildDeprecated("String 1", string1);

            string2 = new GBXString(s);
            AddChildDeprecated("String 2", string2);

            string3 = new GBXString(s);
            AddChildDeprecated("String 3", string3);

            string4 = new GBXString(s);
            AddChildDeprecated("String 4", string4);
        }
    }
}
