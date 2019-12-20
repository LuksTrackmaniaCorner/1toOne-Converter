using Gbx.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxUnread : GbxLeaf
    {
        private readonly int _length;
        private byte[] _value;

        public GbxUnread(int length)
        {
            if (length < 0)
                throw new ArgumentException(nameof(length));

            _length = length;
            _value = new byte[_length];
        }

        public override void FromString(string value)
        {
            var numChars = value.Length;
            if (numChars != _length * 2)
                throw new ArgumentException(nameof(value));

            var buffer = new char[2];
            for(int bytePos = 0; bytePos < _value.Length; bytePos++)
            {
                int stringPos = bytePos * 2;
                buffer[0] = value[stringPos];
                buffer[1] = value[stringPos + 1];

                _value[bytePos] = Convert.ToByte(new string(buffer));
            }
        }

        public override string ToString()
        {
            return BitConverter.ToString(_value).Replace("-","");
        }

        public override void FromStream(BinaryReader reader)
        {
            _value = reader.ReadBytes(_length);
        }

        public override void ToStream(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
