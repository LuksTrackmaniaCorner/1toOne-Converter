using _1toOne_Converter.src.gbx.core.chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.util
{
    public class Flag
    {
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public short X;
        [XmlAttribute]
        public byte Y;
        [XmlAttribute]
        public short Z;

        public Flag()
        {
            Y = 1;
        }

        public Flag(FlagName flagname, short x, short z)
        {
            Name = flagname.Name;
            X = x;
            Y = flagname.Value;
            Z = z;
        }

        public Flag GetRelativeToBlock(short x, short z, byte rot)
        {
            switch (rot)
            {
                case 0:
                    x += this.X;
                    z += this.Z;
                    break;
                case 1:
                    x -= this.Z;
                    z += this.X;
                    break;
                case 2:
                    x -= this.X;
                    z -= this.Z;
                    break;
                case 3:
                    x += this.Z;
                    z -= this.X;
                    break;
                default:
                    throw new Exception();
            }

            return new Flag() { Name = Name, X = x, Z = z };
        }

        public bool ShouldSerializeY() => Y > 1;
    }

    //TODO useless class. remove
    public class FlagName
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public byte Value;

        public FlagName()
        {
            Value = 1;
        }

        public FlagName(string name)
        {
            Name = name;
            Value = 1;
        }

        public bool ShouldSerializeValue() => Value > 1;
    }
}