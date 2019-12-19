using gbx.parser.core;
using gbx.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace gbx.parser.compression
{
    public class GbxZlibDeflateWrapper<T> : GbxCompressionWrapper<T> where T : GbxComponent
    {
        public GbxZlibDeflateWrapper(GbxComposite<T> wrapped, ICompression compression) : base(wrapped, compression)
        {
        }

        private class ZlibDeflateCompression : ICompression
        {
            public void Compress(MemoryStream uncompressedStream, BinaryWriter compressedStream)
            {
                var uncompressedLength = (uint)uncompressedStream.Length;

                using var deflateStream = new DeflateStream(uncompressedStream, CompressionMode.Compress, false);

                compressedStream.Write(uncompressedLength);
                var writeLength = compressedStream.WriteLengthLater(); //skip compressed length until it is known

                deflateStream.CopyTo(compressedStream.BaseStream);

                writeLength(); //write compressed length
            }

            public Stream Decompress(BinaryReader compressedStream)
            {
                var uncompressedSize = compressedStream.ReadInt32();
                if (uncompressedSize < 0)
                    throw new Exception();

                var compressedSize = compressedStream.ReadInt32();
                if (compressedSize < 0 || compressedSize > uncompressedSize)
                    throw new Exception();

                return new DeflateStream(compressedStream.BaseStream, CompressionMode.Decompress, true);
            }
        }
    }
}
