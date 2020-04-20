using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.primitives
{
    public class GBXUInt : Primitive<uint> , IEquatable<GBXUInt>
    {
        private int _formatBase;

        private GBXUInt()
        {
            _formatBase = 10;
        }

        public GBXUInt(Stream s) : this(s.ReadUInt())
        {
            
        }

        public GBXUInt(uint ui)
        {
            Value = ui;
            _formatBase = 10;
        }

        public void SetBase(int formatBase)
        {
            //TODO add some testing
            _formatBase = formatBase;
        }

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();
            result.AddLast(Convert.ToString(Value, _formatBase));
            return result;
        }

        public override void WriteBack(Stream s)
        {
            s.WriteUInt(Value);
        }

        public bool Equals(GBXUInt other)
        {
            return this.Value == other.Value;
        }

        public override FileComponent DeepClone()
        {
            return new GBXUInt(Value);
        }
    }
}
