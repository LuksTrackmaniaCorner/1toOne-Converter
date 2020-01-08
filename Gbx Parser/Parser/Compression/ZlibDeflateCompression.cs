﻿using Gbx.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Gbx.Parser.Compression
{
    public class ZlibDeflateCompression : ICompression
    {
        public void Compress(MemoryStream uncompressedStream, BinaryWriter compressedStream)
        {
            var uncompressedLength = (uint)uncompressedStream.Length;

            using var deflateStream = new DeflateStream(uncompressedStream, CompressionMode.Compress, false);

            compressedStream.Write(uncompressedLength);
            var lengthWriter = compressedStream.CreateLengthWriter(); //skip compressed length until it is known

            deflateStream.CopyTo(compressedStream.BaseStream);

            lengthWriter.WriteLength(); //write compressed length
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