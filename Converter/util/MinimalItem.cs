using Converter.Gbx.core;
using Converter.Gbx.core.primitives;
using Converter.Gbx.primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.util
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
