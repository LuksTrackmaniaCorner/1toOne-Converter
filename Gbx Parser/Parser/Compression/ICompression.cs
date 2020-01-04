using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Compression
{
    public interface ICompression
    {
        /// <summary>
        /// Compresses Data. The algorithm depends on the implementation in the subclass.
        /// </summary>
        /// <param name="uncompressedStream">The uncompressed Data as MemoryStream</param>
        /// <param name="resultStream">The stream compressed Data will be appended</param>
        public abstract void Compress(MemoryStream uncompressedStream, BinaryWriter resultStream);

        /// <summary>
        /// Decompresses Data. The algorithm depends on the implementation in the subclass.
        /// </summary>
        /// <param name="compressedStream"></param>
        /// <returns>Uncompressed Data as Stream</returns>
        public abstract Stream Decompress(BinaryReader compressedStream);
    }
}
