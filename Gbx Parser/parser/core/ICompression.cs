using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gbx.parser.core
{
    public interface ICompression
    {
        public abstract void Compress(byte[] uncompressedData, BinaryWriter resultStream);

        public abstract void Decompress(BinaryReader compressedStream, out byte[] resultData);
    }
}
