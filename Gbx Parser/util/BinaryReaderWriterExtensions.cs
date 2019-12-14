using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.util
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
    }
}
