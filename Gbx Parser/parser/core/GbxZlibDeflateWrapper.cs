using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace gbx.parser.core
{
    public class GbxZlibDeflateWrapper<T> : GbxCompressionWrapper<T> where T : GbxComponent
    {
        public GbxZlibDeflateWrapper(GbxComposite<T> wrapped, ICompression compression) : base(wrapped, compression)
        {
        }

        private class ZlibDeflateCompression : ICompression
        {
            public void Compress(byte[] uncompressedData, BinaryWriter resultStream)
            {
                using var uncompressedStream = new MemoryStream(uncompressedData);
                using var deflateStream = new DeflateStream(uncompressedStream, CompressionMode.Compress, false);
                

                resultStream.Write(uncompressedData.Length);
                //TODO write compressed Length;
                deflateStream.CopyTo(resultStream.BaseStream);
            }

            public void Decompress(BinaryReader compressedStream, out byte[] resultData)
            {
                var uncompressedSize = compressedStream.ReadInt32();
                if (uncompressedSize < 0)
                    throw new Exception();

                var compressedSize = compressedStream.ReadInt32();
                if (compressedSize < 0 || compressedSize > uncompressedSize)
                    throw new Exception();

                using var deflateStream = new DeflateStream(compressedStream.BaseStream, CompressionMode.Decompress, true);
                resultData = new BinaryReader(deflateStream).ReadBytes(compressedSize);
            }
        }
    }
}
