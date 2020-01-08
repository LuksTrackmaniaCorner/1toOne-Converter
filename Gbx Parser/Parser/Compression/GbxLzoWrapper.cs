using System;
using System.Collections.Generic;
using System.Text;
using Gbx.Parser.Core;

namespace Gbx.Parser.Compression
{
    public class GbxLzoWrapper<T> : GbxCompressionWrapper<T> where T : GbxComponent
    {
        private static readonly LzoCompression _lzoInstance = new LzoCompression();

        public GbxLzoWrapper(GbxComposite<T> wrapped) : base(wrapped, _lzoInstance)
        {
        }
    }
}
