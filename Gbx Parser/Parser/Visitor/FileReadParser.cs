using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Gbx.Parser.Compression;
using Gbx.Parser.Core;
using Gbx.Parser.Info;
using Gbx.Util;

namespace Gbx.Parser.Visit
{
    public class FileReadParser : Visitor
    {
        public const string Magic = "GBX";

        public const uint Facade = 0xFACADE01;
        public const uint Skip = 0x534B4950; //SKIP in UTF8

        private ParsingArgs _arg;
        private BinaryReader _reader => _arg.Reader;

        /// <summary>
        /// Node Reference List
        /// </summary>
        private readonly List<GbxNode> _nodes;

        public FileReadParser(BinaryReader reader)
        {
            _arg = new ParsingArgs(reader);
            _nodes = new List<GbxNode>(1);
        }

        internal GbxNode ReadMainNode()
        {
            var magic = _reader.ReadUTF8Chars(3);
            Trace.Assert(magic == Magic, "Not a gbx file");

            var version = _reader.ReadUInt16();
            Trace.Assert(version >= 6, "File too old, only gbx version 6 and above is supported.");

            var format = _reader.ReadUTF8Char();
            Trace.Assert(format == 'B', "Text encrypted files are not supported.");

            var refTableCompressed = _reader.ReadUTF8Char();
            Trace.Assert(refTableCompressed == 'U', "Reference Table must be uncompressed");

            var bodyCompressed = _reader.ReadUTF8Char();
            Trace.Assert(bodyCompressed == 'C', "Files with uncompressed Body not yet supported.");

            var unknown = _reader.ReadUTF8Char();
            Trace.Assert(unknown == 'R', "Unknown value incorrect, interesting file you got there");

            var nodeClassID = _reader.ReadUInt32();
            var nodeClassInfo = GbxInfo.GetClassInfo(nodeClassID);
            var node = new GbxNode(nodeClassInfo);

            ReadHeaderChunks(node);

            var numNodes = _reader.ReadInt32();
            _nodes.Capacity = numNodes;

            ReadReferenceTable(node);

            ReadBodyChunks(node); 

            return node;
        }

        private void ReadHeaderChunks(GbxNode node)
        {
            const uint sizeMask = 0x7FFFFFFF;
            const uint skipMask = ~sizeMask;

            var lengthTester = _reader.CreateLengthTester(); //For observing the user data size

            var numHeaderChunks = _reader.ReadUInt32();
            var headerInfos = new (uint chunkID, uint chunkSize, bool skippable)[numHeaderChunks];
            for (int i = 0; i < numHeaderChunks; i++)
            {
                headerInfos[i].chunkSize = _reader.ReadUInt32();
                var rawSize = _reader.ReadUInt32();
                headerInfos[i].chunkSize = rawSize & sizeMask;
                headerInfos[i].skippable = (rawSize & skipMask) != 0;
            }

            for (int i = 0; i < numHeaderChunks; i++)
            {
                var chunkID = headerInfos[i].chunkID;
                var chunkSize = headerInfos[i].chunkSize;
                var skippable = headerInfos[i].skippable;

                var chunkLengthTester = _reader.CreateLengthTester(chunkSize);

                var chunkInfo = GbxInfo.GetChunkInfo(chunkID);
                Trace.Assert(chunkInfo.IsSkippable == skippable);

                var chunk = chunkInfo.CreateChunk();
                Visit(chunk);
                node.Add(chunk);

                Trace.Assert(chunkLengthTester.TestLength(), "Unexpected Chunk Length");
            }

            Trace.Assert(lengthTester.TestLength(), "Invalid Header Length");
        }

        public void ReadReferenceTable(GbxNode node)
        {
            if (node is null)
            { 
            }

            var numExternalNodes = _reader.ReadUInt32();

            if (numExternalNodes == 0)
                return;

            //TODO
            throw new NotImplementedException("Reftable contains entry, these are not implemented yet");
        }

        public void ReadBodyChunks(GbxNode node)
        {
            var lzo = new LzoCompression();

            var uncompressedReader = new BinaryReader(lzo.Decompress(_reader));
            _arg = new ParsingArgs(uncompressedReader);

            //Read the remaining chunks normally
            ReadChunks(node);
        }

        protected internal override void Visit(GbxComponent component)
        {
            throw new NotImplementedException();
        }

        protected internal override void Visit(GbxLeaf leaf)
        {
            leaf.FromStream(_reader);
        }

        protected internal override void Visit<T>(GbxComposite<T> composite)
        {
            foreach(var child in composite)
            {
                Dispatch(child);
            }
        }

        protected internal override void Visit(GbxLookBackString lookBackString)
        {
            if (_arg.LookBackStringVersion == null)
            {
                var version = _reader.ReadUInt32();
                Trace.Assert(version == 3, "Unsupported Version of the Lookbackstrings");
            }

            lookBackString.FromStream(_reader, _arg.StoredStrings);
        }

        protected internal override void Visit<T>(GbxArray<T> array)
        {
            var count = _reader.ReadUInt32();

            array.Clear();
            for(uint i = 0; i < count; i++)
            {
                Dispatch(array.AddNew()); //Parse all the child elements
            }
        }

        protected internal override void Visit(GbxNodeReference nodeReference)
        {
            var index = _reader.ReadInt32();
            var nodeCount = _nodes!.Count;

            if (index == -1)
            {
                //Empty Node
                nodeReference.RemoveNode();
            }
            else if (index == nodeCount)
            {
                //New Node, read node and append to list
                var newNode = new GbxNode(nodeReference.NodeClassInfo);
                Visit(newNode);
                nodeReference.SetNode(newNode);
                _nodes.Add(newNode);
            }
            else if(index <= nodeCount)
            {
                //Lookup node in the NodeList
                nodeReference.SetNode(_nodes[index]);
            }
            else
            {
                throw new Exception();
            }
        }

        protected internal override void Visit(GbxNode node)
        {
            //Will only be called if this is not the main node
            var classID = _reader.ReadUInt32();
            var classInfo = GbxInfo.GetClassInfo(classID);

            if (node.ClassInfo != classInfo) //TODO consider subclasses
                throw new Exception("Found a different class than expected");

            node.Clear();

            ReadChunks(node);
        }

        private void ReadChunks(GbxNode node)
        {
            uint chunkID;

            while ((chunkID = _reader.ReadUInt32()) != Facade)
            {
                var newChunk = GbxInfo.CreateChunk(chunkID)!; //TODO Chunk not found case
                if (newChunk.ChunkInfo.IsSkippable)
                {
                    //Read Chunk
                    Dispatch(newChunk);
                }
                else
                {
                    //Read Skippable Chunk
                    var skip = _reader.ReadUInt32();
                    if (skip != Skip)
                        throw new Exception("Chunk was expected to be skippable, but it was not");

                    var length = _reader.ReadUInt32();
                    var startPos = _reader.BaseStream.Position;

                    var temp = _arg;
                    _arg = new ParsingArgs(temp.Reader); //Reset all the ParsingArg data.
                    Dispatch(newChunk);
                    _arg = temp;

                    var endPos = _reader.BaseStream.Position;

                    if (startPos - endPos != length)
                        throw new Exception("Chunk length does not match expected Value.");
                }

                node.Add(newChunk);
            }
        }

        private class ParsingArgs
        {
            public BinaryReader Reader { get; }

            public uint? LookBackStringVersion { get; internal set; }

            public List<string> StoredStrings { get; }

            public ParsingArgs(BinaryReader reader)
            {
                Reader = reader;
                StoredStrings = new List<string>();
            }
        }
    }
}
