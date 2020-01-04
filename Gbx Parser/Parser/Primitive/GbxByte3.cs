using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Primitive
{
    public class GbxByte3 : GbxPrimitive3<byte>
    {
        public GbxByte3(byte initialX = 0, byte initialY = 0, byte initialZ = 0,
            Predicate<byte>? xConstraint = null, Predicate<byte>? yConstraint = null, Predicate<byte>? zConstraint = null) :
        base(new GbxByte(initialX, xConstraint), new GbxByte(initialY, yConstraint), new GbxByte(initialZ, zConstraint))
        {
        }
    }
}
