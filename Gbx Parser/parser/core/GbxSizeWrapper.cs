using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.core
{
    public class GbxSizeWrapper<T> : GbxWrapper<T> where T : GbxComponent
    {
        public GbxSizeWrapper(GbxComposite<T> wrapped) : base(wrapped)
        {
        }
    }
}
