using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Util
{
    internal static class BinaryReaderWriterExtensions
    {
        public static string ReadUTF8String(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return ReadUTF8Chars(reader, length);
        }

        public static char ReadUTF8Char(this BinaryReader reader)
        {
            return reader.ReadUTF8Chars(1)[0];
        }

        public static string ReadUTF8Chars(this BinaryReader reader, int length)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        public static void WriteUTF8String(this BinaryWriter writer, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static void WriteUTF8Char(this BinaryWriter writer, char value)
        {
            writer.WriteUTF8Chars(new string(value, 1));
        }

        public static void WriteUTF8Chars(this BinaryWriter writer, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes);
        }

        internal static ILengthTester CreateLengthTester(this BinaryReader reader)
        {
            var expectedLength = reader.ReadUInt32();
            return reader.CreateLengthTester(expectedLength);
        }

        internal static ILengthTester CreateLengthTester(this BinaryReader reader, uint expectedLength)
        {
            return new LengthTester(expectedLength, reader);
        }

        internal static ILengthWriter CreateLengthWriter(this BinaryWriter writer)
        {
            return new LengthWriter(writer);
        }
    }

    public interface ILengthTester
    {
        public abstract bool TestLength();
    }

    public class LengthTester : ILengthTester
    {
        private readonly long _expectedEnd;
        private readonly BinaryReader _reader;

        internal LengthTester(uint expectedLength, BinaryReader reader)
        {
            _reader = reader;
            _expectedEnd = expectedLength + reader.BaseStream.Position;
        }

        public bool TestLength()
        {
            return _expectedEnd == _reader.BaseStream.Position;
        }
    }

    public interface ILengthWriter
    {
        public abstract void WriteLength();
    }

    public class LengthWriter : ILengthWriter
    {
        private readonly long lengthPos;
        private readonly long startPos;
        private readonly BinaryWriter _writer;

        internal LengthWriter(BinaryWriter writer)
        {
            _writer = writer;
            var stream = writer.BaseStream;

            lengthPos = stream.Position;
            stream.Position += sizeof(uint);
            startPos = stream.Position;
        }

        public void WriteLength()
        {
            var stream = _writer.BaseStream;
            var endPos = stream.Position;
            var length = (uint)(endPos - startPos);

            stream.Position = lengthPos;
            _writer.Write(length);
            stream.Position = endPos;
        }
    }
}
