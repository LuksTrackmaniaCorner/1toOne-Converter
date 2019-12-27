using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.util;
using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.gbx.core
{
    public class Node : Structure
    {
        public uint ClassID { get; set; }

        [XmlArrayItem(ElementName = "NamedChunk")]
        public NamedChild[] Chunks
        {
            get => Children;
            set => Children = value;
        }

        private Node()
        {

        }

        public Node(uint classID) => ClassID = classID;

        public Node(Stream s, GBXLBSContext context, GBXNodeRefList list) : this(s, s.ReadUInt(), context, list)
        {

        }

        private Node(Stream s, uint classID, GBXLBSContext context, GBXNodeRefList list) : this(classID) => ReadChunks(s, context, list);

        protected void ReadChunks(Stream s, GBXLBSContext context, GBXNodeRefList list)
        {
            while (true)
            {
                var newChunk = Chunk.ReadBodyChunk(s, context, list, out string key);

                if (newChunk == null)
                    break;

                //Trace.Assert(ClassID == (newChunk.ChunkID & 0xFFFFF000), "Error reading GBX node" + ClassID.ToString("X") + " " + newChunk.ChunkID.ToString("X"));

                AddChunk(key, newChunk);
            }
        }

        public void AddChunk(string key, Chunk newChunk)
        {
            if (newChunk.ChunkID == 0x24003029 || newChunk.ChunkID == 0x03043029 || newChunk.ChunkID == 0x24003014 || newChunk.ChunkID == 0x03043014)
                return; //TODO come up with better solution.

            AddChildDeprecated(key, newChunk);
            newChunk.Parent = this;
        }

        public override void WriteBack(Stream s)
        {
            s.WriteUInt(ClassID);
            base.WriteBack(s); // The chunks TODO: Order them by their chunkid
            s.WriteUInt(Chunk.facade);
        }
    }

    public class MainNode : Node
    {

        private readonly GBXLBSContext _context;
        private readonly GBXNodeRefList _list;

        public MainNode(uint classID) : base(classID)
        {
            _context = new GBXLBSContext();
            _list = new GBXNodeRefList();
        }

        public override GBXLBSContext LBSContext => _context;

        public override GBXNodeRefList NodeRefList => _list;

        public override void WriteBack(Stream s)
        {
            throw new NotImplementedException();
        }

        public void ReadHeaderChunks(Stream s)
        {
            uint userDataSize = s.ReadUInt();
            var userDataStartPos = s.Position;

            uint numHeaderChunks = s.ReadUInt();

            var headerInfos = new (uint chunkID, uint chunkSize)[numHeaderChunks];

            for(int i = 0; i < numHeaderChunks; i++)
            {
                headerInfos[i] = (s.ReadUInt(), s.ReadUInt());
            }

            for(uint i = 0; i < numHeaderChunks; i++)
            {
                uint chunkID = headerInfos[i].chunkID;
                uint chunkSize = headerInfos[i].chunkSize;

                long startPos = s.Position;

                var newChunk = Chunk.ReadHeaderChunk(s, chunkID, out string key);
                AddChunk(key, newChunk);

                long endPos = startPos + (chunkSize & 0x7FFFFFFF);
                if (endPos != s.Position)
                {
                    throw new InternalException("Chunk could not be read. Chunk ID: " + chunkID.ToString("x"));
                }
            }

            Trace.Assert(s.Position == userDataStartPos + userDataSize, "User data size did not match.");
        }

        public void WriteHeaderChunks(Stream s)
        {
            var userDataPos = s.Position;
            s.Position += 4; //Skip UserDataSize for now;
            var userDataStartPos = s.Position;

            var headerChunks = new List<Chunk>();

            foreach (var child in this)
            {
                if (child is Chunk chunk && chunk.IsHeaderChunk)
                    headerChunks.Add(chunk);
            }

            s.WriteUInt((uint)headerChunks.Count);

            var chunkInfoPos = s.Position;
            s.Position += 8 * headerChunks.Count; //Skip Chunk Infos

            for(int i = 0; i < headerChunks.Count; i++)
            {
                var headerChunk = headerChunks[i];

                //Write Chunk
                var sizeStartPos = s.Position;
                headerChunk.WriteBack(s);
                var sizeEndPos = s.Position;

                //Calculat Chunk Size
                var chunkSize = (uint)(sizeEndPos - sizeStartPos);
                if (headerChunk.IsSkippable)
                    chunkSize |= 0x80000000;

                //Write Chunk Info
                s.Position = chunkInfoPos;
                s.WriteUInt(headerChunk.ChunkID);
                s.WriteUInt(chunkSize);
                chunkInfoPos = s.Position;

                //Skip to end of the stream again
                s.Position = sizeEndPos;
            }

            //Calculate User Data Size
            var userDataEndPos = s.Position;
            var userDataSize = (uint)(userDataEndPos - userDataStartPos);

            //Write User Dáta Size
            s.Position = userDataPos;
            s.WriteUInt(userDataSize);
            s.Position = userDataEndPos;
        }

        public void ReadBodyChunks(Stream s)
        {
            uint uncompressedSize = s.ReadUInt();

            uint compressedSize = s.ReadUInt();
            Trace.Assert(compressedSize <= uncompressedSize, "Compressed size must be smaller than uncompressedSize");

            var compressedBody = new Unread(s, (int)compressedSize);
            Trace.Assert(s.Position == s.Length, "Could not read the file");

            var uncompressedBody = new byte[uncompressedSize];
            MiniLZO.Decompress(compressedBody.Get(), uncompressedBody);

            using var ms = new MemoryStream(uncompressedBody);
            try
            {
                ReadChunks(ms, _context, _list);
            }
            catch(UnknownChunkException)
            {
                //TODO Add Warning that a chunk could not be read and all further chunks have been skipped
            }
        }

        public void WriteBodyChunk(Stream s)
        {
            var bodyChunks = from namedChild in Children
                             let chunk = namedChild.Child as Chunk
                             where chunk != null
                             where !chunk.IsHeaderChunk
                             orderby chunk.ChunkID & 0xFFF
                             select chunk;

            byte[] uncompressedBody;

            using (var ms = new MemoryStream())
            {
                foreach (var chunk in bodyChunks)
                    chunk.WriteBack(ms);

                ms.WriteUInt(Chunk.facade);

                uncompressedBody = ms.ToArray();
            }

            //Compressing the body
            MiniLZO.Compress(uncompressedBody, out var compressedBody);

            s.WriteUInt((uint)uncompressedBody.Length);
            s.WriteUInt((uint)compressedBody.Length);
            s.SimpleWrite(compressedBody);
        }
    }
}
