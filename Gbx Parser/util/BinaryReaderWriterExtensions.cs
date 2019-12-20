using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Util
{
    internal static class BinaryReaderWriterExtensions
    {
        public static string ReadUTF8(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        public static void WriteUTF8(this BinaryWriter writer, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        internal static Action WriteLengthLater(this BinaryWriter writer)
        {
            var lengthPos = writer.BaseStream.Position;
            writer.BaseStream.Position += sizeof(uint);
            var startPos = writer.BaseStream.Position;

            return () =>
            {
                var endPos = writer.BaseStream.Position;
                var length = (uint)(endPos - startPos);

                writer.BaseStream.Position = lengthPos;
                writer.Write(length);
                writer.BaseStream.Position = endPos;
            };
        }
    }
}
