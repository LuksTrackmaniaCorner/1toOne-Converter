using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.Streams;
using System.Diagnostics;
using static System.Text.Encoding;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.core.chunks;

namespace _1toOne_Converter.src.gbx
{
    public class Header : Structure
    {
        //Keys for storing the children in the map.
        //Effectively the descriptions for the components.
        public static readonly string magicKey = "Magic";
        public static readonly string versionKey = "Version";
        public static readonly string formatKey = "Format";
        public static readonly string refTableCompressedKey = "Reference Table Compressed";
        public static readonly string bodyCompressedKey = "Body Compressed";
        public static readonly string unknownKey = "Unknown";
        public static readonly string mainClassIDKey = "Main Class ID";
        public static readonly string numNodesKey = "Number of Nodes";

        private GBXFixedLengthString magic;
        private GBXUShort version;
        private GBXFixedLengthString format;
        private GBXFixedLengthString refTableCompressed;
        private GBXFixedLengthString bodyCompressed;
        private GBXFixedLengthString unknown;
        private GBXUInt mainClassID;
        private GBXUInt numNodes;

        public GBXFixedLengthString Magic { get => magic; set { magic = value; AddChildNew(value); } }
        public GBXUShort Version { get => version; set { version = value; AddChildNew(value); } }
        public GBXFixedLengthString Format { get => format; set { format = value; AddChildNew(value); } }
        public GBXFixedLengthString RefTableCompressed { get => refTableCompressed; set { refTableCompressed = value; AddChildNew(value); } }
        public GBXFixedLengthString BodyCompressed { get => bodyCompressed; set { bodyCompressed = value; AddChildNew(value); } }
        public GBXFixedLengthString Unknown { get => unknown; set { unknown = value; AddChildNew(value); } }
        public GBXUInt MainClassID { get => mainClassID; set { mainClassID = value; AddChildNew(value); } }
        public GBXUInt NumNodes { get => numNodes; set { numNodes = value; AddChildNew(value); } }

        public Header(Stream s, out MainNode mainNode)
        {
            Magic = new GBXFixedLengthString(s, 3);
            Trace.Assert(Magic.Get() == "GBX", "magic string not gbx");

            Version = new GBXUShort(s);
            Trace.Assert(Version.Value>= 6, "Only Header Version 6 and above are supported.");

            Format = new GBXFixedLengthString(s, 1);
            Trace.Assert(Format.Get() == "B", "File must be in binary format.");

            RefTableCompressed = new GBXFixedLengthString(s, 1); ;
            Trace.Assert(RefTableCompressed.Get() == "U", "Reference Table must always be uncompressed.");

            BodyCompressed = new GBXFixedLengthString(s, 1); ;
            Trace.Assert(BodyCompressed.Get() == "C", "File Body must be compressed.");

            Unknown = new GBXFixedLengthString(s, 1);

            MainClassID = new GBXUInt(s);

            mainNode = new MainNode(MainClassID.Value);
            mainNode.ReadHeaderChunks(s);

            NumNodes = new GBXUInt(s); // TODO change num nodes if nodes are added 
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(magicKey, Magic);
            result.AddChild(versionKey, Version);
            result.AddChild(formatKey, Format);
            result.AddChild(refTableCompressedKey, RefTableCompressed);
            result.AddChild(bodyCompressedKey, BodyCompressed);
            result.AddChild(unknownKey, Unknown);
            result.AddChild(mainClassIDKey, MainClassID);
            return result;
        }
    }
}