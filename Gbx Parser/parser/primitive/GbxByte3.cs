using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxByte3 : GbxPrimitive3<byte>
    {
        public GbxByte3(
            byte initialX = 0,
            byte initialY = 0,
            byte initialZ = 0,
            Func<byte, bool>? xConstraint = null,
            Func<byte, bool>? yConstraint = null,
            Func<byte, bool>? zConstraint = null) :
        base(
            new GbxByte(initialX, xConstraint),
            new GbxByte(initialY, yConstraint),
            new GbxByte(initialZ, zConstraint))
        {
        }
    }
}
