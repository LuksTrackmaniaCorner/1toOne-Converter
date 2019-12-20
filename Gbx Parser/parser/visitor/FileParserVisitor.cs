using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Gbx.Parser.Core;
using Gbx.Parser.info;

namespace Gbx.Parser.Visitor
{
    public class FileParserVisitor : InOutVisitor<ParsingArgs, object>
    {
        private const uint Facade = 0xFACADE01;

        protected internal override object Visit(GbxComponent component, ParsingArgs arg)
        {
            throw new NotImplementedException();
        }

        protected internal override object Visit(GbxLeaf leaf, ParsingArgs arg)
        {
            leaf.FromStream(arg.Reader);

            throw new NotImplementedException();
        }

        protected internal override object Visit<T>(GbxComposite<T> composite, ParsingArgs arg)
        {
            foreach(var child in composite)
            {
                Dispatch(child, arg);
            }

            throw new NotImplementedException();
        }

        protected internal override object Visit(GbxLookBackString lookBackString, ParsingArgs arg)
        {
            if (arg.LookBackStringVersion == null)
            {
                var version = arg.Reader.ReadUInt32();
                Trace.Assert(version == 3, "Unsupported Version of the Lookbackstrings");
            }

            lookBackString.FromStream(arg.Reader, arg.StoredStrings);

            throw new NotImplementedException();
        }

        protected internal override object Visit<T>(GbxArray<T> array, ParsingArgs arg)
        {
            var count = arg.Reader.ReadUInt32();
            if (count < 0)
                throw new Exception();

            array.Clear();
            for(int i = 0; i < count; i++)
            {
                Dispatch(array.AddNew(), arg); //Parse all the child elements
            }

            throw new NotImplementedException();
        }

        protected internal override object Visit(GbxNode node, ParsingArgs arg)
        {
            var classID = arg.Reader.ReadUInt32();
            var classInfo = GbxInfo.GetClassInfo(classID);

            if (node.ClassInfo != classInfo)
                throw new Exception();

            node.Clear();

            var chunkID = arg.Reader.ReadUInt32();

            while(chunkID != Facade)
            {
                var newChunk = GbxInfo.CreateChunk(chunkID);
                Dispatch(newChunk, arg);

                chunkID = arg.Reader.ReadUInt32();
            }

            throw new NotImplementedException();
        }

        protected internal override object Visit(GbxNodeReference nodeReference, ParsingArgs arg)
        {
            var index = arg.Reader.ReadInt32();
            if (index == -1)
            {
                nodeReference.RemoveNode();
            }
            else if (index == arg.Nodes.Count)
            {
                //New Node, read node and append to list
                var newNode = new GbxNode(nodeReference.NodeClassInfo!); //TODO consider the null case
                Visit(newNode, arg);
                nodeReference.SetNode(newNode);
                arg.Nodes.Add(newNode);
            }
            else
            {
                //Lookup node in the NodeList
                nodeReference.SetNode(arg.Nodes[index]);
            }

            throw new NotImplementedException();
        }
    }

    //TODO make nested class
    public class ParsingArgs
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
