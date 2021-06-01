using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.Util
{
    public struct Marker
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float Z;

        [XmlAttribute]
        public byte Rot;

        public Marker Displace(float x, float y, float z)
        {
            var result = new Marker
            {
                Name = Name,
                X = X + x,
                Y = Y + y,
                Z = Z + z,
                Rot = Rot
            };
            return result;
        }

        public Marker Displace(GBXVec3 vec) => Displace(vec.X, vec.Y, vec.Z);

        public Marker Rotate(byte rot)
        {
            var result = new Marker
            {
                Name = Name,
                Y = Y,
                Rot = (byte)((Rot + rot) % 4)
            };

            switch(rot)
            {
                case 0:
                    result.X = X;
                    result.Z = Z;
                    break;
                case 1:
                    result.X = -Z;
                    result.Z = X;
                    break;
                case 2:
                    result.X = -X;
                    result.Z = -Z;
                    break;
                case 3:
                    result.X = Z;
                    result.Z = -X;
                    break;
                default:
                    throw new Exception();
            }

            return result;

        }
    }
}
