using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.parser.visitor;
using gbx.util;

namespace gbx.parser.core
{
    public sealed class GbxLookBackString : GbxLeaf
    {
        public const uint Unassigned = 0xFFFFFFFF;
        private const uint StringFlag = 1 << 30; // could also be 1 << 31

        public const char UnassignedPrefix = 'U';
        public const char CollectionIDPrefix = 'C';
        public const char StringPrefix = 'S';

        private readonly ControlledVar<string> _var;
        private readonly ControlledVar<uint> _ID;
        private readonly ControlledVar<byte> _type;

        public string Value
        {
            get => _var.GetValue();
            set => _var.SetValue(value, NotifyChange);
        }
        public uint CollectionID
        {
            get => _ID.GetValue();
            set => _ID.SetValue(value, NotifyChange);
        }
        public GbxLookBackStringType Type
        {
            get => (GbxLookBackStringType)_type.GetValue();
            set => _type.SetValue((byte)value, NotifyChange);
        }

        public GbxLookBackString()
        {
            _var = new ControlledVar<string>(string.Empty);
            _ID = new ControlledVar<uint>(0);
            _type = new ControlledVar<byte>(0);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case GbxLookBackStringType.Unassigned:
                    return UnassignedPrefix + "";
                case GbxLookBackStringType.CollectionID:
                    return CollectionIDPrefix + CollectionID.ToString();
                case GbxLookBackStringType.String:
                    return StringPrefix + Value;
                default:
                    throw new Exception();
            }
        }

        public override void FromString(string value)
        {
            switch (value[0])
            {
                case UnassignedPrefix:
                    Type = GbxLookBackStringType.Unassigned;
                    break;
                case CollectionIDPrefix:
                    CollectionID = uint.Parse(value.Substring(1));
                    Type = GbxLookBackStringType.CollectionID;
                    break;
                case StringPrefix:
                    Value = value.Substring(1);
                    Type = GbxLookBackStringType.String;
                    break;
                default:
                    throw new Exception();
            }
        }


        public void ToStream(BinaryWriter writer, List<string> storedStrings)
        {
            switch (Type)
            {
                case GbxLookBackStringType.Unassigned:
                    writer.Write(Unassigned);
                    break;
                case GbxLookBackStringType.CollectionID:
                    writer.Write(CollectionID);
                    break;
                case GbxLookBackStringType.String:
                    var index = storedStrings.IndexOf(Value);
                    if(index < 0) //string not stored
                    {
                        writer.Write(StringFlag);
                        writer.WriteUTF8(Value);
                        storedStrings.Add(Value);
                    }
                    else //string is stored
                    {
                        writer.Write(((uint)index & StringFlag) + 1);
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        public void FromStream(BinaryReader reader, List<string> storedStrings)
        {
            var index = reader.ReadUInt32();

            if (index == Unassigned)
            {
                Type = GbxLookBackStringType.Unassigned;
                return;
            }

            var type = index >> 30;

            switch (type)
            {
                case 0b00:
                    CollectionID = index;
                    Type = GbxLookBackStringType.CollectionID;
                    break;
                case 0b01:
                case 0b10:
                    if ((index &= 0x3FFFFFFF) == 0) //Check for StringFlag
                    {
                        Value = reader.ReadUTF8();
                        Type = GbxLookBackStringType.String;
                        storedStrings.Add(Value);
                    }
                    else
                    {
                        Value = storedStrings[(int)(index - 1)];
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        public override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }

    public enum GbxLookBackStringType : byte
    {
        Unassigned = 0, // default
        CollectionID = 1,
        String = 2
    }
}
