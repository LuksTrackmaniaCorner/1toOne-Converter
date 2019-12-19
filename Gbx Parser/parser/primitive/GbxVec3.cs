using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxVec3 : GbxPrimitive3<float>
    {
        public GbxVec3(
            float initialX = 0,
            float initialY = 0,
            float initialZ = 0,
            Func<float, bool>? xConstraint = null,
            Func<float, bool>? yConstraint = null,
            Func<float, bool>? zConstraint = null) :
        base(
            new GbxFloat(initialX, xConstraint),
            new GbxFloat(initialY, yConstraint),
            new GbxFloat(initialZ, zConstraint))
        {
        }
    }
}
