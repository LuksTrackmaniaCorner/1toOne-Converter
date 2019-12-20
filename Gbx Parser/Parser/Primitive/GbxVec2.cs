using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Primitive
{
    public class GbxVec2 : GbxPrimitive2<float>
    {
        public GbxVec2(float initialX = 0, float initialY = 0,
            Predicate<float>? xConstraint = null, Predicate<float>? yConstraint = null) :
        base(new GbxFloat(initialX, xConstraint), new GbxFloat(initialY, yConstraint))
        {
        }
    }
}
