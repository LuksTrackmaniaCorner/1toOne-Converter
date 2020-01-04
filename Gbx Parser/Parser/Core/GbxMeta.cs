using System;
using System.Collections.Generic;
using System.Text;
using Gbx.Parser.Visit;

namespace Gbx.Parser.Core
{
    public class GbxMeta : GbxComposite<GbxLookBackString>
    {
        public override IEnumerable<GbxLookBackString> GetChildren()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(string, GbxLookBackString)> GetNamedChildren()
        {
            throw new NotImplementedException();
        }

        internal override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
