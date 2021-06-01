using Converter.Gbx.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.Gbx.Primitives
{
    public abstract class PrimitiveVec2<T> : FileComponent
    {
        [XmlAttribute]
        public T X;

        [XmlAttribute]
        public T Y;

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(X.ToString());
            result.AddLast(Y.ToString());
            return result;
        }
    }
}
