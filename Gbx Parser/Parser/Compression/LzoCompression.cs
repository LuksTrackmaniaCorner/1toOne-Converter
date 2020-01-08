using Gbx.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Compression
{
    public class LzoCompression : ICompression
    {
        public void Compress(MemoryStream uncompressedStream, BinaryWriter compressedStream)
        {
            var uncompressedData = uncompressedStream.ToArray();

            MiniLZO.Compress(uncompressedData, out byte[] compressedData);

            compressedStream.Write(uncompressedData.Length);
            compressedStream.Write(compressedData.Length);
            compressedStream.Write(compressedData);
        }

        public Stream Decompress(BinaryReader compressedStream)
        {
            var uncompressedSize = compressedStream.ReadInt32();
            if (uncompressedSize < 0)
                throw new Exception();

            var compressedSize = compressedStream.ReadInt32();
            if (compressedSize < 0 || compressedSize > uncompressedSize)
                throw new Exception();

            var uncompressedData = new byte[uncompressedSize];
            var compressedData = compressedStream.ReadBytes(compressedSize);

            MiniLZO.Decompress(compressedData, uncompressedData);

            return new MemoryStream(uncompressedData);
        }
    }
}
