using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.primitive
{
    public class GbxRgb : GbxVec3
    {
        public GbxPrimitive<float> Red { get => X; }
        public GbxPrimitive<float> Green { get => Y; }
        public GbxPrimitive<float> Blue { get => Z; }

        public GbxRgb(float initialRed = 0, float initialGreen = 0, float initialBlue = 0,
            Predicate<float>? redConstraint = null, Predicate<float>? greenConstraint = null, Predicate<float>? blueConstraint = null) :
        base(initialRed, initialGreen, initialBlue,
            redConstraint, greenConstraint, blueConstraint)
        {
        }

        public override IEnumerable<(string, GbxPrimitive<float>)> GetNamedChildren()
        {
            yield return (nameof(Red), Red);
            yield return (nameof(Green), Green);
            yield return (nameof(Blue), Blue);
        }
    }
}
