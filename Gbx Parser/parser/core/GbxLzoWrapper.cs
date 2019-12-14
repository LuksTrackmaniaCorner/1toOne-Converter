using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.util;

namespace gbx.parser.core
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

            public void Compress(byte[] uncompressedData, BinaryWriter compressedStream)
            {
                MiniLZO.Compress(uncompressedData, out byte[] compressedData);

                compressedStream.Write(uncompressedData.Length);
                compressedStream.Write(compressedData.Length);
                compressedStream.Write(compressedData);
            }

            public void Decompress(BinaryReader compressedStream, out byte[] uncompressedData)
            {
                var uncompressedSize = compressedStream.ReadInt32();
                if (uncompressedSize < 0)
                    throw new Exception();

                var compressedSize = compressedStream.ReadInt32();
                if (compressedSize < 0 || compressedSize > uncompressedSize)
                    throw new Exception();

                uncompressedData = new byte[uncompressedSize];
                var compressedData = compressedStream.ReadBytes(compressedSize);
                MiniLZO.Decompress(compressedData, uncompressedData);
            }
        }
    }
}
