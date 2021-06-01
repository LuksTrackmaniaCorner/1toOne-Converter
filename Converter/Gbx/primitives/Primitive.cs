using Converter.Gbx.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.Gbx.Primitives
{
    public abstract class Primitive<T> : FileComponent
    {
        [XmlAttribute]
        public T Value;

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(Value.ToString());
            return result;
        }
    }
}
