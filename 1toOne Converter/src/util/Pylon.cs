using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.util
{
    public class Pylon : IEquatable<Pylon>
    {
        [XmlAttribute]
        public PylonType Type;
        [XmlAttribute]
        public PylonPosition Pos;
        [XmlAttribute]
        public short X;
        [XmlAttribute]
        public short Y;
        [XmlAttribute]
        public short Z;
        [XmlAttribute]
        public byte Rot;
        [XmlAttribute]
        public bool Optional;

        internal byte NormalizedRot
        {
            get
            {
                return Rot switch
                {
                    1 => 1,
                    2 => 0,
                    _ => throw new InternalException()
                };
            }
        }

        public Pylon Normalize()
        {
            if (Type == PylonType.Forced) //No Normalizing allowed
                return this;

            switch(Rot)
            {
                case 0:
                    Z += 1;
                    Rot = 2;
                    break;
                case 1:
                case 2:
                    return this;
                case 3:
                    X += 1;
                    Rot = 1;
                    break;
                default:
                    throw new InternalException();
            }

            switch(Pos)
            {
                case PylonPosition.Left:
                    Pos = PylonPosition.Right;
                    break;
                case PylonPosition.Right:
                    Pos = PylonPosition.Left;
                    break;
            }

            return this;
        }

        public Pylon GetRelativeToBlock(short x, short y, short z, byte rot)
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

            return new Pylon() { Type = Type, Pos = Pos, X = x, Y = y, Z = z, Rot = rot, Optional = Optional };
        }

        public static IComparer<Pylon> GetComparer() => new PylonComparer();

        class PylonComparer : IComparer<Pylon>
        {
            public int Compare(Pylon p1, Pylon p2)
            {
                //Primary criteria: Height, from top to bottom
                int result = p2.Y.CompareTo(p1.Y);
                if (result != 0)
                    return result;

                //Seondary criteria: Type None -> Prevent -> Top -> Bottom
                result = p1.Type.CompareTo(p2.Type);
                if (result != 0)
                    return result;

                //Tertiary criteria: Optional
                result = p1.Optional.CompareTo(p2.Optional);
                return result;
            }
        }

        public bool IsLeftPosition() => Pos != PylonPosition.Right;

        public bool IsRightPosition() => Pos != PylonPosition.Left;

        public override bool Equals(object obj)
        {
            return Equals(obj as Pylon);
        }

        public bool Equals(Pylon other)
        {
            return other != null &&
                Type == other.Type &&
                Pos == other.Pos &&
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                Rot == other.Rot &&
                Optional == other.Optional;
        }

        public override int GetHashCode()
        {
            var hashCode = 334360292;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Pos.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + Rot.GetHashCode();
            hashCode = hashCode * -1521134295 + Optional.GetHashCode();
            return hashCode;
        }

        public bool ShouldSerializePos() => Pos != PylonPosition.Both;
    }

    public enum PylonType : byte
    {
        None = 0,
        Prevent = 1,
        Top = 2,
        Bottom = 3,
        Forced = 4 //For the Speed Signs
    }

    public enum PylonPosition : byte
    {
        Both = 0,
        Left = 1,
        Right = 2
    }

    public class MultiPylon
    {
        [XmlAttribute]
        public PylonType Type;
        [XmlAttribute]
        public PylonPosition Pos;
        [XmlAttribute]
        public short X;
        [XmlAttribute]
        public short Y;
        [XmlAttribute]
        public short Z;
        [XmlAttribute]
        public bool Optional;
        [XmlAttribute]
        public MultiRot Rot;

        public MultiPylon()
        {
            Rot = MultiRot.All;
        }

        public IEnumerator<Pylon> GetPylons()
        {
            if ((Rot & MultiRot.Zero) != 0)
                yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 0, Optional = Optional };
            if ((Rot & MultiRot.One) != 0)
                yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 1, Optional = Optional };
            if ((Rot & MultiRot.Two) != 0)
                yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 2, Optional = Optional };
            if ((Rot & MultiRot.Three) != 0)
                yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 3, Optional = Optional };
        }

        public static MultiRot GetRot(byte rot) => (MultiRot)(1 << rot);
    }

    [FlagsAttribute]
    public enum MultiRot
    {
        [XmlEnum(Name = "0")]
        Zero = 1,
        [XmlEnum(Name = "1")]
        One = 2,
        [XmlEnum(Name = "2")]
        Two = 4,
        [XmlEnum(Name = "3")]
        Three = 8,
        [XmlEnum(Name = "All")]
        All = 15
    }
}
