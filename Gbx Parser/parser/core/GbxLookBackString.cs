using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gbx.Parser.Primitive;
using Gbx.Parser.Visit;
using Gbx.Util;

namespace Gbx.Parser.Core
{
    public sealed class GbxLookBackString : GbxComposite<GbxLeaf>
    {
        public const uint Unassigned = 0xFFFFFFFF;
        private const uint StringFlag = 1 << 30; // could also be 1 << 31

        public GbxString Content { get; }
        public GbxUInt CollectionID { get; }
        public GbxLookBackStringType Type
        {
            get => (GbxLookBackStringType)_type.GetValue();
            set => _type.SetValue((byte)value, NotifyChange);
        }

        private readonly ControlledVar<byte> _type;

        public GbxLookBackString()
        {
            Content = new GbxString();
            CollectionID = new GbxUInt();
            _type = new ControlledVar<byte>(0);
        }

        public void ToStream(BinaryWriter writer, List<string> storedStrings)
        {
            switch (Type)
            {
                case GbxLookBackStringType.Unassigned:
                    writer.Write(Unassigned);
                    break;
                case GbxLookBackStringType.CollectionID:
                    CollectionID.ToStream(writer);
                    break;
                case GbxLookBackStringType.String:
                    var index = storedStrings.IndexOf(Content.Value);
                    if(index < 0) //string not stored
                    {
                        writer.Write(StringFlag);
                        Content.ToStream(writer);
                        storedStrings.Add(Content.Value);
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
                    CollectionID.Value = index;
                    Type = GbxLookBackStringType.CollectionID;
                    break;
                case 0b01:
                case 0b10:
                    if ((index &= 0x3FFFFFFF) == 0) //Check for StringFlag
                    {
                        Content.FromStream(reader);
                        Type = GbxLookBackStringType.String;
                        storedStrings.Add(Content.Value);
                    }
                    else
                    {
                        Content.Value = storedStrings[(int)(index - 1)];
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        public override IEnumerable<GbxLeaf> GetChildren()
        {
            switch (Type)
            {
                case GbxLookBackStringType.Unassigned:
                    break;
                case GbxLookBackStringType.CollectionID:
                    yield return CollectionID;
                    break;
                case GbxLookBackStringType.String:
                    yield return Content;
                    break;
                default:
                    throw new Exception();
            }
        }

        public override IEnumerable<(string, GbxLeaf)> GetNamedChildren()
        {
            switch (Type)
            {
                case GbxLookBackStringType.Unassigned:
                    break;
                case GbxLookBackStringType.CollectionID:
                    yield return (nameof(CollectionID), CollectionID);
                    break;
                case GbxLookBackStringType.String:
                    yield return (nameof(Content), Content);
                    break;
                default:
                    throw new Exception();
            }
        }

        internal override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public enum GbxLookBackStringType : byte
    {
        Unassigned = 0, // default
        CollectionID = 1,
        String = 2
    }
}
