using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043011 : Chunk
    {
        public static string collectorListKey = "Collector List";
        public static string challengeParametersKey = "Challenge Parameters";
        public static string kindKey = "Kind";

        public readonly GBXNodeRef collectorList;
        public readonly GBXNodeRef challengeParameters;
        public readonly GBXUInt kind;

        public Challenge03043011(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            collectorList = list.ReadGBXNodeRef(s, context);
            AddChildDeprevated(collectorListKey, collectorList);

            challengeParameters = list.ReadGBXNodeRef(s, context);
            AddChildDeprevated(challengeParametersKey, challengeParameters);

            kind = new GBXUInt(s);
            AddChildDeprevated(kindKey, kind);
        }
    }
}
