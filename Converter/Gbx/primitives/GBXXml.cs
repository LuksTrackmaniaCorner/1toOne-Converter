using Converter.Gbx.core;
using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Converter.Gbx.primitives
{
    public sealed class GBXXml<T> : FileComponent
    {
        public T Xml;

        private XmlSerializer _serializer;

        public GBXXml(Stream stream)
        {
            var length = stream.ReadUInt();
            var mems = new MemoryStream(stream.SimpleRead((int)length));

            _serializer = new XmlSerializer(typeof(T));
            Xml = (T)_serializer.Deserialize(mems);
        }

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(Xml.ToString());
            return result;
        }

        public override void WriteBack(Stream s)
        {
            var sizePos = s.Position;
            s.WriteUInt(0); //Size;
            var startPos = s.Position;
            var xmlWriter = XmlWriter.Create(s, new XmlWriterSettings { OmitXmlDeclaration = true });
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            _serializer.Serialize(xmlWriter, Xml, ns);
            var endPos = s.Position;
            s.Position = sizePos;
            s.WriteUInt((uint)(endPos - startPos));
            s.Position = endPos;
        }
    }
}
