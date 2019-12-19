using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.compression
{
    public class GbxCompressionWrapper<T> : GbxWrapper<T> where T : GbxComponent
    {
        public ICompression Compression { get; }

        public GbxCompressionWrapper(GbxComposite<T> wrapped, ICompression compression) : base(wrapped)
        {
            Compression = compression;
        }
    }
}
