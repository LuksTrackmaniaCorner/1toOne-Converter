using Converter.Converion;
using Converter.Gbx.chunks;
using Converter.Gbx.core;
using Converter.Gbx.core.chunks;
using Converter.Gbx.core.primitives;
using Converter.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx
{
    //TODO add type enum by main class id.
    public class GBXFile : Structure
    {
        private const float Yaw0 = 0;
        private const float Yaw1 = -1.57079632679489f;
        private const float Yaw2 = -3.14159265358979f;
        private const float Yaw3 = 1.57079632679489f;

        public const int MaxMapXSize = 64;
        public const int MaxMapYSize = 64;
        public const int MaxMapZSize = 64;

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


        //Data storage for Conversions
        //TODO use a Dictionary<string, object> for all metadata
        //for example metadata["Flags"] = new Dictionary<string, bool[,]>();
        internal GBXVec3 GridSize;
        internal GBXVec3 GridOffset;
        private readonly Dictionary<string, byte[,]> flags;
        private readonly Dictionary<string, HashSet<Clip>> clips;
        private readonly Dictionary<PylonType, HashSet<Pylon>> pylons;
        private readonly List<(string, double)> statistics;

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

            flags = new Dictionary<string, byte[,]>();
            clips = new Dictionary<string, HashSet<Clip>>();
            pylons = new Dictionary<PylonType, HashSet<Pylon>>();
            statistics = new List<(string, double)>();
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
            return (Chunk)MainNode.Get(key);
        }

        public void AddBodyChunk(string key, Chunk chunk)
        {
            MainNode.AddChunk(key, chunk);
        }

        public void AddPylon(Pylon pylon)
        {
            if (!pylons.ContainsKey(pylon.Type))
                pylons.Add(pylon.Type, new HashSet<Pylon>());
            pylons[pylon.Type].Add(pylon.Normalize());
        }

        public void AddPylons(IEnumerable<Pylon> pylonList, byte x, byte y, byte z, byte rot)
        {
            foreach (var pylon in pylonList)
                AddPylon(pylon.GetRelativeToBlock(x, y, z, rot));
        }

        public IEnumerable<Pylon> GetPylons(PylonType type)
        {
            if (!pylons.ContainsKey(type))
                return Enumerable.Empty<Pylon>();
            return pylons[type];
        }

        public void AddClip(Clip clip)
        {
            if (!clips.ContainsKey(clip.Name))
                clips.Add(clip.Name, new HashSet<Clip>());
            clips[clip.Name].Add(clip);
        }

        public void AddClips(IEnumerable<Clip> clipList, byte x, byte y, byte z, byte rot)
        {
            foreach (var clip in clipList)
                AddClip(clip.GetRelativeToBlock(x, y, z, rot));
        }

        public IEnumerable<Clip> GetClips(string name)
        {
            if (!clips.ContainsKey(name))
                return null;
            return clips[name];
        }

        public void SetFlag(Flag flag)
        {
            if (!flags.ContainsKey(flag.Name))
                flags.Add(flag.Name, new byte[MaxMapXSize, MaxMapZSize]);
            flags[flag.Name][flag.X, flag.Z] = flag.Y;
        }

        public void SetFlags(IEnumerable<Flag> flagList, byte x, byte z, byte rot)
        {
            foreach (var flag in flagList)
                SetFlag(flag.GetRelativeToBlock(x, z, rot));
        }

        public byte GetFlag(string name, byte x, byte z)
        {
            if (!flags.ContainsKey(name))
                return 0;
            return flags[name][x, z];
        }

        public bool TestFlag(string name, int x, int z)
        {
            if (!flags.ContainsKey(name))
                return false;
            return flags[name][x, z] != 0;
        }

        public void Move(int x, int y, int z)
        {

            var chunk0304301F = (Challenge0304301F)GetChunk(Chunk.challenge0304301FKey);

            if(chunk0304301F != null)
            {
                foreach (var block in chunk0304301F.Blocks)
                {
                    block.Coords.X = (byte)(block.Coords.X + x);
                    block.Coords.Y = (byte)(block.Coords.Y + y);
                    block.Coords.Z = (byte)(block.Coords.Z + z);
                }
            }

            if(GridSize != null)
            {
                var chunk03043028 = (Challenge03043028)GetChunk(Chunk.challenge03043028Key);

                if(chunk03043028 != null && chunk03043028.SnapshotPosition is GBXVec3 pos)
                {
                    pos.X += GridSize.X * x;
                    pos.Y += GridSize.Y * y; //TODO not generally true, find a better way to move the map
                    pos.Z += GridSize.Z * z;
                }
            }
        }

        #region Grid Operations
        public GBXVec3 ConvertCoords((byte x, byte y, byte z) coords)
        {
            return new GBXVec3(
                coords.x * GridSize.X + GridOffset.X,
                coords.y * GridSize.Y + GridOffset.Y,
                coords.z * GridSize.Z + GridOffset.Z
            );
        }

        public GBXVec3 ConvertCoords((byte x, byte y, byte z) coords, float smallYOffset)
        {
            return new GBXVec3(
                coords.x * GridSize.X + GridOffset.X,
                coords.y * GridSize.Y + GridOffset.Y + smallYOffset,
                coords.z * GridSize.Z + GridOffset.Z
            );
        }

        public GBXVec3 ConvertPylonCoords((byte x, byte y, byte z) coords, byte rot)
        {
            return rot switch
            {
                2 => new GBXVec3(
                     coords.x * GridSize.X + GridOffset.X,
                     coords.y * GridSize.Y + GridOffset.Y,
                    (coords.z - 0.5f) * GridSize.Z + GridOffset.Z
                ),
                1 => new GBXVec3(
                    (coords.x - 0.5f) * GridSize.X + GridOffset.X,
                     coords.y * GridSize.Y + GridOffset.Y,
                     coords.z * GridSize.Z + GridOffset.Z
                ),
                _ => throw new InternalException()
            };
        }

        public static GBXVec3 ConvertRot(byte rot)
        {
            return rot switch
            {
                0 => new GBXVec3(Yaw0, 0, 0),
                1 => new GBXVec3(Yaw1, 0, 0),
                2 => new GBXVec3(Yaw2, 0, 0),
                3 => new GBXVec3(Yaw3, 0, 0),
                _ => throw new InternalException(),
            };
        }
        #endregion

        #region Statistics
        public void AddStatistic(string statistic, double value)
        {
            statistics.Add((statistic, value));
        }

        internal String GetStatistics()
        {
            var result = new StringBuilder();

            foreach((string statistic, double value) in statistics)
            {
                result.Append(statistic);
                result.Append(": ");
                result.Append(value);
                result.Append('\n');
            }

            return result.ToString();
        }
        #endregion
    }
}
