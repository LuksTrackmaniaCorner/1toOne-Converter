using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.parser.core
{
    public abstract class GbxPrimitive2<T> : GbxComposite<GbxPrimitive<T>> where T : IEquatable<T>
    {
        /*
        public override abstract string ToString();
        public abstract void FromString();
        public abstract void ToBytes(StreamWriter writer);
        public abstract void FromBytes(StreamReader reader);
        */
    }
}
