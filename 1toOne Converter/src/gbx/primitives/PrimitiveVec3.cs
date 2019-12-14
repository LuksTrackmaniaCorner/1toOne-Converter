using _1toOne_Converter.src.gbx.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.gbx.primitives
{
    public abstract class PrimitiveVec3<T> : FileComponent
    {
        [XmlAttribute]
        public T X;

        [XmlAttribute]
        public T Y;

        [XmlAttribute]
        public T Z;

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(X.ToString());
            result.AddLast(Y.ToString());
            result.AddLast(Z.ToString());
            return result;
        }
    }
}
