using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class SkippedChunk : Chunk
    {
        public static readonly string bytesKey = "Bytes";

        private Unread bytes;

        public Unread Bytes { get => bytes; set { bytes = value; AddChildNew(value); } }

        private SkippedChunk() : base(null, null)
        {

        }

        public SkippedChunk(Stream s, int length, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            Bytes = new Unread(s, length);
        }

        public SkippedChunk(uint chunkID, byte[] value) : base(null, null)
        {
            this.ChunkID = chunkID;
            Bytes = new Unread(value);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(bytesKey, Bytes);
            return result;
        }
    }
}
