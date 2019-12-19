using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.parser.core;
using gbx.util;

namespace gbx.parser.compression
{
    public class GbxLzoWrapper<T> : GbxCompressionWrapper<T> where T : GbxComponent
    {
        public GbxLzoWrapper(GbxComposite<T> wrapped) : base(wrapped, LzoCompression.Instance)
        {
        }

        private class LzoCompression : ICompression
        {
            public static LzoCompression Instance { get; }

            static LzoCompression()
            {
                Instance = new LzoCompression();
            }

            private LzoCompression()
            {
            }

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
}
