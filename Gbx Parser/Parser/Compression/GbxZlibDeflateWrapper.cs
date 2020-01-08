using Gbx.Parser.Core;
using Gbx.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Compression
{
    public class GbxZlibDeflateWrapper<T> : GbxCompressionWrapper<T> where T : GbxComponent
    {
        private static readonly ZlibDeflateCompression _zlibDeflateInstance = new ZlibDeflateCompression();

        public GbxZlibDeflateWrapper(GbxComposite<T> wrapped) : base(wrapped, _zlibDeflateInstance)
        {
        }
    }
}
