using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxUInt3 : GbxPrimitive3<uint>
    {
        public GbxUInt3(
            uint initialX = 0,
            uint initialY = 0,
            uint initialZ = 0,
            Func<uint, bool>? xConstraint = null,
            Func<uint, bool>? yConstraint = null,
            Func<uint, bool>? zConstraint = null) :
        base(
            new GbxUInt(initialX, xConstraint),
            new GbxUInt(initialY, yConstraint),
            new GbxUInt(initialZ, zConstraint))
        {
        }
    }
}
