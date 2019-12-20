using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxSizeWrapper<T> : GbxWrapper<T> where T : GbxComponent
    {
        public GbxSizeWrapper(GbxComposite<T> wrapped) : base(wrapped)
        {
        }
    }
}
