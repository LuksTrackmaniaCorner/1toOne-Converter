using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Util
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
