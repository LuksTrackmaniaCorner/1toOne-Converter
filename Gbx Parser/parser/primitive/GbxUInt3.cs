using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxUInt3 : GbxPrimitive3<uint>
    {
        public GbxUInt3(uint initialX = 0, uint initialY = 0, uint initialZ = 0,
            Predicate<uint>? xConstraint = null, Predicate<uint>? yConstraint = null, Predicate<uint>? zConstraint = null) :
        base(new GbxUInt(initialX, xConstraint), new GbxUInt(initialY, yConstraint), new GbxUInt(initialZ, zConstraint))
        {
        }
    }
}
