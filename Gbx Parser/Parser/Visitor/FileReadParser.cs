using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Gbx.Parser.Core;
using Gbx.Parser.Info;

namespace Gbx.Parser.Visit
{
    public class FileReadParser : Visitor
    {
        public const uint Facade = 0xFACADE01;
        public const uint Skip = 0x534B4950;

        private ParsingArgs _arg;

        public FileReadParser(BinaryReader reader)
        {
            _arg = new ParsingArgs(reader);
        }

        protected internal override void Visit(GbxComponent component)
        {
            throw new NotImplementedException();
        }

        protected internal override void Visit(GbxLeaf leaf)
        {
            leaf.FromStream(_arg.Reader);

            throw new NotImplementedException();
        }

        protected internal override void Visit<T>(GbxComposite<T> composite)
        {
            foreach(var child in composite)
            {
                Dispatch(child);
            }

            throw new NotImplementedException();
        }

        protected internal override void Visit(GbxLookBackString lookBackString)
        {
            if (_arg.LookBackStringVersion == null)
            {
                var version = _arg.Reader.ReadUInt32();
                Trace.Assert(version == 3, "Unsupported Version of the Lookbackstrings");
            }

            lookBackString.FromStream(_arg.Reader, _arg.StoredStrings);
        }

        protected internal override void Visit<T>(GbxArray<T> array)
        {
            var count = _arg.Reader.ReadUInt32();
            if (count < 0)
                throw new Exception();

            array.Clear();
            for(int i = 0; i < count; i++)
            {
                Dispatch(array.AddNew()); //Parse all the child elements
            }
        }

        protected internal override void Visit(GbxNodeReference nodeReference)
        {
            var index = _arg.Reader.ReadInt32();
            var nodeCount = _arg.Nodes.Count;

            if (index == -1)
            {
                nodeReference.RemoveNode();
            }
            else if (index == nodeCount)
            {
                //New Node, read node and append to list
                var newNode = new GbxNode(nodeReference.NodeClassInfo);
                Visit(newNode);
                nodeReference.SetNode(newNode);
                _arg.Nodes.Add(newNode);
            }
            else if(index >= nodeCount)
            {
                //Lookup node in the NodeList
                nodeReference.SetNode(_arg.Nodes[index]);
            }
            else
            {
                throw new Exception();
            }
        }

        protected internal override void Visit(GbxNode node)
        {
            var classID = _arg.Reader.ReadUInt32();
            var classInfo = GbxInfo.GetClassInfo(classID);

            if (node.ClassInfo != classInfo)
                throw new Exception("Found a different class than expected");

            node.Clear();

            uint chunkID;

            while ((chunkID = _arg.Reader.ReadUInt32()) != Facade)
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
                    var skip = _arg.Reader.ReadUInt32();
                    if (skip != Skip)
                        throw new Exception("Chunk was expected to be skippable, but it was not");

                    var length = _arg.Reader.ReadUInt32();
                    var startPos = _arg.Reader.BaseStream.Position;

                    var temp = _arg;
                    _arg = new ParsingArgs(temp.Reader); //Reset all the ParsingArg data.
                    Dispatch(newChunk);
                    _arg = temp;

                    var endPos = _arg.Reader.BaseStream.Position;

                    if (startPos - endPos != length)
                        throw new Exception("Chunk length does not match expected Value.");
                }
            }
        }

        private class ParsingArgs
        {
            public BinaryReader Reader { get; }

            public uint? LookBackStringVersion { get; internal set; }

            public List<string> StoredStrings { get; }

            public List<GbxNode> Nodes { get; }

            public ParsingArgs(BinaryReader reader)
            {
                Reader = reader;
                StoredStrings = new List<string>();
                Nodes = new List<GbxNode>();
            }
        }
    }
}
