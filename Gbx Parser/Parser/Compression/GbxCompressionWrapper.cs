using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Compression
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
