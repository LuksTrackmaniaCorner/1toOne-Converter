using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043025 : Chunk
    {
        public static readonly string mapCoordsOriginKey = "Map Coords Origin";
        public static readonly string mapCoordsTargetKey = "Map Coords Target";

        public readonly GBXVec2 mapCoordsOrigin;
        public readonly GBXVec2 mapCoordsTarget;

        public Challenge03043025(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            var mapCoordsOrigin = new GBXVec2(s);
            AddChildDeprecated(mapCoordsOriginKey, mapCoordsOrigin);

            var mapCoordsTarget = new GBXVec2(s);
            AddChildDeprecated(mapCoordsTargetKey, mapCoordsTarget);
        }
    }
}
