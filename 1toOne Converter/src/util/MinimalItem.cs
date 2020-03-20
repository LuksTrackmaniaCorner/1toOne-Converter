using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.util
{
    /// <summary>
    /// Shortest possible representation of an item.
    /// </summary>
    public class MinimalItem
    {
        public Meta Meta;
        public GBXVec3 Rot;
        public GBXByte3 BlockCoords;
        public GBXVec3 ItemCoords;
    }
}
