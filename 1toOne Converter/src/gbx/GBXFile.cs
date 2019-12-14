using _1toOne_Converter.src.conversion;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx
{
    //TODO add type enum by main class id.
    public class GBXFile : Structure
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

        public static readonly string refTableKey = "Reference Table";

        public static readonly string mainNodeKey = "Node";

        //Header Data
        private GBXFixedLengthString magic;
        private GBXUShort version;
        private GBXFixedLengthString format;
        private GBXFixedLengthString refTableCompressed;
        private GBXFixedLengthString bodyCompressed;
        private GBXFixedLengthString unknown;
        private GBXUInt mainClassID;
        private GBXUInt numNodes;

        //RefTable
        private RefTable refTable;

        //MainNode
        private MainNode mainNode;
        

        public GBXFixedLengthString Magic { get => magic; set { magic = value; AddChildNew(value); } }
        public GBXUShort Version { get => version; set { version = value; AddChildNew(value); } }
        public GBXFixedLengthString Format { get => format; set { format = value; AddChildNew(value); } }
        public GBXFixedLengthString RefTableCompressed { get => refTableCompressed; set { refTableCompressed = value; AddChildNew(value); } }
        public GBXFixedLengthString BodyCompressed { get => bodyCompressed; set { bodyCompressed = value; AddChildNew(value); } }
        public GBXFixedLengthString Unknown { get => unknown; set { unknown = value; AddChildNew(value); } }
        public GBXUInt MainClassID { get => mainClassID; set { mainClassID = value; AddChildNew(value); } }
        public GBXUInt NumNodes { get => numNodes; set { numNodes = value; AddChildNew(value); } }
        public RefTable RefTable { get => refTable; set { refTable = value; AddChildNew(value); } }
        public MainNode MainNode { get => mainNode; set { mainNode = value; AddChildNew(value); } }


        //Markers
        private readonly Dictionary<string, List<(float X, float Y, float Z)>> markers;

        public GBXFile(Stream s)
        {
            Magic = new GBXFixedLengthString(s, 3);
            Trace.Assert(Magic.Get() == "GBX", "magic string not gbx");

            Version = new GBXUShort(s);
            Trace.Assert(Version.Value >= 6, "Only Header Version 6 and above are supported.");

            Format = new GBXFixedLengthString(s, 1);
            Trace.Assert(Format.Get() == "B", "File must be in binary format.");

            RefTableCompressed = new GBXFixedLengthString(s, 1); ;
            Trace.Assert(RefTableCompressed.Get() == "U", "Reference Table must always be uncompressed.");

            BodyCompressed = new GBXFixedLengthString(s, 1); ;
            Trace.Assert(BodyCompressed.Get() == "C", "File Body must be compressed.");

            Unknown = new GBXFixedLengthString(s, 1);

            MainClassID = new GBXUInt(s);

            MainNode = new MainNode(MainClassID.Value);
            MainNode.ReadHeaderChunks(s);

            NumNodes = new GBXUInt(s); // TODO change num nodes if nodes are added 

            RefTable = new RefTable(s);

            MainNode.ReadBodyChunks(s);

            markers = new Dictionary<string, List<(float, float, float)>>();
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
            result.AddChild(numNodesKey, NumNodes);

            result.AddChild(refTableKey, RefTable);

            result.AddChild(mainNodeKey, mainNode);
            return result;
        }

        public override void WriteBack(Stream s)
        {
            Magic.WriteBack(s);
            Version.WriteBack(s);
            Format.WriteBack(s);
            RefTableCompressed.WriteBack(s);
            BodyCompressed.WriteBack(s);
            Unknown.WriteBack(s);
            MainClassID.WriteBack(s);
            MainNode.WriteHeaderChunks(s);
            NumNodes.WriteBack(s);

            RefTable.WriteBack(s);

            MainNode.WriteBodyChunk(s);
        }

        public Chunk GetChunk(string key)
        {
            return (Chunk)(MainNode.Get(key));
        }

        public void AddBodyChunk(string key, Chunk chunk)
        {
            MainNode.AddChunk(key, chunk);
        }

        public void AddMarker(string name, float x, float y, float z)
        {
            if (!markers.ContainsKey(name))
                markers.Add(name, new List<(float X, float Y, float Z)>());
            markers[name].Add((x, y, z));
        }

        public void AddMarker(string name, GBXVec3 vec) => AddMarker(name, vec.X, vec.Y, vec.Z);

        public IReadOnlyCollection<(float x, float y, float z)> GetMarkers(string name)
        {
            if (markers.ContainsKey(name))
                return markers[name]?.AsReadOnly();
            else
                return new List<(float, float, float)>().AsReadOnly();
        }
    }
}
