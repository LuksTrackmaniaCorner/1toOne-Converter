using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Primitive
{
    public class GbxRotation : GbxVec3
    {
        //TODO investigate how the rotation is actually stored. Is it pitch/yaw/roll or some kind of euler angles?
        public GbxPrimitive<float> Pitch { get => X; }
        public GbxPrimitive<float> Yaw { get => Y; }
        public GbxPrimitive<float> Roll { get => Z; }

        public GbxRotation(
            float initialPitch = 0, float initialYaw = 0, float initialRoll = 0,
            Predicate<float>? pitchConstraint = null, Predicate<float>? yawConstraint = null, Predicate<float>? rollConstraint = null) :
        base(initialPitch, initialYaw, initialRoll,
            pitchConstraint, yawConstraint, rollConstraint)
        {
        }

        public override IEnumerable<(string, GbxPrimitive<float>)> GetNamedChildren()
        {
            yield return (nameof(Pitch), Pitch);
            yield return (nameof(Roll), Roll);
            yield return (nameof(Yaw), Yaw);
        }
    }
}
