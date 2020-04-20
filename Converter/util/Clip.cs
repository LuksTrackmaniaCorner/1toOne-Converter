using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.util
{
    public class Clip : IEquatable<Clip>
    {
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public short X;
        [XmlAttribute]
        public short Y;
        [XmlAttribute]
        public short Z;
        [XmlAttribute]
        public byte Rot;

        public Clip()
        {

        }

        public Clip(string name, short x, short y, short z, byte rot)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            Rot = rot;
        }

        public bool Equals(Clip other)
        {
            if (other == null)
                return false;
            return this.Name.Equals(other.Name) && this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.Rot == other.Rot;
        }

        public override bool Equals(object obj) => Equals(obj as Clip);

        public override int GetHashCode()
        {
            var hashCode = -522377396;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + Rot.GetHashCode();
            return hashCode;
        }

        public Clip GetRelativeToBlock(short x, short y, short z, byte rot)
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
            y += this.Y;
            rot = (byte)((Rot + rot) % 4);

            return new Clip() { Name = Name, X = x, Y = y, Z = z, Rot = rot };
        }

        public static (int x, int y, int z) GetCoordsFacing((int x, int y, int z) coords, byte rot)
        {
            return rot switch
            {
                0 => (coords.x    , coords.y, coords.z + 1),
                1 => (coords.x - 1, coords.y, coords.z    ),
                2 => (coords.x    , coords.y, coords.z - 1),
                3 => (coords.x + 1, coords.y, coords.z    ),
                _ => throw new Exception()
            };
        }
    }
}
