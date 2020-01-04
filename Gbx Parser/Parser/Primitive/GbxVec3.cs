using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Primitive
{
    public class GbxVec3 : GbxPrimitive3<float>
    {
        public GbxVec3(float initialX = 0, float initialY = 0, float initialZ = 0,
            Predicate<float>? xConstraint = null, Predicate<float>? yConstraint = null, Predicate<float>? zConstraint = null) :
        base(new GbxFloat(initialX, xConstraint), new GbxFloat(initialY, yConstraint), new GbxFloat(initialZ, zConstraint))
        {
        }
    }
}
